using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wwd_utils {
    /// <summary>
    /// 
    /// </summary>
    public class FileMonitorStatus {
        private const string NoTime = "-:-:-";
        private const string FileName = "FileMonitorStatus.xml";
        private string UpdateTime;
        private string UpdateMsg;
        private int Progress;
        private int Min, Max;
        private string ProcessingFile;
        private SettingsFile sf;

        #region Properties
        public string CurrentTimeMessage {
            get {
                string s;
                s = UpdateTime + " " + UpdateMsg + " " + ProcessingFile;
                return s;
            }
        }

        public string CurrentFileMessage {
            get {
                string s;
                if (ProcessingFile.Length <= 0)
                    s = "";
                else {
                    s = UpdateTime + "";
                }
                return s;
            }
        }

        public string CurrentMessage {
            get {return UpdateMsg;}
            set { 
                UpdateMsg = value;
                SaveStatus();
            }
        }

        public int CurrentProgress {
            get {
                if (Progress > Max)
                    Progress = Max;
                if (Progress < Min)
                    Progress = Min;
                return Progress;
            }
            set {
                if (value > Max)
                    Progress = Max;
                else if (value < Min)
                    Progress = Min;
                else
                    Progress = value;
                SaveStatus();
            }
        }

        public int MinProgress {
            set { 
                Min = value;
                SaveStatus();
            }
            get { return Min; }
        }

        public int MaxProgress {
            set { 
                Max = value;
                SaveStatus();
            }
            get { return Max; }
        }

        public string CurrentFile {
            get { return ProcessingFile; }
            set {
                ProcessingFile = value;
                SaveStatus();
            }
        }

        #endregion Properties

        public FileMonitorStatus() {
            UpdateTime = NoTime;
            UpdateMsg = "Idle";
            Progress = 0;
            Min = 0;
            Max = 100;
            ProcessingFile = string.Empty;

            sf = new SettingsFile(FileName);
        }

        #region Methods

        public void ProgressInc() {
            Progress++;
            if (Progress > Max)
                Progress = Max;
            else if (Progress < Min)
                Progress = Min;
        }

        public void ProgressDec() {
            Progress--;
            if (Progress > Max)
                Progress = Max;
            else if (Progress < Min)
                Progress = Min;
        }

        public void SaveStatus() {
            UpdateTime = Convert.ToString(DateTime.Now);
            sf.WriteString("UpdateTime", UpdateTime);
            sf.WriteString("UpdateMsg", UpdateMsg);
            sf.WriteString("Min", Convert.ToString(Min));
            sf.WriteString("Max", Convert.ToString(Max));
            sf.WriteString("Progress", Convert.ToString(Progress));
            sf.WriteString("File", ProcessingFile);
        }

        public void LoadStatus() {
            UpdateTime = LoadItem("UpdateTime", DateTime.Now.ToString());
            UpdateMsg = LoadItem("UpdateMsg", "");
            Min = Convert.ToInt32(LoadItem("Min", "0"));
            Max = Convert.ToInt32(LoadItem("Max", "0"));
            Progress = Convert.ToInt32(LoadItem("Progress", "0"));
            ProcessingFile = LoadItem("File", "");
        }

        private string LoadItem(string Name, string DefaultValue) {
            string rval = DefaultValue;
            try {
                rval = sf.ReadString(Name, DefaultValue);
            } catch {
                rval = DefaultValue;
            }
            return rval;
        }

        #endregion Methods
    }
}
