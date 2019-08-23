using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_IP_ASSOCIATION
    {
        public const byte ID = 0x5C;
        public const byte VERSION = 1;
        public class IP_ASSOCIATION_SET
        {
            public const byte ID = 0x01;
            public byte groupingIdentifier;
            public byte[] ipv6Address = new byte[16];
            public byte endPoint;
            public static implicit operator IP_ASSOCIATION_SET(byte[] data)
            {
                IP_ASSOCIATION_SET ret = new IP_ASSOCIATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
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
                    ret.endPoint = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IP_ASSOCIATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.endPoint);
                return ret.ToArray();
            }
        }
        public class IP_ASSOCIATION_GET
        {
            public const byte ID = 0x02;
            public byte groupingIdentifier;
            public byte index;
            public static implicit operator IP_ASSOCIATION_GET(byte[] data)
            {
                IP_ASSOCIATION_GET ret = new IP_ASSOCIATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.index = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IP_ASSOCIATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.index);
                return ret.ToArray();
            }
        }
        public class IP_ASSOCIATION_REPORT
        {
            public const byte ID = 0x03;
            public byte groupingIdentifier;
            public byte index;
            public byte actualNodes;
            public byte[] ipv6Address = new byte[16];
            public byte endPoint;
            public static implicit operator IP_ASSOCIATION_REPORT(byte[] data)
            {
                IP_ASSOCIATION_REPORT ret = new IP_ASSOCIATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.index = data.Length > index ? data[index++] : (byte)0x00;
                    ret.actualNodes = data.Length > index ? data[index++] : (byte)0x00;
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
                    ret.endPoint = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IP_ASSOCIATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.index);
                ret.Add(command.actualNodes);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.endPoint);
                return ret.ToArray();
            }
        }
        public class IP_ASSOCIATION_REMOVE
        {
            public const byte ID = 0x04;
            public byte groupingIdentifier;
            public byte[] ipv6Address = new byte[16];
            public byte endPoint;
            public static implicit operator IP_ASSOCIATION_REMOVE(byte[] data)
            {
                IP_ASSOCIATION_REMOVE ret = new IP_ASSOCIATION_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
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
                    ret.endPoint = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IP_ASSOCIATION_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.endPoint);
                return ret.ToArray();
            }
        }
    }
}

