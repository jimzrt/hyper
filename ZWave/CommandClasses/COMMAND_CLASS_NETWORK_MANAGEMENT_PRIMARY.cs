using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_NETWORK_MANAGEMENT_PRIMARY
    {
        public const byte ID = 0x54;
        public const byte VERSION = 1;
        public class CONTROLLER_CHANGE
        {
            public const byte ID = 0x01;
            public byte seqNo;
            public byte reserved;
            public byte mode;
            public byte txOptions;
            public static implicit operator CONTROLLER_CHANGE(byte[] data)
            {
                CONTROLLER_CHANGE ret = new CONTROLLER_CHANGE();
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
            public static implicit operator byte[](CONTROLLER_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_PRIMARY.ID);
                ret.Add(ID);
                ret.Add(command.seqNo);
                ret.Add(command.reserved);
                ret.Add(command.mode);
                ret.Add(command.txOptions);
                return ret.ToArray();
            }
        }
        public class CONTROLLER_CHANGE_STATUS
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
            public static implicit operator CONTROLLER_CHANGE_STATUS(byte[] data)
            {
                CONTROLLER_CHANGE_STATUS ret = new CONTROLLER_CHANGE_STATUS();
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
                    while (data.Length - 0 > index)
                    {
                        ret.commandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CONTROLLER_CHANGE_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_PRIMARY.ID);
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
    }
}

