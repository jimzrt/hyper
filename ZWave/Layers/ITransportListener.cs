using System;
using System.Net;

namespace ZWave.Layers
{
    public class ReceivedDataArgs
    {
        public byte[] Data { get; set; }
        public string SourceName { get; set; }
        public ushort SourcePort { get; set; }
        public ushort ListenerPort { get; set; }
    }

    public interface IStartListenParams
    {
        IPAddress IpAddress { get; set; }
        ushort PortNo { get; set; }
    }

    public interface IDtlsStartListenParams : IStartListenParams
    {
        string PskKey { get; set; }
    }

    public interface ITransportListener : IDisposable
    {
        event Action<ReceivedDataArgs> DataReceived;
        event Action<string, ushort> ConnectionCreated;
        event Action<string, ushort> ConnectionClosed;
        bool IsListening { get; }
        bool Listen(IStartListenParams listenParams);
        void Close();
    }
}
