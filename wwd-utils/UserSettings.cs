using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace wwd_utils {
   
    class UserSettings {
        private String SrcDirectory;
        private String DstFile;
        private string xmlpath;
        private Settings sets;

        #region properties
        public String SourceDirectory {
            get { return SrcDirectory; }
            set { SrcDirectory = value; }
        }

        public String DestinationFile {
            get { return DstFile; }
            set { DstFile = value; }
        }
        #endregion

        public UserSettings() {
            // Load settings from application settings
//            SrcDirectory = Properties.Settings.Default.SourceDir;
//            DstFile = Properties.Settings.Default.Output;
            xmlpath = GetUserXMLPath();
            sets = new Settings(xmlpath);

            SrcDirectory = sets.GetSetting("SrcDirectory", "");
            DstFile = sets.GetSetting("DstFile", "");
        }

        ~UserSettings() {
        }

        public void SaveSettings() {
            // Save Settings to application settings
            sets.PutSetting("SrcDirectory", SrcDirectory);
            sets.PutSetting("DstFile", DstFile);
//            Properties.Settings.Default.Save();
        }

        #region Paths
        private string GetUserDataPath() {
//            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            dir = System.IO.Path.Combine(dir, "WWD");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private string GetUserXMLPath() {
            string fullpath;
            string DataPath = GetUserDataPath();

            fullpath = System.IO.Path.Combine(DataPath, "settings.xml");

            return fullpath;
        }
        #endregion

    }

}
