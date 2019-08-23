using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ZFingerSinglePressOperation : ControlNApiOperation
    {
        public ZFingerSinglePressOperation()
            : base(CommandTypes.PROPRIETARY_3, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
