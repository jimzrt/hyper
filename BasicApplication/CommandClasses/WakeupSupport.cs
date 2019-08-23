using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class WakeupSupport : ApiAchOperation
    {
        private byte[] _wakeupDelay = new byte[2];
        private SetRFReceiveModeOperation _disableRFReceiveOperation;

        private NetworkViewPoint _network;
        public TransmitOptions TxOptions { get; set; }

        public WakeupSupport(NetworkViewPoint network, TransmitOptions txOptions)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_WAKE_UP_V2.ID))
        {
            _wakeupDelay = new byte[] { 0x00, 0x01, 0x2C }; //300 sec
            _network = network;
            TxOptions = txOptions;
        }

        protected override void CreateInstance()
        {
            _disableRFReceiveOperation = new SetRFReceiveModeOperation(0x00);
            base.CreateInstance();
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] command = ReceivedAchData.Command;
            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
            if (isSuportedScheme && command != null && command.Length > 1)
            {
                if (command[1] == COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_GET)command;
                    var rpt = new COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_REPORT()
                    {
                        minimumWakeUpIntervalSeconds = new byte[] { 0x00, 0x00, 0x3C },
                        maximumWakeUpIntervalSeconds = new byte[] { 0x01, 0x51, 0x80 },
                        defaultWakeUpIntervalSeconds = new byte[] { 0x00, 0x01, 0x2C },
                        wakeUpIntervalStepSeconds = new byte[] { 0x00, 0x00, 0x3C }
                    };
                    var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                    ou.SetNextActionItems(sendData);
                }
                else if (command[1] == COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_GET)command;
                    var rpt = new COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_REPORT()
                    {
                        seconds = _wakeupDelay
                    };
                    var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                    ou.SetNextActionItems(sendData);
                }
                else if (command[1] == COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_SET.ID)
                {
                    var cmd = (COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_SET)command;
                    _wakeupDelay = cmd.seconds;
                }
                else if (command[1] == COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NO_MORE_INFORMATION.ID)
                {
                    _disableRFReceiveOperation.NewToken();
                    ou.SetNextActionItems(_disableRFReceiveOperation);
                }
            }
        }

    }
}
