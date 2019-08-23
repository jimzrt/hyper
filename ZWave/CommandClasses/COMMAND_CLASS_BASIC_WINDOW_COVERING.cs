using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_BASIC_WINDOW_COVERING
    {
        public const byte ID = 0x50;
        public const byte VERSION = 1;
        public class BASIC_WINDOW_COVERING_START_LEVEL_CHANGE
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved1
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte openClose
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte reserved2
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
            public static implicit operator BASIC_WINDOW_COVERING_START_LEVEL_CHANGE(byte[] data)
            {
                BASIC_WINDOW_COVERING_START_LEVEL_CHANGE ret = new BASIC_WINDOW_COVERING_START_LEVEL_CHANGE();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BASIC_WINDOW_COVERING_START_LEVEL_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class BASIC_WINDOW_COVERING_STOP_LEVEL_CHANGE
        {
            public const byte ID = 0x02;
            public static implicit operator BASIC_WINDOW_COVERING_STOP_LEVEL_CHANGE(byte[] data)
            {
                BASIC_WINDOW_COVERING_STOP_LEVEL_CHANGE ret = new BASIC_WINDOW_COVERING_STOP_LEVEL_CHANGE();
                return ret;
            }
            public static implicit operator byte[](BASIC_WINDOW_COVERING_STOP_LEVEL_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_WINDOW_COVERING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
    }
}

