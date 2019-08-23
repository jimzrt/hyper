using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CLOCK
    {
        public const byte ID = 0x81;
        public const byte VERSION = 1;
        public class CLOCK_GET
        {
            public const byte ID = 0x05;
            public static implicit operator CLOCK_GET(byte[] data)
            {
                CLOCK_GET ret = new CLOCK_GET();
                return ret;
            }
            public static implicit operator byte[](CLOCK_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLOCK.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CLOCK_REPORT
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte hour
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte weekday
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public byte minute;
            public static implicit operator CLOCK_REPORT(byte[] data)
            {
                CLOCK_REPORT ret = new CLOCK_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CLOCK_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLOCK.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minute);
                return ret.ToArray();
            }
        }
        public class CLOCK_SET
        {
            public const byte ID = 0x04;
            public struct Tproperties1
            {
                private byte _value;
                public byte hour
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte weekday
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public byte minute;
            public static implicit operator CLOCK_SET(byte[] data)
            {
                CLOCK_SET ret = new CLOCK_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CLOCK_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLOCK.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minute);
                return ret.ToArray();
            }
        }
    }
}

