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

    public partial class Prefs : Form {
        private AppSettings AppSet;
        private string settings_file = "FileMonitor.xml";

        public Prefs() {
            InitializeComponent();
            AppSet = new AppSettings(settings_file);
            AppSet.Load();

            LoadPreferences();
        }

        private void LoadPreferences() {
            TxtServer.Text = AppSet.Server;
            TxtDataBase.Text = AppSet.Database;
            TxtUserName.Text = AppSet.UserName;
            TxtPassWord.Text = AppSet.Password;

            WindowsAuthChk.Checked = AppSet.WindowsAuthentication;
            DebugFlagChk.Checked = AppSet.Debug;

            txtStartDirectory.Text = AppSet.StartDirectory;
            txtIdleTime.Text = AppSet.IdleTimeMinutes.ToString();

            ChkLogFile.Checked = AppSet.LogToFile;
            ChkLogEvent.Checked = AppSet.LogToEventLog;

            chkCloseApp.Checked = (AppSet.UICloseMethod == AppSettings.CloseMethod.ExitApp);
        }

        private void SavePreferences() {
            AppSet.Server = TxtServer.Text;
            AppSet.Database = TxtDataBase.Text;
            AppSet.UserName = TxtUserName.Text;
            AppSet.Password = TxtPassWord.Text;
            AppSet.WindowsAuthentication = WindowsAuthChk.Checked;
            AppSet.Debug = DebugFlagChk.Checked;
            AppSet.StartDirectory = txtStartDirectory.Text;
            AppSet.IdleTimeMinutes = Convert.ToDouble(txtIdleTime.Text);
            AppSet.LogToEventLog = ChkLogEvent.Checked;
            AppSet.LogToFile = ChkLogFile.Checked;
            if (chkCloseApp.Checked)
                AppSet.UICloseMethod = AppSettings.CloseMethod.ExitApp;
            else
                AppSet.UICloseMethod = AppSettings.CloseMethod.MinToTray;

            AppSet.Save();
        }

        private void BtnSave_Click(object sender, EventArgs e) {
            SavePreferences();
        }

        private void BtnClose_Click(object sender, EventArgs e) {
            SavePreferences();
            Close();
        }

        private void label6_Click(object sender, EventArgs e) {
        }

        private void txtStartDirectory_DoubleClick(object sender, EventArgs e) {
            folderBrowserDialog1.ShowDialog();
        }

        private void Prefs_Load(object sender, EventArgs e) {
            statStripLbl1.Text = "Loading Page";
        }

        private void Prefs_Activated(object sender, EventArgs e) {
            statStripLbl1.Text = AppSet.FullFileName;
        }
    }
}