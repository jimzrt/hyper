using ZWave.Layers;
using ZWave.Layers.Frame;

namespace ZWave.BasicApplication
{
    public class XModemFrameLayer : FrameLayer
    {
        public override IFrameClient CreateClient()
        {
            IFrameClient ret = new XModemFrameClient();
            return ret;
        }
    }
}
