namespace Rsync_Copy
{
    partial class frmMain
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
            this.cmbFolder = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnBatch = new System.Windows.Forms.Button();
            this.btnDaemon = new System.Windows.Forms.Button();
            this.cmbRemoteHosts = new System.Windows.Forms.ComboBox();
            this.btnOptions = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbFolder
            // 
            this.cmbFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFolder.Location = new System.Drawing.Point(12, 12);
            this.cmbFolder.Name = "cmbFolder";
            this.cmbFolder.Size = new System.Drawing.Size(263, 21);
            this.cmbFolder.TabIndex = 0;
            this.cmbFolder.SelectedIndexChanged += new System.EventHandler(this.cmbFolder_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(281, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Folder to copy";
            // 
            // txtDestination
            // 
            this.txtDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
            this.txtDestination.Location = new System.Drawing.Point(12, 39);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(230, 20);
            this.txtDestination.TabIndex = 2;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(248, 37);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(281, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Destination folder";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(284, 61);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(87, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnBatch
            // 
            this.btnBatch.Location = new System.Drawing.Point(248, 61);
            this.btnBatch.Name = "btnBatch";
            this.btnBatch.Size = new System.Drawing.Size(25, 23);
            this.btnBatch.TabIndex = 7;
            this.btnBatch.Text = "B";
            this.btnBatch.UseVisualStyleBackColor = true;
            this.btnBatch.Click += new System.EventHandler(this.btnBatch_Click);
            // 
            // btnDaemon
            // 
            this.btnDaemon.Location = new System.Drawing.Point(217, 61);
            this.btnDaemon.Name = "btnDaemon";
            this.btnDaemon.Size = new System.Drawing.Size(25, 23);
            this.btnDaemon.TabIndex = 8;
            this.btnDaemon.Text = "D";
            this.btnDaemon.UseVisualStyleBackColor = true;
            this.btnDaemon.Click += new System.EventHandler(this.btnDaemon_Click);
            // 
            // cmbRemoteHosts
            // 
            this.cmbRemoteHosts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRemoteHosts.FormattingEnabled = true;
            this.cmbRemoteHosts.Location = new System.Drawing.Point(12, 63);
            this.cmbRemoteHosts.Name = "cmbRemoteHosts";
            this.cmbRemoteHosts.Size = new System.Drawing.Size(167, 21);
            this.cmbRemoteHosts.TabIndex = 9;
            this.cmbRemoteHosts.SelectedIndexChanged += new System.EventHandler(this.cmbRemoteHosts_SelectedIndexChanged);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(186, 61);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(25, 23);
            this.btnOptions.TabIndex = 10;
            this.btnOptions.Text = "O";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 92);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.cmbRemoteHosts);
            this.Controls.Add(this.btnDaemon);
            this.Controls.Add(this.btnBatch);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtDestination);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbFolder);
            this.Name = "frmMain";
            this.Text = "RCopy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnBatch;
        private System.Windows.Forms.Button btnDaemon;
        private System.Windows.Forms.ComboBox cmbRemoteHosts;
        private System.Windows.Forms.Button btnOptions;
    }
}

