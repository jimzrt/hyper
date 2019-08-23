using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x38 
    /// ZW->HOST: RES | 0x38 | TxTimeChannel0 | TxTimeChannel1 | TxTimeChannel2
    /// </summary>
    public class GetTxTimerOperation : ApiOperation
    {
        public GetTxTimerOperation() : base(true, CommandTypes.CmdGetTxTimer, false)
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
            _message = new ApiMessage(CommandTypes.CmdGetTxTimer);
            _handler = new ApiHandler(CommandTypes.CmdGetTxTimer);
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            var payload = ou.DataFrame.Payload;
            SpecificResult.TxTimeChannel0 = payload[0];
            SpecificResult.TxTimeChannel1 = payload[1];
            SpecificResult.TxTimeChannel2 = payload[2];
            SetStateCompleted(ou);
        }

        public GetTxTimerResult SpecificResult
        {
            get { return (GetTxTimerResult)Result; }
        }
        protected override ActionResult CreateOperationResult()
        {
            return new GetTxTimerResult();
        }
    }

    public class GetTxTimerResult : ActionResult
    {
        public byte TxTimeChannel0 { get; set; }
        public byte TxTimeChannel1 { get; set; }
        public byte TxTimeChannel2 { get; set; }
    }
}
