using System;
using Utils.Events;

namespace ZWave.Layers
{
    /// <summary>
    /// Provides the features required to support communication with connected Z-Wave Device.
    /// </summary>
    public interface ITransportLayer
    {
        bool SuppressDebugOutput { get; set; }
        ITransportListener Listener { get; set; }
        ITransportClient CreateClient(byte sessionId);
        event EventHandler<EventArgs<DataChunk>> DataTransmitted;
    }
}
