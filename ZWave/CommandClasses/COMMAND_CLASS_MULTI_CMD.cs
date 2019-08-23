using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MULTI_CMD
    {
        public const byte ID = 0x8F;
        public const byte VERSION = 1;
        public class MULTI_CMD_ENCAP
        {
            public const byte ID = 0x01;
            public byte numberOfCommands;
            public class TENCAPSULATEDCOMMAND
            {
                public byte commandLength;
                public byte commandClass;
                public byte command;
                public IList<byte> data = new List<byte>();
            }
            public List<TENCAPSULATEDCOMMAND> encapsulatedCommand = new List<TENCAPSULATEDCOMMAND>();
            public static implicit operator MULTI_CMD_ENCAP(byte[] data)
            {
                MULTI_CMD_ENCAP ret = new MULTI_CMD_ENCAP();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfCommands = data.Length > index ? data[index++] : (byte)0x00;
                    ret.encapsulatedCommand = new List<TENCAPSULATEDCOMMAND>();
                    for (int j = 0; j < ret.numberOfCommands; j++)
                    {
                        TENCAPSULATEDCOMMAND tmp = new TENCAPSULATEDCOMMAND();
                        tmp.commandLength = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.command = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.data = new List<byte>();
                        for (int i = 0; i < tmp.commandLength - 2; i++)
                        {
                            tmp.data.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.encapsulatedCommand.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CMD_ENCAP command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CMD.ID);
                ret.Add(ID);
                ret.Add(command.numberOfCommands);
                if (command.encapsulatedCommand != null)
                {
                    foreach (var item in command.encapsulatedCommand)
                    {
                        ret.Add(item.commandLength);
                        ret.Add(item.commandClass);
                        ret.Add(item.command);
                        if (item.data != null)
                        {
                            foreach (var tmp in item.data)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

