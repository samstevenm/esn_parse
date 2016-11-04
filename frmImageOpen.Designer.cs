namespace ESN_DB_Dumper
{
    partial class frmImageOpen
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
            this.ofdDatabaseImage = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenImage = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOpenImage
            // 
            this.btnOpenImage.Location = new System.Drawing.Point(72, 29);
            this.btnOpenImage.Name = "btnOpenImage";
            this.btnOpenImage.Size = new System.Drawing.Size(75, 23);
            this.btnOpenImage.TabIndex = 0;
            this.btnOpenImage.Text = "Open Image";
            this.btnOpenImage.UseVisualStyleBackColor = true;
            this.btnOpenImage.Click += new System.EventHandler(this.btnOpenImage_Click);
            // 
            // frmImageOpen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 64);
            this.Controls.Add(this.btnOpenImage);
            this.Name = "frmImageOpen";
            this.Text = "Open Database Image";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdDatabaseImage;
        private System.Windows.Forms.Button btnOpenImage;
    }
}

