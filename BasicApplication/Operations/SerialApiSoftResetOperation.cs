using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SerialApiSoftResetOperation : ControlNApiOperation
    {
        public SerialApiSoftResetOperation()
            : base(CommandTypes.CmdSerialApiSoftReset)
        { }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
