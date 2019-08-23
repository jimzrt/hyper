using ZWave.BasicApplication.Enums;
using ZWave.Devices;

namespace ZWave.BasicApplication.Operations
{
    public class GetNodeProtocolInfoOperation : RequestApiOperation
    {
        private byte NodeId { get; set; }
        public GetNodeProtocolInfoOperation(byte nodeId)
            : base(CommandTypes.CmdZWaveGetNodeProtocolInfo, false)
        {
            NodeId = nodeId;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { NodeId };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.NodeId = NodeId;
            SpecificResult.NodeInfo = new NodeInfo
            {
                Capability = ((DataReceivedUnit)ou).DataFrame.Payload[0],
                Security = ((DataReceivedUnit)ou).DataFrame.Payload[1],
                Properties1 = ((DataReceivedUnit)ou).DataFrame.Payload[2],
                Basic = ((DataReceivedUnit)ou).DataFrame.Payload[3],
                Generic = ((DataReceivedUnit)ou).DataFrame.Payload[4],
                Specific = ((DataReceivedUnit)ou).DataFrame.Payload[5]
            };
            base.SetStateCompleted(ou);
        }

        public GetNodeProtocolInfoResult SpecificResult
        {
            get { return (GetNodeProtocolInfoResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetNodeProtocolInfoResult();
        }
    }

    public class GetNodeProtocolInfoResult : ActionResult
    {
        public byte NodeId { get; set; }
        public NodeInfo NodeInfo { get; set; }
    }
}
