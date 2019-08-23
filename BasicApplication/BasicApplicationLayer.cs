using ZWave.BasicApplication.Devices;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Application;

namespace ZWave.BasicApplication
{
    public class BasicApplicationLayer : ApplicationLayer
    {
        public BasicApplicationLayer(ISessionLayer sessionLayer, IFrameLayer frameLayer, ITransportLayer transportLayer)
            : base(ApiTypes.Basic, sessionLayer, frameLayer, transportLayer)
        { }

        public Controller CreateController(IApplicationClient client)
        {
            Controller ret = new Controller(client.SessionId, client.SessionClient, client.FrameClient, client.TransportClient);
            return ret;
        }

        public Controller CreateController()
        {
            var sessionId = NextSessionId();
            Controller ret = new Controller(sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }

        public Slave CreateSlave(IApplicationClient client)
        {
            Slave ret = new Slave(client.SessionId, client.SessionClient, client.FrameClient, client.TransportClient);
            return ret;
        }

        public Slave CreateSlave()
        {
            var sessionId = NextSessionId();
            Slave ret = new Slave(sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }

        public BridgeController CreateBridgeController(IApplicationClient client)
        {
            BridgeController ret = new BridgeController(client.SessionId, client.SessionClient, client.FrameClient, client.TransportClient);
            return ret;
        }

        public BridgeController CreateBridgeController()
        {
            var sessionId = NextSessionId();
            BridgeController ret = new BridgeController(sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }

        public InstallerController CreateInstallerController(IApplicationClient client)
        {
            InstallerController ret = new InstallerController(client.SessionId, client.SessionClient, client.FrameClient, client.TransportClient);
            return ret;
        }

        public InstallerController CreateInstallerController()
        {
            var sessionId = NextSessionId();
            InstallerController ret = new InstallerController(sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }

        public ZFingerDevice CreateZFingerDevice(IApplicationClient client)
        {
            ZFingerDevice ret = new ZFingerDevice(client.SessionId, client.SessionClient, client.FrameClient, client.TransportClient);
            return ret;
        }

        public ZFingerDevice CreateZFingerDevice()
        {
            var sessionId = NextSessionId();
            ZFingerDevice ret = new ZFingerDevice(sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }
    }
}
