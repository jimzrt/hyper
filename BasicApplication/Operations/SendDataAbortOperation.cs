using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SendDataAbortOperation : ControlNApiOperation
    {
        public SendDataAbortOperation()
            : base(CommandTypes.CmdZWaveSendDataAbort)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
