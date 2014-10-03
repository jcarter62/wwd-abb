using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using db;

namespace wwd_utils
{
    /// <summary>
    /// FileInfo class with additional properties, and a new ToString() method.
    /// </summary>
    public class MyFileInfo {
        public DateTime Created;
        public string FileName;
        public string fullpath;
        public string md5;
        public DateTime Modified;
        public long Size;
        public FileInfo FI;

        private string SiteID;
        private int DateFormat;
        public DateTime? LastReadingFile;
        private DateTime? LastReadingDB;
        private AppSettings settings;

        public MyFileInfo() {
            settings = new AppSettings("FileMonitor.xml");
            FileName = "";
            Modified = DateTime.MinValue;
            Created = DateTime.MinValue;
            Size = 0;
            fullpath = "";
            md5 = "";
            FI = null;
            LastReadingDB = null;
            LastReadingFile = null;
        }

        public string InfoString() {
            string s;
            s = string.Format("SiteID:{0} FileName:{1} NewFile?: {2}", SiteID, FileName, FileIsNew );
            s += string.Format("LastReadingDB:{0} LastReadingFile:{1} ",LastReadingDB, LastReadingFile);
            return s;
        }

        public override string ToString() {
            string s;

            // http://msdn.microsoft.com/en-us/library/dd260048(VS.96).aspx
            s = string.Format("{0:000000000000000}", FI.Length);

            s = s +
                FI.DirectoryName + " - " + FI.Name + " - " +
                FI.LastWriteTimeUtc.ToString() + " - " +
                FI.CreationTimeUtc.ToString();

            return s;
        }

        private string FileToString(db.File fi) {
            string s;

            // http://msdn.microsoft.com/en-us/library/dd260048(VS.96).aspx
            s = string.Format("{0:000000000000000}", fi.Length);

            s = s +
                fi.DirectoryName + " - " + fi.Name + " - " +
                fi.LastWriteTimeUtc.ToString() + " - " +
                fi.CreationTimeUtc.ToString();

            return s;
        }

        public bool IsDifferent(db.File f) {
            bool isdiff = false;
            string fstr, tstr;

            fstr = FileToString(f);
            tstr = this.ToString();

            if ( fstr != tstr ) isdiff = true;

            return isdiff;
        }

        public bool FileIsNew {
            get {
                bool rval = false;

                if ( LastReadingFile == null )
                    GetFileInfo();
                if (LastReadingDB == null )
                    GetDBInfo();

                if (LastReadingDB < LastReadingFile)
                    rval = true;

                return rval;
            }
        }

        private void GetDBInfo() {
            DataClasses1DataContext dcc = new DataClasses1DataContext(settings.ConnectionString);
            LastReadingDB = DateTime.MinValue;

            try {
                var maxDate = (from r in dcc.Total_Readings
                               where (r.SiteName == SiteID)
                               select r.dtime).Max();

                LastReadingDB = DateTime.Parse(maxDate.ToString());
            } catch { }

        }

        private string LastLineOfFile() {
            var line = String.Empty;
            using (StreamReader sr = new StreamReader(fullpath)) {
                string tmpline;
                while ((tmpline = sr.ReadLine()) != null) {
                    line = tmpline;
                }
            }
            return line;
        }

        private DateTime GetLastReadingDateTime() {
            DateTime rval = DateTime.MinValue;
            string dt;
            string d;
            string lastLine = LastLineOfFile();


            string[] s = lastLine.Split(delim);

            d = ConvertDateFormat(s[0], DateFormat);
            dt = d + " " + s[1];

            try {
                rval = DateTime.Parse(dt);
            } catch {
                rval = DateTime.MinValue;
            }
            return rval;
        }

        private string ConvertDateFormat(string p, int DFmt) {
            string rval = string.Empty;

            //
            // re-order parts of date, if date format is == 1
            //
            if (DFmt == 0) {
                // DD/MM/YY
                string[] x = p.Split(delim4dates);
                rval = x[1] + "/" + x[0] + "/" + x[2];
            } else {
                rval = p;
            }
            return rval;
        }


        // http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline.aspx
        private void GetFileInfo() {
            string firstline;
            string lastline;
            StreamReader sr = new StreamReader(fullpath);
            firstline = sr.ReadLine();
            sr.Close();

            lastline = LastLineOfFile();

            // go get first line data.
            ParseFirstLine(firstline);

            LastReadingFile = GetLastReadingDateTime();
        }

        private readonly char[] delim = new char[] { ',' };
        private readonly char[] delim4dates = new char[] { '/' };

        private void ParseFirstLine(string fl) {
            string[] s = fl.Split(delim);
            SiteID = s[1];
            DateFormat = int.Parse(s[7]);
        }

    }

    /// <summary>
    /// Traverse RootDirectory & create list of MyFileInfo records for all 
    /// files with names ending in T*
    /// </summary>
    public class EnumFiles {
        private string rootdirectory;
        public List<MyFileInfo> files;

        #region Properties
        public string RootDirectory {
            get { return rootdirectory; }
            set { rootdirectory = value; }
        }
        #endregion Properties

        public EnumFiles() {
            files = new List<MyFileInfo>();
        }

        #region Methods
        public void LoadFiles(string dir) {
            string[] dirs = Directory.GetDirectories(dir);
            // Enumerate all subdirectories.
            foreach (var d in dirs) {
                /*
                 * Since these directories are replicated via DFSR, we do not want to include
                 * any paths that include ...Dfsr..., which is used internally in DFSR, and
                 * not important to our purposes.
                 */
                if (!d.Contains("Dfsr")) {  
                    LoadFiles(d);
                }
            }

            string[] FilesInDir = Directory.GetFiles(dir, "*.T??");

            // Enumerate the files just in this directory.
            foreach (var fi in FilesInDir) {
                FileInfo x = new FileInfo(fi);

//                if (x.LastWriteTime > DateTime.Now.AddDays(-5)) {
                if (fi.Contains(".T")) {
                    MyFileInfo n = new MyFileInfo();
                    n.Created = x.CreationTime;
                    n.FileName = x.Name;
                    n.fullpath = x.FullName;
                    n.Modified = x.LastWriteTime;
                    n.Size = x.Length;
                    n.FI = new FileInfo(fi);

                    files.Add(n);
                }
//                }
                x = null;
            }
        }

        /// <summary>
        /// Generate list of files to console.
        /// </summary>
        public void PrintList() {
            foreach (var x in files) {
                Console.WriteLine(x.ToString());
            }
        }

        #endregion Methods

    }
}
