using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using wwd_utils;

namespace FileMonitorUI {

    public partial class FtpSites : Form {
        private ftpsites f;
        private ftpsite MyCopy;

        public FtpSites() {
            f = new ftpsites();

            InitializeComponent();
            bindcontrols();
        }

        #region Data Binding

        private void bindcontrols() {
            dataGridView1.DataSource = f.Sites;

            BindGridColumns(dataGridView1);

            BindControl(idTextBox, f.Sites, "id");
            BindControl(hostTextBox, f.Sites, "host");
            BindControl(localdirTextBox, f.Sites, "localdir");

            BindControl(remotedirTextBox, f.Sites, "remotedir");
            BindControl(maskTextBox, f.Sites, "mask");
            BindControl(userTextBox, f.Sites, "user");
            BindControl(passwordTextBox, f.Sites, "password");
        }

        private void BindGridColumns(DataGridView gd) {
            gd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            for (int i = 0; i < gd.Columns.Count; i++) {
                if (gd.Columns[i].Name != "host")
                    gd.Columns[i].Visible = false;
            }
            gd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void BindControl(TextBox MyControl, List<ftpsite> DataSrc, string FieldName) {
            if (MyControl.DataBindings.Count > 0)
                MyControl.DataBindings.Clear();
            MyControl.DataBindings.Add("Text", DataSrc, FieldName);
        }

        #endregion Data Binding

        private void btnCopy_Click(object sender, EventArgs e) {
            MyCopy = null;
            MyCopy = new ftpsite();
            string sid;

            sid = idTextBox.Text;

            MyCopy = f.GetSiteByID(sid);
        }

        private void btnPaste_Click(object sender, EventArgs e) {
            if (MyCopy != null) {
                hostTextBox.Text = MyCopy.host;
                localdirTextBox.Text = MyCopy.localdir;
                remotedirTextBox.Text = MyCopy.remotedir;
                userTextBox.Text = MyCopy.user;
                passwordTextBox.Text = MyCopy.password;
                maskTextBox.Text = MyCopy.mask;
            }
        }

        private void btnSaveSites_Click(object sender, EventArgs e) {
            f.Save();
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}