using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_TRANSPORT_SERVICE
    {
        public const byte ID = 0x55;
        public const byte VERSION = 1;
        public class COMMAND_FIRST_FRAGMENT
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
            public struct Tproperties2
            {
                private byte _value;
                public byte sequenceNo
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> payload = new List<byte>();
            public byte[] checksum = new byte[2];
            public static implicit operator COMMAND_FIRST_FRAGMENT(byte[] data)
            {
                COMMAND_FIRST_FRAGMENT ret = new COMMAND_FIRST_FRAGMENT();
                if (data != null)
                {
                    int index = 1;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramSize2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.payload = new List<byte>();
                    while (data.Length - 2 > index)
                    {
                        ret.payload.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.checksum = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_FIRST_FRAGMENT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TRANSPORT_SERVICE.ID);
                ret.Add((byte)((ID & ID_MASK) + command.properties1));
                ret.Add(command.datagramSize2);
                ret.Add(command.properties2);
                if (command.payload != null)
                {
                    foreach (var tmp in command.payload)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.checksum[0]);
                ret.Add(command.checksum[1]);
                return ret.ToArray();
            }
        }
        public class COMMAND_SUBSEQUENT_FRAGMENT
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
            public struct Tproperties2
            {
                private byte _value;
                public byte datagramOffset1
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte sequenceNo
                {
                    get { return (byte)(_value >> 3 & 0x0F); }
                    set { _value &= 0xFF - 0x78; _value += (byte)(value << 3 & 0x78); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public byte datagramOffset2;
            public IList<byte> payload = new List<byte>();
            public byte[] checksum = new byte[2];
            public static implicit operator COMMAND_SUBSEQUENT_FRAGMENT(byte[] data)
            {
                COMMAND_SUBSEQUENT_FRAGMENT ret = new COMMAND_SUBSEQUENT_FRAGMENT();
                if (data != null)
                {
                    int index = 1;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramSize2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datagramOffset2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.payload = new List<byte>();
                    while (data.Length - 2 > index)
                    {
                        ret.payload.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.checksum = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_SUBSEQUENT_FRAGMENT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TRANSPORT_SERVICE.ID);
                ret.Add((byte)((ID & ID_MASK) + command.properties1));
                ret.Add(command.datagramSize2);
                ret.Add(command.properties2);
                ret.Add(command.datagramOffset2);
                if (command.payload != null)
                {
                    foreach (var tmp in command.payload)
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

