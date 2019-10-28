using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SerialApiSetupOperation : ApiOperation
    {
        private readonly byte[] _args;
        public SerialApiSetupOperation(params byte[] args)
            : base(true, CommandTypes.CmdSerialApiSetup, false)
        {
            _args = args;
        }

        private ApiMessage _message;
        private ApiHandler _handler;
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 200, _message));
            ActionUnits.Add(new DataReceivedUnit(_handler, SetStateCompleted, _message));
        }

        protected override void CreateInstance()
        {
            _message = new ApiMessage(SerialApiCommands[0], _args);
            _handler = new ApiHandler(SerialApiCommands[0]);
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.RetValue = ((ApiHandler)ou.ActionHandler).DataFrame.Payload;
            base.SetStateCompleted(ou);
        }

        public SerialApiSetupResult SpecificResult
        {
            get { return (SerialApiSetupResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SerialApiSetupResult();
        }
    }

    public class SerialApiSetupResult : ActionResult
    {
        public byte[] RetValue { get; set; }
    }
}
