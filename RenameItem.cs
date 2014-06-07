using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageBulkRenamer
{
    public class RenameItem
    {
        public RenameItem()
        {
        }


        public RenameItem(RenameItem item)
        {
            this.InputFileName = item.InputFileName;
            this.InputExifTimestamp = item.InputExifTimestamp;
            this.InputFileCreatonTimestamp = item.InputFileCreatonTimestamp;
            this.InputFileModificationTimestamp = item.InputFileModificationTimestamp;
            this.OutputFileName = item.OutputFileName;
            this.OutputFileTimestamp = item.OutputFileTimestamp;
            this.RenameStatus = item.RenameStatus;
        }


        public string InputFileName { get; set; }
        public DateTime InputExifTimestamp { get; set; }
        public DateTime InputFileCreatonTimestamp { get; set; }
        public DateTime InputFileModificationTimestamp { get; set; }

        public string OutputFileName { get; set; }
        public DateTime OutputFileTimestamp { get; set; }

        public string RenameStatus { get; set; }


        public static string BuildFileName(DateTime dt)
        {
            return dt.ToString("yyyyMMdd_HHmmss");
        }
    }
}
