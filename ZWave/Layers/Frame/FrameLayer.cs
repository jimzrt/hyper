using System;
using Utils.Events;

namespace ZWave.Layers.Frame
{
    public abstract class FrameLayer : IFrameLayer
    {
        public bool SuppressDebugOutput { get; set; }
        public event EventHandler<EventArgs<IDataFrame>> FrameTransmitted;
        public abstract IFrameClient CreateClient();
        protected void TransmitCallback(IDataFrame dataFrame)
        {
            if (FrameTransmitted != null)
                FrameTransmitted(this, new EventArgs<IDataFrame>(dataFrame));
        }
    }
}
