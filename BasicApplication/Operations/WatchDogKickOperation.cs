using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{

    public class WatchDogKickOperation : ControlNApiOperation
    {
        public WatchDogKickOperation()
            : base(CommandTypes.CmdZWaveWatchDogKick)
        { }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }


    }
}
