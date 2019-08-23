using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_DMX
    {
        public const byte ID = 0x65;
        public const byte VERSION = 1;
        public class DMX_ADDRESS_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte pageId
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
            public byte channelId;
            public static implicit operator DMX_ADDRESS_SET(byte[] data)
            {
                DMX_ADDRESS_SET ret = new DMX_ADDRESS_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.channelId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DMX_ADDRESS_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DMX.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.channelId);
                return ret.ToArray();
            }
        }
        public class DMX_ADDRESS_GET
        {
            public const byte ID = 0x02;
            public static implicit operator DMX_ADDRESS_GET(byte[] data)
            {
                DMX_ADDRESS_GET ret = new DMX_ADDRESS_GET();
                return ret;
            }
            public static implicit operator byte[](DMX_ADDRESS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DMX.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DMX_ADDRESS_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte pageId
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
            public byte channelId;
            public static implicit operator DMX_ADDRESS_REPORT(byte[] data)
            {
                DMX_ADDRESS_REPORT ret = new DMX_ADDRESS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.channelId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DMX_ADDRESS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DMX.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.channelId);
                return ret.ToArray();
            }
        }
        public class DMX_CAPABILITY_GET
        {
            public const byte ID = 0x04;
            public byte channelId;
            public static implicit operator DMX_CAPABILITY_GET(byte[] data)
            {
                DMX_CAPABILITY_GET ret = new DMX_CAPABILITY_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.channelId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DMX_CAPABILITY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DMX.ID);
                ret.Add(ID);
                ret.Add(command.channelId);
                return ret.ToArray();
            }
        }
        public class DMX_CAPABILITY_REPORT
        {
            public const byte ID = 0x05;
            public byte channelId;
            public byte[] propertyId = new byte[2];
            public byte deviceChannels;
            public byte maxChannels;
            public static implicit operator DMX_CAPABILITY_REPORT(byte[] data)
            {
                DMX_CAPABILITY_REPORT ret = new DMX_CAPABILITY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.channelId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.propertyId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.deviceChannels = data.Length > index ? data[index++] : (byte)0x00;
                    ret.maxChannels = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DMX_CAPABILITY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DMX.ID);
                ret.Add(ID);
                ret.Add(command.channelId);
                ret.Add(command.propertyId[0]);
                ret.Add(command.propertyId[1]);
                ret.Add(command.deviceChannels);
                ret.Add(command.maxChannels);
                return ret.ToArray();
            }
        }
        public class DMX_DATA
        {
            public const byte ID = 0x06;
            public byte source;
            public struct Tproperties1
            {
                private byte _value;
                public byte page
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte sequenceNo
                {
                    get { return (byte)(_value >> 4 & 0x03); }
                    set { _value &= 0xFF - 0x30; _value += (byte)(value << 4 & 0x30); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 6 & 0x03); }
                    set { _value &= 0xFF - 0xC0; _value += (byte)(value << 6 & 0xC0); }
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
            public IList<byte> dmxChannel = new List<byte>();
            public static implicit operator DMX_DATA(byte[] data)
            {
                DMX_DATA ret = new DMX_DATA();
                if (data != null)
                {
                    int index = 2;
                    ret.source = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dmxChannel = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.dmxChannel.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](DMX_DATA command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DMX.ID);
                ret.Add(ID);
                ret.Add(command.source);
                ret.Add(command.properties1);
                if (command.dmxChannel != null)
                {
                    foreach (var tmp in command.dmxChannel)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

