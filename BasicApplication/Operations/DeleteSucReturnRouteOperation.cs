using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x55 | nodeID | funcID
    /// ZW->HOST: RES | 0x55 | retVal
    /// ZW->HOST: REQ | 0x55 | funcID | bStatus
    /// </summary>
    public class DeleteSucReturnRouteOperation : CallbackApiOperation
    {
        public byte NodeId { get; set; }
        public DeleteSucReturnRouteOperation(byte nodeId)
            : base(CommandTypes.CmdZWaveDeleteSucReturnRoute)
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

        public DeleteSucReturnRouteResult SpecificResult
        {
            get { return (DeleteSucReturnRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new DeleteSucReturnRouteResult();
        }
    }

    public class DeleteSucReturnRouteResult : ActionResult
    {
        public byte RetStatus { get; set; }
    }
}
