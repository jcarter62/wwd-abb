using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wwd_utils {

    /// <summary>
    /// Set of procedures to help processing of ABB files.
    /// </summary>
    public class AbbUtils {

        public int data_rows { get; set; }

        public System.DateTime date_from { get; set; }

        public System.DateTime date_to { get; set; }

        public int length { get; set; }

        public DateTime processdate { get; set; }

        public Boolean NeedsProcessing { get; set; }

        public string FileID { get; set; }

        public string FileName { get; set; }

        public string ProcessException { get { return _ProcessException_; } }

        private string _ProcessException_;
        private AppSettings settings;

        #region logging setup

        // Setup Logging to parrent.

        private LogMessager _ParentLogger = null;

        public LogMessager MessageLogger {
            set { _ParentLogger = value; }
            get { return _ParentLogger; }
        }

        private void MyLog(string msg) {
            if (_ParentLogger == null)
                System.Console.WriteLine(msg);
            else
                _ParentLogger(msg);
        }

        #endregion logging setup

        /*

        #region LogSettings

        private AppSettings settings;
        private Boolean Logit = false;
        private string SvcEventSource = "FileMonitor";
        private Logging log;

        private void LoadSettings() {
            settings = new AppSettings("FileMonitor.xml");

            settings.Load();
            Logit = settings.Debug;

            log = new Logging();
            log.FileName = System.Environment.GetEnvironmentVariable("TEMP").ToString() + "\\" + SvcEventSource + ".txt";
            log.EventSource = SvcEventSource;
            switch (settings.LogToDest) {
                case AppSettings.LogTo.Both:
                    log.LoggingTo = Logging.LogTo.FileAndEventLog;
                    break;
                case AppSettings.LogTo.Event:
                    log.LoggingTo = Logging.LogTo.EventLog;
                    break;
                case AppSettings.LogTo.File:
                    log.LoggingTo = Logging.LogTo.File;
                    break;
                case AppSettings.LogTo.None:
                    log.LoggingTo = Logging.LogTo.None;
                    break;
            }
            log.LogLevel = Logging.LoggingLevel.Low;
        }

        #endregion LogSettings

        */

        /// <summary>
        /// Constructor
        /// </summary>
        public AbbUtils() {
            _ParentLogger = null;
            data_rows = 0;
            length = 0;
            date_from = System.DateTime.MinValue;
            date_to = System.DateTime.MinValue;
            processdate = DateTime.Now;
            NeedsProcessing = true;
            FileID = string.Empty;
            _ProcessException_ = string.Empty;

            settings = new AppSettings("FileMonitor.xml");
        }

        /// <summary>
        /// Process current file specified by the guid FileID, also performs an update to the "File" record.
        /// </summary>
        /// <returns>True if processing was successfull, False if failed</returns>
        public Boolean ProcessFile() {
            Boolean rvalue = false;
            ReadT00 r;

            try {
                r = new ReadT00(MyLog);

                System.IO.FileInfo f = new System.IO.FileInfo(FileName);
                length = Convert.ToInt32(f.Length);
                f = null;

                if (length > 100) {
                    r.process_file(FileName);
                    data_rows = r.DataRows;
                    date_from = r.Date_From;
                    date_to = r.Date_To;
                    processdate = System.DateTime.Now;
                    NeedsProcessing = false;

                    UpdateFileRecord();
                    rvalue = true;
                }
            } catch (System.IO.FileNotFoundException) {
                MyLog("File not found: (" + FileID + ") '" + FileName + "'");
                rvalue = UpdateFileRecord(FileID); // this should remove the record
            } catch (Exception e) {
                _ProcessException_ = e.Message.ToString();
                rvalue = false;
            }

            return rvalue;
        }

        private void DeleteFileID(string FileID) {
            throw new NotImplementedException();
        }

        public Boolean ProcessFileID(System.Guid ID) {
            Boolean rvalue = false;

            db.DataClasses1DataContext dcc;
            dcc = new db.DataClasses1DataContext(settings.ConnectionString);

            db.File ThisFile = new db.File();

            try {
                var rdf = (from r in dcc.Files
                           where (r.id == ID)
                           select r);

                if (rdf.Count() > 0) {
                    ThisFile = rdf.First();
                    FileID = ThisFile.id.ToString();
                    FileName = ThisFile.FileName;
                    rvalue = true;
                    MyLog("ProcessFileID:" + ID.ToString());
                }
            } catch (Exception ex) {
                FileID = string.Empty;
                FileName = string.Empty;
                MyLog("ProcessFileID: " + ex.Message);
            }

            if (rvalue) {
                MyLog("ProcessFile:" + FileName);
                rvalue = ProcessFile();
            }

            dcc = null;

            return rvalue;
        }

        /// <summary>
        /// GetFileThatNeedsProcessing
        /// </summary>
        /// <returns>Guid ID of file that requires processing.</returns>
        public string GetFileThatNeedsProcessing() {
            string rvalue = string.Empty;
            db.DataClasses1DataContext dcc;
            dcc = new db.DataClasses1DataContext(settings.ConnectionString);
            string s;

            db.File thisFile = new db.File();

            MyLog("GetFileThatNeedsProcessing()");

            try {
                var rdf = (from r in dcc.Files
                           where
                             (
                               (r.NeedsProcessing == true) 
//                               && (r.LastWriteTimeUtc > DateTime.Now.AddDays(-5))
                             )
                           orderby r.LastWriteTimeUtc descending, r.Length ascending
                           select r);

                if (rdf.Count() > 0) {
                    s = string.Format("GetFile: Found {0} Files for Processing", rdf.Count() );
                    MyLog(s);

                    thisFile = rdf.First();
                    rvalue = thisFile.id.ToString();
                    FileID = rvalue;
                    FileName = thisFile.FileName;

                    s = string.Format("GetFile: id {0}, Name {1}", FileID, FileName);
                    MyLog(s);
                }
            } catch (Exception ex) {
                FileID = string.Empty;
                FileName = string.Empty;
                MyLog("GetFileThatNeedsProcessing: " + ex.Message);
            }
            dcc = null;

            return rvalue;
        }

        /// <summary>
        /// Update the ABB "File" record to indicate the "File" has been processed.  Updated fields includeing:
        /// * md5 = caluclated md5 hash of file
        /// * data_rows = number of data rows
        /// * date_from (date) = data minimum date
        /// * date_to (date) = date maximum date
        /// * length = file length (bytes)
        /// * processdate (datetime)
        /// * NeedsProcessing = False
        /// </summary>
        /// <param name="GuidID"></param>
        /// <returns>True if record was updated, False if update failed</returns>
        public Boolean UpdateFileRecord(string GuidID) {
            Boolean UpdateStatus = false;
            string localmd5;

            if (GuidID == string.Empty) {
                _ProcessException_ = "Update Failed, file ID is empty!";
            } else {
                _ProcessException_ = string.Empty;
                FileCalcs fc = new FileCalcs();
                db.DataClasses1DataContext dcc;
                dcc = new db.DataClasses1DataContext(settings.ConnectionString);

                db.File ThisFile = new db.File();
                try {
                    var rdf = (from r in dcc.Files
                               where r.id.ToString() == GuidID
                               select r);
                    if (rdf.Count() > 0) {
                        ThisFile = rdf.First();
                        localmd5 = fc.calculate_md5(ThisFile.FileName);
                        if (localmd5.Contains("Could not find file")) {
                            MyLog("UpdateFileRecord:" + "Delete file (" + ThisFile.FileName + ")");
                            dcc.Files.DeleteOnSubmit(ThisFile);
                        } else {
                            ThisFile.md5 = localmd5;
                            ThisFile.data_rows = data_rows;
                            ThisFile.date_from = MinMax(date_from);
                            ThisFile.date_to = MinMax(date_to);
                            ThisFile.Length = length;
                            ThisFile.processdate = processdate;
                            ThisFile.NeedsProcessing = false;
                            MyLog("UpdateFileRecord:" + "Update file (" + ThisFile.FileName + ")");
                            UpdateStatus = true;
                        }
                    }

                    // Update the record
                    dcc.SubmitChanges();
                } catch (Exception e) {
                    MyLog("UpdateFileRecord:" + e.Message.ToString());
                    UpdateStatus = false;
                }
                dcc = null;
                fc = null;
            }
            return UpdateStatus;
        }

        public Boolean UpdateFileRecord() {
            return UpdateFileRecord(FileID);
        }

        private DateTime MinMax(DateTime d) {
            DateTime r;
            DateTime DateTimeMin = Convert.ToDateTime("1/1/1753");
            DateTime DateTimeMax = Convert.ToDateTime("12/31/9999");

            r = d;

            if (d < DateTimeMin)
                r = DateTimeMin;
            if (d > DateTimeMax)
                r = DateTimeMax;

            return r;
        }
    }
}