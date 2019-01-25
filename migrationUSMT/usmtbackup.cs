using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Diagnostics;

namespace migrationUSMT
{
    public partial class usmtbackup : Form
    {
        
        String store = string.Empty;
        public usmtbackup()
        {
            InitializeComponent();
            
            try
            {
                DirectoryInfo directory = new DirectoryInfo(@"C:\Users");
                DirectoryInfo[] directories = directory.GetDirectories();

                foreach (DirectoryInfo folder in directories)
                    checkedListBox1.Items.Add(folder.Name);
            }
            catch(Exception a)
            {
                var something = a;
            }

        }

        private void usmtbackup_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            using (PowerShell PS = PowerShell.Create())
            {
                string path = Directory.GetCurrentDirectory() + @"\amd64\prog.log";

                    button1.Hide();

                    string policyUnRestrict = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted; Get-ExecutionPolicy";
                    string policyRestrict = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Restricted; Get-ExecutionPolicy";
                    /*string programList = @"Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Se
                                       lect - Object DisplayName, DisplayVersion, Publisher, InstallDate | Format - Table –AutoSize > C:\Users\administrator\Deskto
                                        p\InstalledProgramsList.txt";*/

                    string DIR = "cd amd64";
                    string scan = "./scanstate " + store + " /localonly /o /c /ue:* /i:MigUser.xml /i:MigApp.xml /v:13 /vsc /progress:prog.log /listfiles:filelist.txt";
                    foreach (var item in checkedListBox1.CheckedItems)
                    {
                        scan += @" /ui:TMCCADMN\" + item;
                    }
                    Debug.WriteLine(scan);

                    PS.AddScript(policyUnRestrict);
                    PS.AddScript(DIR);
                    PS.AddScript(scan);
                    PS.AddScript(policyRestrict);

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

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private bool searchFile(String file, String searchText)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(file);

            String text = reader.ReadToEnd();

            if (System.Text.RegularExpressions.Regex.IsMatch(text, searchText))
            {
               
                return true;
            }
            else
            {
               
                return false;
            }

        }

        private void backUpLocation_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog browser = new FolderBrowserDialog();


            if (browser.ShowDialog() == DialogResult.OK)
            {
                store = browser.SelectedPath;
                richTextBox2.Text = store;
                
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }

    
}
