using ZWave.Layers;
using ZWave.Layers.Frame;

namespace ZWave.TextApplication
{
    public class TextFrameLayer : FrameLayer
    {
        public override IFrameClient CreateClient()
        {
            IFrameClient ret = new TextFrameClient(TransmitCallback);
            return ret;
        }
    }
}
