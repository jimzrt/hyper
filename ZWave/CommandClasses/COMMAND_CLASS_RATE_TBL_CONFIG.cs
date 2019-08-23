using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_RATE_TBL_CONFIG
    {
        public const byte ID = 0x48;
        public const byte VERSION = 1;
        public class RATE_TBL_REMOVE
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte rateParameterSetIds
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
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
            public IList<byte> rateParameterSetId = new List<byte>();
            public static implicit operator RATE_TBL_REMOVE(byte[] data)
            {
                RATE_TBL_REMOVE ret = new RATE_TBL_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.rateParameterSetId = new List<byte>();
                    for (int i = 0; i < ret.properties1.rateParameterSetIds; i++)
                    {
                        ret.rateParameterSetId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.rateParameterSetId != null)
                {
                    foreach (var tmp in command.rateParameterSetId)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class RATE_TBL_SET
        {
            public const byte ID = 0x01;
            public byte rateParameterSetId;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfRateChar
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte rateType
                {
                    get { return (byte)(_value >> 5 & 0x03); }
                    set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                }
                public byte reserved
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
            public IList<byte> rateCharacter = new List<byte>();
            public byte startHourLocalTime;
            public byte startMinuteLocalTime;
            public byte[] durationMinute = new byte[2];
            public struct Tproperties2
            {
                private byte _value;
                public byte consumptionScale
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte consumptionPrecision
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
            public byte[] minConsumptionValue = new byte[4];
            public byte[] maxConsumptionValue = new byte[4];
            public struct Tproperties3
            {
                private byte _value;
                public byte maxDemandScale
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte maxDemandPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public byte[] maxDemandValue = new byte[4];
            public byte dcpRateId;
            public static implicit operator RATE_TBL_SET(byte[] data)
            {
                RATE_TBL_SET ret = new RATE_TBL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.rateCharacter = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfRateChar; i++)
                    {
                        ret.rateCharacter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.startHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationMinute = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minConsumptionValue = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.maxConsumptionValue = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.maxDemandValue = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.dcpRateId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                ret.Add(command.properties1);
                if (command.rateCharacter != null)
                {
                    foreach (var tmp in command.rateCharacter)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.startHourLocalTime);
                ret.Add(command.startMinuteLocalTime);
                ret.Add(command.durationMinute[0]);
                ret.Add(command.durationMinute[1]);
                ret.Add(command.properties2);
                ret.Add(command.minConsumptionValue[0]);
                ret.Add(command.minConsumptionValue[1]);
                ret.Add(command.minConsumptionValue[2]);
                ret.Add(command.minConsumptionValue[3]);
                ret.Add(command.maxConsumptionValue[0]);
                ret.Add(command.maxConsumptionValue[1]);
                ret.Add(command.maxConsumptionValue[2]);
                ret.Add(command.maxConsumptionValue[3]);
                ret.Add(command.properties3);
                ret.Add(command.maxDemandValue[0]);
                ret.Add(command.maxDemandValue[1]);
                ret.Add(command.maxDemandValue[2]);
                ret.Add(command.maxDemandValue[3]);
                ret.Add(command.dcpRateId);
                return ret.ToArray();
            }
        }
    }
}

