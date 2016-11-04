using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using ESN_Utilities;

namespace ESN_DB_Dumper
{
    public partial class frmImageDump : Form
    {
        public ESNBackupFile esnbu;
        public string filename;

        public frmImageDump()
        {
            InitializeComponent();
        }

        private void frmImageDump_Load(object sender, EventArgs e)
        {
            rtbUnpack.Text = this.Text = "\n<filepath>" + filename + "</filepath>";
            rtbUnpack.Text += "\n";
            esnbu = new ESNBackupFile(filename);
            rtbUnpack.Text += esnbu.GetDumper().Unpack();
            try
            {
                StreamWriter sw = new StreamWriter(filename + "_conv_.xml",false,Encoding.ASCII);
                sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\" ?>");
                sw.Write(rtbUnpack.Text);
                sw.Write("</xml>");
                sw.Close();
            }
            catch
            {
            }

        }
    }
}
