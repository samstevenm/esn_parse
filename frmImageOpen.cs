noteusing System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ESN_DB_Dumper
{
    public partial class frmImageOpen : Form
    {
        public frmImageOpen()
        {
            InitializeComponent();
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            if (ofdDatabaseImage.ShowDialog() == DialogResult.OK)
            {
                frmImageDump frm = new frmImageDump();
                frm.filename = ofdDatabaseImage.FileName;
                frm.Show();
            }
        }
    }
}
