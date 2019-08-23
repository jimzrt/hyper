using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class BasicSupport : ApiAchOperation
    {
        private NetworkViewPoint _network;
        public byte Value { get; set; }
        public TransmitOptions TxOptions { get; set; }
        public TransmitOptions2 TxOptions2 { get; set; }
        public TransmitSecurityOptions TxSecOptions { get; set; }

        public BasicSupport(NetworkViewPoint network, TransmitOptions txOptions)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_BASIC.ID))
        {
            _network = network;
            TxOptions = txOptions;
            TxOptions2 = TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE;
            TxSecOptions = TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY;
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            byte[] command = ReceivedAchData.Command;
            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
            if (command != null && command.Length > 1 && isSuportedScheme)
            {
                if (command[1] == COMMAND_CLASS_BASIC.BASIC_GET.ID)
                {
                    ApiOperation sendData = null;
                    var data = new COMMAND_CLASS_BASIC.BASIC_REPORT() { value = Value };
                    sendData = new SendDataExOperation(ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, data, TxOptions, TxSecOptions, scheme, TxOptions2);
                    ou.SetNextActionItems(sendData);
                }
                else if (command[1] == COMMAND_CLASS_BASIC.BASIC_SET.ID)
                {
                    var basicSetCommand = (COMMAND_CLASS_BASIC.BASIC_SET)command;
                    Value = basicSetCommand.value;
                }
            }
        }
    }
}
