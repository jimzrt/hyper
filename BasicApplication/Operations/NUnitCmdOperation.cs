using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class NUnitCmdOperation : ControlNApiOperation
    {
        public NUnitCmdOperation()
            : base(CommandTypes.CmdZWaveNUnitCmd, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
