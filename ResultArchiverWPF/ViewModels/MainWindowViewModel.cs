using ResultArchiverWPF.Classes.Helpers;
using ResultArchiverWPF.JDOs;
using System;
using System.IO;
using System.Threading;
using Constants = ResultArchiverWPF.Classes.Constants;

namespace ResultArchiverWPF.ViewModels
{
    public class MainWindowViewModel
    {
        public string ApplicationVersion { get; set; } = "";

        private readonly SettingsJDO _settings;

        private readonly FileSystemWatcher? _fileSystemWatcher;

        public MainWindowViewModel(SettingsJDO settings)
        {
            ApplicationVersion = Constants.APPLICATION_VERSION;

            _settings = settings;

            ShowFileFilterInformation(_settings);

            _fileSystemWatcher = CreateWatcher(_settings.FileCheckerSettings);
        }

        private static void ArchiveFileIfNotExist(string destinationPath, FileSystemEventArgs e, SettingsJDO settings)
        {
            if (File.Exists(destinationPath))
            {
                App.Logger.Warning($"Archive file already exist. Path: {destinationPath}");
                App.Logger.Warning($"Archiving file is skipped.");

                if (settings.DeleteResultAfterArchivate)
                {
                    FileFolderHelper.DeleteFile(e.FullPath);
                }
            }
            else
            {
                bool archiveFileError = FileFolderHelper.ArchiveFile(e, destinationPath, settings.CompressionLevel);

                if (settings.DeleteResultAfterArchivate && archiveFileError == false)
                {
                    FileFolderHelper.DeleteOriginalFile(e, destinationPath);
                }
            }
        }

        public static void DeleteOldestFileIfAmountBiggerThenSet(SettingsJDO settings)
        {
            if (Directory.Exists(settings.DestinationPath))
            {
                App.Logger.Information($"Checking amounth of file in destination folder.");

                int amountOfFile = FileFolderHelper.CheckAmountOfFile(".zip", settings.DestinationPath);

                App.Logger.Information($"Amounth of file is: {amountOfFile}");

                if (amountOfFile >= settings.MaxAmountOfArchiveFileInFolder)
                {
                    App.Logger.Warning($"Amounth of file is bigger or equal then set. Set is: {settings.MaxAmountOfArchiveFileInFolder}");
                    App.Logger.Information($"Deleting all file above set value. Set is: {settings.MaxAmountOfArchiveFileInFolder}");

                    for (int i = settings.MaxAmountOfArchiveFileInFolder - 1; i < amountOfFile; i++)
                    {
                        FileFolderHelper.DeleteOldestFile(".zip", settings.DestinationPath);
                    }
                }
            }
        }

        private FileSystemWatcher CreateWatcher(FileCheckerSettingsJDO settings)
        {
            App.Logger.Information($"Setuping File/Folder Change Watcher");

            try
            {
                var watcher = new FileSystemWatcher(settings.Path);

                watcher.NotifyFilter = NotifyFilters.FileName;

                watcher.Created += OnCreated;
                watcher.Error += Watcher_Error;

                watcher.Filter = "*" + settings.Extension;
                watcher.IncludeSubdirectories = settings.IncludeSubdirectories;
                watcher.EnableRaisingEvents = true;

                App.Logger.Information($"Setuping File/Folder Change Watcher DONE");

                return watcher;
            }
            catch (Exception ex)
            {
                App.Logger.Error($"Setuping File/Folder Change Watcher ERROR: {ex.Message}");
                App.Logger.Error($"Setuping File/Folder Change Watcher ERROR (Stack Trace): {ex.StackTrace}");
                return new FileSystemWatcher();
            }
        }

        private void Watcher_Error(object sender, ErrorEventArgs e)
        {
            App.Logger.Error($"Watcher ERROR: {e.GetException().Message}");
            App.Logger.Error($"Watcher ERROR (Stack Trace): {e.GetException().StackTrace}");
        }

        private void ShowFileFilterInformation(SettingsJDO settings)
        {
            int fileIgnoreFilterCount = settings.FileIgnoreFilter.Count;

            if (fileIgnoreFilterCount > 0)
            {
                App.Logger.Information($"File filter is Active. Amount of items in filter is: {fileIgnoreFilterCount}");

                foreach (string item in settings.FileIgnoreFilter)
                {
                    App.Logger.Information($"No operations will be performed on the following file. File Name: {item}");
                }
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            foreach (string item in _settings.FileIgnoreFilter)
            {
                if ((item + _settings.FileCheckerSettings.Extension) == e.Name)
                {
                    return;
                }
            }

            App.Logger.Information("-----------------------------------------------------------------------");
            App.Logger.Information($"New File Created: {e.FullPath}");

            string destinationPath = Path.ChangeExtension(_settings.DestinationPath + @"\\" + Path.GetFileName(e.FullPath), ".zip");

            DeleteOldestFileIfAmountBiggerThenSet(_settings);

            Thread.Sleep(1000);

            if (DriveHelper.CheckIfDriveFreeSpaceOK(_settings, e.FullPath))
            {
                ArchiveFileIfNotExist(destinationPath, e, _settings);
            }
            else
            {
                if (_settings.DeleteOldestAndNonResultIfNoFreeSpaceOnDrive)
                {
                    FileFolderHelper.DeleteNonArchiveResultData(_settings);

                    while (DriveHelper.CheckIfDriveFreeSpaceOK(_settings, e.FullPath) == false)
                    {
                        File.Delete(FileFolderHelper.GetOldestFileFromDirectory(".zip", _settings.DestinationPath));
                    }

                    ArchiveFileIfNotExist(destinationPath, e, _settings);
                }
            }
        }
    }
}
