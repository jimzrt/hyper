using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Utils;

namespace ZWave.Layers.Transport
{
    public class StartListenParams : IStartListenParams
    {
        public IPAddress IpAddress { get; set; }
        public ushort PortNo { get; set; }
    }

    internal class BeginReceiveState
    {
        public Socket ListenSocket { get; set; }
        public byte[] Data { get; set; }
        public EndPoint RemotePeer;
    }

    public class UdpClientTransportListener : ITransportListener
    {
        private readonly ushort[] wordsMapped = { 0, 0, 0, 0, 0, 0xffff };
        private const int mappedOffset = 12;

        private const int IPV6_V6ONLY = 27;
        private const int MAX_DATA_BUFFER_LENGTH = 10000;

        private Socket _socket;
        private readonly object _socketAccessLock = new object();

        public event Action<ReceivedDataArgs> DataReceived;

        public event Action<string, ushort> ConnectionCreated;
        public event Action<string, ushort> ConnectionClosed;

        private readonly HashSet<EndPoint> _clientsTable = new HashSet<EndPoint>();

        public bool SuppressDebugOutput { get; set; }
        public ushort PortNumber { get; private set; }

        private volatile bool _isListening;
        public bool IsListening { get { return _isListening; } }

        public bool Listen(IStartListenParams listenParams)
        {
            if (!_isListening)
            {
                PortNumber = listenParams.PortNo;
                try
                {
                    lock (_socketAccessLock)
                    {
                        var listenAddress = listenParams.IpAddress ?? IPAddress.IPv6Any;
                        _socket = new Socket(listenAddress.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
                        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // Enable using the same address by different clients.
                        if (listenAddress.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            _socket.SetSocketOption(SocketOptionLevel.IPv6, (SocketOptionName)IPV6_V6ONLY, 0); // Disable treat wildcard bind as AF_INET6-only.
                        }
                        _socket.Bind(new IPEndPoint(listenAddress, listenParams.PortNo));
                        "Start listening @{0}"._DLOG(listenParams.PortNo);
                        _isListening = true;
                        var state = new BeginReceiveState
                        {
                            ListenSocket = _socket,
                            Data = new byte[MAX_DATA_BUFFER_LENGTH],
                            RemotePeer = new IPEndPoint(((IPEndPoint)_socket.LocalEndPoint).Address, IPEndPoint.MinPort)
                        };

                        var bufferSize = _socket.BeginReceiveFrom(state.Data, 0, state.Data.Length,
                            SocketFlags.None,
                            ref state.RemotePeer,
                            ReceiveCallback,
                            state);
                    }
                }
                catch (SocketException ex)
                {
                    ex.Message._DLOG();
                    CloseSocket();
                }
                catch (System.Security.SecurityException ex)
                {
                    ex.Message._DLOG();
                    CloseSocket();
                }
            }
            return IsListening;
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            var state = asyncResult.AsyncState as BeginReceiveState;
            if (state == null)
                return;

            int bytesRecieved = 0;
            if (state.ListenSocket != null)
            {
                lock (_socketAccessLock)
                {
                    if (state.ListenSocket != null)
                    {
                        try
                        {
                            bytesRecieved = state.ListenSocket.EndReceiveFrom(asyncResult, ref state.RemotePeer);
                            var bufferSize = state.ListenSocket.BeginReceiveFrom(state.Data, 0, state.Data.Length,
                                SocketFlags.None,
                                ref state.RemotePeer,
                                ReceiveCallback,
                                state);
                        }
                        catch (ObjectDisposedException) // Socket has been closed.
                        {
                            if (_clientsTable.Contains(state.RemotePeer))
                            {
                                _clientsTable.Remove(state.RemotePeer);
                                var ipAddress = ((IPEndPoint)state.RemotePeer).Address;
                                CheckIfAddressIsMapped(ref ipAddress);
                                ConnectionClosed?.Invoke(ipAddress.ToString(), (ushort)((IPEndPoint)state.RemotePeer).Port);
                            }
                            return;
                        }
                        catch (SocketException ex) // An error occurred when attempting to access the socket.
                        {
                            ex.Message._DLOG();
                            CloseSocket();
                            return;
                        }
                        catch (InvalidCastException ex)
                        {
                            ex.Message._DLOG();
                        }
                    }
                }
            }

            if (bytesRecieved > 0)
            {
                var ipAddress = ((IPEndPoint)state.RemotePeer).Address;
                CheckIfAddressIsMapped(ref ipAddress);
                if (!_clientsTable.Contains(state.RemotePeer))
                {
                    _clientsTable.Add(state.RemotePeer);
                    ConnectionCreated?.Invoke(ipAddress.ToString(), (ushort)((IPEndPoint)state.RemotePeer).Port);
                }

                // Compose callback data.
                var receivedData = new ReceivedDataArgs { SourceName = ipAddress.ToString(), ListenerPort = PortNumber };
                receivedData.Data = new byte[bytesRecieved];
                Array.Copy(state.Data, receivedData.Data, bytesRecieved);
                DataReceived?.Invoke(receivedData);
                if (!SuppressDebugOutput)
                {
                    "Listener accepted {0} << {1}"._DLOG(receivedData.SourceName, Tools.GetHex(receivedData.Data));
                }
            }
        }

        private void CheckIfAddressIsMapped(ref IPAddress address)
        {
            var ipBytes = address.GetAddressBytes();
            int j = 0;
            for (int i = 0; i < ipBytes.Length; i += 2)
            {
                if (wordsMapped[j] != (ushort)(ipBytes[i] | ipBytes[i + 1] << 8))
                    break;

                if (++j < wordsMapped.Length)
                    continue;

                var ipv4Bytes = new byte[ipBytes.Length - mappedOffset];
                Array.Copy(ipBytes, mappedOffset, ipv4Bytes, 0, ipv4Bytes.Length);
                address = new IPAddress(ipv4Bytes);
                break;
            }
        }

        private void CloseSocket()
        {
            if (_socket != null)
            {
                lock (_socketAccessLock)
                {
                    if (_socket != null)
                    {
                        _socket.Close();
                        _socket = null;
                        "Stop listening @{0}"._DLOG(PortNumber);
                    }
                }
            }
        }

        public void Close()
        {
            if (!_isListening)
                return;

            _isListening = false;
            CloseSocket();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
