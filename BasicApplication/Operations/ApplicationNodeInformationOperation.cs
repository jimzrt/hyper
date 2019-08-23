using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ApplicationNodeInformationOperation : ControlNApiOperation
    {
        private byte Generic { get; set; }
        private byte Specific { get; set; }
        private byte[] NodeParameters { get; set; }
        private DeviceOptions DeviceOptions { get; set; }
        public ApplicationNodeInformationOperation(bool isListening, byte generic, byte specific, byte[] nodeParameters)
            : this(isListening ? DeviceOptions.Listening : DeviceOptions.NoneListening, generic, specific, nodeParameters)
        {
        }

        public ApplicationNodeInformationOperation(DeviceOptions deviceOptions, byte generic, byte specific, byte[] nodeParameters)
            : base(CommandTypes.CmdSerialApiApplNodeInformation)
        {
            DeviceOptions = deviceOptions;
            Generic = generic;
            Specific = specific;
            NodeParameters = nodeParameters;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[NodeParameters.Length + 4];
            ret[0] = (byte)DeviceOptions;
            ret[1] = Generic;
            ret[2] = Specific;
            ret[3] = (byte)NodeParameters.Length;
            for (int i = 0; i < NodeParameters.Length; i++)
            {
                ret[i + 4] = NodeParameters[i];
            }
            return ret;
        }
    }
}
