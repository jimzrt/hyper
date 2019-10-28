using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class PowerLevelSupport : ApiAchOperation
    {
        public byte Value { get; set; }
        public PowerLevelSupport(NetworkViewPoint network)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_POWERLEVEL.ID))
        {
            _network = network;
        }

        private readonly NetworkViewPoint _network;
        private SendTestFrameOperation _sendTest;
        private RFPowerLevelGetOperation _powerLevelGet;
        private RFPowerLevelSetOperation _powerLevelSet;
        private SendDataExOperation _sendPowerLevelTestNodeReport;
        private SendDataExOperation _sendPowerLevelReport;
        private int _testIteration = 0;
        private int _failIterations = 0;
        private int _testFrameCount = 0;
        private byte _testFromNodeId = 0;

        protected override void CreateWorkflow()
        {
            base.CreateWorkflow();
            ActionUnits.Add(new ActionCompletedUnit(_powerLevelGet, OnPowerLevelGet, _sendPowerLevelReport));
            ActionUnits.Add(new ActionCompletedUnit(_sendTest, OnTestCompleted));
        }

        protected override void CreateInstance()
        {
            _powerLevelGet = new RFPowerLevelGetOperation();
            _powerLevelSet = new RFPowerLevelSetOperation(0);
            _sendTest = new SendTestFrameOperation(0, 0x06);
            _sendPowerLevelReport = new SendDataExOperation(0, null, TransmitOptions.TransmitOptionAcknowledge, SecuritySchemes.NONE);
            _sendPowerLevelReport.SubstituteSettings.SetFlag(SubstituteFlags.DenySupervision);
            _sendPowerLevelTestNodeReport = new SendDataExOperation(0, null, TransmitOptions.TransmitOptionAcknowledge, SecuritySchemes.NONE);
            _sendPowerLevelTestNodeReport.SubstituteSettings.SetFlag(SubstituteFlags.DenySupervision);

            base.CreateInstance();
        }

        private void OnPowerLevelGet(ActionCompletedUnit ou)
        {
            _sendPowerLevelReport.Data = new COMMAND_CLASS_POWERLEVEL.POWERLEVEL_REPORT()
            {
                powerLevel = _powerLevelGet.SpecificResult.PowerLevel
            };
        }

        private void OnTestCompleted(ActionCompletedUnit ou)
        {
            _testIteration++;
            if (_sendTest.SpecificResult.TransmitStatus != TransmitStatuses.CompleteOk)
            {
                _failIterations++;
            }
            if (_testIteration >= _testFrameCount)
            {
                _sendPowerLevelTestNodeReport.NewToken();
                _sendPowerLevelTestNodeReport.NodeId = _testFromNodeId;
                _sendPowerLevelTestNodeReport.Data = new COMMAND_CLASS_POWERLEVEL.POWERLEVEL_TEST_NODE_REPORT()
                {
                    statusOfOperation = _failIterations == 0 ? (byte)0x01 : (byte)0x00,
                    testFrameCount = new byte[] { (byte)(_testIteration << 8), (byte)_testIteration },
                    testNodeid = _sendTest.NodeId
                };
                ou.SetNextActionItems(_sendPowerLevelTestNodeReport);
            }
            else
            {
                _sendTest.NewToken();
                ou.SetNextActionItems(_sendTest);
            }
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] command = ReceivedAchData.Command;
            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
            if (command != null && command.Length > 1 && isSuportedScheme)
            {
                if (command[1] == COMMAND_CLASS_POWERLEVEL.POWERLEVEL_GET.ID)
                {
                    _powerLevelGet.NewToken();
                    _sendPowerLevelReport.NewToken();
                    _sendPowerLevelReport.NodeId = nodeId;
                    _sendPowerLevelReport.SecurityScheme = scheme;
                    ou.SetNextActionItems(_powerLevelGet);
                }
                if (command[1] == COMMAND_CLASS_POWERLEVEL.POWERLEVEL_SET.ID)
                {
                    COMMAND_CLASS_POWERLEVEL.POWERLEVEL_SET cmd = command;
                    _powerLevelSet.NewToken();
                    _powerLevelSet.PowerLevel = cmd.powerLevel;
                    ou.SetNextActionItems(_powerLevelSet);
                }
                else if (command[1] == COMMAND_CLASS_POWERLEVEL.POWERLEVEL_TEST_NODE_SET.ID)
                {
                    COMMAND_CLASS_POWERLEVEL.POWERLEVEL_TEST_NODE_SET cmd = command;
                    _sendTest.NewToken();
                    _sendTest.NodeId = cmd.testNodeid;
                    _sendTest.PowerLevel = cmd.powerLevel;

                    _testFromNodeId = nodeId;
                    _testFrameCount = Tools.GetInt32(cmd.testFrameCount);
                    _testIteration = 0;
                    _failIterations = 0;
                    ou.SetNextActionItems(_sendTest);
                }
                else if (command[1] == COMMAND_CLASS_POWERLEVEL.POWERLEVEL_TEST_NODE_GET.ID)
                {
                    COMMAND_CLASS_POWERLEVEL.POWERLEVEL_TEST_NODE_GET cmd = command;
                    _sendPowerLevelTestNodeReport.NewToken();
                    _sendPowerLevelTestNodeReport.NodeId = nodeId;
                    _sendPowerLevelTestNodeReport.SecurityScheme = scheme;
                    _sendPowerLevelTestNodeReport.Data = new COMMAND_CLASS_POWERLEVEL.POWERLEVEL_TEST_NODE_REPORT()
                    {
                        statusOfOperation = _failIterations == 0 ? (byte)0x01 : (byte)0x00,
                        testFrameCount = new byte[] { (byte)(_testIteration << 8), (byte)_testIteration },
                        testNodeid = _sendTest.NodeId
                    };
                    ou.SetNextActionItems(_sendPowerLevelTestNodeReport);
                }
            }
        }
    }
}
