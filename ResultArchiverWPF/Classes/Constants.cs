namespace ResultArchiverWPF.Classes
{
    public static class Constants
    {
        public static readonly string MUTEX_APP_NAME = "NexenTire-ResultArchiver_WPF";

        public static readonly string APPLICATION_VERSION = "V 1.0.1";

        public static readonly string SETTINGS_PATH = "settings.json";

        public static readonly string LOG_PATH = "logs/Log.txt";

        public static readonly string[] SIZE_SUFFIXIES = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static readonly long MIN_FREE_SPACE_ON_DRIVE = 5368709120; // In bytes, In GB: 5.3687 GB
        public static readonly long MAX_SIZE_OF_ARCHIVE_FILE = 104857600; // In bytes, In MB: 104.86 MB
    }
}
