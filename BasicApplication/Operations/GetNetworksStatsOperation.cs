using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x3A 
    /// ZW->HOST: RES | 0x3A | wRFTxFrames_MSB | wRFTxFrames_LSB | wRFTxLBTBackOffs_MSB | wRFTxLBTBackOffs_LSB | wRFRxFrames_MSB | wRFRxFrames_LSB | wRFRxLRCErrors_MSB | wRFRxLRCErrors_LSB | wRFRxCRC16Errors_MSB | wRFRxCRC16Errors_LSB | wRFRxForeignHomeID_MSB | wRFRxForeignHomeID_LSB
    /// </summary>
    public class GetNetworksStatsOperation : RequestApiOperation
    {
        public GetNetworksStatsOperation()
     : base(CommandTypes.CmdGetNetworksStats, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            SpecificResult.RFTxFrames = (ushort)((res[0] << 8) + res[1]);
            SpecificResult.RFTxLBTBackOffs = (ushort)((res[2] << 8) + res[3]);
            SpecificResult.RFRxFrames = (ushort)((res[4] << 8) + res[5]);
            SpecificResult.RFRxLRCErrors = (ushort)((res[6] << 8) + res[7]);
            SpecificResult.RFRxCRC16Errors = (ushort)((res[8] << 8) + res[9]);
            SpecificResult.RFRxForeignHomeID = (ushort)((res[10] << 8) + res[11]);
            base.SetStateCompleted(ou);
        }

        public GetNetworkStatsResult SpecificResult
        {
            get { return (GetNetworkStatsResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetNetworkStatsResult();
        }

    }

    public class GetNetworkStatsResult : ActionResult
    {
        public ushort RFTxFrames { get; set; }
        public ushort RFTxLBTBackOffs { get; set; }
        public ushort RFRxFrames { get; set; }
        public ushort RFRxLRCErrors { get; set; }
        public ushort RFRxCRC16Errors { get; set; }
        public ushort RFRxForeignHomeID { get; set; }
    }
}
