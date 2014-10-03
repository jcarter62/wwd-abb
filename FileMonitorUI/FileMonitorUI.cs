using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using net.sf.tailw;
using wwd_utils;

namespace FileMonitorUI {

    public partial class FileMonitorUI : Form {
        private Boolean CloseAllowed;
        private String PrefsTitle;
        const string ServiceName = "FileMonitor";

        public FileMonitorUI() {
            InitializeComponent();
            CloseAllowed = false;
            PrefsTitle = "Maintain Preferences";
        }

        #region Stuff Used to make App a Tray App

        private void FileMonitorUI_Resize(object sender, EventArgs e) {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e) {
            Show();
            WindowState = FormWindowState.Normal;
        }

        #endregion Stuff Used to make App a Tray App

        #region MainMenu Events

        private void menu_maintain_preferences_Click(object sender, EventArgs e) {
            if (!IsWindowOpen(PrefsTitle)) {
                Prefs f = new Prefs();
                f.MdiParent = this;
                f.Show();
            }
        }

        private void menu_maintain_truncateTables_Click(object sender, EventArgs e) {
            TruncateTables f = new TruncateTables();
            f.MdiParent = this;
            f.Show();
        }

        private void menu_file_exit_Click(object sender, EventArgs e) {
            CloseAllowed = true;
            this.Close();
        }

        #endregion MainMenu Events

        #region Context Menu

        private void context_pref_Click(object sender, EventArgs e) {
            Show();
            WindowState = FormWindowState.Normal;
            menu_maintain_preferences_Click(sender, e);
        }

        #endregion Context Menu

        #region Misc

        private void FileMonitorUI_FormClosing(object sender, FormClosingEventArgs e) {
            switch (e.CloseReason) {
                case CloseReason.ApplicationExitCall:
                    //Handle application exit call
                    break;
                case CloseReason.FormOwnerClosing:
                    //Handle Form owner close
                    break;
                case CloseReason.MdiFormClosing:
                    //Handle MDI parent closing
                    break;
                case CloseReason.None:
                    //Handle unknown reason
                    break;
                case CloseReason.TaskManagerClosing:
                    //Handle taskmanager close
                    break;
                case CloseReason.UserClosing:
                    // Check to see if user has changed CloseMethod ...
                    //
                    AppSettings a = new AppSettings(settings_file);

                    a.Load();
                    if (a.UICloseMethod == AppSettings.CloseMethod.ExitApp)
                        CloseAllowed = true;

                    //
                    if (CloseAllowed == false) {
                        // User clicked the upper right (x), and
                        // we will cancel the close & minimize the
                        // window.
                        e.Cancel = true;
                        Hide();
                    }
                    break;
                case CloseReason.WindowsShutDown:
                    //Handle system shutdown
                    break;
            }
        }

        private Boolean IsWindowOpen(String Title) {
            Boolean rvalue = false;
            int i;

            for (i = 0;
                i < this.MdiChildren.Count();
                i++) {
                if (this.MdiChildren[i].Text.ToUpper().Trim() == Title.ToUpper().Trim())
                    rvalue = true;
            }
            return rvalue;
        }

        #endregion Misc

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e) {
            menu_file_exit_Click(sender, e);
        }

        private void FileMonitorUI_Load(object sender, EventArgs e) {
        }

        private void serviceToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        //
        // run command via process
        // http://www.codeproject.com/KB/cs/wincmdline.aspx
        //
        private void StartStopServiceMenu_Click(object sender, EventArgs e) {
            string strCmdLine;
            //Declare and instantiate a new process component.

            System.Diagnostics.Process p;
            p = new System.Diagnostics.Process();

            //Do not receive an event when the process exits.

            p.EnableRaisingEvents = false;

            strCmdLine = "services.msc ";
            System.Diagnostics.Process.Start(strCmdLine);
            p.Close();
        }

        private void timer1_Tick(object sender, EventArgs e) {
            wwd_utils.FileMonitorStatus fs = new wwd_utils.FileMonitorStatus();

            fs.LoadStatus();
            StatText.Text = fs.CurrentTimeMessage;
            fs = null;
        }

        private AppSettings AppSet;
        private string settings_file = "FileMonitor.xml";

        private void logfileToolStripMenuItem_Click(object sender, EventArgs e) {
            AppSet = new AppSettings(settings_file);
            AppSet.Load();

            try {
                ViewTail t = new ViewTail(AppSet.LogFileName);
                t.MdiParent = this;
                t.Show();
            } catch (Exception err) {
                MessageBox.Show("Error:\n" + err.Message.ToString() + "\n-------------------\n" + err.StackTrace.ToString());
            }
        }

        private void tailToolStripMenuItem_Click(object sender, EventArgs e) {
            AppSet = new AppSettings(settings_file);
            AppSet.Load();
            try {
                net.sf.tailw.Settings s = new net.sf.tailw.Settings();
                s.Load();
                s.AgreementRead = true;
                s.RememberOpenedDocumentsOnExit = false;
                s.RememberHowToExit = true;
                s.Save();
                s = null;
            } catch {
            }

            try {
                net.sf.tailw.MainForm tf = new MainForm();
                tf.AskAtClose = false;
                tf.MdiParent = this;
                tf.Show();
                // Now open document
                DocumentDescription d = new DocumentDescription();
                d.Filename = AppSet.LogFileName;
                tf.OpenFile(d);
            } catch (Exception err) {
                MessageBox.Show("Error:\n" + err.Message.ToString() + "\n-------------------\n" + err.StackTrace.ToString());
            }
        }

        private void ftpSitesToolStripMenuItem_Click(object sender, EventArgs e) {
            FtpSites f = new FtpSites();

            f.MdiParent = this;
            f.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutBox1 f = new AboutBox1();
            f.MdiParent = this;
            f.Show();
        }
    }
}