using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using db;

namespace wwd_utils {

    /// <summary>
    /// Provide mechanism to update the table SiteStatus
    /// </summary>
    public class StatusUpdate {
        #region Private Declarations

        private String _cs;
        private String _SiteName;
        private AppSettings Settings;
        private DataClasses1DataContext dcc;
        private Table<SiteStatus> rds;
        private db.SiteStatus ThisSite = new db.SiteStatus();

        #endregion Private Declarations

        /// <summary>
        /// Type of status update
        /// </summary>
        public enum StatusType { Current, Last }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SiteName"></param>
        public StatusUpdate(string SiteName) {
            Settings = new AppSettings("FileMonitor.xml");
            _SiteName = SiteName;

            _cs = Settings.ConnectionString;

            dcc = new DataClasses1DataContext(_cs);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~StatusUpdate() {
            try {
                dcc.SubmitChanges();
                dcc = null;
            } catch { }
        }

        /// <summary>
        /// Write Message to SiteStatus table
        /// </summary>
        /// <param name="Msg"></param>
        /// <param name="type"></param>
        public void SetMessage(string Msg, StatusType type) {
            try {
                var r = dcc.GetTable<SiteStatus>();

                var q = from rds in r
                        where rds.SiteName == this._SiteName
                        select rds;

                if (q.Count() <= 0) {
                    // There are no records in DB, need to insert a record.
                    SiteStatus rec = new SiteStatus();
                    rec.SiteName = this._SiteName;
                    if (type == StatusType.Current) {
                        rec.CurrentUpdate = Msg;
                        rec.CurrentTimeStamp = DateTime.Now;
                    } else {
                        rec.LastUpdate = Msg;
                        rec.LastTimeStamp = DateTime.Now;
                    }
                    dcc.SiteStatus.InsertOnSubmit(rec);
                } else {
                    foreach (SiteStatus rec in q) {
                        if (type == StatusType.Current) {
                            rec.CurrentUpdate = Msg;
                            rec.CurrentTimeStamp = DateTime.Now;
                        } else {
                            rec.LastUpdate = Msg;
                            rec.LastTimeStamp = DateTime.Now;
                        }
                    }
                }

                dcc.SubmitChanges();
            } catch (Exception e) {
                Console.WriteLine("Exception: {0}", e.Message);
            }
        }

        /// <summary>
        /// Set Status to OK
        /// </summary>
        public void AllOK() {
            SetMessage("", StatusType.Current);
            SetMessage("", StatusType.Last);
        }
    }
}