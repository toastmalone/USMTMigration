using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace migrationUSMT
{
    public partial class Form2 : Form
    {
        private string _username;
        private string _password;
        private string _directory;
        

        public Form2()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void password_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            try
            {
                _username = userName.Text;
                _password = password.Text;
                _directory = @"\\dr-main.tmccadmn.tmcc.edu\sam$";

                string directoryWithAuth = String.Format(@"use q: {2} /user:tmccadmn.tmcc.edu\{0} {1}", _username, _password, _directory);

                Process cmd = new Process();
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.Arguments = directoryWithAuth;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit();

                _directory = @"\\dr-storage.tmccadmn.tmcc.edu\images";
                directoryWithAuth = String.Format(@"use w: {2} /user:tmccadmn.tmcc.edu\{0} {1}", _username, _password, _directory);
                cmd = new Process();
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.Arguments = directoryWithAuth;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit();

                Form1 main = new Form1();


                main.Show();

                this.Hide();
            }
            catch
            {
                _directory = @"use q: /delete";
                Process cmd = new Process();
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.Arguments = _directory;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit();

                _directory = @"use w: /delete";
                cmd = new Process();
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.Arguments = _directory;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit();

                MessageBox.Show("Error Logging in");
            }

        }
    }
}
