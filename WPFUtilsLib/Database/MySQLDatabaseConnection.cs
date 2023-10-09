using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUtilsLib.Enums;
using Timer = System.Timers.Timer;

namespace WPFUtilsLib.Database
{
    public class MySQLDatabaseConnection
    {
        public event Action? StatusChanged;
        public event Action? ReconnectingChanged;
        public event Action? ConfigurationChanged;
        public event Action? NewErrorMessage;

        private readonly Timer _timer = new();

        private Status _status = Status.Offline;
        public Status Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                StatusChanged?.Invoke();

                /*if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke();
                }*/
            }
        }

        private bool _reconnectEnabled = false;
        public bool ReconnectEnabled
        {
            get { return _reconnectEnabled; }
            set
            {
                _reconnectEnabled = value;
                if (value)
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
                ReconnectingChanged?.Invoke();
            }
        }

        private string? _errorMessage;
        public string? ErrorMessage
        {
            get { return _errorMessage; }
            protected set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    NewErrorMessage?.Invoke();
                }
            }
        }

        private string _ipAddress = "127.0.0.1";
        public string IPAddress
        {
            get { return _ipAddress; }
            set 
            { 
                _ipAddress = value;
                ConfigurationChanged?.Invoke();
            }
        }

        private string _database = "db_curing";
        public string Database
        {
            get { return _database; }
            set
            {
                _database = value;
                ConfigurationChanged?.Invoke();
            }
        }

        private string _userID = "root";
        public string UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                ConfigurationChanged?.Invoke();
            }
        }

        private string _password = "";
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                ConfigurationChanged?.Invoke();
            }
        }

        public MySQLDatabaseConnection()
        {
            _timer.Interval = 500;
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;
        }

        private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (!ReconnectEnabled)
            {
                return;
            }

            if (Status == Status.Offline)
            {
                Status = Status.Waiting;
            }
            var status = Task.Run(GetStatusAsync).Result;
            
            if (ReconnectEnabled)
            {
                Status = status;
                _timer.Start();
            }
            else
            {
                Status = Status.Offline;
            }
        }

        public async Task ConnectAsync()
        {
            Status = Status.Waiting;
            Status = await GetStatusAsync();
        }

        public void Disconnect()
        {
            Status = Status.Offline;
        }

        public MySqlConnection CreateConnection()
        {
            string connectionString = $"Server={IPAddress};User ID={UserID};Password={Password};Database={Database}";
            return new MySqlConnection(connectionString);
        }

        public async Task<Status> GetStatusAsync()
        {
            using var connection = CreateConnection();

            try
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Status.Offline;
            }

            if (connection.State.HasFlag(ConnectionState.Open))
            {
                return Status.Online;
            }
            else if (connection.State.HasFlag(ConnectionState.Connecting))
            {
                return Status.Waiting;
            }
            return Status.Offline;
        }
    }
}
