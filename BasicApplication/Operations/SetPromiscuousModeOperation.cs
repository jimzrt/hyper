using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetPromiscuousModeOperation : ControlNApiOperation
    {
        private bool _stateFlag { get; set; }
        public SetPromiscuousModeOperation(bool state)
            : base(CommandTypes.CmdZWaveSetPromiscuousMode)
        {
            _stateFlag = state;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { (byte)(_stateFlag ? 0x01 : 0x00) };
        }
    }
}
