using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MTP_WINDOW_COVERING
    {
        public const byte ID = 0x51;
        public const byte VERSION = 1;
        public class MOVE_TO_POSITION_GET
        {
            public const byte ID = 0x02;
            public static implicit operator MOVE_TO_POSITION_GET(byte[] data)
            {
                MOVE_TO_POSITION_GET ret = new MOVE_TO_POSITION_GET();
                return ret;
            }
            public static implicit operator byte[](MOVE_TO_POSITION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MTP_WINDOW_COVERING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MOVE_TO_POSITION_REPORT
        {
            public const byte ID = 0x03;
            public byte value;
            public static implicit operator MOVE_TO_POSITION_REPORT(byte[] data)
            {
                MOVE_TO_POSITION_REPORT ret = new MOVE_TO_POSITION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.value = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MOVE_TO_POSITION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MTP_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.value);
                return ret.ToArray();
            }
        }
        public class MOVE_TO_POSITION_SET
        {
            public const byte ID = 0x01;
            public byte value;
            public static implicit operator MOVE_TO_POSITION_SET(byte[] data)
            {
                MOVE_TO_POSITION_SET ret = new MOVE_TO_POSITION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.value = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MOVE_TO_POSITION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MTP_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.value);
                return ret.ToArray();
            }
        }
    }
}

