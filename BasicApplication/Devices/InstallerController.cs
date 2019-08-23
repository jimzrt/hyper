using ZWave.BasicApplication.Operations;
using ZWave.Layers;

namespace ZWave.BasicApplication.Devices
{
    public class InstallerController : Controller
    {
        internal InstallerController(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(sessionId, sc, fc, tc)
        { }

        public GetTransmitCountResult GetTransmitCount()
        {
            return (GetTransmitCountResult)Execute(new GetTransmitCountOperation());
        }

        public ActionResult ResetTransmitCount()
        {
            return Execute(new ResetTransmitCountOperation());
        }

        public ActionResult StoreHomeId(byte[] homeId, byte nodeId)
        {
            return Execute(new StoreHomeIdOperation(homeId, nodeId));
        }

        public ActionResult StoreNodeInfo(byte nodeId, byte[] nodeInfo)
        {
            return Execute(new StoreNodeInfoOperation(nodeId, nodeInfo));
        }
    }
}
