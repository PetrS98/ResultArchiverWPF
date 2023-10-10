using System.Collections.Generic;
using System.IO.Compression;

namespace ResultArchiverWPF.JDOs
{
    public class SettingsJDO
    {
        public string DestinationPath { get; set; } = "";
        public List<string> FileIgnoreFilter { get; set; } = new List<string>();
        public bool DeleteResultAfterArchivate { get; set; } = false;
        public bool DeleteOldestAndNonResultIfNoFreeSpaceOnDrive { get; set; } = true;
        public int MaxAmountOfArchiveFileInFolder { get; set; } = 500;
        public CompressionLevel CompressionLevel { get; set; } = 0;
        public FileCheckerSettingsJDO FileCheckerSettings { get; set; } = new FileCheckerSettingsJDO();
    }
}
