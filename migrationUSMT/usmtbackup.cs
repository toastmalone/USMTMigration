using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
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
            this.FormClosing += Form2.Form_Closing;

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
            button1.Hide();

            using (PowerShell PS = PowerShell.Create())
            {
                string path = Directory.GetCurrentDirectory() + @"\amd64\prog.log";
                string machineName = Environment.MachineName;
                string date = DateTime.Today.ToString("yyyy-MM-dd");
                string policyUnRestrict = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Unrestricted; Get-ExecutionPolicy";
                string policyRestrict = "Set-ExecutionPolicy -Scope Process -ExecutionPolicy Restricted; Get-ExecutionPolicy";

                //TODO add program list generator that doesnt include all the extra junk

                /*string programList = @"Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\* | Se
                                    lect - Object DisplayName, DisplayVersion, Publisher, InstallDate | Format - Table –AutoSize > C:\Users\administrator\Deskto
                                    p\InstalledProgramsList.txt";*/
                    
                string DIR = "cd amd64";
                    
                //string scan = "./scanstate " + store + @"\\" +  machineName + "_" + date + " " + "/localonly /c /ue:* /i:MigUser.xml /i:MigApp.xml /i:MigDocs.xml /v:13 /vsc /progress:prog.log /listfiles:filelist.txt";
                string scan = String.Format(@"./scanstate {0}\\{1}_{2} /localonly /c /ue:* /i:MigUser.xml /i:MigApp.xml /i:MigApp.xml /i:MigDocs.xml /i:MigDocs.xml /v:13 /vsc /progress:prog.log /listfiles:filelist.txt"
                                            , store, machineName, date);

                foreach (var item in checkedListBox1.CheckedItems)
                {
                    scan += @" /ui:TMCCADMN\" + item;
                }


                PS.AddScript(policyUnRestrict); //allows scripts to be run in powershell
                PS.AddScript(DIR); // move to the directory where the usmt files are stored
                PS.AddScript(scan); // runs the scan command with provided inputs
                PS.AddScript(policyRestrict); // restrics powershell again

                //PSDataCollection<VerboseRecord> records = new PSDataCollection<VerboseRecord>();
                //IAsyncResult result = PS.BeginInvoke<PSObject, VerboseRecord>(null, records); // async will allow 

                IAsyncResult result = PS.BeginInvoke();

                //checks to see if the powershell script is complete
                //on completion brings button back and prints log info
                //TODO add streaming output from prog.log and a progress bar this is a temporary solution
                while (result.IsCompleted == false)
                {
                    //var stream = PS.Streams.Verbose;
                    
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
                    button1.Show();
                }
            }

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        
        // Opens a directory browser to pick a store location for the .MIG file 
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
