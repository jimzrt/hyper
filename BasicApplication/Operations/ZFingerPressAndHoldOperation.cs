using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ZFingerPressAndHoldOperation : ControlNApiOperation
    {
        public ZFingerPressAndHoldOperation()
            : base(CommandTypes.PROPRIETARY_4, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
