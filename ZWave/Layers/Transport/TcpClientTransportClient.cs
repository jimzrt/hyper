using System;
using System.Threading;
using Utils;
using ZWave.Enums;

namespace ZWave.Layers.Transport
{
    public class TcpClientTransportClient : TransportClientBase
    {
        public override event Action<ITransportClient> Connected;
        public override event Action<ITransportClient> Disconnected;

        private TcpConnection _tcpConnection;

        private Thread _workerThread;
        private readonly object _lockObject = new object();
        private volatile bool _isStoped = true;
        private SocketDataSource _dataSource;
        private readonly Action<DataChunk> _transmitCallback;

        public override bool IsOpen
        {
            get { return _tcpConnection.Connected; }
        }

        public TcpClientTransportClient(Action<DataChunk> transmitCallback)
        {
            _transmitCallback = transmitCallback;
        }

        protected override CommunicationStatuses InnerConnect(IDataSource dataSource)
        {
            InnerDisconnect();
            lock (_lockObject)
            {
                CommunicationStatuses ret = CommunicationStatuses.Busy;
                DataSource = dataSource;
                if (dataSource != null &&
                    dataSource is SocketDataSource &&
                    dataSource.Validate()
                    )
                {
                    _dataSource = (SocketDataSource)dataSource;
                    _tcpConnection = new TcpConnection();
                    int attempts = 1;
                    while (!_tcpConnection.Connect(_dataSource.SourceName, _dataSource.Port) &&
                        attempts > 0
                        )
                    {
                        attempts--;
                        Thread.Sleep(100);
                        "{0:X2} {1} {2} {3}"._DLOG(SessionId, ApiType, _dataSource, _tcpConnection.Connected);
                    }
                    if (_tcpConnection.Connected)
                    {
                        "{0:X2} {1} {2} {3}"._DLOG(SessionId, ApiType, _dataSource, _tcpConnection.Connected);
                        ret = CommunicationStatuses.Done;
                        _isStoped = false;
                        if (Connected != null)
                        {
                            Connected(this);
                        }
                        _workerThread = new Thread(WorkerThreadMethod);
                        _workerThread.Name = "Tcp Client";
                        _workerThread.IsBackground = true;
                        _workerThread.Start();
                    }

                }
                return ret;
            }
        }

        private void WorkerThreadMethod()
        {
            while (!_isStoped)
            {
                byte[] data;
                int numberOfBytesRead = _tcpConnection.Read(out data);
                if (numberOfBytesRead > 0)
                {
                    if (!SuppressDebugOutput)
                    {
                        "{0} << {1}"._DLOG(DataSource, Tools.GetHex(data));
                    }
                    var dataChunk = new DataChunk(data, SessionId, false, ApiType);
                    if (_transmitCallback != null)
                    {
                        _transmitCallback(dataChunk);
                    }
                    if (ReceiveDataCallback != null)
                    {
                        ReceiveDataCallback(dataChunk, false);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        protected override void InnerDisconnect()
        {
            lock (_lockObject)
            {
                if (_isStoped)
                {
                    return;
                }

                _isStoped = true;
                if (_tcpConnection != null)
                {
                    _tcpConnection.Dispose();
                }

                if (_workerThread.ThreadState != ThreadState.Unstarted &&
                    Thread.CurrentThread.ManagedThreadId != _workerThread.ManagedThreadId
                    )
                {
                    _workerThread.Join();
                }
                if (Disconnected != null)
                {
                    Disconnected(this);
                }
                "{0:X2} {1} {2}"._DLOG(SessionId, ApiType, _dataSource);
            }
        }

        protected override int InnerWriteData(byte[] data)
        {
            lock (_lockObject)
            {
                int ret = -1;
                if (_isStoped)
                {
                    return ret;
                }

                if (!SuppressDebugOutput)
                {
                    "{0} >> {1}"._DLOG(DataSource, Tools.GetHex(data));
                }
                DataChunk dc = new DataChunk(data, SessionId, true, ApiType);
                if (_transmitCallback != null)
                {
                    _transmitCallback(dc);
                }

                ret = _tcpConnection.Write(data);
                if (ret != data.Length)
                {
                    "SEND FAILED"._DLOG();
                }
                return ret;
            }
        }

        protected override void InnerDispose()
        {
            InnerDisconnect();
        }
    }
}
