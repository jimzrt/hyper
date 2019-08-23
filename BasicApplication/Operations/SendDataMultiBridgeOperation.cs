using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0xAB | srcNodeID | numberNodes | pNodeIDList[ ] | dataLength | pData[ ] | txOptions | funcID
    /// ZW->HOST: RES | 0xAB | RetVal
    /// ZW->HOST: REQ | 0xAB | funcID | txStatus
    /// </summary>
    public class SendDataMultiBridgeOperation : CallbackApiOperation
    {
        public byte SrcNodeId { get; private set; }
        public byte[] NodeIds { get; private set; }
        public byte[] Data { get; set; }
        public TransmitOptions TxOptions { get; private set; }
        public SendDataMultiBridgeOperation(byte srcNodeId, byte[] nodeIds, byte[] data, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendDataMulti_Bridge)
        {
            SrcNodeId = srcNodeId;
            NodeIds = nodeIds;
            Data = data;
            TxOptions = txOptions;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[4 + NodeIds.Length + Data.Length];
            ret[0] = SrcNodeId;
            ret[1] = (byte)NodeIds.Length;
            for (int i = 0; i < NodeIds.Length; i++)
            {
                ret[i + 2] = NodeIds[i];
            }
            ret[2 + NodeIds.Length] = (byte)Data.Length;
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i + 3 + NodeIds.Length] = Data[i];
            }
            ret[3 + NodeIds.Length + Data.Length] = (byte)TxOptions;
            return ret;
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.TransmitStatus = (TransmitStatuses)ou.DataFrame.Payload[1];
            }
        }

        public SendDataResult SpecificResult
        {
            get { return (SendDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SendDataResult();
        }
    }
}
