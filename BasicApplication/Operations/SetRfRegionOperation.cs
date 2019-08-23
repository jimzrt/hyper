using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x0B | 0x40 | RfRegion
    /// ZW->HOST: RES | 0x0B | 0x40 | cmdRes
    /// </summary>
    public class SetRfRegionOperation : ControlApiOperation
    {
        private const byte SERIAL_API_SETUP_CMD_RF_REGION_SET = 1 << 6;
        private RfRegions _rfRegion;
        public SetRfRegionOperation(RfRegions rfRegion) : base(CommandTypes.CmdSerialApiSetup)
        {
            _rfRegion = rfRegion;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SERIAL_API_SETUP_CMD_RF_REGION_SET, (byte)_rfRegion };
        }
    }
}
