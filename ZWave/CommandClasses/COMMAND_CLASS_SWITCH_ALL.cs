using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SWITCH_ALL
    {
        public const byte ID = 0x27;
        public const byte VERSION = 1;
        public class SWITCH_ALL_GET
        {
            public const byte ID = 0x02;
            public static implicit operator SWITCH_ALL_GET(byte[] data)
            {
                SWITCH_ALL_GET ret = new SWITCH_ALL_GET();
                return ret;
            }
            public static implicit operator byte[](SWITCH_ALL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_ALL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SWITCH_ALL_OFF
        {
            public const byte ID = 0x05;
            public static implicit operator SWITCH_ALL_OFF(byte[] data)
            {
                SWITCH_ALL_OFF ret = new SWITCH_ALL_OFF();
                return ret;
            }
            public static implicit operator byte[](SWITCH_ALL_OFF command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_ALL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SWITCH_ALL_ON
        {
            public const byte ID = 0x04;
            public static implicit operator SWITCH_ALL_ON(byte[] data)
            {
                SWITCH_ALL_ON ret = new SWITCH_ALL_ON();
                return ret;
            }
            public static implicit operator byte[](SWITCH_ALL_ON command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_ALL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SWITCH_ALL_REPORT
        {
            public const byte ID = 0x03;
            public byte mode;
            public static implicit operator SWITCH_ALL_REPORT(byte[] data)
            {
                SWITCH_ALL_REPORT ret = new SWITCH_ALL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SWITCH_ALL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_ALL.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class SWITCH_ALL_SET
        {
            public const byte ID = 0x01;
            public byte mode;
            public static implicit operator SWITCH_ALL_SET(byte[] data)
            {
                SWITCH_ALL_SET ret = new SWITCH_ALL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SWITCH_ALL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_ALL.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
    }
}

