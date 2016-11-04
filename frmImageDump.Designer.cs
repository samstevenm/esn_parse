namespace ESN_DB_Dumper
{
    partial class frmImageDump
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbUnpack = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtbUnpack
            // 
            this.rtbUnpack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbUnpack.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbUnpack.Location = new System.Drawing.Point(0, 0);
            this.rtbUnpack.Name = "rtbUnpack";
            this.rtbUnpack.ReadOnly = true;
            this.rtbUnpack.Size = new System.Drawing.Size(519, 276);
            this.rtbUnpack.TabIndex = 0;
            this.rtbUnpack.Text = "";
            // 
            // frmImageDump
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 276);
            this.Controls.Add(this.rtbUnpack);
            this.Name = "frmImageDump";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmImageDump_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbUnpack;
    }
}