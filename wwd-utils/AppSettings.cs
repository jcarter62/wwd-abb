using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Win32;

namespace wwd_utils {

    /// <summary>
    /// This class is used to abstract application settings.
    /// </summary>
    public class AppSettings {
        private String _Server;
        private String _Database;
        private String _UserName;
        private String _Password;
        private Boolean _WindowsAuthentication;
        private Boolean _LogFile;
        private Boolean _LogEvent;
        private String _Key;
        private String _LogFileName;
        private CloseMethod _CloseMethod;

        private String _ConnectionString;

        private String _StartDirectory;
        private Boolean _DebugFlag;

        private string _filename;

        private long _IdleTime;

        private SettingsFile sf;

        public enum LogTo { None, File, Event, Both };

        public enum CloseMethod { MinToTray, ExitApp };

        #region Properties

        public CloseMethod UICloseMethod {
            get;
            set;
        }

        public long IdleTime {
            get { return _IdleTime; }
            set { _IdleTime = value; }
        }

        public double IdleTimeMinutes {
            get {
                double rval;
                rval = Convert.ToDouble(_IdleTime) / 1000.0 / 60.0;
                return rval;
            }
            set {
                long newval;
                newval = Convert.ToInt64(value * 1000.0 * 60.0);
                _IdleTime = newval;
            }
        }

        /// <summary>
        /// SQL Server, Database Server Name
        /// </summary>
        public String Server {
            get { return _Server; }
            set { _Server = value; }
        }

        /// <summary>
        /// SQL Server, Database Instance
        /// </summary>
        public String Database {
            get { return _Database; }
            set { _Database = value; }
        }

        /// <summary>
        /// The user name required for access to database
        /// </summary>
        public String UserName {
            get { return _UserName; }
            set { _UserName = value; }
        }

        /// <summary>
        /// The password required for access to database
        /// </summary>
        public String Password {
            get { return _Password; }
            set { _Password = value; }
        }

        /// <summary>
        /// Flag that indicates if Integrated authentication is used to gain access to database
        /// </summary>
        public Boolean WindowsAuthentication {
            get { return _WindowsAuthentication; }
            set { _WindowsAuthentication = value; }
        }

        /// <summary>
        /// This is the connection string required to gain access to database
        /// </summary>
        public String ConnectionString {
            get {
                _ConnectionString = BuildConnectionString();
                return _ConnectionString;
            }
        }

        /// <summary>
        /// Root directory where ABB files are located.
        /// </summary>
        public String StartDirectory {
            get { return _StartDirectory; }
            set { _StartDirectory = value; }
        }

        /// <summary>
        /// Simple application flag
        /// </summary>
        public Boolean Debug {
            get { return _DebugFlag; }
            set { _DebugFlag = value; }
        }

        public string FileName {
            get { return _filename; }
        }

        public string FullFileName {
            get { return sf.FullPath; }
        }

        //        private Boolean _LogFile;
        //        private Boolean _LogEvent;

        public Boolean LogToFile {
            get { return _LogFile; }
            set { _LogFile = value; }
        }

        public Boolean LogToEventLog {
            get { return _LogEvent; }
            set { _LogEvent = value; }
        }

        public string LogFileName {
            get { return _LogFileName; }
            set { _LogFileName = value; }
        }

        //        public enum LogTo { File, Event, Both };

        public LogTo LogToDest {
            get {
                LogTo retval = LogTo.None;

                if ((_LogEvent == true) && (_LogFile == true))
                    retval = LogTo.Both;
                else
                    if ((_LogEvent == true) && (_LogFile == false))
                        retval = LogTo.Event;
                    else
                        if ((_LogEvent == false) && (_LogFile == true))
                            retval = LogTo.File;
                return retval;
            }
        }

        #endregion Properties

        #region Startup

        /// <summary>
        ///
        /// </summary>
        public AppSettings() {
            _filename = "settings.xml";
            AppSettingsInit();
        }

        public AppSettings(string filename) {
            _filename = filename;
            AppSettingsInit();
        }

        private void AppSettingsInit() {
            _Key = "BlueTiger";
            sf = new SettingsFile(_filename);
            Load();
        }

        /// <summary>
        ///
        /// </summary>
        ~AppSettings() {
            // Save();
        }

        #endregion Startup

        #region Methods

        /// <summary>
        /// Populate class properties with settings, Server, Database, Username, ...
        /// </summary>
        public void Load() {
            _Server = GetString("Server");
            _Database = GetString("DataBase");
            _UserName = GetString("UserName", true);
            _Password = GetString("PassWord", true);
            _StartDirectory = GetString("StartDirectory");

            if (GetString("UICloseMethod") == "MinToTray") {
                UICloseMethod = CloseMethod.MinToTray;
            } else {
                UICloseMethod = CloseMethod.ExitApp;
            }

            try { _IdleTime = Convert.ToInt64(GetString("IdleTime")); } catch { _IdleTime = 2 * 1000 * 60; }

            _WindowsAuthentication = false;
            if (GetString("WindowsAuthentication") == "Y")
                _WindowsAuthentication = true;

            _StartDirectory = GetString("StartDirectory");

            _DebugFlag = false;
            if (GetString("Debug") == "Y")
                _DebugFlag = true;

            _ConnectionString = BuildConnectionString();

            _LogEvent = false;
            if (GetString("LogToEventLog") == "Y")
                _LogEvent = true;

            _LogFile = false;
            if (GetString("LogToFile") == "Y")
                _LogFile = true;

            _LogFileName = GetString("LogFileName");
        }

        /// <summary>
        /// Save values to settings file.
        /// </summary>
        public void Save() {
            //
            // If we have something to save, then save it.
            //
            if (_Server.Trim().Length > 1) {
                WriteString("Server", _Server);
                WriteString("DataBase", _Database);
                WriteString("UserName", _UserName, true);
                WriteString("PassWord", _Password, true);

                if (UICloseMethod == CloseMethod.MinToTray)
                    WriteString("UICloseMethod", "MinToTray");
                else
                    WriteString("UICloseMethod", "ExitApp");

                if (_WindowsAuthentication)
                    WriteString("WindowsAuthentication", "Y");
                else
                    WriteString("WindowsAuthentication", "N");

                if (_DebugFlag)
                    WriteString("Debug", "Y");
                else
                    WriteString("Debug", "N");

                WriteString("StartDirectory", _StartDirectory);

                WriteString("IdleTime", Convert.ToString(_IdleTime));

                if (_LogEvent)
                    WriteString("LogToEventLog", "Y");
                else
                    WriteString("LogToEventLog", "N");

                if (_LogFile)
                    WriteString("LogToFile", "Y");
                else
                    WriteString("LogToFile", "N");

                WriteString("LogFileName", _LogFileName);
            }
        }

        #endregion Methods

        /// <summary>
        /// Get Setting, clear text
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        private string GetString(string Name) {
            return GetString(Name, false);
        }

        /// <summary>
        /// Get Setting, clear text or decrypted based on parameter
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Decrypt"></param>
        /// <returns></returns>
        private string GetString(string Name, Boolean Decrypt) {
            String se;
            se = sf.ReadString(Name, "");

            if (Decrypt) {
                if (se.Length > 0)
                    se = EncDec.Decrypt(sf.ReadString(Name, ""), _Key + Name);
                else
                    se = "";
            }
            return se;
        }

        /// <summary>
        /// Write setting, encrypt based on parameter
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="Encrypt"></param>
        private void WriteString(String Name, String Value, Boolean Encrypt) {
            String s;
            if (Encrypt) {
                s = EncDec.Encrypt(Value, _Key + Name);
            } else {
                s = Value;
            }
            sf.WriteString(Name, s);
        }

        /// <summary>
        /// Write setting, clear text
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        private void WriteString(String Name, String Value) {
            WriteString(Name, Value, false);
        }

        private String BuildConnectionString() {
            String cs;
            String tmp;
            //
            // Example connection strings.
            //
            // ConnectionString = "Data Source=SQL-SVR\\MSSQLR2;Initial Catalog=abb;Integrated Security=True";
            // ConnectionString = "Data Source=localhost;Initial Catalog=abb;Integrated Security=True";
            // Data Source=SQL-SVR\MSSQLR2;Initial Catalog=abb;Integrated Security=True;Application Name=FileMonitor;Max Pool Size=200;Net=dbmsrpcn;Packet Size=1024;

            // Data Source=DBB1\SQL1050;Initial Catalog=abb;Integrated Security=False;User=filemonitor;Password=9BE0CFE2C4B348C290ADD1D52BF96D3D;Application Name=FileMonitor;Max Pool Size=200;Packet Size=1024;
            // Server=dbb1\sql1050;Database=abb;User Id=filemonitor;Password=9BE0CFE2C4B348C290ADD1D52BF96D3D;

            cs = "Server=";
            tmp = _Server;
            cs = cs + tmp + ";Database=";
            tmp = _Database;

            if (_WindowsAuthentication)
                cs = cs + tmp + ";Integrated Security=True;";
            else {
                cs = cs + tmp + ";User Id=" + _UserName +
                    ";Password=" + _Password + ";";
            }

            cs = cs; // +"Application Name=FileMonitor;Max Pool Size=200;Packet Size=1024;";

            return cs;
        }
    }
}