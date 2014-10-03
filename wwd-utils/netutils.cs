using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using EnterpriseDT.Net.Ftp;
using EnterpriseDT.Util.Debug;

namespace wwd_utils
{
    /// <summary>
    /// Network related class to support ping/ftp/etc.
    /// </summary>
    public class netutils
    {
        private Ping pingClient = new Ping();
        private string hostip;      // ftp server ip address to ping.
        private int waittime;       // stored in ms (milliseconds)
        private string PingResults;
        private long PingTime;
        private string user, pass;
        private string UriString;

        private string LocalDir;

        #region Properties
        /// <summary>
        /// IP address of host to ping
        /// </summary>
        public string IP
        {
            get { return hostip; }
            set { 
                hostip = value;
                UriString = "ftp://" + hostip + "/sdmmc/vrd/";
            }
        }

        /// <summary>
        /// Timeout value in seconds.
        /// </summary>
        public int Timeout
        {
            get { return (waittime / 1000); }
            set { waittime = value * 1000; }
        }

        /// <summary>
        /// Return ping results.
        /// </summary>
        public string Results
        {
            get { return PingResults; }
        }

        /// <summary>
        /// Return ping time as string.
        /// </summary>
        public string ResultTime
        {
            get { return Convert.ToString(PingTime); }
        }

        public string UserName
        {
            get { return user; }
            set { user = value; }
        }

        public string Password
        {
            set { pass = value; }
        }

        #endregion Properties

        /// <summary>
        /// Init 
        /// </summary>
        public netutils()
        {
            hostip = "";
            waittime = 10 * 1000; // 10 seconds.
            PingResults = "";
            PingTime = 0;

            pingClient.PingCompleted += new PingCompletedEventHandler(PingCompleted);

            LocalDir = "D:\\local\\recorders\\";
        }

        /// <summary>
        /// Ping remote system, return true or false depending on results.
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="TimeOutInSecs"></param>
        /// <returns></returns>
        public bool PingIP(string Address, int TimeOutInSecs )
        {
            hostip = Address;
            waittime = (TimeOutInSecs * 1000);
            return PingIP();
        }

        /// <summary>
        /// Perform Ping to IP
        /// </summary>
        /// <returns>True if success, False if failed</returns>
        public bool PingIP()
        {
            bool rval;
            rval = false;

            if (hostip.Length <= 0)
            {
                rval = false;
                PingResults = "IP Invalid";
            }
            else
            {
                PingReply r;
                r = pingClient.Send(hostip, waittime);

                PingResults = r.Status.ToString();
                PingTime = r.RoundtripTime;

                if (r.Status == IPStatus.Success)
                    rval = true;
                else
                    rval = false;
            }

            return rval;
        }

        public bool isAlive()
        {
            bool rval = false;

            if (PingIP())
                rval = true;

            return rval;
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            // Check to see if an error occurred.  If no error, then display 
            // the address used and the ping time in milliseconds.
            if (e.Error == null)
            {
                if (e.Cancelled)
                {
                    PingResults = "Cancelled";
                }
                else
                {
                    if (e.Reply.Status == IPStatus.Success)
                    {
                        PingResults = "OK";
                        PingTime = e.Reply.RoundtripTime;
                    }
                    else
                    {
                        PingResults = GetStatusString(e.Reply.Status);
                        PingTime = 0;
                    }
                }
            }
            else
            {
                // Otherwise display the error.
                PingResults = e.Error.InnerException.Message;
                PingTime = 0;
            }
        }

        private string GetStatusString(IPStatus status)
        {
            switch (status)
            {
                case IPStatus.Success:
                    return "Success.";
                case IPStatus.DestinationHostUnreachable:
                    return "Destination host unreachable.";
                case IPStatus.DestinationNetworkUnreachable:
                    return "Destination network unreachable.";
                case IPStatus.DestinationPortUnreachable:
                    return "Destination port unreachable.";
                case IPStatus.DestinationProtocolUnreachable:
                    return "Destination protocol unreachable.";
                case IPStatus.PacketTooBig:
                    return "Packet too big.";
                case IPStatus.TtlExpired:
                    return "TTL expired.";
                case IPStatus.ParameterProblem:
                    return "Parameter problem.";
                case IPStatus.SourceQuench:
                    return "Source quench.";
                case IPStatus.TimedOut:
                    return "Timed out.";
                default:
                    return "Ping failed.";
            }
        }

        //// Transfer files to directory

        /// <summary>
        /// Transfer Files in directory
        /// </summary>
        /// <returns></returns>
        public bool TransferFiles()
        {
            bool rval = true;

            FTPFile[] files;

            files = GetFiles(hostip);

            foreach (FTPFile f in files)
            {
                if (f.Name.Contains(".T"))
                {
                    DownloadFile(hostip, f);
/*
                    if (IsInDB(hostip, f))
                    {
                        if (FileUpdated(hostip, f))
                        {
                            MarkForDownload(hostip, f);
                        }
                    }
                    else
                    {
                        AddFileToDB(hostip, f);
                    }
 */
                }
            }

            return rval;
        }

        private void DownloadFile(string hostip, FTPFile f)
        {
            string tempfile;
            string targetfile;
            FTPClient ftp;
            string dirsep = "\\";

            tempfile = Path.GetTempPath();
            tempfile = tempfile + Guid.NewGuid().ToString();
            targetfile = LocalDir + hostip + dirsep + f.Name;

            ftp = new FTPClient();

            ftp.ConnectMode = FTPConnectMode.ACTIVE;
            ftp.Timeout = waittime;

            ftp.RemoteHost = hostip;
            ftp.Connect();
            ftp.Login("wwd", "waters");

            ftp.TransferType = FTPTransferType.ASCII;

            ftp.ChDir("/sdmmc/vrd");

            ftp.Get(tempfile, f.Name);

            File.Delete(targetfile);
            File.Move(tempfile, targetfile);
            File.SetCreationTime(targetfile, f.LastModified);
            File.SetLastWriteTime(targetfile, f.LastModified);

            ftp.Quit();
            ftp = null;
        }

        private bool IsInDB(string hostip, FTPFile f)
        {
            return false;
        }

        private FTPFile[] GetFiles(string hostip)
        {
            FTPClient ftp;

            ftp = new FTPClient();

            ftp.ConnectMode = FTPConnectMode.ACTIVE;
            ftp.Timeout = waittime;

            ftp.RemoteHost = hostip;
            ftp.Connect();
            ftp.Login("wwd", "waters");

            ftp.TransferType = FTPTransferType.ASCII;

            ftp.ChDir("/sdmmc/vrd");

            FTPFile[] files = ftp.DirDetails();

            ftp.Quit();
            ftp = null;

            return files;
        }



        //// FTP - Get List of Files

        public void ftplist()
        {
            string Datastring;
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(UriString);
            req.Credentials = new NetworkCredential(user, pass);
            req.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            FtpWebResponse resp = (FtpWebResponse)req.GetResponse();

            Stream responseStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            Datastring = reader.ReadToEnd();

            FileStruct[] dlist = GetList(Datastring);

            foreach (FileStruct fs in dlist)
            {
                if (fs.IsDirectory)
                {
                    Console.WriteLine("<DIR>");
                    Console.WriteLine(fs.Name + ", " + fs.Flags + ", " + fs.CreateTime);
                }
                else
                {
                    Console.WriteLine(fs.Name + ", " + fs.Flags + ", " + fs.CreateTime);
                }
            }

            reader.Close();
            resp.Close();
        }

        public struct FileStruct
        {
            public string Flags;
            public string Owner;
            public string Group;
            public bool IsDirectory;
            public DateTime CreateTime;
            public string Name;
        }
        public enum FileListStyle
        {
            UnixStyle,
            WindowsStyle,
            Unknown
        }

        private FileStruct[] GetList(string datastring)
         {
          List<FileStruct> myListArray = new List<FileStruct>(); 
          string[] dataRecords = datastring.Split('\n');
          FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
          foreach (string s in dataRecords)
          {    
           if (_directoryListStyle != FileListStyle.Unknown && s != "")
           {
            FileStruct f = new FileStruct();
            f.Name = "..";
            switch (_directoryListStyle)
            {
             case FileListStyle.UnixStyle:
              f = ParseFileStructFromUnixStyleRecord(s);
              break;
             case FileListStyle.WindowsStyle:
              f = ParseFileStructFromWindowsStyleRecord(s);
              break;
            }
            if (!(f.Name == "." || f.Name == ".."))
            {
             myListArray.Add(f);     
            }    
           }
          }
          return myListArray.ToArray(); ;
         }
 
         private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
         {
          //Assuming the record style as 
          // 02-03-04  07:46PM       <DIR>          Append
          FileStruct f = new FileStruct();
          string processstr = Record.Trim();
          string dateStr = processstr.Substring(0,8);      
          processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
          string timeStr = processstr.Substring(0, 7);
          processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
          f.CreateTime = DateTime.Parse(dateStr + " " + timeStr);
          if (processstr.Substring(0,5) == "<DIR>")
          {
           f.IsDirectory = true;    
           processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
          }
          else
          {
//            string[] strs = processstr.Split(new char[] { ' ' }, true);
           string[] strs = processstr.Split(new char[] { ' ' });
           processstr = strs[1].Trim();
           f.IsDirectory = false;
          }   
          f.Name = processstr;  //Rest is name   
          return f;
         }
 

         public FileListStyle GuessFileListStyle(string[] recordList)
         {
          foreach (string s in recordList)
          {
           if(s.Length > 10 
            && Regex.IsMatch(s.Substring(0,10),"(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
           {
            return FileListStyle.UnixStyle;
           }      
           else if (s.Length > 8 
            && Regex.IsMatch(s.Substring(0, 8),  "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
           {
            return FileListStyle.WindowsStyle;
           }   
          }
          return FileListStyle.Unknown;
         }
 
         private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
         {
          ///Assuming record style as
          /// dr-xr-xr-x   1 owner    group               0 Nov 25  2002 bussys
          FileStruct f= new FileStruct();   
          string processstr = Record.Trim();        
          f.Flags = processstr.Substring(0,9);
          f.IsDirectory = (f.Flags[0] == 'd');  
          processstr =  (processstr.Substring(11)).Trim();
          _cutSubstringFromStringWithTrim(ref processstr,' ',0);   //skip one part
          f.Owner = _cutSubstringFromStringWithTrim(ref processstr,' ',0);
          f.Group = _cutSubstringFromStringWithTrim(ref processstr,' ',0);
          _cutSubstringFromStringWithTrim(ref processstr,' ',0);   //skip one part
          f.CreateTime = DateTime.Parse(_cutSubstringFromStringWithTrim(ref processstr,' ',8));     
          f.Name =  processstr;   //Rest of the part is name
          return f;
         }
 
     private string _cutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
     {
            int pos1 = s.IndexOf(c, startIndex);
           string retString = s.Substring(0,pos1);
           s = (s.Substring(pos1)).Trim();
           return retString;
       }

    }
}
