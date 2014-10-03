using System;
using System.Collections.Generic;
using System.Linq;
using db;
using System.Threading.Tasks;

namespace wwd_utils
{
    public class SiteFiles {
        public List<SiteFile> Files;
        private AppSettings settings;

        public SiteFiles() {
            settings = new AppSettings("FileMonitor.xml");
            Files = new List<SiteFile>();
            LoadAllFiles();
        }

        private void LoadAllFiles() {
            DataClasses1DataContext dcc = new DataClasses1DataContext(settings.ConnectionString);
            var recs = (from f in dcc.Files
                       select f.id);

            Parallel.ForEach(recs, rec => {
                SiteFile s = new SiteFile(rec);
                if (s.Need2Process)
                    Files.Add(s);
            }
            );

/*
 *          foreach (var rec in recs) {
                SiteFile s = new SiteFile(rec);
                if ( s.Need2Process ) 
                    Files.Add(s);
            }
 */
        }

    }
}
