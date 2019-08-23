using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class GetBackgroundRssiOperation : RequestApiOperation
    {
        public GetBackgroundRssiOperation()
            : base(CommandTypes.GetBackgroundRSSI, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            SpecificResult.BackgroundRSSILevels = res;
            base.SetStateCompleted(ou);
        }

        public GetBackgroundRssiResult SpecificResult
        {
            get { return (GetBackgroundRssiResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetBackgroundRssiResult();
        }
    }

    public class GetBackgroundRssiResult : ActionResult
    {
        public byte[] BackgroundRSSILevels { get; set; }
    }
}
