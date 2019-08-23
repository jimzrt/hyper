using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0xAA | srcNodeID | destNodeID | dataLength | pData[ ] | txOptions | pRoute[4] | funcID
    /// ZW->HOST: RES | 0xAA | RetVal
    /// ZW->HOST: REQ | 0xAA | funcID | txStatus
    /// 
    /// WARNING: Use pRoute[4] equal [0,0,0,0].
    /// </summary>
    public class SendDataMetaBridgeOperation : CallbackApiOperation
    {
        private byte SrcNodeId { get; set; }
        private byte DestNodeId { get; set; }
        private byte[] Data { get; set; }
        private TransmitOptions TxOptions { get; set; }
        public SendDataMetaBridgeOperation(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendDataMeta_Bridge)
        {
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
            Data = data;
            TxOptions = txOptions;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[8 + Data.Length];
            ret[0] = SrcNodeId;
            ret[1] = DestNodeId;
            ret[2] = (byte)Data.Length;
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i + 3] = Data[i];
            }
            ret[3 + Data.Length] = (byte)TxOptions;
            ret[4 + Data.Length] = 0x00;
            ret[5 + Data.Length] = 0x00;
            ret[6 + Data.Length] = 0x00;
            ret[7 + Data.Length] = 0x00;
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
