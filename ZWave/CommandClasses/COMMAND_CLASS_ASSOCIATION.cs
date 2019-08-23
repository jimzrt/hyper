using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ASSOCIATION
    {
        public const byte ID = 0x85;
        public const byte VERSION = 1;
        public class ASSOCIATION_GET
        {
            public const byte ID = 0x02;
            public byte groupingIdentifier;
            public static implicit operator ASSOCIATION_GET(byte[] data)
            {
                ASSOCIATION_GET ret = new ASSOCIATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUPINGS_GET
        {
            public const byte ID = 0x05;
            public static implicit operator ASSOCIATION_GROUPINGS_GET(byte[] data)
            {
                ASSOCIATION_GROUPINGS_GET ret = new ASSOCIATION_GROUPINGS_GET();
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUPINGS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_GROUPINGS_REPORT
        {
            public const byte ID = 0x06;
            public byte supportedGroupings;
            public static implicit operator ASSOCIATION_GROUPINGS_REPORT(byte[] data)
            {
                ASSOCIATION_GROUPINGS_REPORT ret = new ASSOCIATION_GROUPINGS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedGroupings = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_GROUPINGS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.supportedGroupings);
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_REMOVE
        {
            public const byte ID = 0x04;
            public byte groupingIdentifier;
            public IList<byte> nodeId = new List<byte>();
            public static implicit operator ASSOCIATION_REMOVE(byte[] data)
            {
                ASSOCIATION_REMOVE ret = new ASSOCIATION_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.nodeId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                if (command.nodeId != null)
                {
                    foreach (var tmp in command.nodeId)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_REPORT
        {
            public const byte ID = 0x03;
            public byte groupingIdentifier;
            public byte maxNodesSupported;
            public byte reportsToFollow;
            public IList<byte> nodeid = new List<byte>();
            public static implicit operator ASSOCIATION_REPORT(byte[] data)
            {
                ASSOCIATION_REPORT ret = new ASSOCIATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.maxNodesSupported = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeid = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.nodeid.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.maxNodesSupported);
                ret.Add(command.reportsToFollow);
                if (command.nodeid != null)
                {
                    foreach (var tmp in command.nodeid)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ASSOCIATION_SET
        {
            public const byte ID = 0x01;
            public byte groupingIdentifier;
            public IList<byte> nodeId = new List<byte>();
            public static implicit operator ASSOCIATION_SET(byte[] data)
            {
                ASSOCIATION_SET ret = new ASSOCIATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.nodeId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ASSOCIATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                if (command.nodeId != null)
                {
                    foreach (var tmp in command.nodeId)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

