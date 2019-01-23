using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Management.Automation;
using System.Diagnostics;
using System.Management;
using System.IO;
namespace migrationUSMT
{
    public partial class usmtRestore : Form
    {
        String store = string.Empty;
        public usmtRestore()
        {
            InitializeComponent();
            migrate.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();
            
            
            if(browser.ShowDialog() == DialogResult.OK)
            {
                store = browser.SelectedPath;
                richTextBox2.Text = store;
                migrate.Show();
            }
            
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void migrate_Click(object sender, EventArgs e)
        {
            
            
                using (PowerShell PS = PowerShell.Create())
                {
                    string path = Directory.GetCurrentDirectory() + @"\amd64\prog.log";

                    migrate.Hide();

                    string policy = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted; Get-ExecutionPolicy";
                    /*string programList = @"Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Se
                                           lect - Object DisplayName, DisplayVersion, Publisher, InstallDate | Format - Table –AutoSize > C:\Users\administrator\Deskto
                                            p\InstalledProgramsList.txt";*/

                    string DIR = "cd amd64";
                    string scan = "./loadstate " + store + " /c /i:MigUser.xml /i:MigApp.xml /v:13 /progress:prog.log";

                    Debug.WriteLine(scan);

                    PS.AddScript(policy);
                    PS.AddScript(DIR);
                    PS.AddScript(scan);

                    IAsyncResult result = PS.BeginInvoke();

                    while (result.IsCompleted == false)
                    {
                        richTextBox1.Text = "migrating.";
                        Application.DoEvents();
                        Thread.Sleep(500);
                        richTextBox1.Text = "migrating..";
                        Application.DoEvents();
                        Thread.Sleep(500);
                        richTextBox1.Text = "migrating...";
                        Application.DoEvents();
                        Thread.Sleep(500);
                        Application.DoEvents();
                        richTextBox1.Text = "migrating....";
                        Application.DoEvents();
                    }

                    if (result.IsCompleted == true)
                    {
                        richTextBox1.Text = File.ReadAllText(path);
                    }
                }
            
        }
    }
}
