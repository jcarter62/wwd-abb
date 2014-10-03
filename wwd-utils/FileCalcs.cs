using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace wwd_utils {
    /// <summary>
    /// 
    /// </summary>
    public class FileCalcs {
        #region MD5 Calculations
        //
        // see:
        // http://www.codeproject.com/KB/files/Calculating_MD5_Checksum.aspx
        //

        public string calculate_md5(string path) {
            MD5 md5 = MD5.Create();
            string retval;

            retval = string.Empty;

            try {
                FileStream s = System.IO.File.OpenRead(path) ;
                byte[] checksum = md5.ComputeHash(s);
                retval = (BitConverter.ToString(checksum).Replace("-", string.Empty));
                s.Close();
            } catch (Exception e) {
                retval = "";
                //e.Message.ToString().Substring(0, 40);
            }
            return retval;
        }
        #endregion
    }
}
