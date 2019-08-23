using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MULTI_INSTANCE
    {
        public const byte ID = 0x60;
        public const byte VERSION = 1;
        public class MULTI_INSTANCE_CMD_ENCAP
        {
            public const byte ID = 0x06;
            public byte instance;
            public byte commandClass;
            public byte command;
            public IList<byte> parameter = new List<byte>();
            public static implicit operator MULTI_INSTANCE_CMD_ENCAP(byte[] data)
            {
                MULTI_INSTANCE_CMD_ENCAP ret = new MULTI_INSTANCE_CMD_ENCAP();
                if (data != null)
                {
                    int index = 2;
                    ret.instance = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.command = data.Length > index ? data[index++] : (byte)0x00;
                    ret.parameter = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.parameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_INSTANCE_CMD_ENCAP command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_INSTANCE.ID);
                ret.Add(ID);
                ret.Add(command.instance);
                ret.Add(command.commandClass);
                ret.Add(command.command);
                if (command.parameter != null)
                {
                    foreach (var tmp in command.parameter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_INSTANCE_GET
        {
            public const byte ID = 0x04;
            public byte commandClass;
            public static implicit operator MULTI_INSTANCE_GET(byte[] data)
            {
                MULTI_INSTANCE_GET ret = new MULTI_INSTANCE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_INSTANCE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_INSTANCE.ID);
                ret.Add(ID);
                ret.Add(command.commandClass);
                return ret.ToArray();
            }
        }
        public class MULTI_INSTANCE_REPORT
        {
            public const byte ID = 0x05;
            public byte commandClass;
            public byte instances;
            public static implicit operator MULTI_INSTANCE_REPORT(byte[] data)
            {
                MULTI_INSTANCE_REPORT ret = new MULTI_INSTANCE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.instances = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_INSTANCE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_INSTANCE.ID);
                ret.Add(ID);
                ret.Add(command.commandClass);
                ret.Add(command.instances);
                return ret.ToArray();
            }
        }
    }
}

