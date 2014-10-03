using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.Linq;
using System.Security.Cryptography;
using db;

namespace wwd_utils {
    /// <summary>
    /// 
    /// </summary>
    public class ReadT00 {
        #region Private Declarations
        private String _cs;
        private String _SiteName;
        private String Ch1Name;
        private String Ch2Name;
        private String Ch3Name;
        private String Ch4Name;
        private List<string[]> rows;
        private string[] row;
        private String _fname;
        private int _inserted = 0;
        private int _failed = 0;
        private Guid _session;
        private String _md5value;
        public Guid Slot_id;
        private Boolean _DebugFlag;
        private AppSettings Settings;

        private DataClasses1DataContext dcc;
        private Table<TTotal_Reading> rds;

        private db.File ThisFile = new db.File();
//        private Logging lg;
        private int dateformat;

        #endregion

        #region Properties
        public Boolean DebugFlag {
            get { return _DebugFlag; }
            set { _DebugFlag = value; }
        }

        public string SiteName {
            get { return _SiteName; }
        }

        public string connection_string {
            get { return _cs; }
            set { _cs = value; }
        }

        public string filename {
            get { return _fname; }
            set {
                _fname = value;
                _md5value = calculate_md5(_fname);
            }
        }

        public int Inserted {
            get { return _inserted; }
        }

        public int Failed {
            get { return _failed; }
        }

        public Guid Session {
            get { return _session; }
            set { _session = value; }
        }

        public string md5value {
            get { return _md5value; }
        }

        public System.DateTime Date_From { get; set; }
        public System.DateTime Date_To { get; set; }
        public int DataRows { get; set; }

        #endregion

        #region logging setup
        // Setup Logging to parrent.

        public delegate void LogMessage(string msg);
        private LogMessage LM;

        public LogMessage MessageLogger {
            set { LM += value; }
        }

        private void LogToConsole(string msg) {
            Console.WriteLine(msg);
        }
        #endregion


        #region Constructor
        public ReadT00() {
            MessageLogger = LogToConsole;
            ReadT00_Init();
        }

        public ReadT00(LogMessage Logger ) {
            MessageLogger = Logger;
            ReadT00_Init();
        }

        private void ReadT00_Init() {
            Settings = new AppSettings("FileMonitor.xml");
            _SiteName = "";
            Ch1Name = "";
            Ch2Name = "";
            Ch3Name = "";
            Ch4Name = "";

            Slot_id = Guid.NewGuid();
            //             lg.FileName = System.Environment.GetEnvironmentVariable("TEMP").ToString() + "\\" + SvcEventSource + ".txt";

            //lg = new Logging( System.Environment.GetEnvironmentVariable("TEMP").ToString() + "\\" + "ReadT00.txt");
            //lg.LoggingTo = Logging.LogTo.File;
            //lg.Overwrite = false;

            _cs = Settings.ConnectionString;
        }

        ~ReadT00() {
            //lg = null;
        }

        #endregion

        public string process_file(string FileName) {
            filename = FileName;
            return process_file();
        }

        public string process_file() {
            String ReturnMsg = "Skipped";

            dcc = new DataClasses1DataContext(_cs);

            if ( Settings.Debug ) dcc.Log = new DebuggerWriter();

            Remove_Empty_TTotal_Readings();

            LM("Process File: " + _fname);


            if (Need_to_Read_File(_fname)) {
                int ReturnNum = 0;

                rows = parseCSV(_fname);
                if (rows.Count > 7) {
                    _SiteName = rows[0][1];
                    dateformat = Convert.ToInt32(rows[0][7]);
                    ThisFile.data_rows = rows.Count();

                    Ch1Name = rows[4][1];
                    Ch2Name = rows[5][1];
                    Ch3Name = rows[6][1];
                    Ch4Name = rows[7][1];

                    rds = dcc.GetTable<TTotal_Reading>();

                    try {
                        LM("Reading Data Lines");
                        LReadDataLines();
                        dcc.SubmitChanges(ConflictMode.ContinueOnConflict);

                        LM("sp_Copy_TTotal_Readings(" + Slot_id.ToString() + ") ");
                        ReturnNum = dcc.sp_Copy_TTotal_Readings(Slot_id.ToString(), ref ReturnMsg);

                        LM("Results: (" + ReturnMsg + ")");

                        LM("Update_File_Table()");
                        Update_File_Table();
                    }
                    catch ( System.Data.Linq.DuplicateKeyException e ) {
                        LM("DuplicateKeyException:\nStack:" + e.StackTrace + "\nData:" + e.Data.ToString() + "\nMessage:" + e.ToString());
                    } catch (Exception e) {
                        LM("Error/138:\nStack:" + e.StackTrace + "\nData:" + e.Data.ToString() + "\nMessage:" + e.ToString() );
                    } // try
                } // rows.count > 7
            } // Need_to_Read_File

            return ReturnMsg;
        }

        private void Update_File_Table() {
            try {
                dcc.Files.InsertOnSubmit(ThisFile);
                dcc.SubmitChanges();
            } catch {
            }
        }

        private void Remove_Empty_TTotal_Readings() {
            try {
                dcc.sp_Remove_Empty_TTotal_Readings();
            } catch {
            }
        }

        private List<string[]> parseCSV(string path) {
            List<string[]> parsedData = new List<string[]>();

            try {
                using (StreamReader readFile = new StreamReader(path)) {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null) {
                        row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            } catch (Exception e) {
                LM("Error/168: " + e.ToString());
            }

            return parsedData;
        }

        private Boolean IsADate(string s) {
            Boolean r = false;

            try {
                DateTime dt;
                switch (dateformat) {
                    case 0:
                        dt = Convert.ToDateTime(s.Substring(3, 2) + "/" + s.Substring(0, 2) + "/" + s.Substring(6, 2));
                        break;
                    default:
                        dt = Convert.ToDateTime(s);
                        break;
                }

                r = true;
            } catch {
                r = false;
            }
            return (r);
        }

        private void LReadDataLines() {
            Boolean columns_ok = false;
            Boolean ChName_ok = false;
            Boolean Event_ok = false;
            Boolean Row_ok = false;

            int ilast = rows.Count();
            int i = 0;
            List<TTotal_Reading> Records = new List<TTotal_Reading>();
            DataRows = 0;

            for (i = 0; i < ilast; i++) {
                row = rows[i];
                // check to see if this is a good row.
                columns_ok = false;
                ChName_ok = false;
                Event_ok = false;
                Row_ok = false;
                if (row.Count() >= 15) {
                    columns_ok = true;
                    if ( IsValidChannel( row[3] ) )
                        ChName_ok = true;
                    if (string.Compare(row[15], "Timed", true) == 0)
                        Event_ok = true;

                    if (ChName_ok && Event_ok)
                        Row_ok = true;
                }

                if (IsADate(row[0]) && Row_ok ) {
                    try {
                        TTotal_Reading r = new TTotal_Reading();
                        Records.Add(LAddRow(SiteName, row[2], row[0], row[1], row[4], row[5], row[12], row[13], i, ref r));
                        DataRows++;

                    } catch (Exception e) {
                        LM("Error/219: " + e.ToString());
                    }
                }
            }

            try {
                dcc.TTotal_Readings.InsertAllOnSubmit(Records);
            } catch (Exception e) {
                LM("Error/217: " + e.ToString());
            }
        }

        private bool IsValidChannel(string p) {
            bool r;
            r = false;
            if (Ch1Name.Length > 0)
                if (string.Compare(Ch1Name, p, false) == 0)
                    goto OKreturn;

            if (Ch2Name.Length > 0)
                if (string.Compare(Ch2Name, p, false) == 0)
                    goto OKreturn;

            if (Ch3Name.Length > 0)
                if (string.Compare(Ch3Name, p, false) == 0)
                    goto OKreturn;

            if (Ch4Name.Length > 0)
                if (string.Compare(Ch4Name, p, false) == 0)
                    goto OKreturn;

            goto Justreturn;
OKreturn:
            r = true;
Justreturn:
            return r;
        }

        private void ReadDataLines() {
            int ilast = rows.Count();
            int i = 0;

            for (i = 0; i < ilast; i++) {
                row = rows[i];
                if (IsADate(row[0])) {
                    try {
                        AddRow(SiteName, row[2], row[0], row[1], row[4], row[5], row[12], row[13], i);
                        //     sname   , chname, date  , time  , reading,units , flow   , units  , rownumber
                    } catch (Exception e) {
                        LM("Error/231: " + e.ToString());
                    }
                }
            }
        }

        Boolean IsAFloat(string s) {
            double result = 0;
            if (double.TryParse(s, out result))
                return true;
            else
                return false;
        }

        private TTotal_Reading LAddRow(
            string SName,
            string ChanName,
            string rowDate, string rowTime,
            string rowReading, string rowReadingUnits,
            string rowFlow, string rowFlowUnits,
            int rownum,
            ref TTotal_Reading nr
        ) {
            try {
                try {
                    if (IsAFloat(rowReading) && IsADate(rowDate + " " + rowTime) && IsAFloat(rowFlow)) {
                        double tmp_reading, tmp_flow;
                        DateTime tmpdtime;

                        tmp_reading = Convert.ToDouble(rowReading);

                        tmp_flow = Convert.ToDouble(rowFlow);
                        if (tmp_flow < -999)
                            tmp_flow = 0.0;

                        //
                        // convert date & time.
                        //
                        switch (dateformat) {
                            case 0:
                                tmpdtime = Convert.ToDateTime(
                                    rowDate.Substring(3, 2) + "/" + rowDate.Substring(0, 2) + "/" + rowDate.Substring(6, 2) +
                                    " " +
                                    rowTime);
                                break;
                            case 1:
                                tmpdtime = Convert.ToDateTime(rowDate + " " + rowTime);
                                break;
                            default:
                                tmpdtime = Convert.ToDateTime(rowDate + " " + rowTime);
                                break;
                        }
                        {
                            nr.slot_id = Slot_id.ToString();
                            nr.id = Guid.NewGuid();
                            nr.SiteName = SName;
                            nr.ChName = ChanName;
                            nr.dtime = tmpdtime;
                            nr.reading = tmp_reading;
                            nr.reading_units = rowReadingUnits;
                            nr.flow = tmp_flow;
                            nr.flow_units = rowFlowUnits;
                            nr.file_id = ThisFile.id;
                            nr.row = rownum;

                            // Update ThisFile record

                            if (
                                (ThisFile.date_from == null) || 
                                IsNotReasonableDate(Date_From) || 
                                (nr.dtime.Value < ThisFile.date_from.Value)
                                ) 
                            {
                                ThisFile.date_from = nr.dtime;
                                Date_From = Convert.ToDateTime(nr.dtime);
                            }

                            if (
                                (ThisFile.date_to == null) || 
                                IsNotReasonableDate(Date_To) ||       // This will be executed for first record.
                                (nr.dtime.Value > ThisFile.date_to.Value)
                                ) 
                            {
                                ThisFile.date_to = nr.dtime;
                                Date_To = Convert.ToDateTime(nr.dtime);
                            }

                            // dcc.TTotal_Readings.InsertOnSubmit(nr);
                        }
                        _inserted++;
                    }
                } catch {
                    _failed++;
                }
            } catch (Exception e) {
                LM("Error/312: " + e.ToString());
            }

            return nr;
        }

        private Boolean IsNotReasonableDate(DateTime d) {
            Boolean r = false;

            if (d == null) {
                r = true;
            }
            else
            {
                if ((d < DateTime.Now.AddYears(-50)) || (d > DateTime.Now.AddYears(50))) {
                    r = true;
                }
            }

            return r;
        }

        private void AddRow(
            string SName,
            string ChanName,
            string rowDate, string rowTime,
            string rowReading, string rowReadingUnits,
            string rowFlow, string rowFlowUnits,
            int rownum
        ) {

            try {
                try {
                    if (IsAFloat(rowReading) && IsADate(rowDate + " " + rowTime) && IsAFloat(rowFlow)) {
                        double tmp_reading, tmp_flow;
                        DateTime tmpdtime;

                        tmp_reading = Convert.ToDouble(rowReading);

                        tmp_flow = Convert.ToDouble(rowFlow);
                        if (tmp_flow < -999)
                            tmp_flow = 0.0;

                        //
                        // convert date & time.
                        //
                        switch (dateformat) {
                            case 0:
                                tmpdtime = Convert.ToDateTime(
                                    rowDate.Substring(3, 2) + "/" + rowDate.Substring(0, 2) + "/" + rowDate.Substring(6, 2) +
                                    " " +
                                    rowTime);
                                break;
                            case 1:
                                tmpdtime = Convert.ToDateTime(rowDate + " " + rowTime);
                                break;
                            default:
                                tmpdtime = Convert.ToDateTime(rowDate + " " + rowTime);
                                break;
                        }
                        {
                            TTotal_Reading nr = new TTotal_Reading {
                                slot_id = Slot_id.ToString(),
                                id = Guid.NewGuid(),
                                SiteName = SName,
                                ChName = ChanName,
                                dtime = tmpdtime,
                                reading = tmp_reading,
                                reading_units = rowReadingUnits,
                                flow = tmp_flow,
                                flow_units = rowFlowUnits,
                                file_id = ThisFile.id,
                                row = rownum
                            };

                            // Update ThisFile record

                            if ((ThisFile.date_from == null) || (nr.dtime.Value < ThisFile.date_from.Value))
                                ThisFile.date_from = nr.dtime;

                            if ((ThisFile.date_to == null) || (nr.dtime.Value > ThisFile.date_to.Value))
                                ThisFile.date_to = nr.dtime;

                            dcc.TTotal_Readings.InsertOnSubmit(nr);
                        }
                        _inserted++;
                    }
                } catch {
                    _failed++;
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        public Boolean Need_to_Read_File() {
            return Need_to_Read_File(this.filename);
        }

        public Boolean Need_to_Read_File(string path) {
            Boolean rvalue = true;

            try {
                var rdf = (from r in dcc.Files
                           where r.FileName == path
                           select r);

                // Determine if we have a record for "path"
                if (rdf.Count() <= 0) {
                    // No record found, need to add one
                    ThisFile.id = System.Guid.NewGuid();
                    ThisFile.FileName = path;
                    ThisFile.processdate = DateTime.Now;
                    ThisFile.md5 = calculate_md5(path);
                    ThisFile.date_from = null;
                    ThisFile.date_to = null;
                    ThisFile.data_rows = 0;
                    ThisFile.invalid_data = false;
                    //
                    // Indicate we need to read this file.
                    //
                    rvalue = true;
                } else {
                    ThisFile = rdf.First();

                    if (_md5value.CompareTo(ThisFile.md5) == 0) {
                        rvalue = false;
                    }
                }
            } catch (Exception e) {
                LM("Error/426: " + e.ToString());
            }

            return rvalue;
        }

        #region MD5 Calculations
        //
        // see:
        // http://www.codeproject.com/KB/files/Calculating_MD5_Checksum.aspx
        //
        private static MD5 md5 = MD5.Create();

        private string calculate_md5(string path) {
            try {
                using (FileStream stream = System.IO.File.OpenRead(path)) {
                    byte[] checksum = md5.ComputeHash(stream);
                    return (BitConverter.ToString(checksum).Replace("-", string.Empty));
                } // End of using fileStream
            } catch (Exception) {

            }
            return "";
        }
        #endregion
    }

}


