using ZWave.BasicApplication.Devices;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Application;

namespace ZWave.BasicApplication
{
    public class XModemApplicationLayer : ApplicationLayer
    {
        public XModemApplicationLayer(ISessionLayer sessionLayer, IFrameLayer frameLayer, ITransportLayer transportLayer) :
            base(ApiTypes.XModem, sessionLayer, frameLayer, transportLayer)
        {
        }

        public XModemDevice CreateXModem()
        {
            return new XModemDevice(SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(0));
        }
    }
}
