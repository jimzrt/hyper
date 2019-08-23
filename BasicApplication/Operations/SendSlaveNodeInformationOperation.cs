using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0xA2 | srcNode | destNode | txOptions | funcID
    /// ZW->HOST: RES | 0xA2 | retVal
    /// ZW->HOST; REQ | 0xA2 | funcID | txStatus
    /// </summary>
    public class SendSlaveNodeInformationOperation : CallbackApiOperation
    {
        private byte SrcNodeId { get; set; }
        private byte DestNodeId { get; set; }
        private TransmitOptions TxOptions { get; set; }
        public SendSlaveNodeInformationOperation(byte srcNodeId, byte destNodeId, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendSlaveNodeInfo)
        {
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
            TxOptions = txOptions;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SrcNodeId, DestNodeId, (byte)TxOptions };
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
