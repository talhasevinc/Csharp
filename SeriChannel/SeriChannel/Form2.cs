using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeriChannel
{
    public partial class Form2 : Form
    {
        string info = "";

        public string GetValue
        {
            get
            {
                return info;
            }
        }
        public Form2()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            textBox1.Text = "9600";
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 3;
            comboBox3.SelectedIndex = 1;

            
            
        }



        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int baudRate;
            int paritySelect;
            int stopBit;
            int dataBit;
            bool error = false;
            
            if (IsNumeric(textBox1.Text))
                baudRate = int.Parse(textBox1.Text);
            else
            {
                MessageBox.Show("Baud Rate must be numeric.", " Baud Rate Error");
                error = true;
            }
            paritySelect = comboBox1.SelectedIndex;
            dataBit      = comboBox2.SelectedIndex;
            stopBit      = comboBox3.SelectedIndex;

            if (!error)
            {
                info += textBox1.Text;
                info += '\0';
                info += paritySelect;
                info += '\0';
                info += dataBit;
                info += '\0';
                info += stopBit;
                info += '\0';
            }
            

        }
        public bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }
    }
}
