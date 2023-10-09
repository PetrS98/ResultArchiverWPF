using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WPFUtilsLib.Enums;

namespace WPFUtilsLib.TCPIP
{
    public class TCPServer_V1 : IDisposable
    {
        public event Action? ConfigurationChanged;
        public event Action? StatusChanged;
        public event Action? DataReceived;
        public event EventHandler<string>? ClientConnected;

        protected SimpleTcpServer? _server;

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

        public void Start()
        {
            if (_server is not null)
            {
                return;
            }

            Status = Status.Waiting;
            _server = new SimpleTcpServer(IPAddress, Port);
            _server.Events.DataReceived += OnDataReceived;
            _server.Events.ClientConnected += OnClientConnected;
            _server.Start();
            Status = Status.Online;
        }

        private void OnClientConnected(object? sender, ConnectionEventArgs e)
        {
            ClientConnected?.Invoke(sender, e.IpPort);
        }

        public void Stop()
        {
            _server?.Stop();
            if (_server is not null)
            {
                _server.Events.DataReceived -= OnDataReceived;
                _server.Events.ClientConnected -= OnClientConnected;
            }
            _server?.Dispose();
            _server = null;
            Status = Status.Offline;
        }

        public async Task SendAsync(string ipPort, string data)
        {
            if (_server is null) return;

            await _server.SendAsync(ipPort, data);
        }

        public async Task BroadcastAsync(string data)
        {
            if (_server is null) return;

            var clients = _server.GetClients();

            if (clients is null) return;

            List<Task> tasks = new();

            foreach (var client in clients)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        _server.Send(client, data);
                    }
                    catch { }
                }));
            }

            await Task.WhenAll(tasks);
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
                throw new ArgumentException("The server must be offline in order to change it's configuration.");
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
