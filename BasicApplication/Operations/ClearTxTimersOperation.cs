using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ClearTxTimersOperation : ApiOperation
    {
        public ClearTxTimersOperation() : base(true, CommandTypes.ClearTxTimer, false)
        { }
        private ApiMessage _message;
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(SetStateCompleting, 2000, _message));
        }
        protected override void CreateInstance()
        {
            _message = new ApiMessage(CommandTypes.ClearTxTimer);
        }

    }
}
