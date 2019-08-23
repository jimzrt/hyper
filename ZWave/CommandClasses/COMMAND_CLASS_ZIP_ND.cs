using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZIP_ND
    {
        public const byte ID = 0x58;
        public const byte VERSION = 1;
        public class ZIP_NODE_SOLICITATION
        {
            public const byte ID = 0x03;
            public byte reserved;
            public byte nodeId;
            public byte[] ipv6Address = new byte[16];
            public static implicit operator ZIP_NODE_SOLICITATION(byte[] data)
            {
                ZIP_NODE_SOLICITATION ret = new ZIP_NODE_SOLICITATION();
                if (data != null)
                {
                    int index = 2;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ipv6Address = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](ZIP_NODE_SOLICITATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_ND.ID);
                ret.Add(ID);
                ret.Add(command.reserved);
                ret.Add(command.nodeId);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ZIP_INV_NODE_SOLICITATION
        {
            public const byte ID = 0x04;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved1
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte local
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte reserved2
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public byte nodeId;
            public static implicit operator ZIP_INV_NODE_SOLICITATION(byte[] data)
            {
                ZIP_INV_NODE_SOLICITATION ret = new ZIP_INV_NODE_SOLICITATION();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ZIP_INV_NODE_SOLICITATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_ND.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class ZIP_NODE_ADVERTISEMENT
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte validity
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte local
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public byte nodeId;
            public byte[] ipv6Address = new byte[16];
            public byte[] homeId = new byte[4];
            public static implicit operator ZIP_NODE_ADVERTISEMENT(byte[] data)
            {
                ZIP_NODE_ADVERTISEMENT ret = new ZIP_NODE_ADVERTISEMENT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ipv6Address = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.homeId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](ZIP_NODE_ADVERTISEMENT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_ND.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.nodeId);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                if (command.homeId != null)
                {
                    foreach (var tmp in command.homeId)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

