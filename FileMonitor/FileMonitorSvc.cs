using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using wwd_utils;

namespace FileMonitor {

    public partial class FileMonitorService : ServiceBase {
        const string SettingsFile = "FileMonitor.xml";
        const int FtpWorkerCount = 4;

        private TimerClass t; // Timer for file monitor
        private TimerClass x; // Timer for Processing.
        private TimerClass tftp; // Timer for ftp sites downloading.
        //        private System.Timers.Timer tSettings;

        public string SvcEventSource = "FileMonitor";
        public Logging lg;

        private bool Logit;

        private AppSettings Settings;

        private FileMonitorStatus fs;

        private String log_file_name = "";

        private int FtpInProgress = 0;

        Object MyLock = new Object();

        #region BackgroundWorkerArgs

        private class BackgroundArgs {
            public int id;        // id used to identify this worker
            public string status; // current status of worker
            public bool running;  // true if running
            public int NumberOfWorkers;

            public BackgroundArgs() {
                status = "Starting";
                running = false;
                NumberOfWorkers = 4;
            }
        }

        private BackgroundWorker[] FtpWorkers;

        #endregion BackgroundWorkerArgs

        #region Init

        public FileMonitorService() {
            InitializeComponent();
            Settings = new AppSettings(SettingsFile);

            InitFtpWorkers();

            fs = new FileMonitorStatus();
            fs.CurrentMessage = "Starting";

            MyInit();
            LoadSettings();
        }

        private void InitFtpWorkers() {
            FtpWorkers = new BackgroundWorker[FtpWorkerCount];

            for (int i = 0; (i < FtpWorkers.Count()); i++) {
                FtpWorkers[i] = new BackgroundWorker();
                FtpWorkers[i].WorkerReportsProgress = true;
                FtpWorkers[i].DoWork += FtpWorkerN_DoWork;
                FtpWorkers[i].RunWorkerCompleted += FtpWorkerN_RunWorkerCompleted;
            }
        }

        ~FileMonitorService() {
        }

        private void LoadSettings() {
            Settings.Load();
            Logit = Settings.Debug;
            if (Logit)
                lg.LogLevel = Logging.LoggingLevel.Verbose;
            else
                lg.LogLevel = Logging.LoggingLevel.None;
        }

        private void SaveSettings() {
            Settings.LogFileName = log_file_name;
            Settings.Save();
        }

        private void MyInit() {
            log_file_name = LogFileName();

            #region Logging Init

            lg = new Logging(log_file_name);
            lg.EventSource = SvcEventSource;
            switch (Settings.LogToDest) {
                case AppSettings.LogTo.Both:
                    lg.LoggingTo = Logging.LogTo.FileAndEventLog;
                    break;
                case AppSettings.LogTo.Event:
                    lg.LoggingTo = Logging.LogTo.EventLog;
                    break;
                case AppSettings.LogTo.File:
                    lg.LoggingTo = Logging.LogTo.File;
                    break;
                case AppSettings.LogTo.None:
                    lg.LoggingTo = Logging.LogTo.None;
                    break;
            }
            lg.LogLevel = Logging.LoggingLevel.Verbose;

            if (Logit) lg.LogMsg("Output File = " + lg.FileName);

            #endregion Logging Init

            #region Timers

            t = new TimerClass();
            t.TimeToRunWorker = false;
            t.IdleTime = 2 * 60 * 1000; // 2 minutes in ms (milliseconds)
            t.TimeUntilRun = 0;
            t.WorkerRunning = false;
            t.SleepTime = 1000;

            t.IdleTime = Convert.ToInt32(Settings.IdleTime);
            t.TimerEvent = t_ticker;
            t.Interval = t.SleepTime;
            t.Start();

            x = new TimerClass();
            x.TimeToRunWorker = false;
            x.IdleTime = 5 * 1000; // 5 seconds in ms (milliseconds)
            x.TimeUntilRun = 0;
            x.WorkerRunning = false;
            x.SleepTime = 1000;

            //            x.IdleTime = Convert.ToInt32(Settings.IdleTime);
            x.TimerEvent = x_ticker;
            x.Interval = x.SleepTime;
            x.Start();

            // Setup ftp timer.
            tftp = new TimerClass();
            tftp.TimeToRunWorker = false;
            tftp.IdleTime = 2 * 60 * 1000 + 10000;  // 2 minutes + 10 seconds.
            tftp.TimeUntilRun = 0;
            tftp.WorkerRunning = false;
            tftp.SleepTime = 1050;
            tftp.TimerEvent = tftp_ticker;
            tftp.Interval = tftp.SleepTime;
            tftp.Start();

            #endregion Timers

            if (Logit) lg.LogMsg("Init");
        }

        #endregion Init

        private string LogFileName() {
            string s;
            string t;

            t = DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString();
            s = System.Environment.GetEnvironmentVariable("TEMP").ToString() + "\\" + SvcEventSource + "." + t + ".txt";

            return s;
        }

        private string LogFileName(string Name) {
            string s;
            string t;

            t = DateTime.Now.Year.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Day.ToString();
            s = System.Environment.GetEnvironmentVariable("TEMP").ToString() + "\\" + Name + "." + t + ".txt";

            return s;
        }

        private void t_ticker() {
            //
            // Set log_file_name in case day has changed.
            //
            log_file_name = LogFileName();
            Settings.LogFileName = log_file_name;
            Settings.Save();

            lg.FileName = log_file_name;

            if (t.TimeToRunWorker) {
                StartBackgroundWorker(BWorker1);
                t.TimeToRunWorker = false;
                t.WorkerRunning = true;

                if (Logit) lg.LogMsg("Scanning for new and updated files");
                fs.CurrentMessage = "Scanning for new and updated files";
            }

            if (t.WorkerRunning) {
                t.TimeUntilRun = t.IdleTime;
                fs.CurrentMessage = "Scanning for new and updated files...";
            } else {
                if (t.TimeUntilRun > 0) {
                    t.TimeUntilRun = t.TimeUntilRun - t.SleepTime;
                } else {
                    fs.CurrentMessage = "Time to scan for files";
                    if (Logit) lg.LogMsg("Time to scan for files");
                    t.TimeToRunWorker = true;
                }
            }
        }

        private void x_ticker() {
            if (x.TimeToRunWorker) {
                StartBackgroundWorker(FileWorker);
                x.TimeToRunWorker = false;
                x.WorkerRunning = true;
                if (Logit) lg.LogMsg("Time to process a file");
                fs.CurrentMessage = "Time to process a file";
            }

            if (x.WorkerRunning) {
                x.TimeUntilRun = x.IdleTime;
                fs.CurrentMessage = "Processing File ...";
                if (Logit) lg.LogMsg("Processing File ...");
            } else {
                if (x.TimeUntilRun > 0) {
                    x.TimeUntilRun -= x.SleepTime;
                } else {
                    fs.CurrentMessage = "Sleep time over";
                    if (Logit) lg.LogMsg("Sleep time over");
                    x.TimeToRunWorker = true;
                }
            }
        }

        private void tftp_ticker() {
            if (tftp.TimeToRunWorker) {
                // Start background file transfer here.
                StartFTPWorkers();
                //                StartBackgroundWorker(FtpWorker);
                tftp.TimeToRunWorker = false;
                tftp.WorkerRunning = true;
                if (Logit) lg.LogMsg("Time for FTP!");
                fs.CurrentMessage = "Time for FTP!";
            }

            if (tftp.WorkerRunning) {
                tftp.TimeUntilRun = tftp.IdleTime;
            } else {
                if (tftp.TimeUntilRun > 0) {
                    tftp.TimeUntilRun -= tftp.SleepTime;
                } else {
                    tftp.TimeToRunWorker = true;
                }
            }
        }

        private void StartFTPWorkers() {
            // If we are not already running this process, then go ahead.
            if (FtpInProgress <= 0) {
                for (int i = 0; i < FtpWorkers.Count(); i++)
                    StartOneFtpWorker(FtpWorkers[i], i + 1);
            }
        }

        private void StartOneFtpWorker(BackgroundWorker w, int id) {
            BackgroundArgs a = new BackgroundArgs();
            a.id = id;
            a.NumberOfWorkers = FtpWorkers.Count();

            w.RunWorkerAsync(a);
        }

        public void StartBackgroundWorker(BackgroundWorker Wkr1) {
            Wkr1.RunWorkerAsync();
        }

        protected override void OnStart(string[] args) {
            base.OnStart(args);
            if (Logit) lg.LogMsg("Service Starting");

            LoadSettings();
            if (Logit) lg.LogMsg("Load Settings");

            SaveSettings();
            if (Logit) lg.LogMsg("Save Settings");
        }

        private void StopBackgroundWorker(BackgroundWorker W) {
            if (W.ToString() == "BWorker1") {
                while (t.WorkerRunning) {
                    Thread.Sleep(500);
                }
            }
            if (W.ToString() == "FileWorker") {
                while (x.WorkerRunning) {
                    Thread.Sleep(500);
                }
            }
            if (W.ToString().Substring(1, 3) == "Ftp") {
                while (x.WorkerRunning) {
                    Thread.Sleep(500);
                }
                // FtpInProgress--;
            }
        }

        protected override void OnStop() {
            if (Logit) lg.LogMsg("Service Stop");
            StopBackgroundWorker(BWorker1);
            StopBackgroundWorker(FileWorker);
            for (int i = 0; i < FtpWorkers.Count(); i++) {
                while (x.WorkerRunning) {
                    Thread.Sleep(500);
                }
            }
            base.OnStop();
        }

        protected override void OnContinue() {
            base.OnContinue();
            if (Logit) lg.LogMsg("Service Continue");
        }

        private void Logmsg(string Msg) {
            DateTime d = DateTime.Now;
            string s;
            //
            // http://msdn.microsoft.com/en-us/library/zdtaw1bw(VS.96).aspx
            // formats found in link
            //
            s = d.ToUniversalTime().ToString("yyyy.MM.dd.HH.mm.ss") + " : " + Msg;
        }

        private void BWorker1_DoWork(object sender, DoWorkEventArgs e) {
            AppSettings a = new AppSettings(SettingsFile);
            UpdateABBFileList abb = new UpdateABBFileList();

            abb.MessageLogger = lg.LogMsg;

            LoadSettings();

            if (Logit) lg.LogMsg("Updating file list");
            fs.CurrentMessage = "Updating file list";

            t.IdleTime = Convert.ToInt32(a.IdleTime);
            abb.Execute(a.StartDirectory);
            a = null;
            abb = null;
            GC.Collect();
        }

        private void BWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (Logit) lg.LogMsg("Updating file list Complete!");
            fs.CurrentMessage = "Updating file list Completed!";
            t.WorkerRunning = false;
        }

        private void BWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
        }

        private void FileWorker_DoWork(object sender, DoWorkEventArgs e) {
            AbbUtils a = new AbbUtils();
            a.MessageLogger = lg.LogMsg;
            LoadSettings();

            string FileGuid = a.GetFileThatNeedsProcessing();

            if (FileGuid != string.Empty) {
                if (Logit) lg.LogMsg("Process File:" + a.FileName);
                fs.CurrentFile = a.FileName;
                a.ProcessFile();
                // If we just processed a file, then find another one quickly
                x.IdleTime = 1000;
            } else {
                // If we did not find a file, then wait a while.
                x.IdleTime = 5 * 1000;
            }
            a = null;
            GC.Collect();
        }

        private void FileWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (Logit) lg.LogMsg("Process file complete!");
            fs.CurrentFile = string.Empty;
            x.WorkerRunning = false;
        }

        private void FtpWorker_DoWork(object sender, DoWorkEventArgs e) {
            bool OkToRun;
            OkToRun = false;

            lock (MyLock) {
                if (FtpInProgress <= 0)
                    OkToRun = true;
            }

            if (OkToRun) {
                ftpsites f = new ftpsites();

                f.ParentLogger = lg.LogMsg;

                lg.LogMsg("Downloading files via FTP");

                try {
                    f.NotifyFilesHaveChanged = UpdateFileRecord;
                    f.SelectSites();
                    f.GetFiles();
                    f.dq.DeleteAll();
                    f = null;
                } catch {
                    if (Logit)
                        lg.LogMsg("Error in GetFiles() ");
                }
            }
        }

        private void FtpWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (Logit) lg.LogMsg("Completed Downloading files via FTP");
            //FtpInProgress = false;
            tftp.WorkerRunning = false;
        }

        private void FtpWorkerN_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundArgs a;

            a = e.Argument as BackgroundArgs;

            bool OkToRun;
            OkToRun = false;

            lock (MyLock) {
                if (FtpInProgress < a.NumberOfWorkers) {
                    OkToRun = true;
                    FtpInProgress++;
                }
            }

            if (OkToRun) {
                ftpsites f = new ftpsites();

                f.ParentLogger = lg.LogMsg;

                lg.LogMsg("Downloading files via FTP");

                try {
                    f.NotifyFilesHaveChanged = UpdateFileRecord;
                    f.SelectSites(a.NumberOfWorkers, a.id);
                    f.GetFiles();
                    f.dq.DeleteAll();
                    f = null;
                } catch {
                    if (Logit)
                        lg.LogMsg("Error in GetFiles() ");
                }
            }
        }

        private void FtpWorkerN_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            //            BackgroundArgs a = e.Result as BackgroundArgs;
            if (Logit) lg.LogMsg("Completed Downloading files");
            lock (MyLock) {
                FtpInProgress--;
            }

            if (FtpInProgress <= 0)
                tftp.WorkerRunning = false;
        }

        private void FtpWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
        }

        private void UpdateFileRecord(string FullName) {
            MyFileInfo mfi = new MyFileInfo();

            if (FullName.Trim().Length > 0) {
                if (File.Exists(FullName)) {
                    AppSettings a = new AppSettings(SettingsFile);
                    UpdateABBFileList abb = new UpdateABBFileList();

                    abb.MessageLogger = lg.LogMsg;
                    //                    LoadSettings();

                    lg.LogMsg("Attempting to Update file record for: " + FullName);
                    mfi.FI = new FileInfo(FullName);
                    mfi.FileName = mfi.FI.Name;
                    mfi.Created = mfi.FI.LastWriteTime;
                    mfi.Size = mfi.FI.Length;
                    mfi.Created = mfi.FI.LastWriteTime;
                    mfi.Modified = mfi.FI.LastWriteTime;
                    mfi.fullpath = mfi.FI.FullName;

                    abb.UpdateFileInDB(mfi);

                    // private void UpdateFileInDB(MyFileInfo f) {
                    abb = null;
                    a = null;
                }
            }

            mfi = null;
        }
    }
}