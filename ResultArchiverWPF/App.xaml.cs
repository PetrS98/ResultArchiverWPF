using ResultArchiverWPF.Classes.Helpers;
using ResultArchiverWPF.JDOs;
using ResultArchiverWPF.ViewModels;
using Serilog;
using System.Threading;
using System.Windows;
using Constants = ResultArchiverWPF.Classes.Constants;

namespace ResultArchiverWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? _mutex = null;

        public static ILogger Logger = new LoggerConfiguration()
                .WriteTo.File(path: Constants.LOG_PATH, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day)
                .CreateLogger();

        private SettingsJDO _settings = new();

        public MainWindowViewModel MainWindowViewModel { get; set; }

        public App()
        {
            _mutex = new Mutex(true, Constants.MUTEX_APP_NAME, out bool createdNew);
            if (!createdNew)
            {
                Shutdown();
            }

            Logger.Information("Program Starting.");

            ReadAndCheckSettings();

            MainWindowViewModel = new MainWindowViewModel(_settings);
        }

        private void ReadAndCheckSettings()
        {
            _settings = SettingsHelper.ReadSettings(Constants.SETTINGS_PATH);
            bool settingsOK = SettingsHelper.CheckSettings(_settings);

            if (settingsOK == false)
            {
                Shutdown();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow
            {
                DataContext = MainWindowViewModel
            };

            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Logger.Information("Program Ending.");
        }

    }
}
