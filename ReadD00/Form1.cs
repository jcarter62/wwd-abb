using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Linq;

namespace ReadD00
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_go_Click(object sender, EventArgs e)
        {
            c_ReadD00 f = new c_ReadD00();
            f.filename = txt_start.Text;
            f.connection_string = ReadD00.Properties.Settings.Default.cs;
            f.process_file();
            txt1.AppendText("Inserted Records: = " + f.Inserted.ToString() + "\n");
            txt1.AppendText("Failed Records: = " + f.Failed.ToString() + "\n");
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
