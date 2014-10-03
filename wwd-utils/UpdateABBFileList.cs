using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using db;
using wwd_utils;
using System.Threading;
using System.Threading.Tasks;

namespace wwd_utils {

    /// <summary>
    ///
    /// </summary>
    public class UpdateABBFileList : IDisposable {
        #region logging setup

        // Setup Logging to parent.

        private LogMessager _ParentLogger = null;

        public LogMessager MessageLogger {
            set { _ParentLogger = value; }
            get { return _ParentLogger; }
        }

        private void LogToConsole(string msg) {
            Console.WriteLine(msg);
        }

        private void MyLog(string MsgStr) {
            if (_ParentLogger == null)
                System.Console.WriteLine(MsgStr);
            else
                _ParentLogger(MsgStr);
        }

        #endregion logging setup

        /*
         * 1. Create List of Files via EnumFiles
         * 2. For each file:
         *    File in db.files ?
         *    Yes -> Add to db.files
         *    No --> Check to see if file Updated
         *           Yes -> Update db.files record
         *           No -> done
         *
         */
        private DataClasses1DataContext dcc;
        private Table<TTotal_Reading> rds;
        private db.File ThisFile = new db.File();
        private AppSettings Settings;

        private static Boolean Executing = false;

        /// <summary>
        ///
        /// </summary>
        public UpdateABBFileList() {
            MessageLogger = LogToConsole;

            Settings = new AppSettings("FileMonitor.xml");
            dcc = new DataClasses1DataContext(Settings.ConnectionString);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // managed resources
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        ~UpdateABBFileList() {
            Dispose(true);
        }

        /*

          +-----------------+            +----+
          | File Size > 100 +----------->|Done|
          +---------+-------+            +----+
                    |
                    |yes
             +------v-------+            +----+
             | File In DB ? +----------->| no |
             +------+-------+            +--+-+
                    |                       |
                    |yes                    |
          +---------v--------+        +-----v-----+
          | Already Marked   |        | Add to DB |
          | for processing ? |        +-----------+
          +---------+-+------+
                    | |
                    | |
                +---+ +------------+
                |                  |yes
                |                  |
           +----v----------+     +-v---+
           | File Changed ?|     |Done |
           +----+-+--------+     +-----+
                | |
                | +--------------+
                |                |No
           +----v--------+       |
           | Mark for    |     +-v--+
           | Processing  |     |Done|
           +-------------+     +----+

        */

        public void Execute() {
            Execute(Settings.StartDirectory);
        }

        public void Execute(string direct) {
            MyLog("Examine Directory:" + direct);

            EnumFiles enumeratedFiles = new EnumFiles();
            enumeratedFiles.LoadFiles(direct);

            foreach (var f in enumeratedFiles.files) {
                ProcessOneFile(f);
            };


                #region OldCode

                /*
 *              if (f.FI.Length > 100) { // make sure file has data
                    SiteFile sf = new SiteFile(f.fullpath);
                    if (sf.ID == Guid.Empty) { // new file, add to db
                        AddFileToDB(f);
                        sf = null;
                        sf = new SiteFile(f.fullpath);
                    }

                    if (sf.Need2Process) {
                        UpdateFileInDB(f);
                    }
                }
*/

                /*
 * 
                if (f.FI.Length > 100) { // make sure file has data
                    if (!FileInDB(f)) {  // if file is not in db then add
                        AddFileToDB(f);
                        MyLog("ADD:" + f.fullpath);
                    } else {
                        // File is in DB, check to see if file has changed.
                        if ( sf.ID == Guid.Empty ) 

                        if (!NeedsProcessing(f)) {
                            if (FileInDBChanged(f)) {
                                UpdateFileInDB(f);
                                MyLog("UPD:" + f.fullpath);
                            }
                        }
                    }
                }
 */
                #endregion OldCode
            
        }

        public void ProcessOneFile(MyFileInfo f) {
            string s;
            s = string.Format("File: {0}", f.fullpath);

            if (FileIsValid(f)) {
                s += ", Valid";
                if (FileIsInDatabase(f)) {
                    s += ", InDB";
                    if (f.FileIsNew) {
                        s += ", New";
                    } else {
                        s += ", ! New";
                    }
                    UpdateFileInDB(f);
                } else {
                    s += ", ! InDB";
                    // File does not exist in DB, so we need 
                    // to add to DB & mark for processing.
                    AddFileToDB(f);
                }
            } else
                s += ", ! Valid";

            MessageLogger(s);
        }

        private void MarkThisFileForProcessing(MyFileInfo f) {
            try {
                var rec = (from r in dcc.Files
                           where r.FileName == f.fullpath
                           select r).Single();

                if (rec != null) {
                    rec.NeedsProcessing = true;
                    dcc.SubmitChanges();
                }
            } catch { }
        }

        private bool FileIsInDatabase(MyFileInfo f) {
            bool rval = false;

            try {
                int qty;
                qty = (from r in dcc.Files
                       where r.FileName.Trim() == f.fullpath.Trim()
                       select r).Count();

                if (qty == 1)
                    rval = true;
            } catch {
                rval = false;
            }
            return rval;
        }

        /// <summary>
        /// Determine if a file is > 100 bytes & last updated in the last 5 days.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private bool FileIsValid(MyFileInfo f) {
            bool rval = false;
            if ((f.FI.Length > 100) && ( f.FI.LastWriteTime > DateTime.Now.AddDays(-5)))
            {
                rval = true;
            }
            return rval;
        }

        #region Unused Code
        /// <summary>
        /// Determine if "File" record has NeedsProcessing set to true.
        /// </summary>
        /// <param name="f">MyFileInfo</param>
        /// <returns>True if record is found, and needs processing</returns>
        private bool NeedsProcessing(MyFileInfo f) {
            Boolean rvalue = false;

            try {
                var rdf = (from r in dcc.Files
                           where r.FileName == f.fullpath
                           select r);

                // Determine if we have a record for "path"
                if (rdf.Count() <= 0) {
                    rvalue = false;
                } else {
                    var rec = rdf.First();
                    rvalue = (rec.NeedsProcessing == true);

                    rec = null;
                }
                rdf = null;
            } catch (Exception) {
                rvalue = false;
            }
            return rvalue;
        }

        /// <summary>
        /// Determine if a file is found in ABB database.
        /// </summary>
        /// <param name="onefile"></param>
        /// <returns>True if found, False if not found</returns>
        private bool FileInDB(MyFileInfo onefile) {
            Boolean rvalue = false;

            try {
                int qty;
                qty = (from r in dcc.Files
                       where r.FileName.Trim() == onefile.fullpath.Trim()
                       select r).Count();

                if (qty == 0)
                    rvalue = false;
                else if (qty == 1)
                    rvalue = true;
                else
                    rvalue = true;
            } catch {
                rvalue = true;
            }

            return rvalue;
        }

        private bool FileInDBChanged(MyFileInfo f) {
            FileCalcs fc = new FileCalcs();
            string MD5Forf = fc.calculate_md5(f.fullpath);
            bool HasChanged = false;
            /*
             * Find file in DB, and check to see if file has changed.
             *
             */
            var rec = from r in dcc.Files
                      where r.FileName == f.fullpath
                      select r;

            if (rec.Count() > 1) {
                FoundTooManyRecords(f);
            } else {
                foreach (db.File x in rec) {
                    if (x.md5 != MD5Forf)
                        HasChanged = true;
                }
            }
            return HasChanged;
        }



        #endregion Unused Code


        /// <summary>
        /// Update the record describing this file, indicate needs processing.
        /// </summary>
        /// <param name="f"></param>
        public void UpdateFileInDB(MyFileInfo f) {
            try {
                var rec = (from r in dcc.Files
                           where r.FileName == f.fullpath
                           select r).Single();

                if (rec != null) {
                    rec.NeedsProcessing = f.FileIsNew;
                    rec.LastWriteTimeUtc = DT2UTC(f.LastReadingFile);
                    // rec.md5 = fc.calculate_md5(f.FileName);
                    dcc.SubmitChanges();
                }
            } catch { }
        }

        private DateTime? DT2UTC(DateTime? p) {
            DateTime? x;
            DateTime? rval = null;

            if (p == null) {
                rval = null;
            } else {
                x = DateTime.SpecifyKind((DateTime)p, DateTimeKind.Local);
                rval = x.Value.ToUniversalTime();
            }
            return rval;
        }

        private void FoundTooManyRecords(MyFileInfo f) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a files record
        /// </summary>
        /// <param name="f"></param>
        private void AddFileToDB(MyFileInfo f) {
            db.File nr = new db.File();

            nr.id = Guid.NewGuid();
            nr.CreationTimeUtc = f.FI.CreationTimeUtc;
            nr.data_rows = 0;
            nr.date_from = null;
            nr.date_to = null;
            nr.DirectoryName = f.FI.DirectoryName;
            nr.FileName = f.FI.FullName;
            nr.invalid_data = false;
            nr.LastWriteTimeUtc = f.FI.LastWriteTimeUtc;
            nr.Length = f.FI.Length;
            nr.md5 = "";
            nr.Name = f.FI.Name;
            nr.processdate = null;
            nr.NeedsProcessing = true;

            try {
                dcc.Files.InsertOnSubmit(nr);
                dcc.SubmitChanges();
                MyLog("AddFileToDB:" + f.fullpath);
            } catch (Exception e) {
                MyLog("AddFileToDB: Error:" + e.Message.ToString());
                MyLog("AddFileToDB: File: " + f.fullpath);

            }
        }
    }
}