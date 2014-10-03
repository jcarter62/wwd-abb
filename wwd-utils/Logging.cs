using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using wwd_utils;
using System.Threading;

/*
 * from: http://social.msdn.microsoft.com/Forums/en-US/csharplanguage/thread/c8bfbe70-20da-4b3f-91a2-fcdfb43e92d1/
 * 
 * In a Windows Service there is no Console so Console.Write* output is discarded. There are a number of alternatives:
 1.The System.Diagnostics.Trace class has a similar interface to the Console class so you could 
 *   migrate your code quite easily to this. It can then be configured to output to a file.
 2.You can use the System.Diagnostics.EventLog class to write to the Event Log which you can 
 *   then monitor using Event Viewer.
 3.You can use the third-party open-source log4net library which is very flexible.

 */

namespace wwd_utils {
    /// <summary>
    /// Provide Logging facility to log to a file, eventlog, or both.
    /// </summary>
    public class Logging : IDisposable {
        /// <summary>
        /// Logging level setting
        /// </summary>
        public enum LoggingLevel { None=0, Low=1, Med=5, High=7, Verbose=9 };
        /// <summary>
        /// Specifies where to send logging messages
        /// </summary>
        public enum LogTo { File, EventLog, FileAndEventLog, None };

        private string filename = "";
        private FileStream fs;
        private LoggingLevel loglevel;
        private System.Diagnostics.EventLog EvtLog;
        private string EvtSrc = "WWD";
        // File flags
        Boolean FileExists = false;
        Boolean FileOpen = false;
        //
        private static Semaphore MySemaphore;

        //        public bool Overwrite = true;

        #region Properties
        /// <summary>
        /// Output file name, if user has specified LogTo=File or FileAndEventLog.
        /// </summary>
        public string FileName {
            get { return filename; }
            set { filename = value; }
        }
        /// <summary>
        /// User specified Logging Level
        /// </summary>
        public LoggingLevel LogLevel {
            get { return loglevel; }
            set { loglevel = value; }
        }
        /// <summary>
        /// User specified Event Source
        /// </summary>
        public string EventSource {
            get { return EvtSrc; }
            set { 
                EvtSrc = value;
                SetupEventLog();
            }
        }

        /// <summary>
        /// User specified Logging destination
        /// </summary>
        public LogTo LoggingTo { get; set; }

        private void SetupEventLog() {
            try {
                if (!System.Diagnostics.EventLog.SourceExists(EvtSrc)) {
                    System.Diagnostics.EventLog.CreateEventSource(EvtSrc, "Application");
                }
                EvtLog.Source = EvtSrc;
            } catch { }
        }
        #endregion

        /// <summary>
        /// When user does not specify output file, create or use file %temp%\TestFile.txt
        /// </summary>
        public Logging() {
            filename = System.Environment.GetEnvironmentVariable("TEMP").ToString() + "\\logging.txt";
            LoggingInit();
        }

        /// <summary>
        /// User specified a logfile, also set loggingto=File
        /// </summary>
        /// <param name="File"></param>
        public Logging(string File) {
            filename = File;
            LoggingInit();
        }

        private void LoggingInit() {
            // MySemaphore;

            MySemaphore = new Semaphore(0, 1);

            LogLevel = LoggingLevel.None;
            LoggingTo = LogTo.None;
            EvtLog = new System.Diagnostics.EventLog();
            SetupEventLog();

            MySemaphore.Release(1);

        }

        ~Logging() {
            Dispose();
        }

        public virtual void Dispose() {
            if (fs != null) {
                try {
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                } catch { }
            }
        }

        private void OpenFile() {
            FileOpen = false;
            FileExists = false;

            // check to see if file exists, and create if does not.
            if (System.IO.File.Exists(filename))
                FileExists = true;
            else
            {
                try {
                    System.IO.File.Create(filename);
                    FileExists = true;
                } catch {
                    FileExists = false;
                }
            }

            if (FileExists) {
                try {
//                    fs = new FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                    fs = new FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    FileOpen = true;
                } catch (Exception e) {
                    Console.WriteLine("Error " + e.ToString());
                    FileOpen = false;
                }
            }
        }

        /// <summary>
        /// Save Message to Logfile
        /// </summary>
        /// <param name="Message"></param>
        public void LogMsg(string Message) {
            if (LoggingTo != LogTo.None) {
                DateTime d = DateTime.Now;
                string s;
                //
                // http://msdn.microsoft.com/en-us/library/zdtaw1bw(VS.96).aspx
                // formats found in link
                //
                s = d.ToUniversalTime().ToString("yyyy.MM.dd.HH.mm.ss") + " : " + Message;

                if ((LoggingTo == LogTo.File) || (LoggingTo == LogTo.FileAndEventLog)) {
                    MySemaphore.WaitOne(); // Only allow one thread to write to log at a time.
                    OpenFile();
                    if (FileOpen) {
                        if (LogLevel != LoggingLevel.None) {
                            s = s + "\r\n";
                            byte[] bytes = Conversions.StringToBytes(s);
                            fs.Position = fs.Length;
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Flush();
                            fs.Close();
                        }
                    }
                    MySemaphore.Release();
                }
                //////////////////
                if ((LoggingTo == LogTo.EventLog) || (LoggingTo == LogTo.FileAndEventLog)) {
                    EvtLog.WriteEntry(Message);
                }
            }
        }
        /// <summary>
        /// Output log message, if passed Level is > classes loglevel.
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Level"></param>
        public void LogMsg(string Message, LoggingLevel Level) {
            if (Level >= loglevel)
                LogMsg(Message);
        }

    }
}
