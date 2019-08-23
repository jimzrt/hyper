using System.Linq;
using Utils;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class ExpectDataOperation : ApiAchOperation
    {
        public ExpectDataOperation(byte destNodeId, byte srcNodeId, byte[] data, int bytesTocompare, int timeoutMs)
            : base(destNodeId, srcNodeId, data, bytesTocompare)
        {
            TimeoutMs = timeoutMs;
        }

        public ExpectDataOperation(byte destNodeId, byte srcNodeId, byte[] data, int bytesTocompare, ExtensionTypes[] extensionTypes, int timeoutMs)
            : base(destNodeId, srcNodeId, data, bytesTocompare, extensionTypes)
        {
            TimeoutMs = timeoutMs;
        }

        public ExpectDataOperation(byte destNodeId, byte srcNodeId, ByteIndex[] data, int timeoutMs)
            : base(destNodeId, srcNodeId, data)
        {
            TimeoutMs = timeoutMs;
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            SpecificResult.Options = ReceivedAchData.Options;
            SpecificResult.SrcNodeId = ReceivedAchData.SrcNodeId;
            SpecificResult.DestNodeId = ReceivedAchData.DestNodeId;
            SpecificResult.Command = ReceivedAchData.Command;
            SpecificResult.Rssi = ReceivedAchData.Rssi;
            SpecificResult.SecurityScheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            SpecificResult.Extensions = ReceivedAchData.Extensions;
            SpecificResult.SubstituteStatus = (ou.DataFrame.SubstituteIncomingFlags & SubstituteIncomingFlags.Security) > 0 ?
                SubstituteStatuses.Done : SubstituteStatuses.Failed;
            SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            return string.Format("Data={0}", DataToCompare != null && DataToCompare.Length > 0 ? DataToCompare.Select(x => x.ToString()).Aggregate((x, y) => x + " " + y) : "");
        }

        public ExpectDataResult SpecificResult
        {
            get { return (ExpectDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ExpectDataResult();
        }
    }

    public class ExpectDataResult : ActionResult
    {
        public byte Options { get; set; }
        public byte SrcNodeId { get; set; }
        public byte DestNodeId { get; set; }
        public byte[] Command { get; set; }
        public byte Rssi { get; set; }
        public SecuritySchemes SecurityScheme { get; set; }
        public SubstituteStatuses SubstituteStatus { get; set; }
        public Extensions Extensions { get; set; }
    }
}
