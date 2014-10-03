using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using wwd_utils;
using db;

namespace ReadD00
{
    public class c_ReadD00
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

        private DataClasses1DataContext dcc;
        private Table<TReading> rds;

        private db.File ThisFile = new db.File();

        private int dateformat;

        #endregion

        #region Properties
        public string SiteName
        {
            get{ return _SiteName; }
        }

        public string connection_string 
        { 
            get{ return _cs; } 
            set{ _cs = value; } 
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
        public c_ReadD00()
        {
            _SiteName = "";
            Ch1Name = "";
            Ch2Name = "";
            Ch3Name = "";
            Ch4Name = "";

            _session = Guid.NewGuid();

        }
        #endregion

        public void process_file()
        {
            if (_cs.Trim().Length <= 0)
            {
                throw new FileNotFoundException("[Connection String Invalid]");
            }

            dcc = new DataClasses1DataContext(_cs);

//             dcc.Log = new DebuggerWriter();

            if (Need_to_Read_File(_fname))
            {
                rows = parseCSV(_fname);
                _SiteName = rows[0][1];
                dateformat = Convert.ToInt32(rows[0][5]);
                ThisFile.data_rows = rows.Count();

                Ch1Name = rows[4][1];
                Ch2Name = rows[5][1];
                Ch3Name = rows[6][1];
                Ch4Name = rows[7][1];

                rds = dcc.GetTable<TReading>();
                ReadDataLines();
                try
                {
                    dcc.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                //
                // Now we need to copy unique records from "treadings" to "readings" table.
                //

                var cmdstr = "exec sp_Copy_Session_Records @session='" + _session + "' ";
                dcc.ExecuteCommand(cmdstr);

                Update_File_Table();

            } // Need_to_Read_File
        }

        private void Update_File_Table()
        {
            //db.File f = new db.File
            //{
            //    id = Guid.NewGuid(),
            //    FileName = _fname,
            //    processdate = DateTime.Now,
            //    md5 = _md5value
            //};

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

                switch (dateformat)
                {
                    case 0:
                        dt = Convert.ToDateTime(s.Substring(3, 2) + "/" + s.Substring(0, 2) + "/" + s.Substring(6, 2));
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

        private void ReadDataLines()
        {
            int ilast = rows.Count();
            int i = 0;

            for (i = 0; i < ilast; i++)
            {
                row = rows[i];
                if (IsADate(row[0]))
                {
                    if (Ch1Name.ToLower() != "off")
                    {
                        AddRow(SiteName, Ch1Name, row[0], row[1], row[2], i);
                    }
                    if (Ch2Name.ToLower() != "off")
                    {
                        AddRow(SiteName, Ch2Name, row[0], row[1], row[3], i);
                    }
                    if (Ch3Name.ToLower() != "off")
                    {
                        AddRow(SiteName, Ch3Name, row[0], row[1], row[4], i);
                    }
                    if (Ch4Name.ToLower() != "off")
                    {
                        AddRow(SiteName, Ch4Name, row[0], row[1], row[5], i);
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

        private void AddRow(string SName, string ChanName, string rowDate, string rowTime, string rowVal, int rownum)
        {
            try
            {
                try
                {
                    if (IsAFloat(rowVal) && IsADate(rowDate + " " + rowTime))
                    {
                        //
                        // convert date & time.
                        //
                        DateTime tmpdtime;
                        switch (dateformat)
                        {
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

                        //
                        // Check and fix reading value if it is 9999 or -9999
                        //
                        double tmpvalue;
                        tmpvalue = Convert.ToDouble(rowVal);
                        if (Math.Abs(tmpvalue) >= 9999)
                        {
                            tmpvalue = 0.0;
                        }

                        TReading nr = new TReading
                        {
                            id = Guid.NewGuid(),
                            SiteName = SName,
                            ChName = ChanName,
                            dtime = tmpdtime,
                            reading = tmpvalue,
                            session = _session,
                            file_id = ThisFile.id,
                            row = rownum
                        };
                        // Update ThisFile record

                        if ((ThisFile.date_from == null) || (nr.dtime.Value < ThisFile.date_from.Value))
                            ThisFile.date_from = nr.dtime;

                        if ((ThisFile.date_to == null) || (nr.dtime.Value > ThisFile.date_to.Value))
                            ThisFile.date_to = nr.dtime;

                        dcc.TReadings.InsertOnSubmit(nr);
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

        private Boolean Need_to_Read_File(string path)
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
                    // At least one record found.
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
