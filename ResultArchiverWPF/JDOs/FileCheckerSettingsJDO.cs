namespace ResultArchiverWPF.JDOs
{
    public class FileCheckerSettingsJDO
    {
        public string Path { get; set; } = "";
        public string Extension { get; set; } = "";
        public bool IncludeSubdirectories { get; set; }
    }
}
