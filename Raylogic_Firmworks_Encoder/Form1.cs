using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raylogic_Firmworks_Encoder
{
    public partial class Form1 : Form
    {
        // Variables
        int pass_count;

        // Classes

        public Form1()
        {
            InitializeComponent();
            pass_count = 0;
            disable_controls();
        }
        
        private void enable_controls()
        {
            groupBox2.Enabled = true;
        }

        private void disable_controls()
        {
            groupBox2.Enabled = false;
        }

        private void button1_Click (object sender, EventArgs e)
        {
            if((textBox1.Text == "admin") && (textBox2.Text == "ad"))
            {
                enable_controls();
                groupBox1.Enabled = false;
            }
            else
            {
                pass_count++;
                if(pass_count == 4)
                {
                    MessageBox.Show("Maximum Number of Password tries exceeded !");
                    System.Environment.Exit(1);
                }
                disable_controls();
            }
        }

        private void button2_Click (object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Hex files (*.hex)|*.hex|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = openFileDialog1.FileName;
            }
        }
    }
}
