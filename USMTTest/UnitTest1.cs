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
            string testFolder = "ThisIsATestFolder"; //folder name
            string testDrivePath = CreateDirectoryInCDrive(testFolder); //Creates the folder if it doesnt exist at C:\testFolder
            char driveLetter = '\0'; //Drive letter

            //checks if the drive with the path doesnt exist and adds it
            if (!DoesDrivePathExist(testFolder))
            {
                string networkDrivePath = testDrivePath.Remove(0, 3); // grabs path to file without C:
                networkDrivePath = string.Format(@"\\localhost\c$\{0}", networkDrivePath); //creates full network path for local mapping

                var main = new Main(); // create a new instance of 

                driveLetter = main.NetUseAdd(networkDrivePath, null, null);

                if(driveLetter != '1')
                {
                    
                    Assert.IsTrue(DoesDrivePathExist(networkDrivePath));
                    CMDNetUse(string.Format(@"use {0}: /delete", driveLetter));
                    DeleteDirectory(testDrivePath);
                }
                else
                {
                    //fails if the drive was unable to be added
                    Assert.IsFalse(true);
                }
                main.Dispose();
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
            Assert.IsFalse(IsDriveLetterMapped(driveLetter));
            main.Dispose();
        }

        //expect network drives to be removed and all application processes to stop
        //This test is dependent on Main.NetUseAdd working properly
        [TestMethod]
        public void TestCloseForm()
        {
            string testFolder = "ThisIsATestFolder"; //folder name
            string testDrivePath = CreateDirectoryInCDrive(testFolder); // creates a folder if it does not already exist
            string networkDrivePath = testDrivePath.Remove(0, 3); // grabs path to file without C:

            networkDrivePath = string.Format(@"\\localhost\c$\{0}", networkDrivePath); //creates full network path for local mapping

            var main = new Main();

            char driveLetter = main.NetUseAdd(networkDrivePath, null, null);

            if(IsDriveLetterMapped(driveLetter))
            {
                //closes the form calling the default and custom closing event handler
                main.Close();

                //closing event arguments for windows forms used to call Form_Closing
                FormClosingEventArgs closing = new FormClosingEventArgs(CloseReason.ApplicationExitCall, false);

                //deletes the the test directory created earlier
                DeleteDirectory(testDrivePath);

                //call closing event
                Main.Form_Closing(main, closing);

                //if the drives were removed test passed
                Assert.IsFalse(IsDriveLetterMapped(driveLetter));

                
            }
            else
            {
                Assert.Inconclusive("Unable to test because the drive is failing to map initially. Check that Main.NetUseAdd is working properly");
            }

            
        }

        //pass drive letter as Z: or X: ect. to test if the drive shows up as mapped
        public bool IsDriveLetterMapped(char letter)
        {
            
            string output = CMDNetUse("use");

            //checks if the drive letter is mapped or not
            if( output.Contains((letter.ToString() + ":")))
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
            if (Directory.Exists(dir)) { Directory.Delete(dir); }
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
