using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class GetSucNodeIdOperation : RequestApiOperation
    {
        public GetSucNodeIdOperation()
            : base(CommandTypes.CmdZWaveGetSucNodeId, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            SpecificResult.SucNodeId = res[0];
            base.SetStateCompleted(ou);
        }

        public GetSucNodeIdResult SpecificResult
        {
            get { return (GetSucNodeIdResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetSucNodeIdResult();
        }
    }

    public class GetSucNodeIdResult : ActionResult
    {
        public byte SucNodeId { get; set; }
    }
}
