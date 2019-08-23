using System;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0xA9 | srcNodeID | destNodeID | dataLength | pData[ ] | txOptions | pRoute[4] | funcID
    /// ZW->HOST: RES | 0xA9 | RetVal
    /// ZW->HOST: REQ | 0xA9 | funcID | txStatus
    /// 
    /// WARNING: Use pRoute[4] equal [0,0,0,0].
    /// </summary>
    public class SendDataBridgeOperation : CallbackApiOperation
    {
        internal byte SrcNodeId { get; set; }
        internal byte DestNodeId { get; set; }
        internal byte[] Data { get; set; }
        internal int DataDelay { get; set; }
        internal bool IsFollowup { get; set; }
        internal TransmitOptions TxOptions { get; private set; }
        public Action SubstituteCallback { get; set; }
        public object Extensions { get; set; }
        public SendDataBridgeOperation(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendData_Bridge)
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

        public override string AboutMe()
        {
            return string.Format("Status={0}", SpecificResult.TransmitStatus);
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
