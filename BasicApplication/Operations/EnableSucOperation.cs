using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class EnableSucOperation : ControlApiOperation
    {
        private byte _state;
        private byte _capabilities;

        public EnableSucOperation(byte state, byte capabilities)
            : base(CommandTypes.CmdZWaveEnableSuc, false)
        {
            _state = state;
            _capabilities = capabilities;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { _state, _capabilities };
        }
    }
}
