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

            Directory.CreateDirectory(@"c:\" + test);

            string argument = string.Format(@"use * {0}", testDrivePath);

            Process cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = argument;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();

            StreamReader reader = cmd.StandardOutput;
            string output = reader.ReadToEnd();

            cmd.WaitForExit();
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
