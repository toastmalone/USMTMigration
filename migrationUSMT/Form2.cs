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
        private string[] _directory = { @"\\dr-main.tmccadmn.tmcc.edu\sam$", @"\\dr-storage.tmccadmn.tmcc.edu\images" };
        public char[] _letter = { 'q', 'w' };

        public Form2()
        {
            InitializeComponent();
            this.FormClosing += Form_Closing;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void password_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            
         _username = userName.Text;
         _password = password.Text;

            int[] exitCodes = new int[_letter.Length];
            int tmp = 0;
            string errors = "";
            bool failed = false;
            for (int i = 0; i < _letter.Length; i++)
            {
                tmp = NetUseAdd(_letter[i], _directory[i], _username, _password);
                if (tmp != 0)
                {
                    errors = String.Concat(errors, _directory[i] + "\n ");
                    NetUseDelete(_letter[i]);
                    failed = true;
                }
            }

            //display error info
            if (failed)
            {
                MessageBoxButtons buttons = MessageBoxButtons.AbortRetryIgnore;
                DialogResult result;

                result = MessageBox.Show(errors, "Error Logging in to", buttons);

                //closes application
                if(result == System.Windows.Forms.DialogResult.Abort)
                {
                    this.Close();
                }

                if (result == System.Windows.Forms.DialogResult.Retry)
                {

                }

                //proceeds without connecting to network drives
                if (result == System.Windows.Forms.DialogResult.Ignore)
                {
                    this.Hide();
                    Form1 window = new Form1();
                    window.Show();
                }
            }
            else
            {
                //successfully connected to network drives so opens next window
                this.Hide();
                Form1 window = new Form1();
                window.Show();
            }
        }

        //maps a network drive to the computer on the tmccadmn domain
        public int NetUseAdd(char letter, string path, string user, string pass)
        {
            string directoryWithAuth = String.Format(@"use {0}: {1} /user:tmccadmn.tmcc.edu\{2} {3}", letter, path, user, pass);
            Process cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = directoryWithAuth;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
            return cmd.ExitCode;

        }
        
        //removes a mapped network drive given the drive letter
        public static void NetUseDelete(char letter)
        {
            string directory = String.Format(@"use {0}: /delete", letter);
            Process cmd = new Process();
            cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = directory;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();
        }

        //close event handler to remove all mapped drives
        //TO DO remove only the networks that were added
        public static void Form_Closing(object sender, FormClosingEventArgs e)
        {
            Debug.WriteLine("Im closing");
            Process cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = @"use * /delete /y";
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.WaitForExit();

            Application.Exit();
        }
    }

    
}
