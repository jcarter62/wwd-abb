using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileMonitor;
using wwd_utils;

namespace TestApp {

    public partial class TestWindow : Form {
        FileMonitorService fms;

        public TestWindow() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            fms = new FileMonitorService();
        }
    }
}