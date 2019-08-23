using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SendDataOperation : CallbackApiOperation
    {
        internal byte NodeId { get; set; }
        internal byte[] Data { get; set; }
        internal int DataDelay { get; set; }
        internal bool IsFollowup { get; set; }
        internal TransmitOptions TxOptions { get; private set; }
        public Action SubstituteCallback { get; set; }
        public object Extensions { get; set; }
        public SendDataOperation(byte nodeId, byte[] data, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendData)
        {
            NodeId = nodeId;
            Data = data;
            TxOptions = txOptions;
        }

        private ApiMessage messageSendDataAbort;

        protected override void CreateWorkflow()
        {
            base.CreateWorkflow();
            ActionUnitStop = new ActionUnit(messageSendDataAbort);
        }

        protected override void CreateInstance()
        {
            base.CreateInstance();
            SpecificResult.TransmitStatus = TransmitStatuses.ResMissing;
            messageSendDataAbort = new ApiMessage(CommandTypes.CmdZWaveSendDataAbort);
        }

        protected override byte[] CreateInputParameters()
        {
            if (Data == null)
                Data = new byte[0];
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
            var payload = ou.DataFrame.Payload;
            if (payload != null && payload.Length > 1)
            {
                SpecificResult.TransmitStatus = (TransmitStatuses)payload[1];
                int index = 3;
                if (payload.Length > index)
                {
                    SpecificResult.HasTxTransmitReport = true;
                    SpecificResult.TransmitTicks = (ushort)((payload[index - 1] << 8) + payload[index]);
                    index++;
                    if (payload.Length > index)
                    {
                        SpecificResult.RepeatersCount = payload[index];
                        index += 5;
                        if (payload.Length > index)
                        {
                            SpecificResult.RssiValuesIncoming = new sbyte[] { (sbyte)payload[index - 4], (sbyte)payload[index - 3], (sbyte)payload[index - 2], (sbyte)payload[index - 1], (sbyte)payload[index] };
                            index++;
                            if (payload.Length > index)
                            {
                                SpecificResult.AckChannelNo = payload[index];
                                index++;
                                if (payload.Length > index)
                                {
                                    SpecificResult.LastTxChannelNo = payload[index];
                                    index++;
                                    if (payload.Length > index)
                                    {
                                        SpecificResult.RouteSchemeState = payload[index];
                                        index += 4;
                                        if (payload.Length > index)
                                        {
                                            SpecificResult.Repeaters = new byte[] { payload[index - 3], payload[index - 2], payload[index - 1], payload[index] };
                                            index++;
                                            if (payload.Length > index)
                                            {
                                                SpecificResult.RouteSpeed = payload[index];
                                                index++;
                                                if (payload.Length > index)
                                                {
                                                    SpecificResult.RouteTries = payload[index];
                                                    index++;
                                                    if (payload.Length > index)
                                                    {
                                                        SpecificResult.LastFailedLinkFrom = payload[index];
                                                        index++;
                                                        if (payload.Length > index)
                                                        {
                                                            SpecificResult.LastFailedLinkTo = payload[index];
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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

    public class TransmitResult : ActionResult
    {
        public TransmitStatuses TransmitStatus { get; set; }
    }

    public class SendDataResult : TransmitResult
    {
        public bool HasTxTransmitReport { get; set; }
        public ushort TransmitTicks { get; set; }
        public byte RepeatersCount { get; set; }
        public sbyte[] RssiValuesIncoming { get; set; }
        public byte AckChannelNo { get; set; }
        public byte LastTxChannelNo { get; set; }
        public byte RouteSchemeState { get; set; }
        public byte[] Repeaters { get; set; }
        public byte RouteSpeed { get; set; }
        public byte RouteTries { get; set; }
        public byte LastFailedLinkFrom { get; set; }
        public byte LastFailedLinkTo { get; set; }
        public SubstituteStatuses SubstituteStatus { get; set; }
    }
}
