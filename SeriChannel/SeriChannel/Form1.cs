using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
public struct SeriPortSettings
{
    public int baudRate;
    public string portName;
    public Parity parity;
    public StopBits stopbit;
    public int dataBit;

}
namespace SeriChannel
{

    public partial class Form1 : Form
    {


        SeriPortSettings serialportSetting;
        
        SerialPort serial = new SerialPort();
        int baud;
        int XPosition = 0;
        int YPosition = 0;
        public Form1()
        {
            InitializeComponent();

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread mouseTracker = new Thread(MouseTrack);

            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;

            groupBox3.Visible = false;
            groupBox3.Enabled = false;
            groupBox4.Visible = false;
            groupBox4.Enabled = false;


        }

        private void MouseTrack()
        {
            while (true)
            {
                XPosition = MousePosition.X;
                YPosition = MousePosition.Y;
                this.Invoke(new Action(() =>
                {

                    textBox1.Text = "X:" + XPosition + "Y:" + YPosition;

                }));
                

            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] portNames = SerialPort.GetPortNames();

            if (portNames.Length > 0)
            {
                button2.Enabled = true;
                foreach (string names in portNames)
                {
                    comboBox1.Items.Add(names);
                }
            }
            else
                MessageBox.Show("There is no available port. Check your connection...", "Not Found Port");


        }

        private void button2_Click(object sender, EventArgs e)
        {
            string serialportSetting = "";
            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a port.", "PORT Error");
                return;
            }

            using (Form2 frm2 = new Form2())
            {
                if (frm2.ShowDialog() == DialogResult.OK)
                {

                    serialportSetting = frm2.GetValue;
                    
                    int count = 0;
                    string transient = "";

                    if (comboBox1.SelectedIndex != -1)
                        serial.PortName = comboBox1.SelectedItem.ToString();
                    else
                    {
                        MessageBox.Show("Please Select Port...", "Port Select Error");
                        return;
                    }
                   
                    for (int i = 0; i < 4; i++)
                    {
                        while (serialportSetting[count] != '\0')
                        {
                            transient += serialportSetting[count];
                            count++;
                        }
                        

                        if (i == 0)
                            serial.BaudRate = int.Parse(transient);
                        else if (i == 1)
                        {
                            if (transient == "0")
                                serial.Parity = Parity.None;
                            else if(transient == "1")
                                serial.Parity = Parity.Odd;
                            else if (transient == "2")
                                serial.Parity = Parity.Even;
                            else if (transient == "3")
                                serial.Parity = Parity.Mark;
                            else if (transient == "4")
                                serial.Parity = Parity.Space;
                        }
                        else if (i == 2)
                        {
                            serial.DataBits = 5 + int.Parse(transient);
                        }
                        else if (i == 3)
                        {
                            if (transient == "0")
                                serial.StopBits = StopBits.None;
                            else if (transient == "1")
                                serial.StopBits = StopBits.One;
                            else if (transient == "2")
                                serial.StopBits = StopBits.OnePointFive;
                            else if (transient == "3")
                                serial.StopBits = StopBits.Two;

                            
                        }
                        count++;
                        transient = "";
                    }

                    try
                    {
                        serial.Open();
                        serial.DataReceived += Serial_DataReceived;
                        

                        button1.Enabled = false;
                        button2.Enabled = false;
                        button3.Enabled = true;

                        groupBox3.Enabled = true;
                        groupBox3.Visible = true;

                        groupBox4.Enabled = true;
                        groupBox4.Visible = true;

                        radioButton1.Checked = true;
                        radioButton5.Checked = true;
                        radioButton8.Checked = true;

                        label1.Visible = false;

                    }
                    catch
                    {
                        MessageBox.Show("Com Port can't open. Check your connection.","Open Port Error");
                    }

                    frm2.Close();
                
                }
            }
              

            


        }

        private void Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string comingMessage = serial.ReadExisting();
            this.Invoke(new Action(() =>
            {
           
                if (radioButton5.Checked)
                    textBox2.Text += comingMessage;
                else if (radioButton6.Checked)
                {
                    for (int i = 0; i < comingMessage.Length; i++)
                    {
                        textBox2.Text += '[';
                        textBox2.Text += ((byte)comingMessage[i]).ToString("X2");              
                        textBox2.Text += ']';
                    }
                }

            }));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                serial.Close();
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = false;

                groupBox3.Visible = false;
                groupBox4.Visible = false;

                textBox1.Text = "";
                textBox2.Text = "";
               

            }
            catch
            {
                MessageBox.Show("Error when trying to close port...", "Port Close Error");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sendingMessage = textBox1.Text;
            if (radioButton2.Checked)
                sendingMessage += '\r';
            else if (radioButton3.Checked)
                sendingMessage += '\n';
            else if (radioButton4.Checked)
                sendingMessage += "\r\n";

            serial.Write(sendingMessage);
            textBox1.Text = "";
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton5.Checked)
            {
                bool startAnalyze = false;
                int start = 0;
                string analyzed = "";
                string message = textBox2.Text;
                textBox2.Text = "";
                for (int i = 0; i < message.Length; i++)
                {
                    if (message[i] == '[')
                    {
                        startAnalyze = true;
                        start = i;
                    }
                    else if (message[i] == ']')
                    {
                        startAnalyze = false;
                        if(analyzed.Length>1)
                        {
                            
                            byte[] bytes = Encoding.Default.GetBytes(analyzed);
                            if (bytes[0] < 71 && bytes[0] > 64)
                                bytes[0] = (byte) (bytes[0] - 7);
                            if (bytes[1] < 71 && bytes[1] > 64)
                                bytes[1] = (byte)(bytes[1] - 7);

                            int value = (bytes[0] - 48) * 16 + (bytes[1] - 48);
                            textBox2.Text += (char)value;
                            analyzed = "";

                        }
                        
                    }

                    if (startAnalyze && start !=i)
                    {
                        analyzed += message[i];
                    }
                
                }
                message += '\r';
            
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                string comingMessage = textBox2.Text;
                textBox2.Text = "";
                this.Invoke(new Action(() =>
                {

                    if (radioButton5.Checked)
                        textBox2.Text += comingMessage;
                    else if (radioButton6.Checked)
                    {
                        for (int i = 0; i < comingMessage.Length; i++)
                        {
                            textBox2.Text += '[';
                            textBox2.Text += ((byte)comingMessage[i]).ToString("X2");
                            textBox2.Text += ']';
                        }
                    }

                }));
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && radioButton7.Checked)
            {
                string sendingMessage = textBox1.Text;
                string sendCRLF = "";
                for (int i = 0; i < sendingMessage.Length; i++)
                {
                    if (sendingMessage[i] != '\r' && sendingMessage[i] != '\n')
                        sendCRLF += sendingMessage[i];
                }

                if (radioButton2.Checked)
                    sendCRLF += '\r';
                else if (radioButton3.Checked)
                    sendCRLF += '\n';
                else if (radioButton4.Checked)
                    sendCRLF += "\r\n";

                serial.Write(sendCRLF);               
                textBox1.ResetText();
            }
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_MouseHover(object sender, EventArgs e)
        {

        }

        private void radioButton1_MouseLeave(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void radioButton1_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = "Add EOM nothing...";
            label1.Visible = true;
        }

        private void radioButton2_MouseMove(object sender, MouseEventArgs e)
        {
            string s0 = @"\r\n";
            label1.Text = "Add EOM " + s0;
            label1.Visible = true;

        }

        private void radioButton2_MouseLeave(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void radioButton3_MouseMove(object sender, MouseEventArgs e)
        {
            string s0 = @"\n";
            label1.Text = "Add EOM " + s0;
            label1.Visible = true;

        }

        private void radioButton3_MouseLeave(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void radioButton4_MouseMove(object sender, MouseEventArgs e)
        {
            string s0 = @"\r\n";
            label1.Text = "Add EOM "+s0;
            label1.Visible = true;
        }

        private void radioButton4_MouseLeave(object sender, EventArgs e)
        {
            label1.Visible = false;
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = true;
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = true;
        }
    }
}
