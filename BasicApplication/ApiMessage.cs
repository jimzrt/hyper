using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication
{
    public class ApiMessage : CommandMessage
    {
        public ApiMessage(CommandTypes command, params byte[] inputParameters)
        {
            AddData((byte)FrameTypes.Request, (byte)command);
            AddData(inputParameters);
        }
    }

    public class ApiProgMessage : CommandMessage
    {
        public ApiProgMessage(params byte[] inputParameters)
        {
            AddData((byte)CommandTypes.TEST_INTERFACE, 0x00);
            AddData(inputParameters);
        }
    }
}
