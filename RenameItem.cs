using System;
using System.Collections.Generic;
using System.Text;

namespace uk.andyjohnson.ImageBulkRenamer
{
    public class RenameItem
    {
        public string InputImageFileName { get; set; }
        public DateTime InputImageExifTimestamp { get; set; }
        public DateTime InputImageFileCreatonTimestamp { get; set; }
        public DateTime InputImageFileModificationTimestamp { get; set; }

        public string OutputImageFileName { get; set; }
        public DateTime OutputImageFileTimestamp { get; set; }

        public string InputSidecarFileName { get; set; }
        public string OutputSidecarFileName { get; set; }

        public string RenameStatus { get; set; }


        public static string BuildFileName(DateTime dt)
        {
            return dt.ToString("yyyyMMdd_HHmmss");
        }
    }
}
