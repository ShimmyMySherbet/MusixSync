namespace MusixSync.UI
{
    partial class SyncEntityControl
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
            this.components = new System.ComponentModel.Container();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.CMSMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnDenySync = new System.Windows.Forms.ToolStripMenuItem();
            this.CMSMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeviceName.Location = new System.Drawing.Point(4, 0);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(156, 25);
            this.lblDeviceName.TabIndex = 0;
            this.lblDeviceName.Text = "DEVICE NAME";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddress.Location = new System.Drawing.Point(37, 29);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(120, 24);
            this.lblAddress.TabIndex = 1;
            this.lblAddress.Text = "IP ADDRESS";
            // 
            // btnSync
            // 
            this.btnSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSync.Location = new System.Drawing.Point(989, 44);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(94, 40);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // CMSMain
            // 
            this.CMSMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDenySync});
            this.CMSMain.Name = "CMSMain";
            this.CMSMain.Size = new System.Drawing.Size(130, 26);
            // 
            // btnDenySync
            // 
            this.btnDenySync.Name = "btnDenySync";
            this.btnDenySync.Size = new System.Drawing.Size(180, 22);
            this.btnDenySync.Text = "Deny Sync";
            this.btnDenySync.Click += new System.EventHandler(this.btnDenySync_Click);
            // 
            // SyncEntityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ContextMenuStrip = this.CMSMain;
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblDeviceName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.Name = "SyncEntityControl";
            this.Size = new System.Drawing.Size(1086, 87);
            this.CMSMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.ContextMenuStrip CMSMain;
        private System.Windows.Forms.ToolStripMenuItem btnDenySync;
    }
}
