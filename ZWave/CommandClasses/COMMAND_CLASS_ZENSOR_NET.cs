using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZENSOR_NET
    {
        public const byte ID = 0x02;
        public const byte VERSION = 1;
        public class BIND_ACCEPT
        {
            public const byte ID = 0x02;
            public static implicit operator BIND_ACCEPT(byte[] data)
            {
                BIND_ACCEPT ret = new BIND_ACCEPT();
                return ret;
            }
            public static implicit operator byte[](BIND_ACCEPT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZENSOR_NET.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class BIND_COMPLETE
        {
            public const byte ID = 0x03;
            public static implicit operator BIND_COMPLETE(byte[] data)
            {
                BIND_COMPLETE ret = new BIND_COMPLETE();
                return ret;
            }
            public static implicit operator byte[](BIND_COMPLETE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZENSOR_NET.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class BIND_REQUEST
        {
            public const byte ID = 0x01;
            public static implicit operator BIND_REQUEST(byte[] data)
            {
                BIND_REQUEST ret = new BIND_REQUEST();
                return ret;
            }
            public static implicit operator byte[](BIND_REQUEST command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZENSOR_NET.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
    }
}

