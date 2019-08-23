using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_METER_V5
    {
        public const byte ID = 0x32;
        public const byte VERSION = 5;
        public class METER_GET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x07); }
                    set { _value &= 0xFF - 0x38; _value += (byte)(value << 3 & 0x38); }
                }
                public byte rateType
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
            public byte scale2;
            public static implicit operator METER_GET(byte[] data)
            {
                METER_GET ret = new METER_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scale2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_V5.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.scale2);
                return ret.ToArray();
            }
        }
        public class METER_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte meterType
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte rateType
                {
                    get { return (byte)(_value >> 5 & 0x03); }
                    set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                }
                public byte scaleBit2
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
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scaleBits10
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> meterValue = new List<byte>();
            public byte[] deltaTime = new byte[2];
            public IList<byte> previousMeterValue = new List<byte>();
            public byte scale2;
            public static implicit operator METER_REPORT(byte[] data)
            {
                METER_REPORT ret = new METER_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.meterValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.size; i++)
                    {
                        ret.meterValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.deltaTime = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    if (ret.deltaTime[0] > 0 || ret.deltaTime[1] > 0)
                    {
                        ret.previousMeterValue = new List<byte>();
                        for (int i = 0; i < ret.properties2.size; i++)
                        {
                            ret.previousMeterValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                    }
                    ret.scale2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_V5.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                if (command.meterValue != null)
                {
                    foreach (var tmp in command.meterValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.deltaTime[0]);
                ret.Add(command.deltaTime[1]);
                if (command.deltaTime[0] > 0 || command.deltaTime[1] > 0)
                {
                    if (command.previousMeterValue != null)
                    {
                        foreach (var tmp in command.previousMeterValue)
                        {
                            ret.Add(tmp);
                        }
                    }
                }
                ret.Add(command.scale2);
                return ret.ToArray();
            }
        }
        public class METER_RESET
        {
            public const byte ID = 0x05;
            public static implicit operator METER_RESET(byte[] data)
            {
                METER_RESET ret = new METER_RESET();
                return ret;
            }
            public static implicit operator byte[](METER_RESET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_V5.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_SUPPORTED_GET
        {
            public const byte ID = 0x03;
            public static implicit operator METER_SUPPORTED_GET(byte[] data)
            {
                METER_SUPPORTED_GET ret = new METER_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](METER_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_V5.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_SUPPORTED_REPORT
        {
            public const byte ID = 0x04;
            public struct Tproperties1
            {
                private byte _value;
                public byte meterType
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte rateType
                {
                    get { return (byte)(_value >> 5 & 0x03); }
                    set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                }
                public byte meterReset
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
                public byte scaleSupported0
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte mST
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
            public byte numberOfScaleSupportedBytesToFollow;
            public IList<byte> scaleSupported = new List<byte>();
            public static implicit operator METER_SUPPORTED_REPORT(byte[] data)
            {
                METER_SUPPORTED_REPORT ret = new METER_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfScaleSupportedBytesToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scaleSupported = new List<byte>();
                    for (int i = 0; i < ret.numberOfScaleSupportedBytesToFollow; i++)
                    {
                        ret.scaleSupported.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_V5.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.numberOfScaleSupportedBytesToFollow);
                if (command.scaleSupported != null)
                {
                    foreach (var tmp in command.scaleSupported)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

