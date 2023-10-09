using ResultArchiverWPF.JDOs;
using System;
using System.IO;

namespace ResultArchiverWPF.Classes.Helpers
{
    public static class DriveHelper
    {
        public static long GetTotalDriveFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.TotalFreeSpace;
                }
            }
            return -1;
        }

        public static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                Constants.SIZE_SUFFIXIES[mag]);
        }

        public static bool CheckIfDriveFreeSpaceOK(SettingsJDO settings, string filePath)
        {
            long driveFreeSpace = 0;

            App.Logger.Information($"Check if folder/drive exist.");

            if (Directory.Exists(settings.DestinationPath))
            {
                App.Logger.Information($"Folder/Drive exist.");

                string pathRoot = Path.GetPathRoot(settings.DestinationPath)!;

                App.Logger.Information($"Getting free space on drive. Drive: {pathRoot}");

                driveFreeSpace = GetTotalDriveFreeSpace(pathRoot);

                App.Logger.Information($"Free space on drive is: {SizeSuffix(driveFreeSpace, 2)}");
                App.Logger.Information($"Getting new file size.");

                long fileSize = FileFolderHelper.GetFileSize(filePath);

                App.Logger.Information($"New file size is: {SizeSuffix(fileSize, 2)}");
                App.Logger.Information($"Min free space on drive is: {SizeSuffix(Constants.MIN_FREE_SPACE_ON_DRIVE, 2)}");
                App.Logger.Information($"Checking if free space is sufficient for new file.");

                if (driveFreeSpace >= Constants.MIN_FREE_SPACE_ON_DRIVE)
                {
                    App.Logger.Information($"Free space is sufficient for new file.");
                    return true;
                }
                else
                {
                    App.Logger.Warning($"Free space is NOT sufficient for new file.");
                }
            }
            else
            {
                App.Logger.Error($"Folder/Drive NOT exist");
            }

            return false;
        }
    }
}
