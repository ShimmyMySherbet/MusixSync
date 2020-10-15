namespace MusixSync.UI.SubPages
{
    partial class SyncHome
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
            this.btnSyncTo = new System.Windows.Forms.Button();
            this.btnSyncFrom = new System.Windows.Forms.Button();
            this.splitC = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitC)).BeginInit();
            this.splitC.Panel1.SuspendLayout();
            this.splitC.Panel2.SuspendLayout();
            this.splitC.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSyncTo
            // 
            this.btnSyncTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncTo.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSyncTo.Location = new System.Drawing.Point(139, 163);
            this.btnSyncTo.Name = "btnSyncTo";
            this.btnSyncTo.Size = new System.Drawing.Size(218, 102);
            this.btnSyncTo.TabIndex = 0;
            this.btnSyncTo.Text = "Sync To Device";
            this.btnSyncTo.UseVisualStyleBackColor = true;
            this.btnSyncTo.Click += new System.EventHandler(this.btnSyncTo_Click);
            // 
            // btnSyncFrom
            // 
            this.btnSyncFrom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSyncFrom.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSyncFrom.Location = new System.Drawing.Point(165, 251);
            this.btnSyncFrom.Name = "btnSyncFrom";
            this.btnSyncFrom.Size = new System.Drawing.Size(218, 102);
            this.btnSyncFrom.TabIndex = 1;
            this.btnSyncFrom.Text = "Sync From Device";
            this.btnSyncFrom.UseVisualStyleBackColor = true;
            this.btnSyncFrom.Click += new System.EventHandler(this.btnSyncFrom_Click);
            // 
            // splitC
            // 
            this.splitC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitC.Location = new System.Drawing.Point(0, 0);
            this.splitC.Name = "splitC";
            // 
            // splitC.Panel1
            // 
            this.splitC.Panel1.Controls.Add(this.btnSyncTo);
            // 
            // splitC.Panel2
            // 
            this.splitC.Panel2.Controls.Add(this.btnSyncFrom);
            this.splitC.Size = new System.Drawing.Size(1094, 645);
            this.splitC.SplitterDistance = 600;
            this.splitC.TabIndex = 2;
            // 
            // SyncHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(47)))), ((int)(((byte)(51)))));
            this.Controls.Add(this.splitC);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(137)))), ((int)(((byte)(218)))));
            this.Name = "SyncHome";
            this.Size = new System.Drawing.Size(1094, 645);
            this.splitC.Panel1.ResumeLayout(false);
            this.splitC.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitC)).EndInit();
            this.splitC.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSyncTo;
        private System.Windows.Forms.Button btnSyncFrom;
        private System.Windows.Forms.SplitContainer splitC;
    }
}
