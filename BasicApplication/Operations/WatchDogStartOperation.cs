using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class WatchDogStartOperation : ControlNApiOperation
    {
        public WatchDogStartOperation()
            : base(CommandTypes.CmdZWaveWatchDogStart)
        { }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
