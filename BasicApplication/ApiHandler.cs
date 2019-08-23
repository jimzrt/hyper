using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication
{
    public class ApiHandler : CommandHandler
    {
        public ApiHandler(CommandTypes command)
        {
            Mask = new ByteIndex[] { new ByteIndex((byte)FrameTypes.Response), new ByteIndex((byte)command) };
        }

        public ApiHandler(FrameTypes frameType, CommandTypes command)
        {
            Mask = new ByteIndex[] { new ByteIndex((byte)frameType), new ByteIndex((byte)command) };
        }
    }

    public class ApiProgHandler : CommandHandler
    {
        public ApiProgHandler(byte command)
        {
            Mask = new ByteIndex[] { ByteIndex.AnyValue, new ByteIndex((byte)FrameTypes.Response), new ByteIndex(command) };
        }

    }
}
