using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x0B | 0x04 | NormalTxPower | Measured0dBmPower
    /// ZW->HOST: RES | 0x0B | 0x04 | cmdRes
    /// </summary>
    public class SetDefaultTxPowerLevelOperation : ControlApiOperation
    {
        private const byte SERIAL_API_SETUP_CMD_TX_POWERLEVEL_SET = 1 << 2;
        private byte _normalTxPower;
        private byte _measured0dBmPower;

        public SetDefaultTxPowerLevelOperation(byte normalTxPower, byte measured0dBmPower) : base(CommandTypes.CmdSerialApiSetup)
        {
            _normalTxPower = normalTxPower;
            _measured0dBmPower = measured0dBmPower;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SERIAL_API_SETUP_CMD_TX_POWERLEVEL_SET, _normalTxPower, _measured0dBmPower };
        }
    }
}
