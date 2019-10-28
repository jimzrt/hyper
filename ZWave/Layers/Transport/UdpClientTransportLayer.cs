using System.Collections;
using System.Collections.Generic;
using System.Net;
using Utils;

namespace ZWave.Layers.Transport
{
    public class UdpClientTransportLayer : TransportLayer
    {
        private readonly HashSet<ITransportClient> _listeningClientsSet = new HashSet<ITransportClient>();

        private readonly object _lisneterLock = new object();

        private UdpClientTransportListener _listener;
        public override ITransportListener Listener
        {
            get { return _listener; }
            set
            {
                lock (_lisneterLock)
                {
                    if (_listener != null)
                    {
                        _listener.Close();
                        _listener.DataReceived -= Listener_DataReceived;
                    }
                    _listener = value as UdpClientTransportListener;
                    _listener.DataReceived += Listener_DataReceived;
                }
            }
        }

        private void Listener_DataReceived(ReceivedDataArgs receivedDataArgs)
        {
            if (_listeningClientsSet.Count == 0)
            {
                return;
            }
            IPAddress sourceIpAddress = null;
            if (IPAddress.TryParse(receivedDataArgs.SourceName, out sourceIpAddress))
            {
                sourceIpAddress = Tools.MapToIPv4(sourceIpAddress);
            }
            foreach (var client in _listeningClientsSet)
            {
                client.ReceiveDataCallback?.Invoke(new DataChunk(receivedDataArgs.Data, client.SessionId, false, client.ApiType), false);
            }
        }

        public override ITransportClient CreateClient(byte sessionId)
        {
            UdpClientTransportClient ret = new UdpClientTransportClient(TransmitCallback)
            {
                SuppressDebugOutput = SuppressDebugOutput,
                SessionId = sessionId
            };
            //ret.Connected += Client_Co
            return ret;
        }

        public void RegisterListeningClient(ITransportClient transportClient)
        {
            if (_listeningClientsSet.Contains(transportClient))
            {
                return;
            }
            _listeningClientsSet.Add(transportClient);
        }

        private readonly Hashtable _clients = new Hashtable();

        public void RegisterClient(string key, ITransportClient client)
        {
            if (!_clients.Contains(key))
            {
                _clients.Add(key, client);
            }
            else
            {
                _clients[key] = client;
            }
        }

        public void UnregisterClient(string key)
        {
            if (_clients.Contains(key))
            {
                _clients.Remove(key);
            }
        }

        public bool UnregisterListeningClient(ITransportClient transportClient)
        {
            return _listeningClientsSet.Remove(transportClient);
        }
    }
}
