using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC
    {
        public const byte ID = 0x4D;
        public const byte VERSION = 1;
        public class LEARN_MODE_SET
        {
            public const byte ID = 0x01;
            public byte seqNo;
            public byte reserved;
            public byte mode;
            public static implicit operator LEARN_MODE_SET(byte[] data)
            {
                LEARN_MODE_SET ret = new LEARN_MODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](LEARN_MODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.reserved);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class LEARN_MODE_SET_STATUS
        {
            public const byte ID = 0x02;
            public byte seqNo;
            public byte status;
            public byte reserved;
            public byte newNodeId;
            public static implicit operator LEARN_MODE_SET_STATUS(byte[] data)
            {
                LEARN_MODE_SET_STATUS ret = new LEARN_MODE_SET_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.newNodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](LEARN_MODE_SET_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                ret.Add(command.reserved);
                ret.Add(command.newNodeId);
                return ret.ToArray();
            }
        }
        public class NODE_INFORMATION_SEND
        {
            public const byte ID = 0x05;
            public byte seqNo;
            public byte reserved;
            public byte destinationNodeId;
            public byte txOptions;
            public static implicit operator NODE_INFORMATION_SEND(byte[] data)
            {
                NODE_INFORMATION_SEND ret = new NODE_INFORMATION_SEND();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.destinationNodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.txOptions = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NODE_INFORMATION_SEND command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.reserved);
                ret.Add(command.destinationNodeId);
                ret.Add(command.txOptions);
                return ret.ToArray();
            }
        }
        public class NETWORK_UPDATE_REQUEST
        {
            public const byte ID = 0x03;
            public byte seqNo;
            public static implicit operator NETWORK_UPDATE_REQUEST(byte[] data)
            {
                NETWORK_UPDATE_REQUEST ret = new NETWORK_UPDATE_REQUEST();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NETWORK_UPDATE_REQUEST command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                return ret.ToArray();
            }
        }
        public class NETWORK_UPDATE_REQUEST_STATUS
        {
            public const byte ID = 0x04;
            public byte seqNo;
            public byte status;
            public static implicit operator NETWORK_UPDATE_REQUEST_STATUS(byte[] data)
            {
                NETWORK_UPDATE_REQUEST_STATUS ret = new NETWORK_UPDATE_REQUEST_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NETWORK_UPDATE_REQUEST_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class DEFAULT_SET
        {
            public const byte ID = 0x06;
            public byte seqNo;
            public static implicit operator DEFAULT_SET(byte[] data)
            {
                DEFAULT_SET ret = new DEFAULT_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DEFAULT_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                return ret.ToArray();
            }
        }
        public class DEFAULT_SET_COMPLETE
        {
            public const byte ID = 0x07;
            public byte seqNo;
            public byte status;
            public static implicit operator DEFAULT_SET_COMPLETE(byte[] data)
            {
                DEFAULT_SET_COMPLETE ret = new DEFAULT_SET_COMPLETE();
                if (data != null)
                {
                    int index = 2;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DEFAULT_SET_COMPLETE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
    }
}

