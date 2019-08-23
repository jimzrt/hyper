using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x51 | bSrcNodeID | funcID | funcID
    /// The extra funcID is added to ensures backward compatible. 
    /// This parameter has been removed starting from dev. kit  4.1x. and onwards and has therefore no meaning anymore.
    /// ZW->HOST: RES | 0x51 | retVal
    /// ZW->HOST: REQ | 0x51 | funcID | bStatus
    /// </summary>
    public class AssignSucReturnRouteOperation : CallbackApiOperation
    {
        private byte SrcNodeId { get; set; }
        public AssignSucReturnRouteOperation(byte srcNodeId)
            : base(CommandTypes.CmdZWaveAssignSucReturnRoute)
        {
            SrcNodeId = srcNodeId;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SrcNodeId, 0x00 };
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.RetStatus = ou.DataFrame.Payload[1];
            }
        }

        public AssignSucReturnRouteResult SpecificResult
        {
            get { return (AssignSucReturnRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new AssignSucReturnRouteResult();
        }
    }

    public class AssignSucReturnRouteResult : ActionResult
    {
        public byte RetStatus { get; set; }
    }
}
