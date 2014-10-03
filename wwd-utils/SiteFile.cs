using System;
using System.IO;
using System.Linq;
using db;

namespace wwd_utils
{
    public class SiteFile {
        public System.Guid ID { get; set; }
        public string SiteID { get; set; }
        public string FullPath { get; set; }
        public DateTime LastReadingFile { get; set; }
        public DateTime LastReadingDB { get; set; }
        public Boolean Need2Process { get; set; }
        public int DateFormat { get; set; }

        private readonly char[] delim = new char[] { ',' };
        private readonly char[] delim4dates = new char[] { '/' };

        private AppSettings settings;

        public SiteFile(System.Guid FileID) {
            MyInit();
            ID = FileID;
            LoadRecord();
        }

        public SiteFile(string FileName) {
            ID = Guid.Empty;
            // Let's see if file exists in DB.
            MyInit();

            DataClasses1DataContext dcc = new DataClasses1DataContext(settings.ConnectionString);
            try {
                var rec = (from f in dcc.Files
                           where (f.FileName == FileName)
                           select f).Single();

                ID = rec.id;
                LoadRecord();

            } catch ( System.InvalidOperationException e ) {
                if (e.Message == "Sequence contains no elements") {
                    ID = Guid.Empty;
                }
            } catch ( Exception e ) {
                Console.WriteLine("Exeption {0}", e.Message);
            }
        }

        private void MyInit() {
            settings = new AppSettings("FileMonitor.xml");
            SiteID = "";
            FullPath = "";
            LastReadingFile = DateTime.MinValue;
            LastReadingDB = DateTime.MinValue;
            Need2Process = false;
        }

        private void LoadRecord() {
            LoadFileRecord();
            if (FullPath.Length > 0) {
                if (System.IO.File.Exists(FullPath)) {
                    if (IsANewFile(FullPath)) {
                        GetFileInfo();
                        GetDBInfo();
                        CalcStatus();
                    }
                }
            }
            CalcStatus();
        }


        private bool IsANewFile(string FullPath) {
            FileInfo fi = new FileInfo(FullPath);
            if (fi.LastWriteTime > DateTime.Now.AddDays(-5)) 
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, SiteID: {1}, FullPath: {2}, LastReadingFile: {3}, LastReadingDB: {4}, Need2Process: {5}", ID, SiteID, FullPath, LastReadingFile, LastReadingDB, Need2Process);
        }

        /// <summary>
        /// Load full file path from DB record.
        /// </summary>
        private void LoadFileRecord() {
            DataClasses1DataContext dcc = new DataClasses1DataContext(settings.ConnectionString);
            var rec = (from f in dcc.Files
                       where (f.id == ID)
                       select f).Single();

            if ( rec != null ) {
                FullPath = rec.FileName;
            } 
        }

        // http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline.aspx
        private void GetFileInfo() {
            string firstline;
            string lastline;
            StreamReader sr = new StreamReader(FullPath);
            firstline = sr.ReadLine();
            lastline = GetLastLine(FullPath);

            // go get first line data.
            ParseFirstLine(firstline);

            LastReadingFile = GetDateTimeFromLastLine(lastline);
        }

        private DateTime GetDateTimeFromLastLine(string ll) {
            DateTime rval = DateTime.MinValue;
            string dt;
            string d;
            string[] s = ll.Split(delim);

            d = ConvertDateFormat(s[0], DateFormat);
            dt = d + " " + s[1];

            rval = DateTime.Parse(dt);
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

        private void ParseFirstLine(string fl) {
            string[] s = fl.Split(delim);
            SiteID = s[1];
            DateFormat = int.Parse(s[7]);
        }

        private String GetLastLine(String fileName) {
            var line = String.Empty;
            using (StreamReader sr = new StreamReader(fileName)) {
                string tmpline;
                while ((tmpline = sr.ReadLine()) != null) {
                    line = tmpline;
                }
            }
            return line;
        }

        private void GetDBInfo() {
            DataClasses1DataContext dcc = new DataClasses1DataContext(settings.ConnectionString);

            var records = (from r in dcc.Total_Readings
                        where ( r.SiteName == SiteID )
                        orderby r.dtime descending
                        select r.dtime  );

            DateTime maxdt = DateTime.MinValue;

            foreach ( var rlist in records ) {
                if ( rlist.Value > maxdt ) {
                    maxdt = rlist.Value;
                }
            }
            LastReadingDB = maxdt;
        }

        private void CalcStatus() {
            if (LastReadingDB < LastReadingFile)
                Need2Process = true;
            else
                Need2Process = false;
        }
    
    }
}
