using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ReadT00
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btn_go_Click(object sender, EventArgs e)
        {
            c_ReadT00 f = new c_ReadT00();
            Semaphore s = new Semaphore(0, 1);
            
            f.filename = txt_start.Text;
            f.connection_string = "Data Source=SQL-SVR\\MSSQLR2;Initial Catalog=abb;Integrated Security=True";
            f.process_file( ref s );
            txt1.AppendText("Inserted Records: = " + f.Inserted.ToString() + "\n");
            txt1.AppendText("Failed Records: = " + f.Failed.ToString() + "\n");

            //c_ReadD00 f = new c_ReadD00();
            //f.filename = txt_start.Text;
            //f.connection_string = ReadD00.Properties.Settings.Default.cs;
            //f.process_file();
            //txt1.AppendText("Inserted Records: = " + f.Inserted.ToString() + "\n");
            //txt1.AppendText("Failed Records: = " + f.Failed.ToString() + "\n");



        }
    }
}
