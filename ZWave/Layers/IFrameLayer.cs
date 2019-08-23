using System;
using Utils.Events;

namespace ZWave.Layers
{
    /// <summary>
    /// Provides the features required to support Data Frame manipulation.
    /// </summary>
    public interface IFrameLayer
    {
        bool SuppressDebugOutput { get; set; }
        IFrameClient CreateClient();
        event EventHandler<EventArgs<IDataFrame>> FrameTransmitted;
    }
}
