namespace db
{
    using System;

    partial class DataClasses1DataContext {
        partial void OnCreated() {
            // Information regarding CommandTimeout found at:
            // http://www.codeproject.com/Articles/26283/Change-The-Default-CommandTimeout-of-LINQ-DataCont
            // http://stackoverflow.com/questions/840334/what-causes-timeout-expired-sqlexceptions-in-linq-to-sql
            //
            this.CommandTimeout = db.Properties.Settings.Default.CommandTimeout;
        }

        #region Extensibility Method Definintions
        //partial void UpdateFile(File instance) {
        //    if (instance.FileName.Length > 0) {
        //        if (System.IO.File.Exists(instance.FileName)) {
        //            bool? CurVal;
        //            CurVal = instance.NeedsProcessing;
        //            //
        //            // Determine if this file needs processing.
        //            //
        //            try {
        //                FileInfo fi = new FileInfo(instance.FileName, this.Connection.ConnectionString);
        //                instance.NeedsProcessing = fi.NeedsProcessing();
        //                this.ExecuteDynamicUpdate(instance);
        //            } catch { }
        //        }
        //    }
        //}
        #endregion

    }

    partial class File
    {
        public override string ToString()
        {
            // return string.Format("File: _id: {0}, _FileName: {1}, _processdate: {2}, _md5: {3}, _date_from: {4}, _date_to: {5}, _data_rows: {6}, _invalid_data: {7}, _DirectoryName: {8}, _Name: {9}, _LastWriteTimeUtc: {10}, _CreationTimeUtc: {11}, _Length: {12}, _NeedsProcessing: {13}, id: {14}, FileName: {15}, processdate: {16}, md5: {17}, date_from: {18}, date_to: {19}, data_rows: {20}, invalid_data: {21}, DirectoryName: {22}, Name: {23}, LastWriteTimeUtc: {24}, CreationTimeUtc: {25}, Length: {26}, NeedsProcessing: {27}", _id, _FileName, _processdate, _md5, _date_from, _date_to, _data_rows, _invalid_data, _DirectoryName, _Name, _LastWriteTimeUtc, _CreationTimeUtc, _Length, _NeedsProcessing, id, FileName, processdate, md5, date_from, date_to, data_rows, invalid_data, DirectoryName, Name, LastWriteTimeUtc, CreationTimeUtc, Length, NeedsProcessing);
            return string.Format("File: id: {0}, LastWriteTimeUtc: {1}, NeedsProcessing: {2}", id, LastWriteTimeUtc, NeedsProcessing);
        }
    }

    partial class Total_Reading {
        partial void OnCreated() {
            if (id == null) {
                id = Guid.NewGuid();
            }
        }
    }

    partial class TReading {
        partial void OnCreated() {
            if (id == null) {
                id = Guid.NewGuid();
            }
        }
    }

    partial class TTotal_Reading {
        
    }

    partial class File {
        partial void OnCreated() {
            if (id == null) {
                id = Guid.NewGuid();
            }
            if (NeedsProcessing == null) {
                NeedsProcessing = true;
                md5 = "";
            }
        }

    }
}
