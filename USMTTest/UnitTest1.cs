using Microsoft.VisualStudio.TestTools.UnitTesting;
using migrationUSMT;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

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
            string testDrivePath = string.Format(@"\\localhost\c$\{0}", test);
            char driveLetter = '0';

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
            if (output.Contains("successfully"))
            {
                driveLetter = output[6];
            }
            else
            {
                // failure to map test drive results in an inconclusive test
                Assert.Inconclusive("failed to map drive");
            }

            var main = new Main();

            //checks that the drive was mapped and if so delete it using Main.NetUseDelete
            if (IsDriveLetterMapped(driveLetter.ToString() + ":") && driveLetter != '0')
            {
                //expect this to delete the drive we just mapped
                main.NetUseDelete(driveLetter);

                //delete the test folder
                Directory.Delete(@"c:\" + test);

                //checks that we successfully removed the drive
                Assert.IsFalse(IsDriveLetterMapped(driveLetter.ToString() + ":"));
            }
            else
            {
                Assert.Inconclusive("Did not detect the test drive " + driveLetter + " as mapped" );
            }

            

        }

        //expect network drives to close and all application processes to stop
        [TestMethod]
        public void TestCloseForm()
        {

        }

        //pass drive letter as Z: or X: to test if the drive shows up as connected
        public bool IsDriveLetterMapped(string letter)
        {
            Process cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = "use";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();

            //capures the output from Process
            StreamReader reader = cmd.StandardOutput;
            string output = reader.ReadToEnd();

            cmd.WaitForExit();

            //checks if the drive letter is mapped or not
            if( output.Contains(letter))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
