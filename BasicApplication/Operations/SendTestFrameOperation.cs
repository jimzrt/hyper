using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SendTestFrameOperation : CallbackApiOperation
    {
        internal byte NodeId { get; set; }
        internal byte PowerLevel { get; set; }
        public SendTestFrameOperation(byte nodeId, byte powerLevel)
            : base(CommandTypes.CmdZWaveSendTestFrame)
        {
            NodeId = nodeId;
            PowerLevel = powerLevel;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { NodeId, PowerLevel };
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.TransmitStatus = (TransmitStatuses)ou.DataFrame.Payload[1];
            }
        }

        public TransmitResult SpecificResult
        {
            get { return (TransmitResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new TransmitResult();
        }
    }
}
