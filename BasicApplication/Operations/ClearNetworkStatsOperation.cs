using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    internal class ClearNetworkStatsOperation : ApiOperation
    {
        public ClearNetworkStatsOperation() : base(true, CommandTypes.ClearNetworkStats, false)
        { }

        private ApiMessage _message;
        private ApiHandler _handler;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 2000, _message));
            ActionUnits.Add(new DataReceivedUnit(_handler, OnReceived));
        }
        protected override void CreateInstance()
        {
            _message = new ApiMessage(CommandTypes.ClearNetworkStats);
            _handler = new ApiHandler(CommandTypes.ClearNetworkStats);
        }
        private void OnReceived(DataReceivedUnit ou)
        {
            var payload = ou.DataFrame.Payload;
            SpecificResult.RetValue = payload[0];
            SetStateCompleted(ou);
        }

        public ClearNetworkStatsResult SpecificResult
        {
            get { return (ClearNetworkStatsResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ClearNetworkStatsResult();
        }

    }

    public class ClearNetworkStatsResult : ActionResult
    {
        public byte RetValue { get; set; }
    }
}
