using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION
    {
        public const byte ID = 0x34;
        public const byte VERSION = 1;
        public class FAILED_NODE_REMOVE
        {
            public const byte ID = 0x07;
            public byte seqNo;
            public byte nodeId;
            public static implicit operator FAILED_NODE_REMOVE(byte[] data)
            {
                FAILED_NODE_REMOVE ret = new FAILED_NODE_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FAILED_NODE_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class FAILED_NODE_REMOVE_STATUS
        {
            public const byte ID = 0x08;
            public byte seqNo;
            public byte status;
            public byte nodeId;
            public static implicit operator FAILED_NODE_REMOVE_STATUS(byte[] data)
            {
                FAILED_NODE_REMOVE_STATUS ret = new FAILED_NODE_REMOVE_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FAILED_NODE_REMOVE_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class NODE_ADD
        {
            public const byte ID = 0x01;
            public byte seqNo;
            public byte reserved;
            public byte mode;
            public byte txOptions;
            public static implicit operator NODE_ADD(byte[] data)
            {
                NODE_ADD ret = new NODE_ADD();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.txOptions = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_ADD command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.reserved);
                ret.Add(command.mode);
                ret.Add(command.txOptions);
                return ret.ToArray();
            }
        }
        public class NODE_ADD_STATUS
        {
            public const byte ID = 0x02;
            public byte seqNo;
            public byte status;
            public byte reserved;
            public byte newNodeId;
            public byte nodeInfoLength;
            public struct Tproperties1
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
            public byte basicDeviceClass;
            public byte genericDeviceClass;
            public byte specificDeviceClass;
            public IList<byte> commandClass = new List<byte>();
            public static implicit operator NODE_ADD_STATUS(byte[] data)
            {
                NODE_ADD_STATUS ret = new NODE_ADD_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.newNodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeInfoLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.basicDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.genericDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.specificDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClass = new List<byte>();
                    for (int i = 0; i < ret.nodeInfoLength - 6; i++)
                    {
                        ret.commandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](NODE_ADD_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                ret.Add(command.reserved);
                ret.Add(command.newNodeId);
                ret.Add(command.nodeInfoLength);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
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
        public class NODE_REMOVE
        {
            public const byte ID = 0x03;
            public byte seqNo;
            public byte reserved;
            public byte mode;
            public static implicit operator NODE_REMOVE(byte[] data)
            {
                NODE_REMOVE ret = new NODE_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.reserved);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class NODE_REMOVE_STATUS
        {
            public const byte ID = 0x04;
            public byte seqNo;
            public byte status;
            public byte nodeid;
            public static implicit operator NODE_REMOVE_STATUS(byte[] data)
            {
                NODE_REMOVE_STATUS ret = new NODE_REMOVE_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_REMOVE_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                ret.Add(command.nodeid);
                return ret.ToArray();
            }
        }
        public class FAILED_NODE_REPLACE
        {
            public const byte ID = 0x09;
            public byte seqNo;
            public byte nodeId;
            public byte txOptions;
            public byte mode;
            public static implicit operator FAILED_NODE_REPLACE(byte[] data)
            {
                FAILED_NODE_REPLACE ret = new FAILED_NODE_REPLACE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.txOptions = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FAILED_NODE_REPLACE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.nodeId);
                ret.Add(command.txOptions);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class FAILED_NODE_REPLACE_STATUS
        {
            public const byte ID = 0x0A;
            public byte seqNo;
            public byte status;
            public byte nodeId;
            public static implicit operator FAILED_NODE_REPLACE_STATUS(byte[] data)
            {
                FAILED_NODE_REPLACE_STATUS ret = new FAILED_NODE_REPLACE_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FAILED_NODE_REPLACE_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class NODE_NEIGHBOR_UPDATE_REQUEST
        {
            public const byte ID = 0x0B;
            public byte seqNo;
            public byte nodeId;
            public static implicit operator NODE_NEIGHBOR_UPDATE_REQUEST(byte[] data)
            {
                NODE_NEIGHBOR_UPDATE_REQUEST ret = new NODE_NEIGHBOR_UPDATE_REQUEST();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_NEIGHBOR_UPDATE_REQUEST command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class NODE_NEIGHBOR_UPDATE_STATUS
        {
            public const byte ID = 0x0C;
            public byte seqNo;
            public byte status;
            public static implicit operator NODE_NEIGHBOR_UPDATE_STATUS(byte[] data)
            {
                NODE_NEIGHBOR_UPDATE_STATUS ret = new NODE_NEIGHBOR_UPDATE_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_NEIGHBOR_UPDATE_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class RETURN_ROUTE_ASSIGN
        {
            public const byte ID = 0x0D;
            public byte seqNo;
            public byte sourceNodeId;
            public byte destinationNodeId;
            public static implicit operator RETURN_ROUTE_ASSIGN(byte[] data)
            {
                RETURN_ROUTE_ASSIGN ret = new RETURN_ROUTE_ASSIGN();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sourceNodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.destinationNodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RETURN_ROUTE_ASSIGN command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.sourceNodeId);
                ret.Add(command.destinationNodeId);
                return ret.ToArray();
            }
        }
        public class RETURN_ROUTE_ASSIGN_COMPLETE
        {
            public const byte ID = 0x0E;
            public byte seqNo;
            public byte status;
            public static implicit operator RETURN_ROUTE_ASSIGN_COMPLETE(byte[] data)
            {
                RETURN_ROUTE_ASSIGN_COMPLETE ret = new RETURN_ROUTE_ASSIGN_COMPLETE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RETURN_ROUTE_ASSIGN_COMPLETE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class RETURN_ROUTE_DELETE
        {
            public const byte ID = 0x0F;
            public byte seqNo;
            public byte nodeId;
            public static implicit operator RETURN_ROUTE_DELETE(byte[] data)
            {
                RETURN_ROUTE_DELETE ret = new RETURN_ROUTE_DELETE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RETURN_ROUTE_DELETE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class RETURN_ROUTE_DELETE_COMPLETE
        {
            public const byte ID = 0x10;
            public byte seqNo;
            public byte status;
            public static implicit operator RETURN_ROUTE_DELETE_COMPLETE(byte[] data)
            {
                RETURN_ROUTE_DELETE_COMPLETE ret = new RETURN_ROUTE_DELETE_COMPLETE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RETURN_ROUTE_DELETE_COMPLETE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
    }
}

