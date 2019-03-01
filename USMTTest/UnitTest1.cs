using Microsoft.VisualStudio.TestTools.UnitTesting;
using migrationUSMT;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace migrationUSMTTest
{
    [TestClass]
    public class MainTest
    {

        [TestMethod]
        public void TestNetUseAdd()
        {
            string test = "ThisIsATestFolder";
            string testDrivePath = CreateDirectoryInCDrive(test);
            char driveLetter = '0';
            if (!DoesDrivePathExist(test))
            {
                testDrivePath = testDrivePath.Remove(0, 3);
                testDrivePath = string.Format(@"use * \\localhost\c$\{0}", testDrivePath);

                var main = new Main();

                driveLetter = main.NetUseAdd(testDrivePath, null, null);

                if(driveLetter != '1')
                {
                    Assert.IsTrue(DoesDrivePathExist(test));
                    CMDNetUse(string.Format(@"use {0}: /delete", driveLetter));
                }
            }
           
        }

        //creates a local folder and adds it as a mapped drive then deletes it using Main.NetUseDelete()
        //expect the drive to be deleted after being added
        [TestMethod]
        public void TestNetUseDelete()
        {

            string test = "ThisIsATestFolder";
            string testDrivePath = CreateDirectoryInCDrive(test);
            char driveLetter = '0';

            string argument = string.Format(@"use * {0}", testDrivePath);

            driveLetter = MapLocalFolder(testDrivePath);

            //gets the drive letter that was mapped to our local test folder
            if (driveLetter == '1')
            {
                Assert.Inconclusive("failed to map drive");
            }
           
            var main = new Main();

            //expect this to delete the drive we just mapped
            main.NetUseDelete(driveLetter);

            //delete the test folder
            DeleteDirectory(testDrivePath);

            //checks that we successfully removed the drive
            Assert.IsFalse(IsDriveLetterMapped(driveLetter.ToString() + ":"));
            
        }

        //expect network drives to close and all application processes to stop
        [TestMethod]
        public void TestCloseForm()
        {

        }

        //pass drive letter as Z: or X: ect. to test if the drive shows up as mapped
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

        //creates a directory if it does not already exist
        public string CreateDirectoryInCDrive(string dir)
        {
            if(!Directory.Exists(@"c:\" + dir)) { Directory.CreateDirectory(@"c:\" + dir); }
            return (@"c:\" + dir);
        }

        //deletes a directory if it exists
        public void DeleteDirectory(string dir)
        {
            if (!Directory.Exists(dir)) { Directory.Delete(dir); }
        }

        //maps a local folder to a drive letter
        public char MapLocalFolder(string path)
        {
            //if local directory exists map first available drive letter to it
            if (Directory.Exists(path))
            {
                path = path.Remove(0, 3);
                string argument = string.Format(@"use * \\localhost\c$\{0}", path);

                string output = CMDNetUse(argument);

                //gets the drive letter that was mapped to our local test folder
                if (output.Contains("successfully"))
                {
                    return output[6];
                }
                else
                {
                    return '1';
                }
            }
            else
            {
                return '1';
            }

        }

        public bool DoesDrivePathExist(string path)
        { 
            string output = CMDNetUse("use");
            
            if(output.Contains(path))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string CMDNetUse(string arg)
        {
            Process cmd = new Process();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.FileName = "net.exe";
            cmd.StartInfo.Arguments = arg;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();

            StreamReader reader = cmd.StandardOutput;
            string output = reader.ReadToEnd();

            return output;
        }

    }
}
