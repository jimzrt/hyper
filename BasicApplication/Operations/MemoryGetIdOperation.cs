using Utils;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class MemoryGetIdOperation : RequestApiOperation
    {
        public MemoryGetIdOperation()
            : base(CommandTypes.CmdMemoryGetId, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            SpecificResult.HomeId = new byte[] { res[0], res[1], res[2], res[3] };
            SpecificResult.NodeId = res[4];
            base.SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            return string.Format("Id={0}, HomeId={1}", SpecificResult.NodeId, SpecificResult.HomeId == null ? "" : Tools.GetHexShort(SpecificResult.HomeId));
        }

        public MemoryGetIdResult SpecificResult
        {
            get { return (MemoryGetIdResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new MemoryGetIdResult();
        }
    }

    public class MemoryGetIdResult : ActionResult
    {
        public byte NodeId { get; set; }
        public byte[] HomeId { get; set; }
    }
}
