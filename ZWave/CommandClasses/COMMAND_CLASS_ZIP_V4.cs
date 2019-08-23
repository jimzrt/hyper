using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZIP_V4
    {
        public const byte ID = 0x23;
        public const byte VERSION = 4;
        public class COMMAND_ZIP_PACKET
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved1
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte nackOptionError
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte nackQueueFull
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte nackWaiting
                {
                    get { return (byte)(_value >> 4 & 0x01); }
                    set { _value &= 0xFF - 0x10; _value += (byte)(value << 4 & 0x10); }
                }
                public byte nackResponse
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte ackResponse
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte ackRequest
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
                public byte reserved2
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte secureOrigin
                {
                    get { return (byte)(_value >> 4 & 0x01); }
                    set { _value &= 0xFF - 0x10; _value += (byte)(value << 4 & 0x10); }
                }
                public byte moreInformation
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte zWaveCmdIncluded
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte headerExtIncluded
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
            public byte seqNo;
            public struct Tproperties3
            {
                private byte _value;
                public byte sourceEndPoint
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte reserved3
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
            public struct Tproperties4
            {
                private byte _value;
                public byte destinationEndPoint
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte bitAddress
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties4(byte data)
                {
                    Tproperties4 ret = new Tproperties4();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties4 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties4 properties4;
            public byte headerLength;
            public IList<byte> headerExtension = new List<byte>();
            public IList<byte> zWaveCommand = new List<byte>();
            public static implicit operator COMMAND_ZIP_PACKET(byte[] data)
            {
                COMMAND_ZIP_PACKET ret = new COMMAND_ZIP_PACKET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.seqNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties4 = data.Length > index ? data[index++] : (byte)0x00;
                    if (ret.properties2.headerExtIncluded > 0)
                    {
                        ret.headerLength = data.Length > index ? data[index++] : (byte)0x00;
                    }
                    if (ret.properties2.headerExtIncluded > 0)
                    {
                        ret.headerExtension = new List<byte>();
                        for (int i = 0; i < ret.headerLength - 1; i++)
                        {
                            ret.headerExtension.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                    }
                    if (ret.properties2.zWaveCmdIncluded > 0)
                    {
                        ret.zWaveCommand = new List<byte>();
                        while (data.Length - 0 > index)
                        {
                            ret.zWaveCommand.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_ZIP_PACKET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.seqNo);
                ret.Add(command.properties3);
                ret.Add(command.properties4);
                if (command.properties2.headerExtIncluded > 0)
                {
                    ret.Add(command.headerLength);
                }
                if (command.properties2.headerExtIncluded > 0)
                {
                    if (command.headerExtension != null)
                    {
                        foreach (var tmp in command.headerExtension)
                        {
                            ret.Add(tmp);
                        }
                    }
                }
                if (command.properties2.zWaveCmdIncluded > 0)
                {
                    if (command.zWaveCommand != null)
                    {
                        foreach (var tmp in command.zWaveCommand)
                        {
                            ret.Add(tmp);
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class COMMAND_ZIP_KEEP_ALIVE
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
                public byte ackResponce
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte ackRequest
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
            public static implicit operator COMMAND_ZIP_KEEP_ALIVE(byte[] data)
            {
                COMMAND_ZIP_KEEP_ALIVE ret = new COMMAND_ZIP_KEEP_ALIVE();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_ZIP_KEEP_ALIVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
    }
}

