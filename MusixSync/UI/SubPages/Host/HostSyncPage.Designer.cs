namespace MusixSync.UI.SubPages.Host
{
    partial class HostSyncPage
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
            this.lblSyncWith = new System.Windows.Forms.Label();
            this.pbSyncProgress = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.rtbConsole = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // lblSyncWith
            // 
            this.lblSyncWith.AutoSize = true;
            this.lblSyncWith.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSyncWith.Location = new System.Drawing.Point(4, 4);
            this.lblSyncWith.Name = "lblSyncWith";
            this.lblSyncWith.Size = new System.Drawing.Size(289, 25);
            this.lblSyncWith.TabIndex = 0;
            this.lblSyncWith.Text = "Syncing with: DEVICE NAME";
            // 
            // pbSyncProgress
            // 
            this.pbSyncProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSyncProgress.Location = new System.Drawing.Point(25, 538);
            this.pbSyncProgress.Name = "pbSyncProgress";
            this.pbSyncProgress.Size = new System.Drawing.Size(1035, 23);
            this.pbSyncProgress.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(21, 515);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(169, 20);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "STATUS MESSAGE...";
            // 
            // rtbConsole
            // 
            this.rtbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(43)))), ((int)(((byte)(47)))));
            this.rtbConsole.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbConsole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.rtbConsole.Location = new System.Drawing.Point(25, 32);
            this.rtbConsole.Name = "rtbConsole";
            this.rtbConsole.ReadOnly = true;
            this.rtbConsole.Size = new System.Drawing.Size(1035, 480);
            this.rtbConsole.TabIndex = 3;
            this.rtbConsole.Text = ">Con";
            // 
            // HostSyncPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.rtbConsole);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbSyncProgress);
            this.Controls.Add(this.lblSyncWith);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.Name = "HostSyncPage";
            this.Size = new System.Drawing.Size(1088, 575);
            this.Load += new System.EventHandler(this.HostSyncPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSyncWith;
        private System.Windows.Forms.ProgressBar pbSyncProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.RichTextBox rtbConsole;
    }
}
