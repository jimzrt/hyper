using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class NUnitInitOperation : ControlNApiOperation
    {
        public NUnitInitOperation()
            : base(CommandTypes.CmdZWaveNUnitInit, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
