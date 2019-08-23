using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetSleepModeOperation : ControlNApiOperation
    {
        private SleepModes Mode { get; set; }
        private byte IntEnable { get; set; }
        public SetSleepModeOperation(SleepModes mode, byte intEnable)
            : base(CommandTypes.CmdZWaveSetSleepMode)
        {
            Mode = mode;
            IntEnable = intEnable;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { (byte)Mode, IntEnable };
        }
    }
}
