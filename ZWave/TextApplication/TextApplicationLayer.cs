using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Application;

namespace ZWave.TextApplication
{
    public class TextApplicationLayer : ApplicationLayer
    {
        public TextApplicationLayer(ISessionLayer sessionLayer, IFrameLayer frameLayer, ITransportLayer transportLayer)
            : base(ApiTypes.Text, sessionLayer, frameLayer, transportLayer)
        {
        }

        public TextDevice CreateTextDevice()
        {
            var sessionId = NextSessionId();
            TextDevice ret = new TextDevice(sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }
    }
}
