using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Microsoft.Win32;

namespace wwd_utils
{
    /// <summary>
    /// Developed to handle saving of values in registry.  This method
    /// of saving application settings should be avoided.  Settings
    /// should be saved via xml using AppSettings class or something
    /// similar.
    /// </summary>
    public class regutils
    {
        private string _path = "";
        private string _CompanyName = "";
        private string _ApplicationName = "";

        public string Company
        {
            get { return _CompanyName; }
            set 
            { 
                _CompanyName = value;
                set_path();
            }
        }

        public string App
        {
            get { return _ApplicationName; }
            set 
            { 
                _ApplicationName = value;
                set_path();
            }
        }

        public string Path
        {
            get { return _path; }
        }

        // constructor
        public regutils()
        {
            _CompanyName = Application.CompanyName.ToString();
            _ApplicationName = Application.ProductName.ToString();
            set_path();
        }

        public regutils(String CompanyName, String ApplicationName) {
            _CompanyName = CompanyName;
            _ApplicationName = ApplicationName;
            set_path();
        }

        private void set_path()
        {
            _path = "SOFTWARE\\" + _CompanyName + "\\" + _ApplicationName;
        }

        // destructor
        ~regutils()
        {
            _path = "";
            _CompanyName = "";
            _ApplicationName = "";
        }

        //* 
        //* Reference found at: http://www.java2s.com/Code/CSharp/Windows/WriteaTextandDWordValuetotheRegistry.htm
        //* RegistryKey RegKeyWrite = Registry.CurrentUser;
        //     RegKeyWrite = RegKeyWrite.CreateSubKey("Software\\CSHARP\\WriteRegistryValue");
        //     RegKeyWrite.SetValue("StringValue","TRUE");
        //     RegKeyWrite.SetValue("Number",100);
        //     RegKeyWrite.Close();
        //* *

        public string getstring(string key)
        {
            string returnvalue = "";

            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.CreateSubKey(_path);
            try
            {
                returnvalue = regkey.GetValue(key).ToString();
            }
            catch
            {
                returnvalue = "";
            }

            return (returnvalue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void writestring(string key, string val)
        {
            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.CreateSubKey(_path);
            regkey.SetValue(key, val);
        }

        //        RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\Company\Priduct\Version");
        //if (key!=null)
        //{
        //string ini = key.GetValue("localinifile","").ToString();
        //key.Close();
        //}

    }
}
