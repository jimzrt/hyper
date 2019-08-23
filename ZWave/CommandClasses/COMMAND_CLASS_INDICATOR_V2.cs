using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_INDICATOR_V2
    {
        public const byte ID = 0x87;
        public const byte VERSION = 2;
        public class INDICATOR_GET
        {
            public const byte ID = 0x02;
            public byte indicatorId;
            public static implicit operator INDICATOR_GET(byte[] data)
            {
                INDICATOR_GET ret = new INDICATOR_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.indicatorId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](INDICATOR_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INDICATOR_V2.ID);
                ret.Add(ID);
                ret.Add(command.indicatorId);
                return ret.ToArray();
            }
        }
        public class INDICATOR_REPORT
        {
            public const byte ID = 0x03;
            public byte indicator0Value;
            public struct Tproperties1
            {
                private byte _value;
                public byte indicatorObjectCount
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public class TVG1
            {
                public byte indicatorId;
                public byte propertyId;
                public byte value;
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator INDICATOR_REPORT(byte[] data)
            {
                INDICATOR_REPORT ret = new INDICATOR_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.indicator0Value = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.properties1.indicatorObjectCount; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.indicatorId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.propertyId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.value = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](INDICATOR_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INDICATOR_V2.ID);
                ret.Add(ID);
                ret.Add(command.indicator0Value);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.indicatorId);
                        ret.Add(item.propertyId);
                        ret.Add(item.value);
                    }
                }
                return ret.ToArray();
            }
        }
        public class INDICATOR_SET
        {
            public const byte ID = 0x01;
            public byte indicator0Value;
            public struct Tproperties1
            {
                private byte _value;
                public byte indicatorObjectCount
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public class TVG1
            {
                public byte indicatorId;
                public byte propertyId;
                public byte value;
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator INDICATOR_SET(byte[] data)
            {
                INDICATOR_SET ret = new INDICATOR_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.indicator0Value = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.properties1.indicatorObjectCount; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.indicatorId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.propertyId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.value = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](INDICATOR_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INDICATOR_V2.ID);
                ret.Add(ID);
                ret.Add(command.indicator0Value);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.indicatorId);
                        ret.Add(item.propertyId);
                        ret.Add(item.value);
                    }
                }
                return ret.ToArray();
            }
        }
        public class INDICATOR_SUPPORTED_GET
        {
            public const byte ID = 0x04;
            public byte indicatorId;
            public static implicit operator INDICATOR_SUPPORTED_GET(byte[] data)
            {
                INDICATOR_SUPPORTED_GET ret = new INDICATOR_SUPPORTED_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.indicatorId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](INDICATOR_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INDICATOR_V2.ID);
                ret.Add(ID);
                ret.Add(command.indicatorId);
                return ret.ToArray();
            }
        }
        public class INDICATOR_SUPPORTED_REPORT
        {
            public const byte ID = 0x05;
            public byte indicatorId;
            public byte nextIndicatorId;
            public struct Tproperties1
            {
                private byte _value;
                public byte propertySupportedBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public IList<byte> propertySupportedBitMask = new List<byte>();
            public static implicit operator INDICATOR_SUPPORTED_REPORT(byte[] data)
            {
                INDICATOR_SUPPORTED_REPORT ret = new INDICATOR_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.indicatorId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nextIndicatorId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.propertySupportedBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.propertySupportedBitMaskLength; i++)
                    {
                        ret.propertySupportedBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](INDICATOR_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INDICATOR_V2.ID);
                ret.Add(ID);
                ret.Add(command.indicatorId);
                ret.Add(command.nextIndicatorId);
                ret.Add(command.properties1);
                if (command.propertySupportedBitMask != null)
                {
                    foreach (var tmp in command.propertySupportedBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

