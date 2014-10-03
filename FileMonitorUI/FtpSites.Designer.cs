namespace FileMonitorUI {
    partial class FtpSites {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.Label passwordLabel;
            System.Windows.Forms.Label userLabel;
            System.Windows.Forms.Label maskLabel;
            System.Windows.Forms.Label remotedirLabel;
            System.Windows.Forms.Label localdirLabel;
            System.Windows.Forms.Label idLabel;
            System.Windows.Forms.Label hostLabel;
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSaveSites = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.maskTextBox = new System.Windows.Forms.TextBox();
            this.remotedirTextBox = new System.Windows.Forms.TextBox();
            this.localdirTextBox = new System.Windows.Forms.TextBox();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            passwordLabel = new System.Windows.Forms.Label();
            userLabel = new System.Windows.Forms.Label();
            maskLabel = new System.Windows.Forms.Label();
            remotedirLabel = new System.Windows.Forms.Label();
            localdirLabel = new System.Windows.Forms.Label();
            idLabel = new System.Windows.Forms.Label();
            hostLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new System.Drawing.Point(16, 200);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new System.Drawing.Size(55, 13);
            passwordLabel.TabIndex = 53;
            passwordLabel.Text = "password:";
            // 
            // userLabel
            // 
            userLabel.AutoSize = true;
            userLabel.Location = new System.Drawing.Point(41, 174);
            userLabel.Name = "userLabel";
            userLabel.Size = new System.Drawing.Size(30, 13);
            userLabel.TabIndex = 52;
            userLabel.Text = "user:";
            // 
            // maskLabel
            // 
            maskLabel.AutoSize = true;
            maskLabel.Location = new System.Drawing.Point(36, 148);
            maskLabel.Name = "maskLabel";
            maskLabel.Size = new System.Drawing.Size(35, 13);
            maskLabel.TabIndex = 51;
            maskLabel.Text = "mask:";
            // 
            // remotedirLabel
            // 
            remotedirLabel.AutoSize = true;
            remotedirLabel.Location = new System.Drawing.Point(18, 122);
            remotedirLabel.Name = "remotedirLabel";
            remotedirLabel.Size = new System.Drawing.Size(53, 13);
            remotedirLabel.TabIndex = 50;
            remotedirLabel.Text = "remotedir:";
            // 
            // localdirLabel
            // 
            localdirLabel.AutoSize = true;
            localdirLabel.Location = new System.Drawing.Point(28, 96);
            localdirLabel.Name = "localdirLabel";
            localdirLabel.Size = new System.Drawing.Size(43, 13);
            localdirLabel.TabIndex = 49;
            localdirLabel.Text = "localdir:";
            // 
            // idLabel
            // 
            idLabel.AutoSize = true;
            idLabel.Location = new System.Drawing.Point(53, 44);
            idLabel.Name = "idLabel";
            idLabel.Size = new System.Drawing.Size(18, 13);
            idLabel.TabIndex = 48;
            idLabel.Text = "id:";
            // 
            // hostLabel
            // 
            hostLabel.AutoSize = true;
            hostLabel.Location = new System.Drawing.Point(41, 70);
            hostLabel.Name = "hostLabel";
            hostLabel.Size = new System.Drawing.Size(30, 13);
            hostLabel.TabIndex = 47;
            hostLabel.Text = "host:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(346, 321);
            this.panel1.TabIndex = 36;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(346, 321);
            this.dataGridView1.TabIndex = 24;
            // 
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "id";
            this.id.Name = "id";
            this.id.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSaveSites);
            this.panel2.Controls.Add(this.btnPaste);
            this.panel2.Controls.Add(this.btnCopy);
            this.panel2.Controls.Add(passwordLabel);
            this.panel2.Controls.Add(this.passwordTextBox);
            this.panel2.Controls.Add(userLabel);
            this.panel2.Controls.Add(this.userTextBox);
            this.panel2.Controls.Add(maskLabel);
            this.panel2.Controls.Add(this.maskTextBox);
            this.panel2.Controls.Add(remotedirLabel);
            this.panel2.Controls.Add(this.remotedirTextBox);
            this.panel2.Controls.Add(localdirLabel);
            this.panel2.Controls.Add(this.localdirTextBox);
            this.panel2.Controls.Add(idLabel);
            this.panel2.Controls.Add(this.idTextBox);
            this.panel2.Controls.Add(hostLabel);
            this.panel2.Controls.Add(this.hostTextBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(346, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(555, 321);
            this.panel2.TabIndex = 37;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(458, 190);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(93, 23);
            this.btnClose.TabIndex = 44;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSaveSites
            // 
            this.btnSaveSites.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSites.Location = new System.Drawing.Point(458, 148);
            this.btnSaveSites.Name = "btnSaveSites";
            this.btnSaveSites.Size = new System.Drawing.Size(93, 23);
            this.btnSaveSites.TabIndex = 43;
            this.btnSaveSites.Text = "Save Sites";
            this.btnSaveSites.UseVisualStyleBackColor = true;
            this.btnSaveSites.Click += new System.EventHandler(this.btnSaveSites_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPaste.Location = new System.Drawing.Point(458, 86);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(93, 23);
            this.btnPaste.TabIndex = 46;
            this.btnPaste.Text = "Paste";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(458, 41);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(93, 23);
            this.btnCopy.TabIndex = 45;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.passwordTextBox.Location = new System.Drawing.Point(75, 197);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(368, 20);
            this.passwordTextBox.TabIndex = 42;
            // 
            // userTextBox
            // 
            this.userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userTextBox.Location = new System.Drawing.Point(75, 171);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(368, 20);
            this.userTextBox.TabIndex = 41;
            // 
            // maskTextBox
            // 
            this.maskTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.maskTextBox.Location = new System.Drawing.Point(75, 145);
            this.maskTextBox.Name = "maskTextBox";
            this.maskTextBox.Size = new System.Drawing.Size(368, 20);
            this.maskTextBox.TabIndex = 40;
            // 
            // remotedirTextBox
            // 
            this.remotedirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.remotedirTextBox.Location = new System.Drawing.Point(75, 119);
            this.remotedirTextBox.Name = "remotedirTextBox";
            this.remotedirTextBox.Size = new System.Drawing.Size(368, 20);
            this.remotedirTextBox.TabIndex = 39;
            // 
            // localdirTextBox
            // 
            this.localdirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localdirTextBox.Location = new System.Drawing.Point(75, 93);
            this.localdirTextBox.Name = "localdirTextBox";
            this.localdirTextBox.Size = new System.Drawing.Size(368, 20);
            this.localdirTextBox.TabIndex = 38;
            // 
            // idTextBox
            // 
            this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.idTextBox.Location = new System.Drawing.Point(75, 41);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.Size = new System.Drawing.Size(368, 20);
            this.idTextBox.TabIndex = 36;
            // 
            // hostTextBox
            // 
            this.hostTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hostTextBox.Location = new System.Drawing.Point(75, 67);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(368, 20);
            this.hostTextBox.TabIndex = 37;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(346, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 321);
            this.splitter1.TabIndex = 38;
            this.splitter1.TabStop = false;
            // 
            // FtpSites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 321);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FtpSites";
            this.Text = "FtpSites";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSaveSites;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.TextBox maskTextBox;
        private System.Windows.Forms.TextBox remotedirTextBox;
        private System.Windows.Forms.TextBox localdirTextBox;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.Splitter splitter1;

    }
}