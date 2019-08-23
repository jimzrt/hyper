using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class IoPortOperation : ControlNApiOperation
    {
        public IoPortOperation()
            : base(CommandTypes.CmdZWaveIoPort, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
