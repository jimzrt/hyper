using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY
    {
        public const byte ID = 0x52;
        public const byte VERSION = 1;
        public class NODE_INFO_CACHED_GET
        {
            public const byte ID = 0x03;
            public byte seqNo;
            public struct Tproperties1
            {
                private byte _value;
                public byte maxAge
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public static implicit operator NODE_INFO_CACHED_GET(byte[] data)
            {
                NODE_INFO_CACHED_GET ret = new NODE_INFO_CACHED_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_INFO_CACHED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.properties1);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class NODE_INFO_CACHED_REPORT
        {
            public const byte ID = 0x04;
            public byte seqNo;
            public struct Tproperties1
            {
                private byte _value;
                public byte age
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte status
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public struct Tproperties2
            {
                private byte _value;
                public byte zWaveProtocolSpecificPart1
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte listening
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
            public struct Tproperties3
            {
                private byte _value;
                public byte zWaveProtocolSpecificPart2
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte opt
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties3(byte data)
                {
                    Tproperties3 ret = new Tproperties3();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties3 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties3 properties3;
            public byte reserved;
            public byte basicDeviceClass;
            public byte genericDeviceClass;
            public byte specificDeviceClass;
            public IList<byte> commandClass = new List<byte>();
            public static implicit operator NODE_INFO_CACHED_REPORT(byte[] data)
            {
                NODE_INFO_CACHED_REPORT ret = new NODE_INFO_CACHED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.basicDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.genericDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.specificDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClass = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.commandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](NODE_INFO_CACHED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.properties3);
                ret.Add(command.reserved);
                ret.Add(command.basicDeviceClass);
                ret.Add(command.genericDeviceClass);
                ret.Add(command.specificDeviceClass);
                if (command.commandClass != null)
                {
                    foreach (var tmp in command.commandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class NODE_LIST_GET
        {
            public const byte ID = 0x01;
            public byte seqNo;
            public static implicit operator NODE_LIST_GET(byte[] data)
            {
                NODE_LIST_GET ret = new NODE_LIST_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_LIST_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                return ret.ToArray();
            }
        }
        public class NODE_LIST_REPORT
        {
            public const byte ID = 0x02;
            public byte seqNo;
            public byte status;
            public byte nodeListControllerId;
            public IList<byte> nodeListData = new List<byte>();
            public static implicit operator NODE_LIST_REPORT(byte[] data)
            {
                NODE_LIST_REPORT ret = new NODE_LIST_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeListControllerId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeListData = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.nodeListData.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](NODE_LIST_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                ret.Add(command.nodeListControllerId);
                if (command.nodeListData != null)
                {
                    foreach (var tmp in command.nodeListData)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

