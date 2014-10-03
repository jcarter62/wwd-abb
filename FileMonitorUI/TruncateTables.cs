using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using wwd_utils;
using db;

namespace FileMonitorUI {

    public partial class TruncateTables : Form {
        // Private Variables
        private static String cs;
        public DataClasses1DataContext dcc;

        // Constructor
        public TruncateTables() {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void BtnTruncate_Click(object sender, EventArgs e) {

            TruncateAllTables();
        }

        private void TruncateAllTables() {
            dbutils dbu = new dbutils("FileMonitor.xml");

            cs = dbu.BuildConnectionString();
            dcc = new DataClasses1DataContext(cs);

            dbu.Truncate_Table(dbutils.TableName.Tbl_File, ref dcc);
            dbu.Truncate_Table(dbutils.TableName.Tbl_Total_Readings , ref dcc);
            dbu.Truncate_Table(dbutils.TableName.Tbl_TReading, ref dcc);

            MessageBox.Show("Completed!");
        }

    }
}
