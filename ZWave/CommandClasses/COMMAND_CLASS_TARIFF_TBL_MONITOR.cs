using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_TARIFF_TBL_MONITOR
    {
        public const byte ID = 0x4B;
        public const byte VERSION = 1;
        public class TARIFF_TBL_COST_GET
        {
            public const byte ID = 0x05;
            public byte rateParameterSetId;
            public byte[] startYear = new byte[2];
            public byte startMonth;
            public byte startDay;
            public byte startHourLocalTime;
            public byte startMinuteLocalTime;
            public byte[] stopYear = new byte[2];
            public byte stopMonth;
            public byte stopDay;
            public byte stopHourLocalTime;
            public byte stopMinuteLocalTime;
            public static implicit operator TARIFF_TBL_COST_GET(byte[] data)
            {
                TARIFF_TBL_COST_GET ret = new TARIFF_TBL_COST_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.startMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.stopMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TARIFF_TBL_COST_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                ret.Add(command.startYear[0]);
                ret.Add(command.startYear[1]);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHourLocalTime);
                ret.Add(command.startMinuteLocalTime);
                ret.Add(command.stopYear[0]);
                ret.Add(command.stopYear[1]);
                ret.Add(command.stopMonth);
                ret.Add(command.stopDay);
                ret.Add(command.stopHourLocalTime);
                ret.Add(command.stopMinuteLocalTime);
                return ret.ToArray();
            }
        }
        public class TARIFF_TBL_COST_REPORT
        {
            public const byte ID = 0x06;
            public byte rateParameterSetId;
            public struct Tproperties1
            {
                private byte _value;
                public byte rateType
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte reserved1
                {
                    get { return (byte)(_value >> 2 & 0x3F); }
                    set { _value &= 0xFF - 0xFC; _value += (byte)(value << 2 & 0xFC); }
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
            public byte[] startYear = new byte[2];
            public byte startMonth;
            public byte startDay;
            public byte startHourLocalTime;
            public byte startMinuteLocalTime;
            public byte[] stopYear = new byte[2];
            public byte stopMonth;
            public byte stopDay;
            public byte stopHourLocalTime;
            public byte stopMinuteLocalTime;
            public byte[] currency = new byte[3];
            public struct Tproperties2
            {
                private byte _value;
                public byte reserved2
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte costPrecision
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
            public byte[] costValue = new byte[4];
            public static implicit operator TARIFF_TBL_COST_REPORT(byte[] data)
            {
                TARIFF_TBL_COST_REPORT ret = new TARIFF_TBL_COST_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.startMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.stopMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.currency = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.costValue = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](TARIFF_TBL_COST_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                ret.Add(command.properties1);
                ret.Add(command.startYear[0]);
                ret.Add(command.startYear[1]);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHourLocalTime);
                ret.Add(command.startMinuteLocalTime);
                ret.Add(command.stopYear[0]);
                ret.Add(command.stopYear[1]);
                ret.Add(command.stopMonth);
                ret.Add(command.stopDay);
                ret.Add(command.stopHourLocalTime);
                ret.Add(command.stopMinuteLocalTime);
                ret.Add(command.currency[0]);
                ret.Add(command.currency[1]);
                ret.Add(command.currency[2]);
                ret.Add(command.properties2);
                ret.Add(command.costValue[0]);
                ret.Add(command.costValue[1]);
                ret.Add(command.costValue[2]);
                ret.Add(command.costValue[3]);
                return ret.ToArray();
            }
        }
        public class TARIFF_TBL_GET
        {
            public const byte ID = 0x03;
            public byte rateParameterSetId;
            public static implicit operator TARIFF_TBL_GET(byte[] data)
            {
                TARIFF_TBL_GET ret = new TARIFF_TBL_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TARIFF_TBL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                return ret.ToArray();
            }
        }
        public class TARIFF_TBL_REPORT
        {
            public const byte ID = 0x04;
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
            public static implicit operator TARIFF_TBL_REPORT(byte[] data)
            {
                TARIFF_TBL_REPORT ret = new TARIFF_TBL_REPORT();
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
            public static implicit operator byte[](TARIFF_TBL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_TBL_MONITOR.ID);
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
        public class TARIFF_TBL_SUPPLIER_GET
        {
            public const byte ID = 0x01;
            public static implicit operator TARIFF_TBL_SUPPLIER_GET(byte[] data)
            {
                TARIFF_TBL_SUPPLIER_GET ret = new TARIFF_TBL_SUPPLIER_GET();
                return ret;
            }
            public static implicit operator byte[](TARIFF_TBL_SUPPLIER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class TARIFF_TBL_SUPPLIER_REPORT
        {
            public const byte ID = 0x02;
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
            public static implicit operator TARIFF_TBL_SUPPLIER_REPORT(byte[] data)
            {
                TARIFF_TBL_SUPPLIER_REPORT ret = new TARIFF_TBL_SUPPLIER_REPORT();
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
            public static implicit operator byte[](TARIFF_TBL_SUPPLIER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TARIFF_TBL_MONITOR.ID);
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

