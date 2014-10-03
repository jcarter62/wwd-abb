using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace wwd_utils {

    /// <summary>
    /// Abstraction of an interface to an xml settings file.
    /// </summary>
    public class SettingsFile {
        private string xmlpath;
        private string filename;
        private string companyname;
        private Settings sets;

        #region properties

        public string FullPath {
            get { return xmlpath; }
        }

        public string FileName {
            get { return filename; }
            set {
                filename = value;
                CalcNewFileName();
            }
        }

        public string CompanyName {
            get { return companyname; }
            set {
                companyname = value;
                CalcNewFileName();
            }
        }

        #endregion properties

        #region Constructor Destructor

        public SettingsFile(string XMLFileName) {
            filename = XMLFileName;
            SettingsFileInit();
        }

        public SettingsFile() {
            filename = "settings.xml";
            SettingsFileInit();
        }

        private void SettingsFileInit() {
            companyname = "WWD";
            CalcNewFileName();
        }

        ~SettingsFile() {
        }

        #endregion Constructor Destructor

        #region Private Routines

        private void CalcNewFileName() {
            xmlpath = GetUserXMLPath(filename);
            if (sets != null)
                sets = null;
            sets = new Settings(xmlpath);
        }

        #endregion Private Routines

        #region Paths

        /*
         * Microsoft & specialfolders
         * http://msdn.microsoft.com/en-us/library/system.environment.specialfolder.aspx
         *
         * http://stackoverflow.com/questions/895723/environment-getfolderpath-commonapplicationdata-is-still-returning-c-docum
         * Examples shown
         *
            Output On Windows Server 2003:
            SpecialFolder.ApplicationData: C:\Documents and Settings\blake\Application Data
            SpecialFolder.CommonApplicationData: C:\Documents and Settings\All Users\Application Data
            SpecialFolder.ProgramFiles: C:\Program Files
            SpecialFolder.CommonProgramFiles: C:\Program Files\Common Files
            SpecialFolder.DesktopDirectory: C:\Documents and Settings\blake\Desktop
            SpecialFolder.LocalApplicationData: C:\Documents and Settings\blake\Local Settings\Application Data
            SpecialFolder.MyDocuments: C:\Documents and Settings\blake\My Documents
            SpecialFolder.System: C:\WINDOWS\system32`

            Output on Vista: SpecialFolder.ApplicationData: C:\Users\blake\AppData\Roaming
            SpecialFolder.CommonApplicationData: C:\ProgramData
            SpecialFolder.ProgramFiles: C:\Program Files
            SpecialFolder.CommonProgramFiles: C:\Program Files\Common Files
            SpecialFolder.DesktopDirectory: C:\Users\blake\Desktop
            SpecialFolder.LocalApplicationData: C:\Users\blake\AppData\Local
            SpecialFolder.MyDocuments: C:\Users\blake\Documents
            SpecialFolder.System: C:\Windows\system32
         *
         */

        private string GetUserDataPath() {
            // If we were using a user private settings file, then we would use ...ApplicationData
            // string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // In this case we are using a system wide settings file (since the service must be able to access).
            //
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            dir = System.IO.Path.Combine(dir, companyname);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private string GetUserXMLPath(string xmlfilename) {
            string fullpath;
            string DataPath = GetUserDataPath();

            fullpath = System.IO.Path.Combine(DataPath, xmlfilename);

            return fullpath;
        }

        #endregion Paths

        #region Public Methods

        public string ReadString(string Name, string Dflt) {
            string rvalue;
            rvalue = sets.GetSetting(Name.Trim().ToUpper(), Dflt);
            return rvalue;
        }

        public void WriteString(string Name, string Value) {
            sets.PutSetting(Name.Trim().ToUpper(), Value);
        }

        #endregion Public Methods
    }

    /// <summary>
    /// This class provides read/write to an xml file, the actual file is provided
    /// as a parameter to the constructor.
    /// </summary>
    public class Settings {
        XmlDocument xmlDocument = new XmlDocument();
        string documentPath;

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public Settings(string path) {
            documentPath = path;
            try {
                xmlDocument.Load(documentPath);
            } catch {
                xmlDocument.LoadXml("<settings></settings>");
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetSetting(string xPath, int defaultValue) {
            return Convert.ToInt16(GetSetting(xPath, Convert.ToString(defaultValue)));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="value"></param>
        public void PutSetting(string xPath, int value) {
            PutSetting(xPath, Convert.ToString(value));
            xmlDocument.Save(documentPath);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetSetting(string xPath, string defaultValue) {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) {
                return xmlNode.InnerText;
            } else {
                return defaultValue;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xPath"></param>
        /// <param name="value"></param>
        public void PutSetting(string xPath, string value) {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null) {
                xmlNode = createMissingNode("settings/" + xPath);
            }
            xmlNode.InnerText = value;
            try {
                xmlDocument.Save(documentPath);
            } catch (Exception e) {
            }
        }

        private XmlNode createMissingNode(string xPath) {
            string[] xPathSections = xPath.Split('/');
            string currentXPath = "";
            XmlNode testNode = null;
            XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
            foreach (string xPathSection in xPathSections) {
                currentXPath += xPathSection;
                testNode = xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null) {
                    currentNode.InnerXml += "<" +
                                xPathSection + "></" +
                                xPathSection + ">";
                }
                currentNode = xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }
    }
}