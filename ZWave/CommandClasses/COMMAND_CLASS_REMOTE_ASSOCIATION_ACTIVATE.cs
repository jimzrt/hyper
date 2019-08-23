using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_REMOTE_ASSOCIATION_ACTIVATE
    {
        public const byte ID = 0x7C;
        public const byte VERSION = 1;
        public class REMOTE_ASSOCIATION_ACTIVATE
        {
            public const byte ID = 0x01;
            public byte groupingIdentifier;
            public static implicit operator REMOTE_ASSOCIATION_ACTIVATE(byte[] data)
            {
                REMOTE_ASSOCIATION_ACTIVATE ret = new REMOTE_ASSOCIATION_ACTIVATE();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](REMOTE_ASSOCIATION_ACTIVATE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_REMOTE_ASSOCIATION_ACTIVATE.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                return ret.ToArray();
            }
        }
    }
}

