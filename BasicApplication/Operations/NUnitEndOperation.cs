using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class NUnitEndOperation : ControlNApiOperation
    {
        public NUnitEndOperation()
            : base(CommandTypes.CmdZWaveNUnitEnd, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
