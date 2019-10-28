using System;
using System.Net;
using System.Net.Sockets;
using Utils;

namespace ZWave.Layers.Transport.TcpToSerialBridge
{
    public class TcpToSerialBridge
    {
        private static readonly object _portsLocker = new object();
        private bool _shouldListen;
        private readonly TcpListener _tcpListener;
        private SocketBinding _socketToSerial;
        private SerialPortTransportClient _transportClient;
        private readonly string _comPortSource;
        private readonly int _baudRate;

        public Action<string> LogOutput { get; set; }

        public TcpToSerialBridge(byte[] localHost, int port, SerialPortTransportClient transportClient, Action<string> logOutput)
        {
            LogOutput = logOutput;
            _transportClient = transportClient;
            var serialDataSource = (SerialPortDataSource)transportClient.DataSource;
            _comPortSource = serialDataSource.SourceName;
            _baudRate = (int)serialDataSource.BaudRate;

            var ipv4 = localHost != null ? new IPAddress(localHost) : GetLocalIPv4Address();
            _tcpListener = new TcpListener(ipv4, port);
            if (LogOutput != null)
            {
                LogOutput(string.Format("Listening to {0}:{1}", ipv4.ToString(), port));
            }

            byte[] addressBytes = ipv4.GetAddressBytes();
            if (LogOutput != null)
            {
                LogOutput(string.Format("Registering listener: {0}.{1}.{2}.{3}:{4} for {5}@{6}",
                    addressBytes[0],
                    addressBytes[1],
                    addressBytes[2],
                    addressBytes[3],
                    port,
                    _comPortSource,
                    _baudRate));
            }
            _shouldListen = true;
        }

        private IPAddress GetLocalIPv4Address()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new InvalidOperationException("Local IP Address Not Found!");
        }

        public void Run()
        {
            _tcpListener.Start();
            while (_shouldListen)
            {
                try
                {
                    var socket = _tcpListener.AcceptSocket();
                    _socketToSerial = new SocketBinding(socket, _transportClient, LogOutput);
                    if (LogOutput != null)
                    {
                        LogOutput("Got connection from: " + socket.RemoteEndPoint + " for: " + _comPortSource + "@" + _baudRate);
                    }
                    _socketToSerial.Run();
                }
                catch (Exception ex)
                {
                    if (LogOutput != null)
                    {
                        LogOutput(ex.Message);
                    }
                    ex.Message._DLOG();
                }
            }
        }

        public void Start()
        {
            _shouldListen = true;
        }

        public void Stop()
        {
            _shouldListen = false;
            _tcpListener.Stop();
            if (_socketToSerial != null)
            {
                _socketToSerial.Stop();
            }
            _transportClient = null;
        }
    }
}
