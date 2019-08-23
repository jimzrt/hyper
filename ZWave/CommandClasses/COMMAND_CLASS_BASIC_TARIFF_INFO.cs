using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_BASIC_TARIFF_INFO
    {
        public const byte ID = 0x36;
        public const byte VERSION = 1;
        public class BASIC_TARIFF_INFO_GET
        {
            public const byte ID = 0x01;
            public static implicit operator BASIC_TARIFF_INFO_GET(byte[] data)
            {
                BASIC_TARIFF_INFO_GET ret = new BASIC_TARIFF_INFO_GET();
                return ret;
            }
            public static implicit operator byte[](BASIC_TARIFF_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_TARIFF_INFO.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class BASIC_TARIFF_INFO_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte totalNoImportRates
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved1
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte dual
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
                public byte e1CurrentRateInUse
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved2
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public byte[] e1RateConsumptionRegister = new byte[4];
            public byte e1TimeForNextRateHours;
            public byte e1TimeForNextRateMinutes;
            public byte e1TimeForNextRateSeconds;
            public struct Tproperties3
            {
                private byte _value;
                public byte e2CurrentRateInUse
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved3
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public byte[] e2RateConsumptionRegister = new byte[4];
            public static implicit operator BASIC_TARIFF_INFO_REPORT(byte[] data)
            {
                BASIC_TARIFF_INFO_REPORT ret = new BASIC_TARIFF_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.e1RateConsumptionRegister = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.e1TimeForNextRateHours = data.Length > index ? data[index++] : (byte)0x00;
                    ret.e1TimeForNextRateMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.e1TimeForNextRateSeconds = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.e2RateConsumptionRegister = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](BASIC_TARIFF_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BASIC_TARIFF_INFO.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.e1RateConsumptionRegister[0]);
                ret.Add(command.e1RateConsumptionRegister[1]);
                ret.Add(command.e1RateConsumptionRegister[2]);
                ret.Add(command.e1RateConsumptionRegister[3]);
                ret.Add(command.e1TimeForNextRateHours);
                ret.Add(command.e1TimeForNextRateMinutes);
                ret.Add(command.e1TimeForNextRateSeconds);
                ret.Add(command.properties3);
                ret.Add(command.e2RateConsumptionRegister[0]);
                ret.Add(command.e2RateConsumptionRegister[1]);
                ret.Add(command.e2RateConsumptionRegister[2]);
                ret.Add(command.e2RateConsumptionRegister[3]);
                return ret.ToArray();
            }
        }
    }
}

