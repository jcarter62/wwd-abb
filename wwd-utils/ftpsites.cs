using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using EnterpriseDT.Net.Ftp;

namespace wwd_utils {
    //
    // defined in wwdClasses.cs
    //
    // public delegate void LogMessager(string Msg);

    /// <summary>
    ///
    /// </summary>
    /// <param name="FileName"></param>
    public delegate void NotifyFilesChanged(string FileName);

    public delegate void DQAdd(string FileNameOrDirectory, string ItemType);

    public class DeleteQueueItem {
        public string ItemType;
        public string Path;
    }

    public class DeleteQueue {
        List<DeleteQueueItem> Files;

        public LogMessager ParentLogger {
            set { _ParentLogger = value; }
            get { return _ParentLogger; }
        }

        private LogMessager _ParentLogger;

        // LogMsg
        private void LogMsg(string MsgStr) {
            if (_ParentLogger == null)
                System.Console.WriteLine(MsgStr);
            else
                _ParentLogger(MsgStr);
        }

        public DeleteQueue() {
            Files = new List<DeleteQueueItem>();
        }

        ~DeleteQueue() {
            DeleteAll();
        }

        public void Add(string FileNameOrDirectory, string DirOrFile) {
            string msg;
            DeleteQueueItem q = new DeleteQueueItem();
            q.ItemType = DirOrFile;
            q.Path = FileNameOrDirectory;

            msg = string.Format("ADD {0}, {1} to delete queue", q.ItemType, q.Path);
            LogMsg(msg);

            Files.Insert(0, q);
            //            Files.Add(q);
        }

        public void DeleteAll() {
            string msg;

            msg = string.Format("DeleteAll {0} Items", Files.Count);
            LogMsg(msg);
            // delete files
            foreach (DeleteQueueItem q in Files) {
                try {
                    if (q.ItemType == "File") {
                        msg = string.Format("DEL {0}: {1}", q.ItemType, q.Path);
                        LogMsg(msg);
                        System.IO.File.Delete(q.Path);
                    }
                } catch {
                    msg = string.Format("FAIL DEL {0}: {1}", q.ItemType, q.Path);
                    LogMsg(msg);
                }
            }
            // delete directories
            foreach (DeleteQueueItem q in Files) {
                try {
                    if (q.ItemType != "File") {
                        msg = string.Format("DEL {0}: {1}", q.ItemType, q.Path);
                        LogMsg(msg);
                        System.IO.Directory.Delete(q.Path, true);
                    }
                } catch {
                    msg = string.Format("FAIL DEL {0}: {1}", q.ItemType, q.Path);
                    LogMsg(msg);
                }
            }
            // clear queue
            Files.Clear();
        }
    }

    /// <summary>
    /// List of sites described in xml settings file.
    /// </summary>
    public class ftpsites {

        /// <summary>
        /// List of sites
        /// </summary>
        public List<ftpsite> Sites;

        public List<qclass> Q;

        public DeleteQueue dq = new DeleteQueue();

        public string Session { get; set; }

        /// <summary>
        /// Log File Name
        /// </summary>
        public string LogFile;

        /// <summary>
        /// The last message logged.
        /// </summary>
        public string LastLogMessage {
            get;
            set;
        }

        private SettingsFile sf;
        private string filename;

        private NotifyFilesChanged NFC;

        public NotifyFilesChanged NotifyFilesHaveChanged {
            set {
                NFC = value;
                foreach (ftpsite x in Sites) {
                    x.NotifyFileHasChanged = value;
                }
            }
        }

        public LogMessager ParentLogger {
            set {
                _ParentLogger = value;
                if (dq != null) {
                    dq.ParentLogger = value;
                }
            }
            get { return _ParentLogger; }
        }

        private LogMessager _ParentLogger;

        // LogMsg
        public void LogMsg(string MsgStr) {
            if (_ParentLogger == null)
                System.Console.WriteLine(MsgStr);
            else
                _ParentLogger(MsgStr);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ftpsites() {
            _ParentLogger = null;

            Initftpsites();
        }

        private void Initftpsites() {
            filename = "abbrecorders.xml";
            sf = new SettingsFile(filename);
            sf.CompanyName = "WWD";
            Sites = new List<ftpsite>();
            Session = Guid.NewGuid().ToString();
            Load();

            // Select all Sites
            for (int i = 0; i < Sites.Count - 1; i++) {
                Sites[i].Selected = true;
            }
        }

        /// <summary>
        /// Load all the ftp sites from xml data file.
        /// </summary>
        public void Load() {
            LoadSitesIntoQueue();
        }

        /// <summary>
        /// SelectSites:
        /// </summary>
        /// <param name="NumberOfSplits">This is the number of times you wish to split the sites.  If this value is 0, then all sites are selected.</param>
        /// <param name="SplitNumber">Identifies which split you wish to select.</param>
        public void SelectSites(int NumberOfSplits, int SplitNumber) {
            int TotalQty = LastSite(Sites);
            int SplitQty = (TotalQty / NumberOfSplits) + 1;
            int start, end;

            if (NumberOfSplits == 0) {
                start = 0;
                end = TotalQty;
            } else {
                start = SplitQty * (SplitNumber - 1);
                end = start + SplitQty;
                if (end > TotalQty)
                    end = TotalQty;
            }

            // Clear all selections
            foreach (ftpsite f in Sites)
                f.Selected = false;

            // Select only the site we wish to include
            for (int i = start; i < end; i++) {
                Sites[i].Selected = true;
            }
        }

        private void printselected() {
            for (int i = 0; i < Sites.Count; i++) {
                string x;
                x = string.Format("Host[{0}]={1} Selected={2}", i, Sites[i].host, Sites[i].Selected.ToString());
                LogMsg(x);
            }
        }

        /// <summary>
        /// Determine the highest index of a non-empty site.
        /// </summary>
        /// <param name="SiteList"></param>
        /// <returns></returns>
        private int LastSite(List<ftpsite> SiteList) {
            int i;
            int MaxSite = 0;

            for (i = 0; i < SiteList.Count; i++) {
                if (SiteList[i].host.Trim().Length > 2)
                    MaxSite = i + 1;
            }

            return MaxSite;
        }

        /// <summary>
        /// Select ALL sites.
        /// </summary>
        public void SelectSites() {
            SelectSites(0, 0);
        }

        /// <summary>
        /// Store all ftp sites to the xml data file.
        /// </summary>
        public void Save() {
            int i;

            for (i = 0; i < 200; i++) {
                SaveSite(i, Sites[i]);
            }
        }

        private void SaveSite(int i, ftpsite onesite) {
            string s;
            string myid;

            myid = "Site" + i.ToString();

            s = onesite.ToString();
            sf.WriteString(myid, s);
        }

        private ftpsite LoadSite(string istr) {
            ftpsite f = new ftpsite();

            string s = sf.ReadString(istr, "");

            string[] a;

            try {
                a = s.Split(f.sep);

                f.id = a[0];
                f.host = a[1];
                f.user = Encryption.Decrypt(a[2]);
                f.password = Encryption.Decrypt(a[3]);
                f.localdir = a[4];
                f.remotedir = a[5];
                f.mask = a[6];
                f.Session = this.Session;
            } catch {
                f.id = Guid.NewGuid().ToString();
                f.host = "";
                f.user = "";
                f.password = "";
                f.localdir = "";
                f.remotedir = "";
                f.mask = "";
                f.Session = this.Session;
            }

            return f;
        }

        /// <summary>
        /// Find the ftpsite record by guid/site id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ftpsite GetSiteByID(string id) {
            ftpsite f;

            f = null;
            foreach (ftpsite fs in Sites) {
                if (fs.id == id) {
                    f = fs;
                    break;
                }
            }

            return f;
        }

        /// <summary>
        /// Download all files matching mask from each site.
        /// </summary>
        public void GetFiles() {
            LogMsg("Get Files - Started");

            for (int i = 0; i < 200; i++) {
                if (Sites[i].Selected) {
                    if (Sites[i].host.Trim().Length > 0) {
                        LogMsg("Checking Site: " + Sites[i].host);

                        Sites[i].ParentLogger = LogMsg;
                        Sites[i].DeleteQueue = dq.Add;
                        Sites[i].GetFiles();
                    }
                }
            }
            LogMsg("Get Files - Completed");
        }

        #region ftp queue

        public void GetFilesAsync(int NumOfWorkers) {
            bool AreWeDone = false;
            int NumOfWorkersRunning = 0;
            int i;
            DateTime MaxTime;

            MaxTime = DateTime.Now.AddMinutes(20);

            LoadSitesIntoQueue();
            QueueAllSites();

            while (!AreWeDone) {
                if (DateTime.Now > MaxTime)
                    AreWeDone = true;

                if (!IsQueueEmpty()) {
                    // Is there a worker slot available ?
                    NumOfWorkersRunning = GetRunningWorkerCount();

                    if (NumOfWorkers > NumOfWorkersRunning) {
                        i = GetQueueItemToRun();
                        RunQueueItem(i);
                    }
                } else {
                    AreWeDone = true;
                }

                if (!AreWeDone) {
                    TakeANap();
                }
            }
        }

        private void RunQueueItem(int i) {
            try {
                //                Q[i].worker.Site.Name = "Queue:" + i.ToString();
                Q[i].worker.DoWork += Worker_DoWork;                        // new DoWorkEventHandler(Worker_DoWork);
                Q[i].worker.RunWorkerCompleted += Worker_Completed;         // new RunWorkerCompletedEventHandler(Worker_Completed);
                Q[i].worker.RunWorkerAsync(Q[i]);
            } catch { }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e) {
            int i;
            qclass qc;
            ftpsite mysite;

            BackgroundWorker worker = sender as BackgroundWorker;
            qc = (qclass)e.Argument;
            i = qc.id;
            Q[i].State = qstate.Running;

            mysite = Sites[i];

            if (mysite.GetFiles()) {
                // ok
                qc.State = qstate.RunOK;
            } else {
                // failed
                qc.State = qstate.RunFailed;
            }
            e.Result = qc;
        }

        private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e) {
            int i;
            qclass qc;

            BackgroundWorker worker = sender as BackgroundWorker;
            qc = (qclass)e.Result;
            i = qc.id;
            switch (qc.State) {
                case qstate.RunOK:
                    Q[i].State = qstate.Completed;
                    break;
                default:
                    //                    Q[i].State = qstate.Waiting;
                    Q[i].State = qstate.CompletedWithError;
                    break;
            }
            //
            // Remove event calls for this queue item.
            //
            Q[i].worker.DoWork -= Worker_DoWork;                        // new DoWorkEventHandler(Worker_DoWork);
            Q[i].worker.RunWorkerCompleted -= Worker_Completed;         // new RunWorkerCompletedEventHandler(Worker_Completed);
        }

        private void AsyncWorker_DoWork(object sender, DoWorkEventArgs e) {
            qclass q;
            q = (qclass)e.Argument;

            Sites[q.id].GetFiles();
        }

        private int GetRunningWorkerCount() {
            int runningCount = 0;

            foreach (qclass q in Q) {
                if (q.State == qstate.Running) {
                    runningCount++;
                }
            }
            return runningCount;
        }

        private int GetQueueItemToRun() {
            foreach (qclass q in Q) {
                if (q.State == qstate.Waiting) {
                    return q.id;
                }
            }
            return 0;
        }

        // Determine if we have any queue items waiting.
        private bool IsQueueEmpty() {
            foreach (qclass q in Q) {
                if (q.State == qstate.Waiting) {
                    return false;
                }
            }
            return true;
        }

        // Set all sites in queue to "Waiting"
        private void QueueAllSites() {
            for (int i = 0; i < Q.Count; i++) {
                if (Sites[i].host.Trim().Length > 0)
                    Q[i].State = qstate.Waiting;
            }
        }

        private void LoadSitesIntoQueue() {
            qclass oneq;

            if (Q == null) {
                Q = new List<qclass>();
            } else {
                foreach (qclass q in Q) {
                    if (q.worker == null) {
                        q.worker = new BackgroundWorker();
                        q.id = 0;
                        q.State = qstate.NoEntry;
                    }
                }
            }

            for (int i = 0; i < 200; i++) {
                ftpsite f = new ftpsite();

                string istr = "Site" + i.ToString();
                f = LoadSite(istr);
                Sites.Add(f);

                if (Q.Count < i) {
                    oneq = new qclass(i);
                    Q.Add(oneq);
                }
            }

            // Now set the queue entries.
            // to NoEntry for any entries where the host is empty.
            foreach (qclass q in Q) {
                int i = q.id;
                if (Sites[i].host.Trim().Length <= 0)
                    q.State = qstate.NoEntry;
                else
                    q.State = qstate.NoState;
            }
        }

        private void TakeANap() {
            System.Threading.Thread.Sleep(1000);
        }

        #endregion ftp queue
    }

    /// <summary>
    /// ftpsite
    /// </summary>
    public class ftpsite : INotifyPropertyChanged {
        private string _id;
        private string _host;
        private string _user;
        private string _password;
        private string _localdir;
        private string _remotedir;
        private string _mask;

        public string Session { get; set; }

        public bool Selected { get; set; }

        public LogMessager ParentLogger {
            set { _ParentLogger = value; }
            get { return _ParentLogger; }
        }

        private LogMessager _ParentLogger;

        private void LogMsg(string MsgStr) {
            if (_ParentLogger == null)
                System.Console.WriteLine(MsgStr);
            else
                _ParentLogger(MsgStr);
        }

        public DQAdd DeleteQueue {
            set { _DeleteQueue = value; }
            get { return _DeleteQueue; }
        }

        private DQAdd _DeleteQueue;

        private void AddToDeleteQueue(string ItemPath, string ItemType) {
            if (_DeleteQueue == null)
                LogMsg("Unable to add " + ItemPath + " to delete queue.");
            else {
                _DeleteQueue(ItemPath, ItemType);
            }
        }

        private NotifyFilesChanged NFC;

        public NotifyFilesChanged NotifyFileHasChanged {
            set { NFC = value; }
        }

        #region Properties

        public string id {
            get { return _id; }
            set {
                _id = value;
                this.NotifyPropertyChanged("id");
            }
        }

        /// <summary>
        /// ftp site: IP address, or dns name
        /// </summary>
        public string host {
            get { return _host; }
            set {
                _host = value;
                this.NotifyPropertyChanged("host");
            }
        }

        /// <summary>
        /// ftp site: Username
        /// </summary>
        public string user {
            get { return _user; }
            set {
                _user = value;
                this.NotifyPropertyChanged("user");
            }
        }

        /// <summary>
        /// ftp site: password
        /// </summary>
        public string password {
            get { return _password; }
            set {
                _password = value;
                this.NotifyPropertyChanged("password");
            }
        }

        /// <summary>
        /// Local directory used to store files downloaded
        /// </summary>
        public string localdir {
            get { return _localdir; }
            set {
                _localdir = value;
                this.NotifyPropertyChanged("localdir");
            }
        }

        /// <summary>
        /// Remote Directory
        /// </summary>
        public string remotedir {
            get { return _remotedir; }
            set {
                _remotedir = value;
                this.NotifyPropertyChanged("remotedir");
            }
        }

        /// <summary>
        /// File name mask or pattern
        /// </summary>
        public string mask {
            get { return _mask; }
            set {
                _mask = value;
                this.NotifyPropertyChanged("mask");
            }
        }

        /// <summary>
        /// Character used to separate fields when "tostring" is called.
        /// </summary>
        public char sep { get; set; }

        #endregion Properties

        /// <summary>
        /// Class that describes one ftp site.
        /// </summary>
        public ftpsite() {
            host = "";
            user = "";
            password = "";
            localdir = "";
            remotedir = "";
            mask = "";
            sep = '|';
            NFC = null;
            _ParentLogger = null;
            Selected = false;
        }

        private void FileHasChanged(string fname) {
            if (NFC != null)
                NFC(fname);
        }

        /// <summary>
        /// This event allows the class to be attached to data aware components.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Generates a "packed" version of this class to be used to store the contents into an xml file.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            string rval;

            rval = "";

            rval += isnull(id, "") + sep;
            rval += isnull(host, "") + sep;
            rval += Encryption.Encrypt(isnull(user, "")) + sep;
            rval += Encryption.Encrypt(isnull(password, "")) + sep;
            rval += isnull(localdir, "") + sep;
            rval += isnull(remotedir, "") + sep;
            rval += isnull(mask, "") + sep;

            return rval;
        }

        private string isnull(string InputString, string DefaultValue) {
            if (string.IsNullOrEmpty(InputString))
                return DefaultValue;
            else
                return InputString.Trim();
        }

        /// <summary>
        /// Copy from existing ftpsite to this.
        /// </summary>
        /// <param name="from"></param>
        public void Copy(ftpsite from) {
            this.host = from.host;
            this.user = from.user;
            this.password = from.password;
            this.localdir = from.localdir;
            this.remotedir = from.remotedir;
            this.mask = from.mask;
        }

        /// <summary>
        /// Transmit Files for a site.
        /// </summary>
        /// <returns>True if successful, False if failed</returns>
        public bool GetFiles() {
            bool retstatus = false;
            FTPConnection ftp;
            string f;
            string pattern;
            string localpath, remotefile;
            string localfile;
            string TempPath;
            // string Session;
            bool lol = true;
            //
            StatusUpdate statupdt = new StatusUpdate(this.host);

            if (SiteParamsOK()) {
                //DateTime dt;
                //dt = DateTime.Now;
                //String s;
                ////s = String.Format("{0:yyy}{1:mm}{2:dd}.{3:hh}:{4:mm}:{5:ss}.{6:ffffff}",
                ////    dt.Year, dt.Month, dt.Day,
                ////    dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                //s = dt.ToString("yyymmdd.hhmmss.ffffff", CultureInfo.InvariantCulture);
                //Session = s;

                //Session = Guid.NewGuid().ToString();
                TempPath = Path.GetTempPath() + Session;

                if (!System.IO.Directory.Exists(TempPath)) {
                    System.IO.Directory.CreateDirectory(TempPath);
                    AddToDeleteQueue(TempPath, "Dir");
                }

                TempPath = Path.GetTempPath() + Session + "\\" + this.host;

                System.IO.Directory.CreateDirectory(TempPath);
                AddToDeleteQueue(TempPath, "Dir");
                LogMsg("Create Directory: " + TempPath);

                ftp = new FTPConnection();

                statupdt.SetMessage("Connecting", StatusUpdate.StatusType.Current);

                ftp.Downloading += MyDownloading;
                ftp.Downloaded += MyDownloaded;
                ftp.BytesTransferred += MyBT;

                ftp.ConnectMode = FTPConnectMode.ACTIVE;
                ftp.Timeout = 30000; // value in MS
                ftp.TransferNotifyInterval = 20480;

                ftp.ServerAddress = this.host;
                ftp.UserName = this.user;
                ftp.Password = this.password;
                if (lol) LogMsg(ftp.ServerAddress + ":Attempt to connect to");

                try {
                    ftp.Connect();
                    statupdt.SetMessage("Connected", StatusUpdate.StatusType.Current);

                    if (lol) LogMsg(ftp.ServerAddress + ":after connect");

                    if (ftp.IsConnected) {
                        int FilesFound = 0;

                        ftp.ChangeWorkingDirectory(this.remotedir);

                        if (lol) LogMsg(ftp.ServerAddress + ":after cwd");

                        statupdt.SetMessage("Listing Directory", StatusUpdate.StatusType.Current);

                        FTPFile[] fdir = ftp.GetFileInfos();

                        pattern = this.mask;

                        for (int i = 0; i < fdir.Length; i++) {
                            f = fdir[i].Name;
                            // if (lol) LogMsg(ftp.ServerAddress + ":fdir[i].Name = " + fdir[i].Name);

                            if (f.Contains(pattern)) {
                                FilesFound++;
                                localpath = this.localdir;
                                remotefile = fdir[i].Name;
                                localfile = localpath + "\\" + remotefile;

                                if (HasFileChanged(localfile, fdir[i])) {
                                    LogMsg(fdir[i].Path + "/" + fdir[i].Name + " Has Changed, downloading ");

                                    if (lol) LogMsg(ftp.ServerAddress + ":downloadfile(" + TempPath + "," + remotefile + ")");

                                    ftp.DownloadFile(TempPath, remotefile);

                                    AddToDeleteQueue(TempPath + "\\" + remotefile, "File");

                                    if (DownloadedFileOK(TempPath, fdir[i])) {
                                        MoveFile(TempPath, localpath, remotefile);
                                        if (lol) LogMsg(ftp.ServerAddress + ":downloaded!");
                                    }

                                    SetFileTime(localfile, fdir[i].LastModified);
                                    FileHasChanged(localfile);
                                    statupdt.SetMessage("Downloaded File OK", StatusUpdate.StatusType.Current);
                                }
                            }
                        }
                        if (FilesFound <= 0)
                            statupdt.SetMessage("No .T## Files Found!", StatusUpdate.StatusType.Current);

                        retstatus = true;
                    } // if (ftp.IsConnected)
                    else {
                        retstatus = false;
                    }

                    ftp.Close();
                    statupdt.SetMessage("OK", StatusUpdate.StatusType.Last);
                } catch {
                    statupdt.SetMessage("Connect Failed!", StatusUpdate.StatusType.Current);
                    statupdt.SetMessage("Connect Failed!", StatusUpdate.StatusType.Last);
                    LogMsg("Error Transmitting Files from Site: " + this.host);
                } // Try
            } else {
                statupdt.SetMessage("Site Config Error.", StatusUpdate.StatusType.Last);
                retstatus = false;
            }

            return retstatus;
        }

        private void MyDownloading(object sender, FTPFileTransferEventArgs e) {
            string s;
            s = string.Format("Downloading:{0}:{1}/{2}", this.host, e.RemoteDirectory, e.RemoteFile);
            LogMsg(s);
        }

        private void MyDownloaded(object sender, FTPFileTransferEventArgs e) {
            string s;
            s = string.Format("Downloaded:{0}:{1}/{2}", this.host, e.RemoteDirectory, e.RemoteFile);
            LogMsg(s);
        }

        //                        ftp.BytesTransferred += MyBT;
        private void MyBT(object sender, BytesTransferredEventArgs e) {
            string s;
            s = string.Format("Update:{0}:{1}/{2} Bytes = {3}", this.host, e.RemoteDirectory, e.RemoteFile, e.ByteCount);
            LogMsg(s);
            //LogMsg("Update: " + e.RemoteDirectory + "\\" + e.RemoteFile + " Bytes = " + e.ByteCount.ToString());
        }

        /// <summary>
        /// Determine if file size matches Downloaded Temp file & remote ftp file.
        /// </summary>
        /// <param name="TempPath"></param>
        /// <param name="fTPFile"></param>
        /// <returns>true if matches, otherwise false</returns>
        private bool DownloadedFileOK(string TempPath, FTPFile fTPFile) {
            bool retval;
            try {
                System.IO.FileInfo fi = new FileInfo(TempPath + "\\" + fTPFile.Name);

                if ((fTPFile.Size > 0) && (fi.Length > 0))
                    retval = true;
                else
                    retval = false;
            } catch {
                retval = false;
            }
            return retval;
        }

        /// <summary>
        /// Perform file move from temp path to permanent/localpath.
        /// </summary>
        /// <param name="TempPath"></param>
        /// <param name="localpath"></param>
        /// <param name="remotefile"></param>
        private void MoveFile(string TempPath, string localpath, string remotefile) {
            string TempName, PermName;
            TempName = TempPath + "\\" + remotefile;
            PermName = localpath + "\\" + remotefile;

            File.Delete(PermName);
            File.Copy(TempName, PermName);
        }

        /// <summary>
        /// Determine if remote file has changed
        /// </summary>
        /// <param name="localfile"></param>
        /// <param name="fTPFile"></param>
        /// <returns>True if file has changed, False otherwise</returns>
        private bool HasFileChanged(string localfile, FTPFile fTPFile) {
            long LocalSize, RemoteSize;
            DateTime LocalDT, RemoteDT;
            System.IO.FileInfo fi = new FileInfo(localfile);

            RemoteSize = fTPFile.Size;
            RemoteDT = fTPFile.LastModified;

            if (fi.Exists) {
                LocalSize = fi.Length;
                LocalDT = fi.LastWriteTime;
            } else {
                LocalSize = -1;
                LocalDT = DateTime.MinValue;
            }

            if ((LocalDT != RemoteDT) | (LocalSize != RemoteSize))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Set the local file date & time to the same as remote file.
        /// </summary>
        /// <param name="localfile"></param>
        /// <param name="dateTime"></param>
        private void SetFileTime(string localfile, DateTime dateTime) {
            System.IO.File.SetLastWriteTime(localfile, dateTime);
        }

        /// <summary>
        /// Check to see if ftpsite parameters are ok
        /// </summary>
        /// <returns></returns>
        private bool SiteParamsOK() {
            bool retval = true;

            if (this.localdir.Trim().Length <= 1) {
                retval = false;
                return retval;
            }

            if (!System.IO.Directory.Exists(this.localdir)) {
                try {
                    System.IO.Directory.CreateDirectory(this.localdir);
                } catch {
                    // It did not work... :-(
                }
            }

            if (!System.IO.Directory.Exists(this.localdir)) {
                retval = false;
                return retval;
            }

            if (this.host.Trim().Length <= 0) {
                retval = false;
                return retval;
            }

            // An empty mask implies '*'
            if (this.mask.Trim().Length <= 0)
                this.mask = "*";

            return retval;
        }
    }
}