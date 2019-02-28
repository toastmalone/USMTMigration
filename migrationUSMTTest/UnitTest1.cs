using Microsoft.VisualStudio.TestTools.UnitTesting;
using migrationUSMT;
using System.Diagnostics;
using System.IO;

namespace migrationUSMTTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestAddDrive()
        {
            
            
            
        }

        //expect the added test drive to be deleted
        [TestMethod]
        public void TestDeleteDrive()
        {
            
            string test = "ThisIsATestFolder";
            string testDrivePath = string.Format(@"\\\\localhost\\c$\\{0}", test);
            char driveLetter;

            //creates a test folder that is mapped to a drive letter using net use
            Directory.CreateDirectory(@"c:\" + test);

            string argument = string.Format(@"use * {0}", testDrivePath);

            // maps the local folder 
            Process cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = argument;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();
            
            //capures the output from Process
            StreamReader reader = cmd.StandardOutput;
            string output = reader.ReadToEnd();

            cmd.WaitForExit();

            //gets the drive letter that was mapped to our local test folder
            driveLetter = output[6];

            var main = new Main();
            
            main.NetUseDelete(driveLetter);
        }

        //expect network drives to close and all application processes to stop
        [TestMethod]
        public void TestCloseForm()
        {

        }

        
    }
}
