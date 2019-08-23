using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x0B | 0x08
    /// ZW->HOST: RES | 0x0B | 0x08 | NormalTxPower | Measured0dBmPower
    /// </summary>
    /// 
    public class GetDefaultTxPowerLevelOperation : RequestApiOperation
    {
        private byte SERIAL_API_SETUP_CMD_TX_POWERLEVEL_GET = 1 << 3;

        public GetDefaultTxPowerLevelOperation() :
            base(CommandTypes.CmdSerialApiSetup, false)
        {
            TimeoutMs = 1000;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SERIAL_API_SETUP_CMD_TX_POWERLEVEL_GET };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.NormalTxPower = ((DataReceivedUnit)ou).DataFrame.Payload[1];
            SpecificResult.Measured0dBmPower = ((DataReceivedUnit)ou).DataFrame.Payload[2];
            base.SetStateCompleted(ou);
        }

        public DefaultTxPowerLevelGetResult SpecificResult
        {
            get { return (DefaultTxPowerLevelGetResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new DefaultTxPowerLevelGetResult();
        }
    }

    public class DefaultTxPowerLevelGetResult : ActionResult
    {
        public byte NormalTxPower { get; set; }
        public byte Measured0dBmPower { get; set; }
    }
}
