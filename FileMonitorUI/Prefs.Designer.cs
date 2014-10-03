namespace FileMonitorUI
{
    partial class Prefs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Prefs));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.WindowsAuthChk = new System.Windows.Forms.CheckBox();
            this.TxtPassWord = new System.Windows.Forms.TextBox();
            this.TxtUserName = new System.Windows.Forms.TextBox();
            this.TxtServer = new System.Windows.Forms.TextBox();
            this.TxtDataBase = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BtnClose = new System.Windows.Forms.Button();
            this.BtnSave = new System.Windows.Forms.Button();
            this.DebugFlagChk = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtStartDirectory = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.txtIdleTime = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ChkLogFile = new System.Windows.Forms.CheckBox();
            this.ChkLogEvent = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkCloseApp = new System.Windows.Forms.CheckBox();
            this.statStrip = new System.Windows.Forms.StatusStrip();
            this.statStripLbl1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(86, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SQL Server:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(125, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Windows Authentication:";
            // 
            // WindowsAuthChk
            // 
            this.WindowsAuthChk.AutoSize = true;
            this.WindowsAuthChk.Location = new System.Drawing.Point(157, 84);
            this.WindowsAuthChk.Name = "WindowsAuthChk";
            this.WindowsAuthChk.Size = new System.Drawing.Size(15, 14);
            this.WindowsAuthChk.TabIndex = 2;
            this.WindowsAuthChk.UseVisualStyleBackColor = true;
            // 
            // TxtPassWord
            // 
            this.TxtPassWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtPassWord.Location = new System.Drawing.Point(157, 130);
            this.TxtPassWord.Name = "TxtPassWord";
            this.TxtPassWord.Size = new System.Drawing.Size(245, 20);
            this.TxtPassWord.TabIndex = 4;
            // 
            // TxtUserName
            // 
            this.TxtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtUserName.Location = new System.Drawing.Point(157, 104);
            this.TxtUserName.Name = "TxtUserName";
            this.TxtUserName.Size = new System.Drawing.Size(245, 20);
            this.TxtUserName.TabIndex = 3;
            // 
            // TxtServer
            // 
            this.TxtServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtServer.Location = new System.Drawing.Point(157, 32);
            this.TxtServer.Name = "TxtServer";
            this.TxtServer.Size = new System.Drawing.Size(245, 20);
            this.TxtServer.TabIndex = 0;
            // 
            // TxtDataBase
            // 
            this.TxtDataBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtDataBase.Location = new System.Drawing.Point(157, 58);
            this.TxtDataBase.Name = "TxtDataBase";
            this.TxtDataBase.Size = new System.Drawing.Size(245, 20);
            this.TxtDataBase.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(95, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Database:";
            // 
            // BtnClose
            // 
            this.BtnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnClose.Location = new System.Drawing.Point(369, 273);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(75, 23);
            this.BtnClose.TabIndex = 12;
            this.BtnClose.Text = "&Close";
            this.BtnClose.UseVisualStyleBackColor = true;
            this.BtnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // BtnSave
            // 
            this.BtnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnSave.Location = new System.Drawing.Point(12, 273);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 23);
            this.BtnSave.TabIndex = 11;
            this.BtnSave.Text = "&Save";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // DebugFlagChk
            // 
            this.DebugFlagChk.AutoSize = true;
            this.DebugFlagChk.Location = new System.Drawing.Point(157, 160);
            this.DebugFlagChk.Name = "DebugFlagChk";
            this.DebugFlagChk.Size = new System.Drawing.Size(15, 14);
            this.DebugFlagChk.TabIndex = 5;
            this.DebugFlagChk.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(86, 160);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Debug Flag:";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // txtStartDirectory
            // 
            this.txtStartDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStartDirectory.Location = new System.Drawing.Point(157, 180);
            this.txtStartDirectory.Name = "txtStartDirectory";
            this.txtStartDirectory.Size = new System.Drawing.Size(245, 20);
            this.txtStartDirectory.TabIndex = 6;
            this.txtStartDirectory.DoubleClick += new System.EventHandler(this.txtStartDirectory_DoubleClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(74, 183);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Start Directory:";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select Start Directory";
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // txtIdleTime
            // 
            this.txtIdleTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIdleTime.Location = new System.Drawing.Point(157, 206);
            this.txtIdleTime.Name = "txtIdleTime";
            this.txtIdleTime.Size = new System.Drawing.Size(245, 20);
            this.txtIdleTime.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 209);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(133, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "File Monitor Idle Time(Min):";
            // 
            // ChkLogFile
            // 
            this.ChkLogFile.AutoSize = true;
            this.ChkLogFile.Location = new System.Drawing.Point(157, 232);
            this.ChkLogFile.Name = "ChkLogFile";
            this.ChkLogFile.Size = new System.Drawing.Size(42, 17);
            this.ChkLogFile.TabIndex = 8;
            this.ChkLogFile.Text = "File";
            this.ChkLogFile.UseVisualStyleBackColor = true;
            // 
            // ChkLogEvent
            // 
            this.ChkLogEvent.AutoSize = true;
            this.ChkLogEvent.Location = new System.Drawing.Point(243, 232);
            this.ChkLogEvent.Name = "ChkLogEvent";
            this.ChkLogEvent.Size = new System.Drawing.Size(75, 17);
            this.ChkLogEvent.TabIndex = 9;
            this.ChkLogEvent.Text = "Event Log";
            this.ChkLogEvent.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(108, 232);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Log to :";
            // 
            // chkCloseApp
            // 
            this.chkCloseApp.AutoSize = true;
            this.chkCloseApp.Location = new System.Drawing.Point(157, 255);
            this.chkCloseApp.Name = "chkCloseApp";
            this.chkCloseApp.Size = new System.Drawing.Size(132, 17);
            this.chkCloseApp.TabIndex = 10;
            this.chkCloseApp.Text = "Exit Closes Application";
            this.chkCloseApp.UseVisualStyleBackColor = true;
            // 
            // statStrip
            // 
            this.statStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statStripLbl1});
            this.statStrip.Location = new System.Drawing.Point(0, 299);
            this.statStrip.Name = "statStrip";
            this.statStrip.Size = new System.Drawing.Size(456, 22);
            this.statStrip.TabIndex = 19;
            this.statStrip.Text = "statusStrip1";
            // 
            // statStripLbl1
            // 
            this.statStripLbl1.Name = "statStripLbl1";
            this.statStripLbl1.Size = new System.Drawing.Size(10, 17);
            this.statStripLbl1.Text = " ";
            // 
            // Prefs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 321);
            this.Controls.Add(this.statStrip);
            this.Controls.Add(this.chkCloseApp);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ChkLogEvent);
            this.Controls.Add(this.ChkLogFile);
            this.Controls.Add(this.txtIdleTime);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtStartDirectory);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.DebugFlagChk);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BtnSave);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.TxtDataBase);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TxtServer);
            this.Controls.Add(this.TxtUserName);
            this.Controls.Add(this.TxtPassWord);
            this.Controls.Add(this.WindowsAuthChk);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Prefs";
            this.Text = "Maintain Preferences";
            this.Activated += new System.EventHandler(this.Prefs_Activated);
            this.Load += new System.EventHandler(this.Prefs_Load);
            this.statStrip.ResumeLayout(false);
            this.statStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox WindowsAuthChk;
        private System.Windows.Forms.TextBox TxtPassWord;
        private System.Windows.Forms.TextBox TxtUserName;
        private System.Windows.Forms.TextBox TxtServer;
        private System.Windows.Forms.TextBox TxtDataBase;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BtnClose;
        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.CheckBox DebugFlagChk;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtStartDirectory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox txtIdleTime;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox ChkLogFile;
        private System.Windows.Forms.CheckBox ChkLogEvent;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkCloseApp;
        private System.Windows.Forms.StatusStrip statStrip;
        private System.Windows.Forms.ToolStripStatusLabel statStripLbl1;
    }
}