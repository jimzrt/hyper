using System.Collections.Generic;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SerialApiGetInitDataOperation : RequestApiOperation
    {
        public SerialApiGetInitDataOperation()
            : base(CommandTypes.CmdSerialApiGetInitData, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.SerialApiVersion = ((DataReceivedUnit)ou).DataFrame.Payload[0];
            SpecificResult.SerialApiCapability = ((DataReceivedUnit)ou).DataFrame.Payload[1];

            byte nodeIdx = 0;
            List<byte> includedNodes = new List<byte>();
            for (int i = 0; i < ((DataReceivedUnit)ou).DataFrame.Payload[2]; i++)
            {
                byte availabilityMask = ((DataReceivedUnit)ou).DataFrame.Payload[3 + i];
                for (byte bit = 0; bit < 8; bit++)
                {
                    nodeIdx++;
                    if ((availabilityMask & (1 << bit)) > 0)
                    {
                        includedNodes.Add(nodeIdx);
                    }
                }
            }
            SpecificResult.IncludedNodes = includedNodes.ToArray();
            SpecificResult.ChipType = (ChipTypes)((DataReceivedUnit)ou).DataFrame.Payload[3 + ((DataReceivedUnit)ou).DataFrame.Payload[2]];
            SpecificResult.ChipRevision = ((DataReceivedUnit)ou).DataFrame.Payload[4 + ((DataReceivedUnit)ou).DataFrame.Payload[2]];
            base.SetStateCompleted(ou);
        }

        public SerialApiGetInitDataResult SpecificResult
        {
            get { return (SerialApiGetInitDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SerialApiGetInitDataResult();
        }
    }

    public class SerialApiGetInitDataResult : ActionResult
    {
        public byte SerialApiVersion { get; set; }
        public byte SerialApiCapability { get; set; }
        public ChipTypes ChipType { get; set; }
        public byte ChipRevision { get; set; }
        public byte[] IncludedNodes { get; set; }
    }
}
