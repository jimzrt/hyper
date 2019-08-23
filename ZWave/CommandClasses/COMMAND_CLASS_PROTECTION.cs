using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_PROTECTION
    {
        public const byte ID = 0x75;
        public const byte VERSION = 1;
        public class PROTECTION_GET
        {
            public const byte ID = 0x02;
            public static implicit operator PROTECTION_GET(byte[] data)
            {
                PROTECTION_GET ret = new PROTECTION_GET();
                return ret;
            }
            public static implicit operator byte[](PROTECTION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_PROTECTION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class PROTECTION_REPORT
        {
            public const byte ID = 0x03;
            public byte protectionState;
            public static implicit operator PROTECTION_REPORT(byte[] data)
            {
                PROTECTION_REPORT ret = new PROTECTION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.protectionState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](PROTECTION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_PROTECTION.ID);
                ret.Add(ID);
                ret.Add(command.protectionState);
                return ret.ToArray();
            }
        }
        public class PROTECTION_SET
        {
            public const byte ID = 0x01;
            public byte protectionState;
            public static implicit operator PROTECTION_SET(byte[] data)
            {
                PROTECTION_SET ret = new PROTECTION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.protectionState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](PROTECTION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_PROTECTION.ID);
                ret.Add(ID);
                ret.Add(command.protectionState);
                return ret.ToArray();
            }
        }
    }
}

