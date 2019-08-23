using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class WatchDogEnableOperation : ControlNApiOperation
    {
        public WatchDogEnableOperation()
            : base(CommandTypes.CmdZWaveWatchDogEnable)
        { }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
