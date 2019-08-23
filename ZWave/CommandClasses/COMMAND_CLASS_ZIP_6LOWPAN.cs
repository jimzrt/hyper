using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZIP_6LOWPAN
    {
        public const byte ID = 0x4F;
        public const byte VERSION = 1;
        public class LOWPAN_FIRST_FRAGMENT
        {
            public const byte ID = 0xC0;
            public const byte ID_MASK = 0xF8;
            public struct Tproperties1
            {
                private byte _value;
                public byte datagramSize1
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
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
            public byte datagramSize2;
            public byte datagramTag;
            public IList<byte> payload = new List<byte>();
            public static implicit operator LOWPAN_FIRST_FRAGMENT(byte[] data)
            {
                LOWPAN_FIRST_FRAGMENT ret = new LOWPAN_FIRST_FRAGMENT();
                if (data != null)
                {
                    int index = 1;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramSize2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramTag = data.Length > index ? data[index++] : (byte)0x00;
                    ret.payload = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.payload.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](LOWPAN_FIRST_FRAGMENT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_6LOWPAN.ID);
                ret.Add((byte)((ID & ID_MASK) + command.properties1));
                ret.Add(command.datagramSize2);
                ret.Add(command.datagramTag);
                if (command.payload != null)
                {
                    foreach (var tmp in command.payload)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class LOWPAN_SUBSEQUENT_FRAGMENT
        {
            public const byte ID = 0xE0;
            public const byte ID_MASK = 0xF8;
            public struct Tproperties1
            {
                private byte _value;
                public byte datagramSize1
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
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
            public byte datagramSize2;
            public byte datagramTag;
            public byte datagramOffset;
            public IList<byte> payload = new List<byte>();
            public static implicit operator LOWPAN_SUBSEQUENT_FRAGMENT(byte[] data)
            {
                LOWPAN_SUBSEQUENT_FRAGMENT ret = new LOWPAN_SUBSEQUENT_FRAGMENT();
                if (data != null)
                {
                    int index = 1;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramSize2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramTag = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramOffset = data.Length > index ? data[index++] : (byte)0x00;
                    ret.payload = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.payload.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](LOWPAN_SUBSEQUENT_FRAGMENT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_6LOWPAN.ID);
                ret.Add((byte)((ID & ID_MASK) + command.properties1));
                ret.Add(command.datagramSize2);
                ret.Add(command.datagramTag);
                ret.Add(command.datagramOffset);
                if (command.payload != null)
                {
                    foreach (var tmp in command.payload)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

