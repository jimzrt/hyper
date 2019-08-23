using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication
{
    public class SetSmartStartAction : ApiOperation
    {
        private bool _isStart;
        public SetSmartStartAction(bool isStart)
            : base(false, null, false)
        {
            _isStart = isStart;
        }

        private ApiMessage _messageStart;
        private ApiMessage _messageStop;
        protected override void CreateWorkflow()
        {
            if (_isStart)
            {
                ActionUnits.Add(new StartActionUnit(null, 0, _messageStart, new TimeInterval(0, 500)));
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(null, 0, _messageStop, new TimeInterval(0, 500)));
            }
            ActionUnits.Add(new TimeElapsedUnit(0, SetStateCompleted, 0));
        }

        protected override void CreateInstance()
        {
            _messageStart = new ApiMessage(CommandTypes.CmdZWaveAddNodeToNetwork, (byte)(Modes.NodeOptionNetworkWide | Modes.NodeSmartStart));
            _messageStart.SetSequenceNumber(SequenceNumber);

            _messageStop = new ApiMessage(CommandTypes.CmdZWaveAddNodeToNetwork, new byte[] { (byte)Modes.NodeStop });
            _messageStop.SetSequenceNumber(0); //NULL funcID = 0
        }

        public override string AboutMe()
        {
            return _isStart ? "ON" : "OFF";
        }
    }
}
