using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3
    {
        public const byte ID = 0x8E;
        public const byte VERSION = 3;
        public class MULTI_CHANNEL_ASSOCIATION_GET
        {
            public const byte ID = 0x02;
            public byte groupingIdentifier;
            public static implicit operator MULTI_CHANNEL_ASSOCIATION_GET(byte[] data)
            {
                MULTI_CHANNEL_ASSOCIATION_GET ret = new MULTI_CHANNEL_ASSOCIATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_ASSOCIATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET
        {
            public const byte ID = 0x05;
            public static implicit operator MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET(byte[] data)
            {
                MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET ret = new MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET();
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT
        {
            public const byte ID = 0x06;
            public byte supportedGroupings;
            public static implicit operator MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT(byte[] data)
            {
                MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT ret = new MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedGroupings = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID);
                ret.Add(ID);
                ret.Add(command.supportedGroupings);
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_ASSOCIATION_REMOVE
        {
            public const byte ID = 0x04;
            public byte groupingIdentifier;
            public IList<byte> nodeId = new List<byte>();
            private readonly byte[] marker = { 0x00 };
            public class TVG
            {
                public byte multiChannelNodeId;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte endPoint
                    {
                        get { return (byte)(_value >> 0 & 0x7F); }
                        set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                    }
                    public byte bitAddress
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
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator MULTI_CHANNEL_ASSOCIATION_REMOVE(byte[] data)
            {
                MULTI_CHANNEL_ASSOCIATION_REMOVE ret = new MULTI_CHANNEL_ASSOCIATION_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = new List<byte>();
                    while (data.Length - 0 > index && (data.Length - 1 < index || data[index + 0] != ret.marker[0]))
                    {
                        ret.nodeId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    index++; //Marker
                    ret.vg = new List<TVG>();
                    while (data.Length - 0 > index)
                    {
                        TVG tmp = new TVG();
                        tmp.multiChannelNodeId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_ASSOCIATION_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                if (command.nodeId != null)
                {
                    foreach (var tmp in command.nodeId)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.marker[0]);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.multiChannelNodeId);
                        ret.Add(item.properties1);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_ASSOCIATION_REPORT
        {
            public const byte ID = 0x03;
            public byte groupingIdentifier;
            public byte maxNodesSupported;
            public byte reportsToFollow;
            public IList<byte> nodeId = new List<byte>();
            private readonly byte[] marker = { 0x00 };
            public class TVG
            {
                public byte multiChannelNodeId;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte endPoint
                    {
                        get { return (byte)(_value >> 0 & 0x7F); }
                        set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                    }
                    public byte bitAddress
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
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator MULTI_CHANNEL_ASSOCIATION_REPORT(byte[] data)
            {
                MULTI_CHANNEL_ASSOCIATION_REPORT ret = new MULTI_CHANNEL_ASSOCIATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.maxNodesSupported = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = new List<byte>();
                    while (data.Length - 0 > index && (data.Length - 1 < index || data[index + 0] != ret.marker[0]))
                    {
                        ret.nodeId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    index++; //Marker
                    ret.vg = new List<TVG>();
                    while (data.Length - 0 > index)
                    {
                        TVG tmp = new TVG();
                        tmp.multiChannelNodeId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_ASSOCIATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.maxNodesSupported);
                ret.Add(command.reportsToFollow);
                if (command.nodeId != null)
                {
                    foreach (var tmp in command.nodeId)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.marker[0]);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.multiChannelNodeId);
                        ret.Add(item.properties1);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_ASSOCIATION_SET
        {
            public const byte ID = 0x01;
            public byte groupingIdentifier;
            public IList<byte> nodeId = new List<byte>();
            private readonly byte[] marker = { 0x00 };
            public class TVG
            {
                public byte multiChannelNodeId;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte endPoint
                    {
                        get { return (byte)(_value >> 0 & 0x7F); }
                        set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                    }
                    public byte bitAddress
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
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator MULTI_CHANNEL_ASSOCIATION_SET(byte[] data)
            {
                MULTI_CHANNEL_ASSOCIATION_SET ret = new MULTI_CHANNEL_ASSOCIATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = new List<byte>();
                    while (data.Length - 0 > index && (data.Length - 1 < index || data[index + 0] != ret.marker[0]))
                    {
                        ret.nodeId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    index++; //Marker
                    ret.vg = new List<TVG>();
                    while (data.Length - 0 > index)
                    {
                        TVG tmp = new TVG();
                        tmp.multiChannelNodeId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_ASSOCIATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                if (command.nodeId != null)
                {
                    foreach (var tmp in command.nodeId)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.marker[0]);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.multiChannelNodeId);
                        ret.Add(item.properties1);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

