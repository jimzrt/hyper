using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SendNodeInformationOperation : CallbackApiOperation
    {
        private byte Destination { get; set; }
        private TransmitOptions TxOptions { get; set; }
        public SendNodeInformationOperation(byte destination, TransmitOptions txOptions)
            : base(CommandTypes.CmdZWaveSendNodeInformation)
        {
            Destination = destination;
            TxOptions = txOptions;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { Destination, (byte)TxOptions };
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
