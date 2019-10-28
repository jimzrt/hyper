using System.Text;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ListenDebugDataOperation : ApiOperation
    {
        private readonly ListenDebugDataDelegate _listenCallback;
        public ListenDebugDataOperation(ListenDebugDataDelegate listenCallback)
            : base(false, null, false)
        {
            _listenCallback = listenCallback;
        }

        private ApiHandler _debugDataReceived;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 0));
            ActionUnits.Add(new DataReceivedUnit(_debugDataReceived, OnHandled));
        }

        protected override void CreateInstance()
        {
            _debugDataReceived = new ApiHandler(FrameTypes.Request, CommandTypes.CmdDebugOutput);
        }

        private void OnHandled(DataReceivedUnit ou)
        {
            if (ou.DataFrame != null && ou.DataFrame.Payload != null)
            {
                string msg = null;
                try
                {
                    var chars = Encoding.ASCII.GetChars(ou.DataFrame.Payload);
                    if (chars != null && chars.Length > 0)
                    {
                        msg = new string(chars);
                    }
                }
                catch
                {
                }
                if (msg != null)
                {
                    _listenCallback(msg);
                }
            }
        }
    }
    public delegate void ListenDebugDataDelegate(string message);
}
