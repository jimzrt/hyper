using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2
    {
        public const byte ID = 0x72;
        public const byte VERSION = 2;
        public class MANUFACTURER_SPECIFIC_GET
        {
            public const byte ID = 0x04;
            public static implicit operator MANUFACTURER_SPECIFIC_GET(byte[] data)
            {
                MANUFACTURER_SPECIFIC_GET ret = new MANUFACTURER_SPECIFIC_GET();
                return ret;
            }
            public static implicit operator byte[](MANUFACTURER_SPECIFIC_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MANUFACTURER_SPECIFIC_REPORT
        {
            public const byte ID = 0x05;
            public byte[] manufacturerId = new byte[2];
            public byte[] productTypeId = new byte[2];
            public byte[] productId = new byte[2];
            public static implicit operator MANUFACTURER_SPECIFIC_REPORT(byte[] data)
            {
                MANUFACTURER_SPECIFIC_REPORT ret = new MANUFACTURER_SPECIFIC_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.manufacturerId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.productTypeId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.productId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](MANUFACTURER_SPECIFIC_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2.ID);
                ret.Add(ID);
                ret.Add(command.manufacturerId[0]);
                ret.Add(command.manufacturerId[1]);
                ret.Add(command.productTypeId[0]);
                ret.Add(command.productTypeId[1]);
                ret.Add(command.productId[0]);
                ret.Add(command.productId[1]);
                return ret.ToArray();
            }
        }
        public class DEVICE_SPECIFIC_GET
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte deviceIdType
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
            public static implicit operator DEVICE_SPECIFIC_GET(byte[] data)
            {
                DEVICE_SPECIFIC_GET ret = new DEVICE_SPECIFIC_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DEVICE_SPECIFIC_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class DEVICE_SPECIFIC_REPORT
        {
            public const byte ID = 0x07;
            public struct Tproperties1
            {
                private byte _value;
                public byte deviceIdType
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
            public struct Tproperties2
            {
                private byte _value;
                public byte deviceIdDataLengthIndicator
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte deviceIdDataFormat
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public IList<byte> deviceIdData = new List<byte>();
            public static implicit operator DEVICE_SPECIFIC_REPORT(byte[] data)
            {
                DEVICE_SPECIFIC_REPORT ret = new DEVICE_SPECIFIC_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.deviceIdData = new List<byte>();
                    for (int i = 0; i < ret.properties2.deviceIdDataLengthIndicator; i++)
                    {
                        ret.deviceIdData.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](DEVICE_SPECIFIC_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                if (command.deviceIdData != null)
                {
                    foreach (var tmp in command.deviceIdData)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

