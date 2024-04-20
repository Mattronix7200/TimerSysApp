using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer2;

namespace TimerSys
{
    public partial class Form3 : Form
    {
        public Form1 MainForm { get; set; } // Dodaj to pole

        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string password = textBox1.Text;
            string hashedPassword = ComputeMD5Hash(password);
            string dataLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/psd.dat");
            string savedPassword = File.ReadAllText(dataLocation);

            if (hashedPassword == savedPassword)
            {
                MainForm.tabControl1.TabPages["tabPage1"].Enabled = true;
                MainForm.tabControl1.TabPages["tabPage2"].Enabled = true;
                MainForm.tabControl1.TabPages["tabPage3"].Enabled = true;
                MainForm.button6.Enabled = true;
                MainForm.label19.Visible = false;
                MainForm.label19.Enabled = false;
                MainForm.pictureBox6.Enabled = false;
                MainForm.pictureBox6.Visible = false;
                this.Close();
            }
            else
            {
                label2.Visible = true;
                label2.Enabled = true;
            }
        }

        private string ComputeMD5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label2.Visible = false;
            label2.Enabled = false;
        }

    }
}
