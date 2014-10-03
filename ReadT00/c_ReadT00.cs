using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Messaging;
using System.Transactions;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Text;
using wwd_utils;
using db;
using System.Threading;

namespace ReadT00
{
    public class c_ReadT00
    {
        #region Private Declarations
        private static String _cs;
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
        public String ParentQueueName;
        public String MyQueueName;
        public int ProcessSlotNumber;

        private DataClasses1DataContext dcc;
        private Table<TTotal_Reading> rds;

        private db.File ThisFile = new db.File();

        private int dateformat;

        #endregion

        #region Properties
        public Boolean DebugFlag {
            get { return _DebugFlag; }
            set { _DebugFlag = value; }
        }

        public string SiteName
        {
            get { return _SiteName; }
        }

        public string connection_string
        {
            get { return _cs; }
            set { _cs = value; }
        }

        public string filename
        {
            get { return _fname; }
            set
            {
                _fname = value;
                _md5value = calculate_md5(_fname);
            }
        }

        public int Inserted
        {
            get { return _inserted; }
        }

        public int Failed
        {
            get { return _failed; }
        }

        public Guid Session
        {
            get { return _session; }
            set { _session = value; }
        }

        public string md5value
        {
            get { return _md5value; }
        }
        #endregion

        #region Constructor
        public c_ReadT00()
        {
            _SiteName = "";
            Ch1Name = "";
            Ch2Name = "";
            Ch3Name = "";
            Ch4Name = "";

            _session = Guid.NewGuid();
            Slot_id = Guid.NewGuid();
        }

        #endregion

        public string process_file(ref Semaphore DBLock )
        {
            String ReturnMsg = "Skipped";


            if ( Slot_id.ToString().Length <= 0 ) {
                Slot_id = Guid.NewGuid();
            }

            if (_cs.Trim().Length <= 0)
            {
                throw new FileNotFoundException("[Connection String Invalid]");
            }

            dcc = new DataClasses1DataContext(_cs);

            if ( _DebugFlag ) 
                dcc.Log = new DebuggerWriter();

            if (Need_to_Read_File(_fname))
            {
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

//                    DBLock.WaitOne();

                    try {

                        LReadDataLines();
                        DBLock.WaitOne();
                        dcc.SubmitChanges(ConflictMode.ContinueOnConflict);
                        ReturnNum = dcc.sp_Copy_TTotal_Readings(Slot_id.ToString(), ref ReturnMsg);

                        Update_File_Table();

                        DBLock.Release();

                    } catch (Exception e) {
                        DBLock.Release();
                        Console.WriteLine(e.ToString());
                    }

                        ////
                        //// Now we need to move records from "TTotal_Reading" to "Total_Reading" table.
                        ////



                } // rows.count > 7
            } // Need_to_Read_File

            return ReturnMsg;
        }

        private void Update_File_Table()
        {
            try
            {
                dcc.Files.InsertOnSubmit(ThisFile);
                dcc.SubmitChanges();
            }
            catch
            {
            }
        }

        private List<string[]> parseCSV(string path)
        {
            List<string[]> parsedData = new List<string[]>();

            try
            {
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return parsedData;
        }

        private Boolean IsADate(string s)
        {
            Boolean r = false;

            try
            {
                DateTime dt;
                switch ( dateformat ) {
                    case 0:
                        dt = Convert.ToDateTime( s.Substring(3,2)+"/"+s.Substring(0,2)+"/"+s.Substring(6,2) );
                        break;
                    default:
                        dt = Convert.ToDateTime(s);
                        break;
                }

                r = true;
            }
            catch
            {
                r = false;
            }
            return (r);
        }

        private void LReadDataLines() {
            int ilast = rows.Count();
            int i = 0;
            List<TTotal_Reading> Records  = new List<TTotal_Reading>();

            for (i = 0; i < ilast; i++) {
                row = rows[i];
                if (IsADate(row[0])) {
                    try {
                        TTotal_Reading r = new TTotal_Reading();

                        Records.Add(LAddRow(SiteName, row[2], row[0], row[1], row[4], row[5], row[12], row[13], i, ref r));

                    } catch (Exception e) {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            try {
                dcc.TTotal_Readings.InsertAllOnSubmit(Records);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReadDataLines()
        {
            int ilast = rows.Count();
            int i = 0;

            for (i = 0; i < ilast; i++)
            {
                row = rows[i];
                if (IsADate(row[0]))
                {
                    try {
                        AddRow(SiteName, row[2], row[0], row[1], row[4], row[5], row[12], row[13], i);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        Boolean IsAFloat(string s)
        {
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
        )
        {
            try
            {
                try
                {
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

                            if ((ThisFile.date_from == null) || (nr.dtime.Value < ThisFile.date_from.Value))
                                ThisFile.date_from = nr.dtime;

                            if ((ThisFile.date_to == null) || (nr.dtime.Value > ThisFile.date_to.Value))
                                ThisFile.date_to = nr.dtime;

                            // dcc.TTotal_Readings.InsertOnSubmit(nr);
                        }
                        _inserted++;
                    }
                }
                catch
                {
                    _failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return nr;
        }

        private void AddRow(
            string SName, 
            string ChanName, 
            string rowDate, string rowTime, 
            string rowReading, string rowReadingUnits,
            string rowFlow, string rowFlowUnits,
            int rownum
        )
        {

            try
            {
                try
                {
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
                }
                catch
                {
                    _failed++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public Boolean Need_to_Read_File() {
            return Need_to_Read_File(this.filename);
        }

        public Boolean Need_to_Read_File(string path)
        {
            Boolean rvalue = true;

            try
            {
                var rdf = (from r in dcc.Files
                           where r.FileName == path
                           select r);

                // Determine if we have a record for "path"
                if (rdf.Count() <= 0)
                {
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
                }
                else 
                {
                    ThisFile = rdf.First();

                    if (_md5value.CompareTo(ThisFile.md5) == 0)
                    {
                        rvalue = false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: {0}\n", e.Message.Trim());
                // No records exist, simply return true
            }

            return rvalue;
        }

        #region MD5 Calculations
        //
        // see:
        // http://www.codeproject.com/KB/files/Calculating_MD5_Checksum.aspx
        //
        private static MD5 md5 = MD5.Create();

        private string calculate_md5(string path)
        {
            try
            {
                using (FileStream stream = System.IO.File.OpenRead(path))
                {
                    byte[] checksum = md5.ComputeHash(stream);
                    return (BitConverter.ToString(checksum).Replace("-", string.Empty));
                } // End of using fileStream
            }
            catch (Exception)
            {

            }
            return "";
        }
        #endregion


    }
}
