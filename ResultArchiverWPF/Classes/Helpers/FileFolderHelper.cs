using ResultArchiverWPF.JDOs;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ResultArchiverWPF.Classes.Helpers
{
    static class FileFolderHelper
    {
        public static string GetApplicationFolder()
        {
            string fullPath = AppDomain.CurrentDomain.BaseDirectory;

            if (fullPath is null)
            {
                return "";
            }
            else
            {
                return fullPath;
            }
        }

        public static string GetOldestFileFromDirectory(string extension, string path)
        {
            return new DirectoryInfo(path).GetFiles("*" + extension).MinBy(o => o.CreationTime)!.FullName;
        }

        public static string[] GetAllFile(string path)
        {
            return Directory.GetFiles(path);
        }

        public static string[] GetAllSubDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public static long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public static bool ArchiveFile(FileSystemEventArgs e, string destinationPath, CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            bool error = false;

            try
            {
                App.Logger.Information($"Archivate file: {e.Name}");

                using (ZipArchive archive = ZipFile.Open(destinationPath, ZipArchiveMode.Create))
                {
                    archive.CreateEntryFromFile(e.FullPath, Path.GetFileName(e.FullPath), compressionLevel);
                }

                App.Logger.Information($"Archiving DONE. Archive Path: {destinationPath}");
            }
            catch (Exception ex)
            {
                error = true;

                App.Logger.Error($"Archiving FAILED. Error: {ex.Message}");
                App.Logger.Error($"Watcher ERROR (Stack Trace): {ex.StackTrace}");
            }

            return error;
        }

        public static void DeleteDuplicitFile(string path)
        {
            try
            {
                App.Logger.Information($"Deleting duplicit file. Path: {path}");

                File.Delete(path);

                App.Logger.Information($"Deleting duplicit file DONE.");
            }
            catch (Exception ex)
            {
                App.Logger.Error($"Deleting duplicit file ERROR: {ex.Message}");
                App.Logger.Error($"Deleting duplicit file ERROR (Stack Trace): {ex.StackTrace}");
            }
        }

        public static void DeleteOriginalFile(FileSystemEventArgs e, string destinationPath)
        {
            try
            {
                App.Logger.Information($"Check if new archive exist.");

                if (File.Exists(destinationPath))
                {
                    App.Logger.Information($"New archive exist. Deleting original file. Path: {e.FullPath}");
                    File.Delete(e.FullPath);
                }

                App.Logger.Information($"Deleting DONE.");

            }
            catch (Exception ex)
            {
                App.Logger.Error($"Error while checking or deleting file: {ex.Message}");
                App.Logger.Error($"Error while checking or deleting file (Stack Trace): {ex.StackTrace}");
            }
        }

        public static void DeleteFile(string path)
        {
            try
            {
                App.Logger.Information($"Deleting file. Path: {path}");

                File.Delete(path);

                App.Logger.Information($"Deleting file DONE.");
            }
            catch (Exception ex)
            {
                App.Logger.Error($"Deleting file ERROR: {ex.Message}");
                App.Logger.Error($"Deleting file ERROR (Stack Trace): {ex.StackTrace}");
            }
        }

        public static int CheckAmountOfFile(string extension, string path)
        {
            try
            {
                string[] files = files = Directory.GetFiles(path, "*" + extension);
                return files.Length;
            }
            catch
            {
                return 0;
            }
        }

        public static void DeleteOldestFile(string extension, string destinationPath)
        {
            App.Logger.Information($"Getting oldest file path.");

            string oldestFilePath = GetOldestFileFromDirectory(extension, destinationPath);

            App.Logger.Information($"Oldest file path is: {oldestFilePath}");
            App.Logger.Information($"Deleting oldest file.");

            if (File.Exists(oldestFilePath))
            {
                File.Delete(oldestFilePath);
                App.Logger.Information($"Oldest file deleted.");
            }
            else
            {
                App.Logger.Error($"Oldest file deleting ERROR: File not exist.");
            }
        }

        public static void DeleteNonArchiveResultData(SettingsJDO settings)
        {
            App.Logger.Information($"Deleting all directories, non .zip files and .zip files bigger then {DriveHelper.SizeSuffix(Constants.MAX_SIZE_OF_ARCHIVE_FILE, 2)}");

            string[] allFile = GetAllFile(settings.DestinationPath);
            string[] allSubDirectories = GetAllSubDirectories(settings.DestinationPath);

            foreach (string subDirectory in allSubDirectories)
            {
                Directory.Delete(subDirectory);
            }

            foreach (string file in allFile)
            {
                if (File.Exists(file))
                {
                    if (Path.GetExtension(file) == ".zip")
                    {
                        FileInfo fileInfo = new(file);
                        if (fileInfo.Length > Constants.MAX_SIZE_OF_ARCHIVE_FILE)
                        {
                            DeleteFile(file);
                        }
                    }
                    else
                    {
                        DeleteFile(file);
                    }
                }
            }

            App.Logger.Information($"Deleting DONE.");
        }
    }
}
