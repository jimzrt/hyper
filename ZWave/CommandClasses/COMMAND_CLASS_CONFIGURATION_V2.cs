using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CONFIGURATION_V2
    {
        public const byte ID = 0x70;
        public const byte VERSION = 2;
        public class CONFIGURATION_BULK_GET
        {
            public const byte ID = 0x08;
            public byte[] parameterOffset = new byte[2];
            public byte numberOfParameters;
            public static implicit operator CONFIGURATION_BULK_GET(byte[] data)
            {
                CONFIGURATION_BULK_GET ret = new CONFIGURATION_BULK_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterOffset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.numberOfParameters = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CONFIGURATION_BULK_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONFIGURATION_V2.ID);
                ret.Add(ID);
                ret.Add(command.parameterOffset[0]);
                ret.Add(command.parameterOffset[1]);
                ret.Add(command.numberOfParameters);
                return ret.ToArray();
            }
        }
        public class CONFIGURATION_BULK_REPORT
        {
            public const byte ID = 0x09;
            public byte[] parameterOffset = new byte[2];
            public byte numberOfParameters;
            public byte reportsToFollow;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x07); }
                    set { _value &= 0xFF - 0x38; _value += (byte)(value << 3 & 0x38); }
                }
                public byte handshake
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte mdefault
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
            public class TVG
            {
                public IList<byte> parameter = new List<byte>();
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator CONFIGURATION_BULK_REPORT(byte[] data)
            {
                CONFIGURATION_BULK_REPORT ret = new CONFIGURATION_BULK_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterOffset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.numberOfParameters = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg = new List<TVG>();
                    for (int j = 0; j < ret.numberOfParameters; j++)
                    {
                        TVG tmp = new TVG();
                        tmp.parameter = new List<byte>();
                        for (int i = 0; i < ret.properties1.size; i++)
                        {
                            tmp.parameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CONFIGURATION_BULK_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONFIGURATION_V2.ID);
                ret.Add(ID);
                ret.Add(command.parameterOffset[0]);
                ret.Add(command.parameterOffset[1]);
                ret.Add(command.numberOfParameters);
                ret.Add(command.reportsToFollow);
                ret.Add(command.properties1);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        if (item.parameter != null)
                        {
                            foreach (var tmp in item.parameter)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class CONFIGURATION_BULK_SET
        {
            public const byte ID = 0x07;
            public byte[] parameterOffset = new byte[2];
            public byte numberOfParameters;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x07); }
                    set { _value &= 0xFF - 0x38; _value += (byte)(value << 3 & 0x38); }
                }
                public byte handshake
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte mdefault
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
            public class TVG
            {
                public IList<byte> parameter = new List<byte>();
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator CONFIGURATION_BULK_SET(byte[] data)
            {
                CONFIGURATION_BULK_SET ret = new CONFIGURATION_BULK_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterOffset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.numberOfParameters = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg = new List<TVG>();
                    for (int j = 0; j < ret.numberOfParameters; j++)
                    {
                        TVG tmp = new TVG();
                        tmp.parameter = new List<byte>();
                        for (int i = 0; i < ret.properties1.size; i++)
                        {
                            tmp.parameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CONFIGURATION_BULK_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONFIGURATION_V2.ID);
                ret.Add(ID);
                ret.Add(command.parameterOffset[0]);
                ret.Add(command.parameterOffset[1]);
                ret.Add(command.numberOfParameters);
                ret.Add(command.properties1);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        if (item.parameter != null)
                        {
                            foreach (var tmp in item.parameter)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class CONFIGURATION_GET
        {
            public const byte ID = 0x05;
            public byte parameterNumber;
            public static implicit operator CONFIGURATION_GET(byte[] data)
            {
                CONFIGURATION_GET ret = new CONFIGURATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONFIGURATION_V2.ID);
                ret.Add(ID);
                ret.Add(command.parameterNumber);
                return ret.ToArray();
            }
        }
        public class CONFIGURATION_REPORT
        {
            public const byte ID = 0x06;
            public byte parameterNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public IList<byte> configurationValue = new List<byte>();
            public static implicit operator CONFIGURATION_REPORT(byte[] data)
            {
                CONFIGURATION_REPORT ret = new CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.configurationValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.configurationValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONFIGURATION_V2.ID);
                ret.Add(ID);
                ret.Add(command.parameterNumber);
                ret.Add(command.properties1);
                if (command.configurationValue != null)
                {
                    foreach (var tmp in command.configurationValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CONFIGURATION_SET
        {
            public const byte ID = 0x04;
            public byte parameterNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x0F); }
                    set { _value &= 0xFF - 0x78; _value += (byte)(value << 3 & 0x78); }
                }
                public byte mdefault
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
            public IList<byte> configurationValue = new List<byte>();
            public static implicit operator CONFIGURATION_SET(byte[] data)
            {
                CONFIGURATION_SET ret = new CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.configurationValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.configurationValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONFIGURATION_V2.ID);
                ret.Add(ID);
                ret.Add(command.parameterNumber);
                ret.Add(command.properties1);
                if (command.configurationValue != null)
                {
                    foreach (var tmp in command.configurationValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

