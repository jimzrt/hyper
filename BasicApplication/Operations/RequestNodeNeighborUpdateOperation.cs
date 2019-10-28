using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x48 | nodeID | funcID
    /// ZW->HOST: REQ | 0x48 | funcID | bStatus
    /// </summary>
    public class RequestNodeNeighborUpdateOperation : ApiOperation
    {
        public static int TIMEOUT = 60000;

        private readonly byte mNodeId;
        internal int TimeoutMs { get; set; }
        public RequestNodeNeighborUpdateOperation(byte nodeId, int timeoutMs)
            : base(true, CommandTypes.CmdZWaveRequestNodeNeighborUpdate, true)
        {
            mNodeId = nodeId;
            TimeoutMs = timeoutMs;
            if (TimeoutMs <= 0)
                TimeoutMs = TIMEOUT;
        }

        private ApiMessage messageStart;
        private ApiHandler handlerStarted;             // code=0x21
        private ApiHandler handlerDone;                // code=0x22
        private ApiHandler handlerFailed;              // code=0x23

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, TimeoutMs, messageStart));
            ActionUnits.Add(new DataReceivedUnit(handlerStarted, OnHandlerStarted));
            ActionUnits.Add(new DataReceivedUnit(handlerDone, OnHandlerDone));
            ActionUnits.Add(new DataReceivedUnit(handlerFailed, OnHandlerFailed));
        }

        protected override void CreateInstance()
        {
            messageStart = new ApiMessage(SerialApiCommands[0], new byte[] { mNodeId });
            messageStart.SetSequenceNumber(SequenceNumber);

            handlerStarted = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerStarted.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)RequestNeighborUpdateStatuses.Started));

            handlerDone = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)RequestNeighborUpdateStatuses.Done));

            handlerFailed = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerFailed.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)RequestNeighborUpdateStatuses.Failed));
        }

        private void OnHandlerStarted(DataReceivedUnit ou)
        {
            SpecificResult.NeighborUpdateStatus = RequestNeighborUpdateStatuses.Started;
        }

        private void OnHandlerDone(DataReceivedUnit ou)
        {
            SpecificResult.NeighborUpdateStatus = RequestNeighborUpdateStatuses.Done;
            SetStateCompleted(ou);
        }

        private void OnHandlerFailed(DataReceivedUnit ou)
        {
            SpecificResult.NeighborUpdateStatus = RequestNeighborUpdateStatuses.Failed;
            SetStateCompleted(ou);
        }

        public RequestNodeNeighborUpdateResult SpecificResult
        {
            get { return (RequestNodeNeighborUpdateResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new RequestNodeNeighborUpdateResult();
        }
    }

    public class RequestNodeNeighborUpdateResult : ActionResult
    {
        public RequestNeighborUpdateStatuses NeighborUpdateStatus { get; set; }
    }
}
