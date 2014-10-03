using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using wwd_utils;
using FileMonitorUI;
using db;
using System.Threading.Tasks;

namespace TestApp {
    public partial class Form2 : Form {
        public Form2() {
            InitializeComponent();
        }

        private void btnGetTime_Click(object sender, EventArgs e) {
            txtLocal.Text = DateTime.Now.ToString();
            txtUTC.Text = DateTime.UtcNow.ToString();
        }

        private void btnCompare_Click(object sender, EventArgs e) {
            DateTime convertedDate = DateTime.SpecifyKind(DateTime.Parse(txtUTC.Text), DateTimeKind.Utc);
            var kind = convertedDate.Kind;

            txtResult.Text = convertedDate.ToString() + ": " + kind.ToString() + ", " +
                convertedDate.ToLocalTime().ToString() + ": local";
        }

        private void button1_Click(object sender, EventArgs e) {
            DateTime start = DateTime.Now;
            DateTime end;
            SiteFiles sf = new SiteFiles();
            end = DateTime.Now;
            
            MessageBox.Show("SiteFiles Seconds: " + (end - start).TotalSeconds.ToString() );

        }

        private void button2_Click(object sender, EventArgs e) {
            Prefs f = new Prefs();
            f.Show();
        }

        private void button3_Click(object sender, EventArgs e) {
            UpdateABBFileList up = new UpdateABBFileList();
            const string SettingsFile = "FileMonitor.xml";
            AppSettings a = new AppSettings(SettingsFile);
//            DataClasses1DataContext dcc = new DataClasses1DataContext(a.ConnectionString);

            up.Execute(a.StartDirectory);
        }

        private void button4_Click(object sender, EventArgs e) {

        }
    }
}
