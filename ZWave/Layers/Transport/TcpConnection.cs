using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Utils;

namespace ZWave.Layers.Transport
{
    public class TcpConnection : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private NetworkStream _tcpStream;

        public bool Connected
        {
            get { return _tcpStream != null; }
        }

        public TcpConnection()
        {
            _tcpClient = new TcpClient();
        }

        public bool Connect(string hostname, int portNo)
        {
            try
            {
                _tcpClient.Connect(hostname, portNo);
                _tcpStream = _tcpClient.GetStream();
                return true;
            }
            catch (SocketException ex)
            {
                _tcpStream = null;
                ex.Message._DLOG();
                return false;
            }
            catch (ObjectDisposedException ex)
            {
                _tcpStream = null;
                ex.Message._DLOG();
                return false;
            }
        }

        public int Write(byte[] data)
        {
            int ret = -1;
            if (_tcpStream == null)
            {
                return ret;
            }

            try
            {
                if (_tcpStream.CanWrite)
                {
                    _tcpStream.Write(data, 0, data.Length);
                    ret = data.Length;
                }
            }
            catch (ObjectDisposedException ex)
            {
                ex.Message._DLOG();
            }
            catch (System.IO.IOException ex)
            {
                ex.Message._DLOG();
            }
            return ret;
        }

        public int Read(out byte[] data)
        {
            data = null;
            int ret = -1;
            if (_tcpStream == null)
            {
                return ret;
            }

            try
            {
                if (_tcpStream.CanRead)
                {
                    var bytesRead = new List<byte>();
                    do
                    {
                        var bufferToRead = new byte[1024];
                        int numberOfBytesRead = _tcpStream.Read(bufferToRead, 0, bufferToRead.Length);
                        if (numberOfBytesRead > 0)
                        {
                            bytesRead.AddRange(bufferToRead.Take(numberOfBytesRead));
                        }
                    }
                    while (_tcpStream.DataAvailable);
                    ret = bytesRead.Count;
                    if (ret > 0)
                    {
                        data = bytesRead.ToArray();
                    }
                }
            }
            catch (ObjectDisposedException ex)
            {
                ex.Message._DLOG();
            }
            catch (SocketException ex)
            {
                ex.Message._DLOG();
            }
            catch (System.IO.IOException ex)
            {
                ex.Message._DLOG();
            }
            return ret;
        }

        public void Dispose()
        {
            _tcpClient.Close();
            if (_tcpStream != null)
            {
                _tcpStream.Close();
                _tcpStream = null;
            }
        }
    }
}
