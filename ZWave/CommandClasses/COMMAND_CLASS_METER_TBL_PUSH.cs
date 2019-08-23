using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_METER_TBL_PUSH
    {
        public const byte ID = 0x3E;
        public const byte VERSION = 1;
        public class METER_TBL_PUSH_CONFIGURATION_GET
        {
            public const byte ID = 0x02;
            public static implicit operator METER_TBL_PUSH_CONFIGURATION_GET(byte[] data)
            {
                METER_TBL_PUSH_CONFIGURATION_GET ret = new METER_TBL_PUSH_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](METER_TBL_PUSH_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_PUSH.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_TBL_PUSH_CONFIGURATION_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte operatingStatusPushMode
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte ps
                {
                    get { return (byte)(_value >> 4 & 0x01); }
                    set { _value &= 0xFF - 0x10; _value += (byte)(value << 4 & 0x10); }
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
            public byte[] pushDataset = new byte[3];
            public byte intervalMonths;
            public byte intervalDays;
            public byte intervalHours;
            public byte intervalMinutes;
            public byte pushNodeId;
            public static implicit operator METER_TBL_PUSH_CONFIGURATION_REPORT(byte[] data)
            {
                METER_TBL_PUSH_CONFIGURATION_REPORT ret = new METER_TBL_PUSH_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.pushDataset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.intervalMonths = data.Length > index ? data[index++] : (byte)0x00;
                    ret.intervalDays = data.Length > index ? data[index++] : (byte)0x00;
                    ret.intervalHours = data.Length > index ? data[index++] : (byte)0x00;
                    ret.intervalMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.pushNodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_PUSH_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_PUSH.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.pushDataset[0]);
                ret.Add(command.pushDataset[1]);
                ret.Add(command.pushDataset[2]);
                ret.Add(command.intervalMonths);
                ret.Add(command.intervalDays);
                ret.Add(command.intervalHours);
                ret.Add(command.intervalMinutes);
                ret.Add(command.pushNodeId);
                return ret.ToArray();
            }
        }
        public class METER_TBL_PUSH_CONFIGURATION_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte operatingStatusPushMode
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte ps
                {
                    get { return (byte)(_value >> 4 & 0x01); }
                    set { _value &= 0xFF - 0x10; _value += (byte)(value << 4 & 0x10); }
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
            public byte[] pushDataset = new byte[3];
            public byte intervalMonths;
            public byte intervalDays;
            public byte intervalHours;
            public byte intervalMinutes;
            public byte pushNodeId;
            public static implicit operator METER_TBL_PUSH_CONFIGURATION_SET(byte[] data)
            {
                METER_TBL_PUSH_CONFIGURATION_SET ret = new METER_TBL_PUSH_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.pushDataset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.intervalMonths = data.Length > index ? data[index++] : (byte)0x00;
                    ret.intervalDays = data.Length > index ? data[index++] : (byte)0x00;
                    ret.intervalHours = data.Length > index ? data[index++] : (byte)0x00;
                    ret.intervalMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.pushNodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_PUSH_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_PUSH.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.pushDataset[0]);
                ret.Add(command.pushDataset[1]);
                ret.Add(command.pushDataset[2]);
                ret.Add(command.intervalMonths);
                ret.Add(command.intervalDays);
                ret.Add(command.intervalHours);
                ret.Add(command.intervalMinutes);
                ret.Add(command.pushNodeId);
                return ret.ToArray();
            }
        }
    }
}

