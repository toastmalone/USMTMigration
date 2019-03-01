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
    public partial class Main : Form
    {
        //max number of drives this program can map
        const int NUMDRIVES = 3;

        private string _username;
        private string _password;
        private string[] _directory = Properties.Settings.Default.DriveAddress.Split(',');


        public Main()
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

            int[] exitCodes = new int[NUMDRIVES];
            char tmp;
            string errors = "";
            bool failed = false;
            
            
            for (int i = 0; i < NUMDRIVES; i++)
            {
                if (_directory[i] != "null")
                {
                    tmp = NetUseAdd(_directory[i], _username, _password);
                    if (tmp == '1')
                    {
                        errors = String.Concat(errors, _directory[i] + "\n ");
                        failed = true;
                    }
                    else
                    {
                        Properties.Settings.Default.DriveChar = String.Concat(Properties.Settings.Default.DriveChar, tmp.ToString());
                    }
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
                    BackupRestore window = new BackupRestore();
                    window.Show();
                }
            }
            else
            {
                //successfully connected to network drives so opens next window
                this.Hide();
                BackupRestore window = new BackupRestore();
                window.Show();
            }
        }

        //maps a network drive to the computer on the tmccadmn domain
        public char NetUseAdd(string path, string user, string pass)
        {
            using (Process cmd = new Process())
            {
                string directoryWithAuth = String.Format(@"use * {0} /user:tmccadmn.tmcc.edu\{1} {2}", path, user, pass);
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.Arguments = directoryWithAuth;
                cmd.StartInfo.UseShellExecute = false;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.Start();

                StreamReader reader = cmd.StandardOutput;
                string output = reader.ReadToEnd();

                //checks that the drive was mapped and grabs the drive letter
                if (output.Contains("Drive "))
                {
                    cmd.WaitForExit();
                    return output[6];
                }
                else
                {
                    cmd.WaitForExit();
                    return '1';
                }
                
               
            }

        }
        
        //removes a mapped network drive given the drive letter
        public void NetUseDelete(char letter)
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
        public static void Form_Closing(object sender, FormClosingEventArgs e)
        {
            string tmp = Properties.Settings.Default.DriveChar;
            Debug.WriteLine("Im closing");
            Process cmd = new Process();
            for (int i = 0; i < tmp.Length; i++  )
            {
                string directory = String.Format(@"use {0}: /delete", tmp[i]);
                
                cmd = new Process();
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.FileName = "net.exe";
                cmd.StartInfo.Arguments = directory;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit();
            }
            
            Application.Exit();
        }
    }

    
}
