using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ZFingerQuadPressOperation : ControlNApiOperation
    {
        public ZFingerQuadPressOperation()
            : base(CommandTypes.PROPRIETARY_2, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
