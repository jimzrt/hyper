using System;
using Utils;
using ZWave.Enums;

namespace ZWave.Layers
{
    public abstract class TransportClientBase : ITransportClient
    {
        public abstract event Action<ITransportClient> Connected;
        public abstract event Action<ITransportClient> Disconnected;

        public byte SessionId { get; set; }
        public ApiTypes ApiType { get; set; }
        public IDataSource DataSource { get; set; }
        public Action<DataChunk, bool> ReceiveDataCallback { get; set; }

        public bool SuppressDebugOutput { get; set; }
        public abstract bool IsOpen { get; }

        public TransportClientBase()
        {
        }

        // Create connection.
        public CommunicationStatuses Connect(IDataSource dataSource)
        {
            CommunicationStatuses ret = CommunicationStatuses.Failed;
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            if (!dataSource.Validate())
                throw new ArgumentException("Not valid dataSource");

            if (DataSource == null || !DataSource.Equals(dataSource))
            {
                DataSource = dataSource;
            }

            ret = InnerConnect(dataSource);
            return ret;
        }

        public CommunicationStatuses Connect()
        {
            return Connect(DataSource);
        }

        protected abstract CommunicationStatuses InnerConnect(IDataSource dataSource);
        //

        // Disconnect.
        public void Disconnect()
        {
            if (IsOpen)
            {
                InnerDisconnect();
            }
        }
        protected abstract void InnerDisconnect();
        //

        // Write data.
        public int WriteData(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length == 0)
                throw new ArgumentException("Empty data");

            return InnerWriteData(data);
        }
        protected abstract int InnerWriteData(byte[] data);
        //

        #region IDisposable Members

        public void Dispose()
        {
            InnerDispose();
        }
        protected abstract void InnerDispose();

        #endregion
    }
}
