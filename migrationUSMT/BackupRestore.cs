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
    public partial class BackupRestore : Form
    {
        public BackupRestore()
        {
          
                InitializeComponent();

            this.FormClosing += Main.Form_Closing;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var doot = new USMTBackup();
            doot.Show();
            this.Hide();
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var restore = new USMTRestore();
            restore.Show();
            this.Hide();
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            

            

        }
    }
}
