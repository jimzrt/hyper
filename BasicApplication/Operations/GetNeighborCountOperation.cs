using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class GetNeighborCountOperation : RequestApiOperation
    {
        private byte NodeId { get; set; }
        public GetNeighborCountOperation(byte nodeId)
            : base(CommandTypes.CmdZWaveGetNeighborCount, false)
        {
            NodeId = nodeId;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { NodeId };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.RetValue = ((DataReceivedUnit)ou).DataFrame.Payload[0];
            base.SetStateCompleted(ou);
        }

        public GetNeighborCountResult SpecificResult
        {
            get { return (GetNeighborCountResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetNeighborCountResult();
        }
    }

    public class GetNeighborCountResult : ActionResult
    {
        public byte RetValue { get; set; }
    }
}
