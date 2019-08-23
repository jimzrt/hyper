using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2
    {
        public const byte ID = 0x42;
        public const byte VERSION = 2;
        public class THERMOSTAT_OPERATING_STATE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator THERMOSTAT_OPERATING_STATE_GET(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_GET ret = new THERMOSTAT_OPERATING_STATE_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_OPERATING_STATE_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte operatingState
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
            public static implicit operator THERMOSTAT_OPERATING_STATE_REPORT(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_REPORT ret = new THERMOSTAT_OPERATING_STATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_OPERATING_STATE_LOGGING_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator THERMOSTAT_OPERATING_STATE_LOGGING_SUPPORTED_GET(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_LOGGING_SUPPORTED_GET ret = new THERMOSTAT_OPERATING_STATE_LOGGING_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_LOGGING_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_OPERATING_LOGGING_SUPPORTED_REPORT
        {
            public const byte ID = 0x04;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator THERMOSTAT_OPERATING_LOGGING_SUPPORTED_REPORT(byte[] data)
            {
                THERMOSTAT_OPERATING_LOGGING_SUPPORTED_REPORT ret = new THERMOSTAT_OPERATING_LOGGING_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.bitMask = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_LOGGING_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2.ID);
                ret.Add(ID);
                if (command.bitMask != null)
                {
                    foreach (var tmp in command.bitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_OPERATING_STATE_LOGGING_GET
        {
            public const byte ID = 0x05;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator THERMOSTAT_OPERATING_STATE_LOGGING_GET(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_LOGGING_GET ret = new THERMOSTAT_OPERATING_STATE_LOGGING_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.bitMask = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_LOGGING_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2.ID);
                ret.Add(ID);
                if (command.bitMask != null)
                {
                    foreach (var tmp in command.bitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_OPERATING_STATE_LOGGING_REPORT
        {
            public const byte ID = 0x06;
            public byte reportsToFollow;
            public class TVG1
            {
                public struct Tproperties1
                {
                    private byte _value;
                    public byte operatingStateLogType
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
                public byte usageTodayHours;
                public byte usageTodayMinutes;
                public byte usageYesterdayHours;
                public byte usageYesterdayMinutes;
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator THERMOSTAT_OPERATING_STATE_LOGGING_REPORT(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_LOGGING_REPORT ret = new THERMOSTAT_OPERATING_STATE_LOGGING_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.reportsToFollow; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.usageTodayHours = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.usageTodayMinutes = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.usageYesterdayHours = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.usageYesterdayMinutes = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_LOGGING_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE_V2.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.properties1);
                        ret.Add(item.usageTodayHours);
                        ret.Add(item.usageTodayMinutes);
                        ret.Add(item.usageYesterdayHours);
                        ret.Add(item.usageYesterdayMinutes);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

