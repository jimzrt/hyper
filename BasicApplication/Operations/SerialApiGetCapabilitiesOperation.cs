using System.Collections.Generic;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SerialApiGetCapabilitiesOperation : RequestApiOperation
    {
        public SerialApiGetCapabilitiesOperation()
            : base(CommandTypes.CmdSerialApiGetCapabilities)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            SpecificResult.SerialApplicationVersion = res[0];
            SpecificResult.SerialApplicationRevision = res[1];
            SpecificResult.ManufacturerId = (ushort)((res[2] << 8) + res[3]);
            SpecificResult.ManufacturerProductType = (ushort)((res[4] << 8) + res[5]);
            SpecificResult.ManufacturerProductId = (ushort)((res[6] << 8) + res[7]);
            byte funcIdx = 0;
            List<byte> SupportedFuncIds = new List<byte>();
            for (int j = 8; j < res.Length; j++)
            {
                byte availabilityMask = res[j];
                for (byte bit = 0; bit < 8; bit++)
                {
                    funcIdx++;
                    if ((availabilityMask & (1 << bit)) > 0)
                    {
                        SupportedFuncIds.Add(funcIdx);
                    }
                }
            }
            SpecificResult.SupportedSerialApiCommands = SupportedFuncIds.ToArray();
            base.SetStateCompleted(ou);
        }

        public SerialApiGetCapabilitiesResult SpecificResult
        {
            get { return (SerialApiGetCapabilitiesResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SerialApiGetCapabilitiesResult();
        }
    }

    public class SerialApiGetCapabilitiesResult : ActionResult
    {
        public byte SerialApplicationVersion { get; set; }
        public byte SerialApplicationRevision { get; set; }
        public ushort ManufacturerId { get; set; }
        public ushort ManufacturerProductType { get; set; }
        public ushort ManufacturerProductId { get; set; }
        public byte[] SupportedSerialApiCommands { get; set; }
    }
}
