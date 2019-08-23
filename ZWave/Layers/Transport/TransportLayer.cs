using System;
using Utils.Events;

namespace ZWave.Layers.Transport
{
    public abstract class TransportLayer : ITransportLayer
    {
        public bool SuppressDebugOutput { get; set; }
        public event EventHandler<EventArgs<DataChunk>> DataTransmitted;
        public abstract ITransportListener Listener { get; set; }
        public abstract ITransportClient CreateClient(byte sessionId);
        protected void TransmitCallback(DataChunk dataChunk)
        {
            DataTransmitted?.Invoke(this, new EventArgs<DataChunk>(dataChunk));
        }
    }
}
