namespace FileMonitor {
    partial class FileMonitorService {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.BWorker1 = new System.ComponentModel.BackgroundWorker();
            this.FileWorker = new System.ComponentModel.BackgroundWorker();
            this.FtpWorker = new System.ComponentModel.BackgroundWorker();
            // 
            // BWorker1
            // 
            this.BWorker1.WorkerReportsProgress = true;
            this.BWorker1.WorkerSupportsCancellation = true;
            this.BWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BWorker1_DoWork);
            this.BWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BWorker1_ProgressChanged);
            this.BWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BWorker1_RunWorkerCompleted);
            // 
            // FileWorker
            // 
            this.FileWorker.WorkerReportsProgress = true;
            this.FileWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FileWorker_DoWork);
            this.FileWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FileWorker_RunWorkerCompleted);
            // 
            // FtpWorker
            // 
            this.FtpWorker.WorkerReportsProgress = true;
            this.FtpWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.FtpWorker_DoWork);
            this.FtpWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.FtpWorker_ProgressChanged);
            this.FtpWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FtpWorker_RunWorkerCompleted);
            // 
            // FileMonitorService
            // 
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.ServiceName = "FileMonitorService";

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BWorker1;
        private System.ComponentModel.BackgroundWorker FileWorker;
        private System.ComponentModel.BackgroundWorker FtpWorker;


    }
}
