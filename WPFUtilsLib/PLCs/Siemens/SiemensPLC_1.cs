using Sharp7;
using System;
using System.Threading.Tasks;
using WPFUtilsLib.Enums;
using Timer = System.Timers.Timer;

namespace WPFUtilsLib.PLCs.Siemens
{
    public class SiemensPLC_1
    {
        private object _lock = new();

        private byte LiveUIntSameCounter = 0;

        public Action? LiveUIntNotChanged { get; set; }

        public event EventHandler<ushort>? LiveUIntChanged;
        public event EventHandler<Status>? StatusChanged;

        public event EventHandler<int>? ConnectionStatusCodeChanged;

        public event EventHandler<int>? ReadStatusCodeChanged;
        public event EventHandler<int>? WriteStatusCodeChanged;
        public event EventHandler<int>? LiveUIntStatusCodeChanged;

        public event EventHandler<byte[]?>? DataBufferReceived_1;
        public event EventHandler<byte[]?>? DataBufferReceived_2;
        public event EventHandler<byte[]?>? DataBufferReceived_3;

        public event Action? UpdateDataForSending;

        private readonly S7Client client = new();

        private readonly Timer timerUpdating = new();
        private readonly Timer timerReconnecting = new();

        private int connectionStatusCode = 666;
        private int readStatusCode = 666;
        private int writeStatusCode = 666;
        private int liveUIntStatusCode = 666;

        private ushort liveUInt = 0;
        private bool reconnectEnabled = false;

        private byte[]? readDataBuffer_1 = null;
        private byte[]? readDataBuffer_2 = null;
        private byte[]? readDataBuffer_3 = null;

        private byte[]? writeDataBuffer = null;
        private Status status;

        public int ConnectionStatusCode
        {
            get => connectionStatusCode;
            private set
            {
                bool changed = connectionStatusCode != value;
                connectionStatusCode = value;
                if (changed) ConnectionStatusCodeChanged?.Invoke(this, value);
            }
        }

        public int ReadStatusCode
        {
            get => readStatusCode;
            private set
            {
                bool changed = readStatusCode != value;
                readStatusCode = value;
                if (changed) ReadStatusCodeChanged?.Invoke(this, value);
            }
        }

        public int WriteStatusCode
        {
            get => writeStatusCode;
            private set
            {
                bool changed = writeStatusCode != value;
                writeStatusCode = value;
                if (changed) WriteStatusCodeChanged?.Invoke(this, value);
            }
        }

        public int LiveUIntStatusCode
        {
            get => liveUIntStatusCode;
            private set
            {
                bool changed = liveUIntStatusCode != value;
                liveUIntStatusCode = value;
                if (changed) LiveUIntStatusCodeChanged?.Invoke(this, value);
            }
        }

        public ushort LiveUInt
        {
            get => liveUInt;
            private set
            {
                if (liveUInt == value && Status == Status.Online)
                {
                    LiveUIntSameCounter++;
                    LiveUIntNotChanged?.Invoke();
                }
                else
                {
                    LiveUIntSameCounter = 0;
                }

                if (LiveUIntSameCounter >= 3)
                {
                    Status = Status.Offline;
                    LiveUIntSameCounter = 0;
                }
                else
                {
                    Status = Status.Online;
                }

                //Status = liveUInt != value ? Status.Online : Status.Offline;

                liveUInt = value;
                if (Status == Status.Online) LiveUIntChanged?.Invoke(this, value);
            }
        }

        public Status Status
        {
            get => status;
            private set
            {
                bool changed = status != value;
                status = value;
                StatusChanged?.Invoke(this, value);
                if (!changed) return;
                if (value == Status.Online) timerUpdating.Start();
                else timerUpdating.Stop();
                UpdateReconnectingTimer();
                //StatusChanged?.Invoke(this, value);
            }
        }

        public int UpdateInterval { get => (int)timerUpdating.Interval; set => timerUpdating.Interval = value; }
        public int ReconnectInterval { get => (int)timerReconnecting.Interval; set => timerReconnecting.Interval = value; }
        public bool ReconnectEnabled { get => reconnectEnabled; set { reconnectEnabled = value; UpdateReconnectingTimer(); } }

        public bool ReadEnable_2 { get; set; } = false;
        public bool ReadEnable_3 { get; set; } = false;

        public byte[]? ReadDataBuffer_1 { get => readDataBuffer_1; private set { readDataBuffer_1 = value; DataBufferReceived_1?.Invoke(this, value); } }
        public byte[]? ReadDataBuffer_2 { get => readDataBuffer_2; private set { readDataBuffer_2 = value; DataBufferReceived_2?.Invoke(this, value); } }
        public byte[]? ReadDataBuffer_3 { get => readDataBuffer_3; private set { readDataBuffer_3 = value; DataBufferReceived_3?.Invoke(this, value); } }

        public byte[]? WriteDataBuffer { get { return writeDataBuffer; } set { writeDataBuffer = value; } }

        public string IPAddress { get; set; } = "192.168.1.25";
        public int Rack { get; set; } = 0;
        public int Slot { get; set; } = 1;

        #region Read Settings
        #region Read 1

        public int ReadDBNumber_1 { get; set; } = 1;
        public int ReadDataBufferOffset_1 { get; set; } = 0;
        public int ReadDataBufferSize_1 { get; set; } = 1;

        #endregion
        #region Read 2
        public int ReadDBNumber_2 { get; set; } = 1;
        public int ReadDataBufferOffset_2 { get; set; } = 0;
        public int ReadDataBufferSize_2 { get; set; } = 1;

        #endregion
        #region Read 3

        public int ReadDBNumber_3 { get; set; } = 1;
        public int ReadDataBufferOffset_3 { get; set; } = 0;
        public int ReadDataBufferSize_3 { get; set; } = 1;

        #endregion
        #endregion
        #region Write Settings

        public int WriteDBNumber { get; set; } = 1;
        public int WriteDataBufferOffset { get; set; } = 0;
        public int WriteDataBufferSize { get; set; } = 1;

        #endregion
        #region Live UInt Settings
        public int LiveUIntDBNumber { get; set; } = 0;
        public int LiveUIntOffset { get; set; } = 0;
        public int LiveUIntBufferSize { get; set; } = 1;

        #endregion

        public SiemensPLC_1()
        {
            timerUpdating.Interval = 150;
            timerReconnecting.Interval = 5000;
            timerUpdating.Elapsed += UpdateData;
            timerReconnecting.Elapsed += TryReconnect;

            Status = Status.Offline;
        }

        public void Connect()
        {
            Disconnect();

            Status = Status.Waiting;
            ConnectionStatusCode = client.ConnectTo(IPAddress, Rack, Slot);
            Status = ConnectionStatusCode == 0 ? Status.Online : Status.Offline;
        }

        public async Task ConnectAsync() => await Task.Run(Connect);
        private void TryReconnect(object? sender, EventArgs e) => Connect();

        public void Disconnect()
        {
            client.Disconnect();
            Status = Status.Offline;
        }

        private void UpdateReconnectingTimer()
        {
            if (reconnectEnabled && status == Status.Offline) timerReconnecting.Start();
            else timerReconnecting.Stop();
        }

        private void UpdateData(object? sender, EventArgs e)
        {
            if (Status != Status.Online)
            {
                return;
            }

            ReadLiveUIntFromPLC();

            ReadDataFromPLC_1();

            if (ReadEnable_2) ReadDataFromPLC_2();
            if (ReadEnable_3) ReadDataFromPLC_3();

            WriteDataToPLC();
        }

        private void ReadLiveUIntFromPLC()
        {
            lock (_lock)
            {
                byte[] buffer = new byte[LiveUIntBufferSize];

                LiveUIntStatusCode = client.DBRead(LiveUIntDBNumber, LiveUIntOffset, LiveUIntBufferSize, buffer);
                Status = LiveUIntStatusCode == 0 ? Status.Online : Status.Offline;
                if (Status == Status.Offline) return;

                LiveUInt = S7.GetUIntAt(buffer, 0);
            }
        }

        private void ReadDataFromPLC_1()
        {
            lock (_lock)
            {
                byte[] buffer = new byte[ReadDataBufferSize_1];

                ReadStatusCode = client.DBRead(ReadDBNumber_1, ReadDataBufferOffset_1, ReadDataBufferSize_1, buffer);
                ReadDataBuffer_1 = buffer;
            }
        }

        private void ReadDataFromPLC_2()
        {
            lock (_lock)
            {
                byte[] buffer = new byte[ReadDataBufferSize_2];

                ReadStatusCode = client.DBRead(ReadDBNumber_2, ReadDataBufferOffset_2, ReadDataBufferSize_2, buffer);
                ReadDataBuffer_2 = buffer;
            }
        }

        private void ReadDataFromPLC_3()
        {
            lock (_lock)
            {
                byte[] buffer = new byte[ReadDataBufferSize_3];

                ReadStatusCode = client.DBRead(ReadDBNumber_3, ReadDataBufferOffset_3, ReadDataBufferSize_3, buffer);
                ReadDataBuffer_3 = buffer;
            }
        }

        private void WriteDataToPLC()
        {
            UpdateDataForSending?.Invoke();

            lock (_lock)
            {
                byte[] buffer = new byte[WriteDataBufferSize];

                if (WriteDataBuffer != null)
                {
                    buffer = WriteDataBuffer;
                }

                try
                {
                    WriteStatusCode = client.DBWrite(WriteDBNumber, WriteDataBufferOffset, WriteDataBufferSize, buffer);
                }
                catch
                {
                }


            }
        }

        public string GetErrorMessage(int ErrorCode)
        {
            return client.ErrorText(ErrorCode);
        }
    }
}
