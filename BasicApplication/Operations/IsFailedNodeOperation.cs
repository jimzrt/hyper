using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class IsFailedNodeOperation : RequestApiOperation
    {
        internal byte NodeId { get; set; }
        public IsFailedNodeOperation(byte nodeId)
            : base(CommandTypes.CmdZWaveIsFailedNode, false)
        {
            NodeId = nodeId;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { NodeId };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.RetValue = ((DataReceivedUnit)ou).DataFrame.Payload[0] == 1;
            base.SetStateCompleted(ou);
        }

        public IsFailedNodeResult SpecificResult
        {
            get { return (IsFailedNodeResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new IsFailedNodeResult();
        }
    }

    public class IsFailedNodeResult : ActionResult
    {
        public bool RetValue { get; set; }
    }
}
