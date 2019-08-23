using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x14 | numberNodes | pNodeIDList[ ] | dataLength | pData[ ] | txOptions | funcID
    /// ZW->HOST: RES | 0x14 | RetVal
    /// ZW->HOST: REQ | 0x14 | funcID | txStatus
    /// </summary>
    public class SendDataMultiOperation : CallbackApiOperation
    {
        public byte[] NodeIds { get; private set; }
        public byte[] Data { get; set; }
        public TransmitOptions TxOptions { get; private set; }
        public SendDataMultiOperation(byte[] nodeIds, byte[] data, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendDataMulti)
        {
            NodeIds = nodeIds;
            Data = data;
            TxOptions = txOptions;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[3 + NodeIds.Length + Data.Length];
            ret[0] = (byte)NodeIds.Length;
            for (int i = 0; i < NodeIds.Length; i++)
            {
                ret[i + 1] = NodeIds[i];
            }
            ret[1 + NodeIds.Length] = (byte)Data.Length;
            for (int i = 0; i < Data.Length; i++)
            {
                ret[i + 2 + NodeIds.Length] = Data[i];
            }
            ret[2 + NodeIds.Length + Data.Length] = (byte)TxOptions;
            return ret;
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.TransmitStatus = (TransmitStatuses)ou.DataFrame.Payload[1];
            }
        }

        public override string AboutMe()
        {
            return string.Format("Data={0}; Status={1}", Data.GetHex(), SpecificResult.TransmitStatus);
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
