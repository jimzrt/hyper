using ZWave.Layers;
using ZWave.Layers.Frame;

namespace ZWave.BasicApplication
{
    public class BasicFrameLayer : FrameLayer
    {
        public override IFrameClient CreateClient()
        {
            IFrameClient ret = new BasicFrameClient(TransmitCallback);
            return ret;
        }
    }
}
