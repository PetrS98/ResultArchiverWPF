using Newtonsoft.Json;
using ResultArchiverWPF.JDOs;
using System.IO;

namespace ResultArchiverWPF.Classes.Helpers
{
    public static class SettingsHelper
    {
        public static SettingsJDO ReadSettings(string path)
        {
            App.Logger.Information($@"Read Configuration File from: {FileFolderHelper.GetApplicationFolder()}\{path}");

            SettingsJDO? _settings = null;

            try
            {
                _settings = JsonConvert.DeserializeObject<SettingsJDO>(File.ReadAllText(path));
            }
            catch { }

            if (_settings == null)
            {
                _settings = new();
                File.WriteAllText(path, JsonConvert.SerializeObject(_settings, Formatting.Indented));

                App.Logger.Fatal("Read Configuration File FAILED. Created default configuration file.");
            }

            return _settings;
        }

        public static bool CheckSettings(SettingsJDO settings)
        {
            App.Logger.Information("Check Configuration");

            bool error = true;

            if (Directory.Exists(settings!.FileCheckerSettings.Path) == false)
            {
                App.Logger.Fatal("File checker folder path do not exist.");
                error = false;
            }

            if (Directory.Exists(settings!.DestinationPath) == false)
            {
                App.Logger.Fatal("Destination folder path do not exist.");
                error = false;
            }

            if (settings.FileCheckerSettings.Extension == "")
            {
                App.Logger.Fatal("Extension is not set.");
                error = false;
            }

            if (error == false)
            {
                App.Logger.Fatal("CONFIG ERROR. CLOSING APPLICATION");
                return error;
            }

            App.Logger.Information("Configuration is OK.");
            return error;
        }
    }
}
