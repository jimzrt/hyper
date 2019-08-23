using System;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x0B | 0x20
    /// ZW->HOST: RES | 0x0B | 0x20 | RfRegion
    /// </summary>
    /// 
    public class GetRfRegionOperation : RequestApiOperation
    {
        private byte SERIAL_API_SETUP_CMD_RF_REGION_GET = 1 << 5;

        public GetRfRegionOperation() :
            base(CommandTypes.CmdSerialApiSetup, false)
        {
            TimeoutMs = 1000;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { SERIAL_API_SETUP_CMD_RF_REGION_GET };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            var region = ((DataReceivedUnit)ou).DataFrame.Payload[1];
            if (Enum.TryParse(region.ToString(), out RfRegions rfRegion))
            {
                SpecificResult.RfRegion = rfRegion;
            }
            base.SetStateCompleted(ou);
        }

        public RfRegionGetResult SpecificResult
        {
            get { return (RfRegionGetResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new RfRegionGetResult();
        }
    }

    public class RfRegionGetResult : ActionResult
    {
        public RfRegions RfRegion { get; set; }
    }
}
