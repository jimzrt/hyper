using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_TARIFF_CONFIG
    {
        public const byte ID = 0x4A;
        public const byte VERSION = 1;
        public class TARIFF_TBL_REMOVE
        {
            public const byte ID = 0x03;
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
            public static implicit operator TARIFF_TBL_REMOVE(byte[] data)
            {
                TARIFF_TBL_REMOVE ret = new TARIFF_TBL_REMOVE();
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
            public static implicit operator byte[](TARIFF_TBL_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_CONFIG.ID);
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
        public class TARIFF_TBL_SET
        {
            public const byte ID = 0x02;
            public byte rateParameterSetId;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte tariffPrecision
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
            public byte[] tariffValue = new byte[4];
            public static implicit operator TARIFF_TBL_SET(byte[] data)
            {
                TARIFF_TBL_SET ret = new TARIFF_TBL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.tariffValue = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](TARIFF_TBL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                ret.Add(command.properties1);
                ret.Add(command.tariffValue[0]);
                ret.Add(command.tariffValue[1]);
                ret.Add(command.tariffValue[2]);
                ret.Add(command.tariffValue[3]);
                return ret.ToArray();
            }
        }
        public class TARIFF_TBL_SUPPLIER_SET
        {
            public const byte ID = 0x01;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourLocalTime;
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public byte[] currency = new byte[3];
            public struct Tproperties1
            {
                private byte _value;
                public byte standingChargePeriod
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte standingChargePrecision
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
            public byte[] standingChargeValue = new byte[4];
            public struct Tproperties2
            {
                private byte _value;
                public byte numberOfSupplierCharacters
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
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
            public IList<byte> supplierCharacter = new List<byte>();
            public static implicit operator TARIFF_TBL_SUPPLIER_SET(byte[] data)
            {
                TARIFF_TBL_SUPPLIER_SET ret = new TARIFF_TBL_SUPPLIER_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.year = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.month = data.Length > index ? data[index++] : (byte)0x00;
                    ret.day = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.secondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.currency = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.standingChargeValue = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supplierCharacter = new List<byte>();
                    for (int i = 0; i < ret.properties2.numberOfSupplierCharacters; i++)
                    {
                        ret.supplierCharacter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](TARIFF_TBL_SUPPLIER_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourLocalTime);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                ret.Add(command.currency[0]);
                ret.Add(command.currency[1]);
                ret.Add(command.currency[2]);
                ret.Add(command.properties1);
                ret.Add(command.standingChargeValue[0]);
                ret.Add(command.standingChargeValue[1]);
                ret.Add(command.standingChargeValue[2]);
                ret.Add(command.standingChargeValue[3]);
                ret.Add(command.properties2);
                if (command.supplierCharacter != null)
                {
                    foreach (var tmp in command.supplierCharacter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

