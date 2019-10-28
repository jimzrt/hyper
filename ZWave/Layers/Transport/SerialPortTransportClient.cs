using System;
using System.Threading;
using Utils;
using ZWave.Enums;

namespace ZWave.Layers.Transport
{
    public class SerialPortTransportClient : TransportClientBase
    {
        public override event Action<ITransportClient> Connected;
        public override event Action<ITransportClient> Disconnected;

        public override bool IsOpen
        {
            get { return _serialPortProvider != null ? _serialPortProvider.IsOpen : false; }
        }

        private ISerialPortProvider _serialPortProvider;
        public ISerialPortProvider SerialPortProvider
        {
            get { return _serialPortProvider ?? (_serialPortProvider = new SerialPortProvider()); }
            set { _serialPortProvider = value; }
        }

        private Thread _workerThread;
        private readonly object _lockObject = new object();
        private bool _isCancelled = false;
        private readonly byte[] _buffer = new byte[Transport.SerialPortProvider.BUFFER_SIZE];
        private readonly byte[] _ack = { 0x06 };
        private readonly AutoResetEvent _readDataSignal = new AutoResetEvent(false);
        private readonly Action<DataChunk> _transmitCallback;
        public SerialPortTransportClient(Action<DataChunk> transmitCallback)
        {
            _transmitCallback = transmitCallback;
        }

        protected override CommunicationStatuses InnerConnect(IDataSource ds)
        {
            var ret = CommunicationStatuses.Busy;
            DataSource = ds;
            InnerDisconnect();
            _readDataSignal.Reset();
            if (ds != null && ds is SerialPortDataSource)
            {
                var spds = (SerialPortDataSource)ds;
                if (SerialPortProvider.Open(spds.SourceName, (int)spds.BaudRate, PInvokeParity.None, 8, spds.StopBits))
                {
                    ret = CommunicationStatuses.Done;
                    SerialPortProvider.ReadExisting();
                    if (Connected != null)
                    {
                        Connected(this);
                    }
                    _isCancelled = false;
                    _workerThread = new Thread(DoWork);
                    _workerThread.Name = "Serial Port Client";
                    _workerThread.IsBackground = true;
                    _workerThread.Start();
                }
                "{0:X2} {1} {2}@{3}.{4} {5}"._DLOG(SessionId, ApiType, spds.SourceName, spds.BaudRate, spds.StopBits, ret);
            }
            Thread.Sleep(200); // Wait after connect (Zniffer may not respond with GetFrequencies).
            return ret;
        }

        protected override void InnerDisconnect()
        {
            lock (_lockObject)
            {
                _isCancelled = true;
            }

            SerialPortProvider.Close();
            if (_workerThread != null && Thread.CurrentThread.ManagedThreadId != _workerThread.ManagedThreadId)
            {
                _workerThread.Join();
            }
            if (Disconnected != null)
            {
                Disconnected(this);
            }
            var spds = (SerialPortDataSource)DataSource;
            if (spds != null)
            {
                "{0:X2} {1} {2}@{3}.{4}"._DLOG(SessionId, ApiType, spds.SourceName, spds.BaudRate, spds.StopBits);
            }
        }

        protected override int InnerWriteData(byte[] data)
        {
            if (!SuppressDebugOutput)
            {
                "{0:X2} {1} {2} >> {3}"._DLOG(SessionId, ApiType, DataSource.SourceName, Tools.GetHex(data));
            }
            var dc = new DataChunk(data, SessionId, true, ApiType);
            if (_transmitCallback != null)
            {
                _transmitCallback(dc);
            }

            return SerialPortProvider.Write(data, data.Length);
        }

        private void DoWork()
        {
            while (!_isCancelled)
            {
                var readData = SerialPortProvider.Read(_buffer, (int)Transport.SerialPortProvider.BUFFER_SIZE);
                if (readData > 0)
                {
                    var data = new byte[readData];
                    Array.Copy(_buffer, 0, data, 0, readData);
                    if (!SuppressDebugOutput)
                    {
                        "{0:X2} {1} {2} << {3}"._DLOG(SessionId, ApiType, SerialPortProvider.PortName, Tools.GetHex(data));
                    }
                    DataChunk dc = new DataChunk(data, SessionId, false, ApiType);
                    if (_transmitCallback != null)
                    {
                        _transmitCallback(dc);
                    }
                    if (ReceiveDataCallback != null)
                    {
                        ReceiveDataCallback(dc, false);
                    }
                }
            }
        }

        protected override void InnerDispose()
        {
            InnerDisconnect();
        }

#if NETCOREAPP
        public static string[] GetPortNames()
        {
            return Transport.SerialPortProvider.GetPortNames();
        }
#endif
    }
}
