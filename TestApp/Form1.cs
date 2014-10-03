using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileMonitorUI;

namespace TestApp {

    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void prefsToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowPrefs();
        }

        private void ShowPrefs() {
            FileMonitorUI.Prefs f = new FileMonitorUI.Prefs();
            f.MdiParent = this;
            f.Show();
        }

        private void testWinToolStripMenuItem_Click(object sender, EventArgs e) {
            TestWindow f = new TestWindow();
            f.MdiParent = this;
            f.Show();
        }

        private void form2ToolStripMenuItem_Click(object sender, EventArgs e) {
            Form2 f = new Form2();
            f.MdiParent = this;
            f.Show();
        }
    }
}