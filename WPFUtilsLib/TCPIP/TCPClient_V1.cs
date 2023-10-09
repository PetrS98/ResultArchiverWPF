using SuperSimpleTcp;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using WPFUtilsLib.Enums;

namespace WPFUtilsLib.TCPIP
{
    public class TCPClient_V1 : IDisposable
    {
        public event Action? ConfigurationChanged;
        public event Action? StatusChanged;
        public event Action? DataReceived;

        private Timer _timer = new Timer();

        private SimpleTcpClient? _client;

        private string _ipAddress = "127.0.0.1";
        public string IPAddress
        {
            get { return _ipAddress; }
            set
            {
                ThrowIfNotOffline();
                if (_ipAddress != value)
                {
                    _ipAddress = value;
                    OnConfigurationChanged();
                }
            }
        }

        private int _port = 8080;
        public int Port
        {
            get { return _port; }
            set
            {
                ThrowIfNotOffline();

                if (value < 0 || value > 65535)
                {
                    throw new ArgumentOutOfRangeException(nameof(Port), $"Port must be in the range 0-65535 ({value} given).");
                }

                if (_port != value)
                {
                    _port = value;
                    OnConfigurationChanged();
                }
            }
        }

        private Status _status = Status.Offline;
        public Status Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke();
                }
            }
        }

        private string? _data;
        public string? Data
        {
            get { return _data; }
            private set
            {
                _data = value;
                DataReceived?.Invoke();
            }
        }

        public TCPClient_V1()
        {
            _timer.Interval = 1000;
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            using SimpleTcpClient client = new(IPAddress, Port);

            try
            {
                client.Connect();
                if (client.IsConnected == false)
                {
                    Status = Status.Offline;
                }
            }
            catch
            {
                Status = Status.Offline;
            }
        }

        public void Connect()
        {
            Disconnect();
            if (_client is not null)
            {
                return;
            }

            Status = Status.Waiting;
            _client = new SimpleTcpClient(IPAddress, Port);
            _client.Events.DataReceived += OnDataReceived;
            _client.Events.Connected += OnConnected;
            _client.Events.Disconnected += OnDisconnected;
            try
            {
                _client.Connect();
                _timer.Start();
            }
            catch
            {
                Disconnect();
            }
        }

        public async Task ConnectAsync()
        {
            await Task.Run(Connect);
        }

        public void Disconnect()
        {
            if (_client is null)
            {
                return;
            }

            _client.Events.DataReceived -= OnDataReceived;
            _client.Events.Connected -= OnConnected;
            _client.Events.Disconnected -= OnDisconnected;
            _client.Disconnect();
            _client.Dispose();
            _client = null;
            Status = Status.Offline;
            _timer.Stop();
        }

        public async Task DisconnectAsync()
        {
            if (_client is null)
            {
                return;
            }

            _client.Events.DataReceived -= OnDataReceived;
            _client.Events.Connected -= OnConnected;
            _client.Events.Disconnected -= OnDisconnected;
            await _client.DisconnectAsync();
            _client.Dispose();
            _client = null;
            Status = Status.Offline;
        }

        public void SendData(string data)
        {
            if (Status != Status.Online)
            {
                throw new InvalidOperationException("The client is not connected.");
            }

            _client!.Send(data);
        }

        private void OnDisconnected(object? sender, ConnectionEventArgs e)
        {
            Disconnect();
        }

        private void OnConnected(object? sender, ConnectionEventArgs e)
        {
            Status = Status.Online;
        }

        private void OnDataReceived(object? sender, DataReceivedEventArgs e)
        {
            Data = Encoding.UTF8.GetString(e.Data);
        }

        protected void OnConfigurationChanged()
        {
            ConfigurationChanged?.Invoke();
        }

        protected void ThrowIfNotOffline()
        {
            if (Status != Status.Offline)
            {
                throw new ArgumentException("The client must be offline in order to change it's configuration.");
            }
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}
