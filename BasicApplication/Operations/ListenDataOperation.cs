using System.Linq;

namespace ZWave.BasicApplication.Operations
{
    public class ListenDataOperation : ApiAchOperation
    {
        private ListenDataDelegate _listenCallback;
        public ListenDataOperation(ListenDataDelegate listenCallback)
            : base(0, 0, null)
        {
            _listenCallback = listenCallback;
        }

        static byte[] emptyArray = new byte[0];
        byte handlingRequestFromNode = 0;
        byte[] handlingRequest = emptyArray;
        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] cmd = ReceivedAchData.Command;
            if ((cmd != null && cmd.Length > 1) || ReceivedAchData.Extensions != null)
            {
                if (handlingRequestFromNode != nodeId || !handlingRequest.SequenceEqual(cmd))
                {
                    SpecificResult.TotalCount++;
                    handlingRequestFromNode = nodeId;
                    handlingRequest = cmd;
                    if (_listenCallback != null)
                    {
                        _listenCallback(ReceivedAchData);
                    }
                    handlingRequestFromNode = 0;
                    handlingRequest = emptyArray;
                }
            }
        }

        public ListenDataResult SpecificResult
        {
            get { return (ListenDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ListenDataResult();
        }
    }

    public class ListenDataResult : ActionResult
    {
        public int TotalCount { get; set; }
    }

    public delegate void ListenDataDelegate(AchData appCmdHandlerData);
}
