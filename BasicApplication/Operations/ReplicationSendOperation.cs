using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x45 | destNodeID | dataLength | pData[ ] | txOptions | funcID
    /// ZW->HOST: RES | 0x45 | RetVal
    /// ZW->HOST: REQ | 0x45 | funcID | txStatus
    /// </summary>
    public class ReplicationSendOperation : CallbackApiOperation
    {
        private byte NodeId { get; set; }
        private byte[] Data { get; set; }
        private TransmitOptions TxOptions { get; set; }
        public ReplicationSendOperation(byte nodeId, byte[] data, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveReplicationSendData)
        {
            NodeId = nodeId;
            Data = data;
            TxOptions = txOptions;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[3 + Data.Length];
            ret[0] = NodeId;
            ret[1] = (byte)Data.Length;
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i + 2] = Data[i];
            }
            ret[2 + Data.Length] = (byte)TxOptions;
            return ret;
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
