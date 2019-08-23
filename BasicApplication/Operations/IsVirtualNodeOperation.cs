using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class IsVirtualNodeOperation : RequestApiOperation
    {
        private byte NodeId { get; set; }
        public IsVirtualNodeOperation(byte nodeId)
            : base(CommandTypes.CmdZWaveIsVirtualNode, false)
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

        public IsVirtualNodeResult SpecificResult
        {
            get { return (IsVirtualNodeResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new IsVirtualNodeResult();
        }
    }

    public class IsVirtualNodeResult : ActionResult
    {
        public bool RetValue { get; set; }
    }
}
