using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_BASIC_V2
    {
        public const byte ID = 0x20;
        public const byte VERSION = 2;
        public class BASIC_GET
        {
            public const byte ID = 0x02;
            public static implicit operator BASIC_GET(byte[] data)
            {
                BASIC_GET ret = new BASIC_GET();
                return ret;
            }
            public static implicit operator byte[](BASIC_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class BASIC_REPORT
        {
            public const byte ID = 0x03;
            public byte currentValue;
            public byte targetValue;
            public byte duration;
            public static implicit operator BASIC_REPORT(byte[] data)
            {
                BASIC_REPORT ret = new BASIC_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.currentValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.targetValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BASIC_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_V2.ID);
                ret.Add(ID);
                ret.Add(command.currentValue);
                ret.Add(command.targetValue);
                ret.Add(command.duration);
                return ret.ToArray();
            }
        }
        public class BASIC_SET
        {
            public const byte ID = 0x01;
            public byte value;
            public static implicit operator BASIC_SET(byte[] data)
            {
                BASIC_SET ret = new BASIC_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.value = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BASIC_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_V2.ID);
                ret.Add(ID);
                ret.Add(command.value);
                return ret.ToArray();
            }
        }
    }
}

