using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace db {
    class FileInfo {
        private int DateFormat { get; set; }
        private string FullPath { get; set; }
        private DateTime LastReadingDB { get; set; }
        private DateTime LastReadingFile { get; set; }
        private string SiteID { get; set; }

        private readonly char[] delim = new char[] { ',' };
        private readonly char[] delim4dates = new char[] { '/' };


        private string ConStr { get; set; }

        public FileInfo(string FlPath,string ConnectionString ) {
            FullPath = FlPath;
            ConStr = ConnectionString;
        }

        public override string ToString()
        {
            // return string.Format("FileInfo: DateFormat: {0}, FullPath: {1}, LastReadingDB: {2}, LastReadingFile: {3}, SiteID: {4}, delim: {5}, delim4dates: {6}, ConStr: {7}", DateFormat, FullPath, LastReadingDB, LastReadingFile, SiteID, delim, delim4dates, ConStr);
            return string.Format("FileInfo: FullPath: {0}, LastReadingDB: {1}, LastReadingFile: {2}", FullPath, LastReadingDB, LastReadingFile );
        }

        public bool NeedsProcessing()
        {
            bool rval = false;

            // This will open the file, and get the last reading datetime.
            //
            GetFileInfo();
            //
            // If the last reading in this file is in last 5 days, it may 
            // need to be updated.
            //
            if (LastReadingFile > (DateTime.Now.AddDays(-5))) {
                rval = true;
            }

            //
            // Check to see if the data in file is newer than DB.
            //
            if (rval) {
                GetDBInfo();
                if (LastReadingDB < LastReadingFile) 
                    rval = true;
                else
                    rval = false;
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

        private void GetDBInfo() {
            DataClasses1DataContext dcc = new DataClasses1DataContext(ConStr);

            var records = (from r in dcc.Total_Readings
                           where (r.SiteName == SiteID)
                           orderby r.dtime descending
                           select r.dtime);

            DateTime maxdt = DateTime.MinValue;

            foreach (var rlist in records) {
                if (rlist.Value > maxdt) {
                    maxdt = rlist.Value;
                }
            }
            LastReadingDB = maxdt;
        }

        // http://msdn.microsoft.com/en-us/library/system.io.streamreader.readline.aspx
        private void GetFileInfo() {
            LastReadingFile = DateTime.MinValue;
            string firstline;
            string lastline;
            StreamReader sr = new StreamReader(FullPath);
            firstline = sr.ReadLine();
            lastline = GetLastLine(FullPath);

            // go get first line data.
            ParseFirstLine(firstline);

            LastReadingFile = GetDateTimeFromLastLine(lastline);
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

        private void ParseFirstLine(string fl) {
            string[] s = fl.Split(delim);
            SiteID = s[1];
            DateFormat = int.Parse(s[7]);
        }

    }
}
