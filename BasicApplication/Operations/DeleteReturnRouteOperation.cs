using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x47 | nodeID | funcID
    /// ZW->HOST: RES | 0x47 | retVal
    /// ZW->HOST: REQ | 0x47 | funcID | bStatus
    /// </summary>
    public class DeleteReturnRouteOperation : CallbackApiOperation
    {
        internal byte NodeId { get; set; }
        public DeleteReturnRouteOperation(byte nodeId)
            : base(CommandTypes.CmdZWaveDeleteReturnRoute)
        {
            NodeId = nodeId;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { NodeId };
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.RetStatus = ou.DataFrame.Payload[1];
            }
        }

        public DeleteReturnRouteResult SpecificResult
        {
            get { return (DeleteReturnRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new DeleteReturnRouteResult();
        }
    }

    public class DeleteReturnRouteResult : ActionResult
    {
        public byte RetStatus { get; set; }
    }
}
