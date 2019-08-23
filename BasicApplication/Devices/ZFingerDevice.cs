using System.Threading;
using ZWave.BasicApplication.Operations;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Application;

namespace ZWave.BasicApplication.Devices
{
    public class ZFingerDevice : ApplicationClient
    {

        internal ZFingerDevice(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(ApiTypes.Basic, sessionId, sc, fc, tc)
        {
        }

        public virtual ActionResult Execute(ActionBase action)
        {
            action.Token.LogEntryPointCategory = "Basic";
            action.Token.LogEntryPointSource = DataSource == null ? "" : DataSource.SourceName;
            SessionClient.ExecuteAsync(action);
            action.Token.WaitCompletedSignal();
            ActionResult ret = action.Token.Result;
            return ret;
        }

        public ActionResult PB1SinglePress()
        {
            ActionResult ret = Execute(new ZFingerSinglePressOperation());
            return ret;
        }


        public ActionResult PB1QuadPress()
        {
            ActionResult ret = Execute(new ZFingerQuadPressOperation());
            return ret;
        }

        public ActionResult PB1PressAndHold(int holdTimeOutMs)
        {
            ActionResult ret = Execute(new ZFingerPressAndHoldOperation());
            if (ret)
            {
                Thread.Sleep(holdTimeOutMs);
                ret = Execute(new ZFingerSinglePressOperation());
            }
            return ret;
        }

    }
}