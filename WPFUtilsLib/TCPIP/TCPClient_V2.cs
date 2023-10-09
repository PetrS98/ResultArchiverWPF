using EasyTcp4;
using EasyTcp4.ClientUtils;
using EasyTcp4.ClientUtils.Async;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPFUtilsLib.Enums;
using Timer = System.Timers.Timer;

namespace WPFUtilsLib.TCPIP
{
    public class TCPClient_V2 : IDisposable
    {
        public event Action? StatusChanged;
        public event Action? ReconnectingChanged;
        public event Action? ConfigurationChanged;
        public event Action? DataReceived;

        private readonly Timer _timer = new();
        private CancellationTokenSource? _cts;

        private EasyTcpClient? _client;

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
                /*if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke();
                }*/

                _status = value;
                StatusChanged?.Invoke();
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

        private bool _reconnectEnabled = false;
        public bool ReconnectEnabled
        {
            get { return _reconnectEnabled; }
            set
            {
                if (_reconnectEnabled != value)
                {
                    _reconnectEnabled = value;
                    ReconnectingChanged?.Invoke();
                }
            }
        }

        public TCPClient_V2()
        {
            _timer.Interval = 500;
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = false;
        }

        private async void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                await CheckConnection();
            }
            catch
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async Task CheckConnection()
        {
            if (Status == Status.Offline && ReconnectEnabled)
            {
                Status = Status.Waiting;
            }

            var status = await CheckStatusAsync();

            if (_cts is not null)
            {
                _cts.Token.ThrowIfCancellationRequested();
            }

            if (!ReconnectEnabled)
            {
                Status = status;
                _timer.Start();
                return;
            }

            if (Status != Status.Online && status == Status.Online)
            {
                Disconnect();
                await ConnectAsync();
            }
            Status = status;
            _timer.Start();
        }

        public void Connect()
        {
            if (_client is not null)
            {
                return;
            }

            Status = Status.Waiting;
            _client = new EasyTcpClient();
            _client.OnDataReceive += OnDataReceived;
            try
            {
                _client.Connect(IPAddress, (ushort)Port);
            }
            catch
            {
                Disconnect();
            }

            _timer.Start();
        }

        public async Task ConnectAsync()
        {
            if (_client is not null)
            {
                return;
            }

            Status = Status.Waiting;
            _client = new EasyTcpClient();
            _client.OnDataReceive += OnDataReceived;

            _cts = new CancellationTokenSource();

            try
            {
                await _client.ConnectAsync(IPAddress, (ushort)Port);
                _cts.Token.ThrowIfCancellationRequested();
            }
            catch
            {
                Disconnect();
                _cts?.Dispose();
                _cts = null;
                return;
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }

            _timer.Start();
        }

        public void CancelConnecting()
        {
            if (_cts is null)
            {
                return;
            }

            if (_cts.Token.CanBeCanceled)
            {
                _cts.Cancel();
            }
        }

        public void Disconnect()
        {
            CancelConnecting();
            _timer.Stop();
            if (_client is null)
            {
                return;
            }

            _client.OnDataReceive -= OnDataReceived;
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

        private async Task<Status> CheckStatusAsync()
        {
            using var client = new EasyTcpClient();
            try
            {
                await client.ConnectAsync(IPAddress, (ushort)Port);
            }
            catch { }

            return client.IsConnected() ? Status.Online : Status.Offline;
        }

        private void OnDataReceived(object? sender, Message e)
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
