using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimerSys
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();

            string password = "123456";
            string hashedPassword = ComputeMD5Hash(password);
            string dataLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/psd.dat");
            File.WriteAllText(dataLocation, hashedPassword);
            //this.Close();
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

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
