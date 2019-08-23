using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2
    {
        public const byte ID = 0x59;
        public const byte VERSION = 2;
        public class ASSOCIATION_GROUP_NAME_GET
        {
            public const byte ID = 0x01;
            public byte groupingIdentifier;
            public static implicit operator ASSOCIATION_GROUP_NAME_GET(byte[] data)
            {
                ASSOCIATION_GROUP_NAME_GET ret = new ASSOCIATION_GROUP_NAME_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUP_NAME_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUP_NAME_REPORT
        {
            public const byte ID = 0x02;
            public byte groupingIdentifier;
            public byte lengthOfName;
            public IList<byte> name = new List<byte>();
            public static implicit operator ASSOCIATION_GROUP_NAME_REPORT(byte[] data)
            {
                ASSOCIATION_GROUP_NAME_REPORT ret = new ASSOCIATION_GROUP_NAME_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lengthOfName = data.Length > index ? data[index++] : (byte)0x00;
                    ret.name = new List<byte>();
                    for (int i = 0; i < ret.lengthOfName; i++)
                    {
                        ret.name.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUP_NAME_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.lengthOfName);
                if (command.name != null)
                {
                    foreach (var tmp in command.name)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUP_INFO_GET
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte listMode
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte refreshCache
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
            public byte groupingIdentifier;
            public static implicit operator ASSOCIATION_GROUP_INFO_GET(byte[] data)
            {
                ASSOCIATION_GROUP_INFO_GET ret = new ASSOCIATION_GROUP_INFO_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUP_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.groupingIdentifier);
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUP_INFO_REPORT
        {
            public const byte ID = 0x04;
            public struct Tproperties1
            {
                private byte _value;
                public byte groupCount
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte dynamicInfo
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte listMode
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
            public class TVG1
            {
                public byte groupingIdentifier;
                public byte mode;
                public byte profile1;
                public byte profile2;
                public byte reserved;
                public byte[] eventCode = new byte[2];
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator ASSOCIATION_GROUP_INFO_REPORT(byte[] data)
            {
                ASSOCIATION_GROUP_INFO_REPORT ret = new ASSOCIATION_GROUP_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.properties1.groupCount; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.mode = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.profile1 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.profile2 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.reserved = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.eventCode = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUP_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.groupingIdentifier);
                        ret.Add(item.mode);
                        ret.Add(item.profile1);
                        ret.Add(item.profile2);
                        ret.Add(item.reserved);
                        ret.Add(item.eventCode[0]);
                        ret.Add(item.eventCode[1]);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUP_COMMAND_LIST_GET
        {
            public const byte ID = 0x05;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte allowCache
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
            public byte groupingIdentifier;
            public static implicit operator ASSOCIATION_GROUP_COMMAND_LIST_GET(byte[] data)
            {
                ASSOCIATION_GROUP_COMMAND_LIST_GET ret = new ASSOCIATION_GROUP_COMMAND_LIST_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUP_COMMAND_LIST_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.groupingIdentifier);
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUP_COMMAND_LIST_REPORT
        {
            public const byte ID = 0x06;
            public byte groupingIdentifier;
            public byte listLength;
            public IList<byte> command = new List<byte>();
            public static implicit operator ASSOCIATION_GROUP_COMMAND_LIST_REPORT(byte[] data)
            {
                ASSOCIATION_GROUP_COMMAND_LIST_REPORT ret = new ASSOCIATION_GROUP_COMMAND_LIST_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.listLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.command = new List<byte>();
                    for (int i = 0; i < ret.listLength; i++)
                    {
                        ret.command.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUP_COMMAND_LIST_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_GRP_INFO_V2.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.listLength);
                if (command.command != null)
                {
                    foreach (var tmp in command.command)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

