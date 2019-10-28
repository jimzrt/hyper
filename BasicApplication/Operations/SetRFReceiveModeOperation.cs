using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetRFReceiveModeOperation : ApiOperation
    {
        private int _maxAttempts = 2;
        private readonly int _startDelay = 5000;
        private readonly int _iterationDelay = 200;
        private byte Mode { get; set; }
        private readonly bool _isNoDelay = false;
        public SetRFReceiveModeOperation(byte mode)
            : base(true, CommandTypes.CmdZWaveSetRFReceiveMode, false)
        {
            Mode = mode;
        }
        public SetRFReceiveModeOperation(byte mode, bool isNoDelay)
            : base(true, CommandTypes.CmdZWaveSetRFReceiveMode, false)
        {
            Mode = mode;
            _isNoDelay = isNoDelay;
        }

        private ApiMessage message;
        private ApiHandler handler;

        protected override void CreateWorkflow()
        {
            if (_isNoDelay)
            {
                ActionUnits.Add(new StartActionUnit(null, 0, message));
                ActionUnits.Add(new DataReceivedUnit(handler, OnReceived));
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(null, _iterationDelay * _maxAttempts + _startDelay + 1000, new TimeInterval(0, _startDelay)));
                ActionUnits.Add(new TimeElapsedUnit(0, null, _iterationDelay * _maxAttempts + 1000, message));
                ActionUnits.Add(new DataReceivedUnit(handler, OnReceived, new TimeInterval(1, _iterationDelay)));
                ActionUnits.Add(new TimeElapsedUnit(1, null, 0, message));
            }
        }

        protected override void CreateInstance()
        {
            message = new ApiMessage(CommandTypes.CmdZWaveSetRFReceiveMode, Mode);
            handler = new ApiHandler(CommandTypes.CmdZWaveSetRFReceiveMode);
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            byte[] ret = ou.DataFrame.Payload;
            SpecificResult.RetVal = ret[0];
            _maxAttempts--;
            if (SpecificResult.RetVal > 0 || _maxAttempts <= 0)
            {
                SetStateCompleted(ou);
            }
            else
            {

            }
        }

        public SetRFReceiveModeResult SpecificResult
        {
            get { return (SetRFReceiveModeResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SetRFReceiveModeResult();
        }
    }

    public class SetRFReceiveModeResult : ActionResult
    {
        public byte RetVal { get; set; }
    }
}