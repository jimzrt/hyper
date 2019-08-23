using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_APPLICATION_CAPABILITY
    {
        public const byte ID = 0x57;
        public const byte VERSION = 1;
        public class COMMAND_COMMAND_CLASS_NOT_SUPPORTED
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte dynamic
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte offendingCommandClass;
            public byte offendingCommand;
            public static implicit operator COMMAND_COMMAND_CLASS_NOT_SUPPORTED(byte[] data)
            {
                COMMAND_COMMAND_CLASS_NOT_SUPPORTED ret = new COMMAND_COMMAND_CLASS_NOT_SUPPORTED();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.offendingCommandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.offendingCommand = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_COMMAND_CLASS_NOT_SUPPORTED command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_APPLICATION_CAPABILITY.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.offendingCommandClass);
                ret.Add(command.offendingCommand);
                return ret.ToArray();
            }
        }
    }
}

