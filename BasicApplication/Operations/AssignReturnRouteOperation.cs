using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x46 | bSrcNodeID | bDstNodeID | funcID
    /// ZW->HOST: RES | 0x46 | retVal
    /// ZW->HOST: REQ | 0x46 | funcID | bStatus
    /// </summary>
    public class AssignReturnRouteOperation : CallbackApiOperation
    {
        internal byte SrcNodeId { get; set; }
        internal byte DestNodeId { get; set; }
        public AssignReturnRouteOperation(byte srcNodeId, byte destNodeId)
            : base(CommandTypes.CmdZWaveAssignReturnRoute)
        {
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SrcNodeId, DestNodeId };
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.RetStatus = ou.DataFrame.Payload[1];
            }
        }

        public AssignReturnRouteResult SpecificResult
        {
            get { return (AssignReturnRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new AssignReturnRouteResult();
        }
    }

    public class AssignReturnRouteResult : ActionResult
    {
        public byte RetStatus { get; set; }
    }
}
