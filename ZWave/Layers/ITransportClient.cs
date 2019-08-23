using System;
using Utils;
using ZWave.Enums;

namespace ZWave.Layers
{
    public interface ITransportClient : IDisposable
    {
        event Action<ITransportClient> Connected;
        event Action<ITransportClient> Disconnected;

        byte SessionId { get; set; }
        ApiTypes ApiType { get; set; }
        IDataSource DataSource { get; set; }
        bool IsOpen { get; }
        CommunicationStatuses Connect(IDataSource dataSource);
        CommunicationStatuses Connect();
        void Disconnect();
        int WriteData(byte[] data);
        /// <summary>
        /// bool value indicates if received data is from file
        /// </summary>
        Action<DataChunk, bool> ReceiveDataCallback { get; set; }
    }
}
