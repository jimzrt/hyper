using System;
using System.Net.Sockets;
using Utils;

namespace ZWave.Layers.Transport.TcpToSerialBridge
{
    public class SocketBinding
    {
        private string _comPortSource;
        private int _baudRate;
        private Socket _socket;
        private byte[] _inBuffer = new byte[1024];
        private SerialPortTransportClient _transportClient;
        public Action<string> LogOutput { get; set; }

        public SocketBinding(Socket socket, SerialPortTransportClient transportClient, Action<string> logOutput)
        {
            _socket = socket;
            _transportClient = transportClient;
            var serialDataSource = (SerialPortDataSource)transportClient.DataSource;
            _comPortSource = serialDataSource.SourceName;
            _baudRate = (int)serialDataSource.BaudRate;
            _transportClient.ReceiveDataCallback = OnDataChunkReceived;
            LogOutput = logOutput;
        }

        public void Run()
        {
            int inBufferCount;
            try
            {
                while (true)
                {
                    inBufferCount = _socket.Receive(_inBuffer);
                    if (inBufferCount <= 0)
                    {
                        break;
                    }
                    var data = new byte[inBufferCount];
                    Array.Copy(_inBuffer, data, inBufferCount);
                    _transportClient.WriteData(data);
                }
            }
            catch (Exception ex)
            {
                ex.Message._DLOG();
            }
            finally
            {
                if (LogOutput != null)
                {
                    LogOutput("Lost connection from: " + _socket.RemoteEndPoint + " for: " + _comPortSource + "@" + _baudRate);
                }
                try
                {
                    _transportClient.ReceiveDataCallback = null;
                    _transportClient = null;
                    _socket.Close();
                }
                catch
                {
                }
            }
        }

        private void OnDataChunkReceived(DataChunk dataChunk, bool isFromFile)
        {
            if (_socket != null)
            {
                _socket.Send(dataChunk.GetDataBuffer());
            }
        }

        public void Stop()
        {
            if (_socket != null)
            {
                _socket.Close();
            }
        }
    }
}
