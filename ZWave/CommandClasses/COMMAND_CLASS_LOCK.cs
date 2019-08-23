using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_LOCK
    {
        public const byte ID = 0x76;
        public const byte VERSION = 1;
        public class LOCK_GET
        {
            public const byte ID = 0x02;
            public static implicit operator LOCK_GET(byte[] data)
            {
                LOCK_GET ret = new LOCK_GET();
                return ret;
            }
            public static implicit operator byte[](LOCK_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_LOCK.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class LOCK_REPORT
        {
            public const byte ID = 0x03;
            public byte lockState;
            public static implicit operator LOCK_REPORT(byte[] data)
            {
                LOCK_REPORT ret = new LOCK_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.lockState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](LOCK_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_LOCK.ID);
                ret.Add(ID);
                ret.Add(command.lockState);
                return ret.ToArray();
            }
        }
        public class LOCK_SET
        {
            public const byte ID = 0x01;
            public byte lockState;
            public static implicit operator LOCK_SET(byte[] data)
            {
                LOCK_SET ret = new LOCK_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.lockState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](LOCK_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_LOCK.ID);
                ret.Add(ID);
                ret.Add(command.lockState);
                return ret.ToArray();
            }
        }
    }
}

