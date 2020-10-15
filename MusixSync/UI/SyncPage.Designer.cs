namespace MusixSync.UI
{
    partial class SyncPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSync = new System.Windows.Forms.Label();
            this.pnPage = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblSync
            // 
            this.lblSync.AutoSize = true;
            this.lblSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSync.Location = new System.Drawing.Point(4, 4);
            this.lblSync.Name = "lblSync";
            this.lblSync.Size = new System.Drawing.Size(60, 25);
            this.lblSync.TabIndex = 0;
            this.lblSync.Text = "Sync";
            // 
            // pnPage
            // 
            this.pnPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnPage.Location = new System.Drawing.Point(3, 32);
            this.pnPage.Name = "pnPage";
            this.pnPage.Size = new System.Drawing.Size(1094, 645);
            this.pnPage.TabIndex = 1;
            // 
            // SyncPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.pnPage);
            this.Controls.Add(this.lblSync);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.Name = "SyncPage";
            this.Size = new System.Drawing.Size(1100, 680);
            this.Load += new System.EventHandler(this.SyncPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSync;
        private System.Windows.Forms.Panel pnPage;
    }
}
