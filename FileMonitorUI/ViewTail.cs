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
    public partial class ViewTail : Form {

        private long CurPos = 0;
        private long NewPos = 0;
        private System.IO.FileInfo fi;
        private System.IO.FileStream fs;
        private string Path;
        private string FileName;
        private string FullPath;

/*
        public ViewTail() {
            InitializeComponent();

            Path = "C:\\Windows\\ServiceProfiles\\NetworkService\\AppData\\Local\\Temp";
            FileName = "FileMonitor.txt";
            FullPath = Path + "\\" + FileName;

            ViewTailInit();
        }
*/

        public ViewTail(string File_Name) {
            InitializeComponent();

            FullPath = File_Name;
            System.IO.FileInfo fi = new System.IO.FileInfo(FullPath);

            Path = fi.DirectoryName;
            FileName = fi.Name;
            fi = null;

            ViewTailInit();
        }

        private void ViewTailInit() {
            fsw.EnableRaisingEvents = true;
            fsw.Filter = "*.*";

        }

        private void btn_close_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ViewTail_Load(object sender, EventArgs e) {
            fi = new System.IO.FileInfo(FullPath);
            fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
            CurPos = fi.Length;

            fs.Seek(CurPos, 0);

            fs.Close();
            fs = null;
            fi = null;

            fsw.Path = Path;
            fsw.NotifyFilter = 
                System.IO.NotifyFilters.LastWrite | 
                System.IO.NotifyFilters.CreationTime | 
                System.IO.NotifyFilters.LastAccess | 
                System.IO.NotifyFilters.Size;
        }

        private void fsw_Changed(object sender, System.IO.FileSystemEventArgs e) {

            txtMsg.Text = "File Updated at: " + System.DateTime.Now.ToString() + " / " + e.Name;
            if (e.Name == FileName) 
                dotail();
        }

        Boolean tail_paused = false;

        private void dotail() {
            if ( tail_paused == false ) 
            try {
                fi = new System.IO.FileInfo(FullPath);
                fs = new System.IO.FileStream(FullPath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                // CurPos = fi.Length;

                fs.Seek(CurPos, 0);
                NewPos = fi.Length;

                byte[] bytes = new byte[NewPos - CurPos];

                fs.Position = CurPos;
                fs.Read(bytes, 0, bytes.Length);

                txt1.Text = txt1.Text + Conversions.BytesToString(bytes);

                CurPos = NewPos;

                txt1.SelectionStart = txt1.Text.Length;
                txt1.ScrollToCaret();

            } catch {

            }
        }

        private void fsw_Created(object sender, System.IO.FileSystemEventArgs e) {
            txtMsg.Text = "File Created at: " + System.DateTime.Now.ToString() + " / " + e.Name;
            if (e.Name == "FileMonitor.txt")
                dotail();
        }

        private void fsw_Deleted(object sender, System.IO.FileSystemEventArgs e) {
            txtMsg.Text = "File Deleted at: " + System.DateTime.Now.ToString() + " / " + e.Name;
        }

        private void fsw_Renamed(object sender, System.IO.RenamedEventArgs e) {
            txtMsg.Text = "File Renamed at: " + System.DateTime.Now.ToString() + " / " + e.Name;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            dotail();
        }

        private void btn_pause_go_Click(object sender, EventArgs e) {
           System.Windows.Forms.Button b = sender as System.Windows.Forms.Button;
           if (b.Text.ToUpper().Contains("PAUSE")) {
               // change to go
               tail_paused = true;
               btn_pause_go.Text = "Go";
           } else {
               // change to pause
               tail_paused = false;
               btn_pause_go.Text = "Pause";
           }
        }

    }
}
