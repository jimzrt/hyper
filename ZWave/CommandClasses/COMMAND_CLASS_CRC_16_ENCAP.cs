using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CRC_16_ENCAP
    {
        public const byte ID = 0x56;
        public const byte VERSION = 1;
        public class CRC_16_ENCAP
        {
            public const byte ID = 0x01;
            public byte commandClass;
            public byte command;
            public IList<byte> data = new List<byte>();
            public byte[] checksum = new byte[2];
            public static implicit operator CRC_16_ENCAP(byte[] data)
            {
                CRC_16_ENCAP ret = new CRC_16_ENCAP();
                if (data != null)
                {
                    int index = 2;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.command = data.Length > index ? data[index++] : (byte)0x00;
                    ret.data = new List<byte>();
                    while (data.Length - 2 > index)
                    {
                        ret.data.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.checksum = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](CRC_16_ENCAP command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CRC_16_ENCAP.ID);
                ret.Add(ID);
                ret.Add(command.commandClass);
                ret.Add(command.command);
                if (command.data != null)
                {
                    foreach (var tmp in command.data)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.checksum[0]);
                ret.Add(command.checksum[1]);
                return ret.ToArray();
            }
        }
    }
}

