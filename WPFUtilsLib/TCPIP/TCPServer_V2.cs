using EasyTcp4;
using EasyTcp4.ServerUtils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WPFUtilsLib.Enums;

namespace WPFUtilsLib.TCPIP
{
    public class TCPServer_V2 : IDisposable
    {
        private object _lock = new object();

        public event Action? ConfigurationChanged;
        public event Action? StatusChanged;
        public event Action? DataReceived;

        protected EasyTcpServer? _server;

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

            try
            {
                Status = Status.Waiting;

                _server = new EasyTcpServer();
                _server.OnDataReceive += OnDataReceived;
                _server.Start(IPAddress, (ushort)Port);

                Status = Status.Online;
            }
            catch
            {
                Stop();
            }
        }

        public void Stop()
        {
            if (_server is null)
            {
                return;
            }
            _server.OnDataReceive -= OnDataReceived;
            _server.Dispose();
            _server = null;

            Status = Status.Offline;
        }

        public async Task BroadcastAsync(string data)
        {
            if (_server is null || !_server.IsRunning)
            {
                return;
            }

            List<Task> tasks = new();

            foreach (var client in _server.GetConnectedClients())
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        client.Protocol.SendMessage(client, Encoding.UTF8.GetBytes(data));
                    }
                    catch { }
                }));
            }

            await Task.WhenAll(tasks);
        }

        private void OnDataReceived(object? sender, Message e)
        {
            lock (_lock)
            {
                Data = Encoding.UTF8.GetString(e.Data);
            }
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
