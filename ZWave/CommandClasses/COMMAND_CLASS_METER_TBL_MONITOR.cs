using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_METER_TBL_MONITOR
    {
        public const byte ID = 0x3D;
        public const byte VERSION = 1;
        public class METER_TBL_STATUS_REPORT
        {
            public const byte ID = 0x0B;
            public byte reportsToFollow;
            public byte[] currentOperatingStatus = new byte[3];
            public class TVG
            {
                public struct Tproperties1
                {
                    private byte _value;
                    public byte operatingStatusEventId
                    {
                        get { return (byte)(_value >> 0 & 0x1F); }
                        set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                    }
                    public byte reserved
                    {
                        get { return (byte)(_value >> 5 & 0x03); }
                        set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                    }
                    public byte type
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
                public byte[] year = new byte[2];
                public byte month;
                public byte day;
                public byte hourLocalTime;
                public byte minuteLocalTime;
                public byte secondLocalTime;
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator METER_TBL_STATUS_REPORT(byte[] data)
            {
                METER_TBL_STATUS_REPORT ret = new METER_TBL_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.currentOperatingStatus = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.vg = new List<TVG>();
                    for (int j = 0; j < ret.reportsToFollow; j++)
                    {
                        TVG tmp = new TVG();
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.year = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        tmp.month = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.day = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.hourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.minuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.secondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                ret.Add(command.currentOperatingStatus[0]);
                ret.Add(command.currentOperatingStatus[1]);
                ret.Add(command.currentOperatingStatus[2]);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.properties1);
                        ret.Add(item.year[0]);
                        ret.Add(item.year[1]);
                        ret.Add(item.month);
                        ret.Add(item.day);
                        ret.Add(item.hourLocalTime);
                        ret.Add(item.minuteLocalTime);
                        ret.Add(item.secondLocalTime);
                    }
                }
                return ret.ToArray();
            }
        }
        public class METER_TBL_STATUS_DATE_GET
        {
            public const byte ID = 0x0A;
            public byte maximumReports;
            public byte[] startYear = new byte[2];
            public byte startMonth;
            public byte startDay;
            public byte startHourLocalTime;
            public byte startMinuteLocalTime;
            public byte startSecondLocalTime;
            public byte[] stopYear = new byte[2];
            public byte stopMonth;
            public byte stopDay;
            public byte stopHourLocalTime;
            public byte stopMinuteLocalTime;
            public byte stopSecondLocalTime;
            public static implicit operator METER_TBL_STATUS_DATE_GET(byte[] data)
            {
                METER_TBL_STATUS_DATE_GET ret = new METER_TBL_STATUS_DATE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.maximumReports = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.startMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startSecondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.stopMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopSecondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_STATUS_DATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.maximumReports);
                ret.Add(command.startYear[0]);
                ret.Add(command.startYear[1]);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHourLocalTime);
                ret.Add(command.startMinuteLocalTime);
                ret.Add(command.startSecondLocalTime);
                ret.Add(command.stopYear[0]);
                ret.Add(command.stopYear[1]);
                ret.Add(command.stopMonth);
                ret.Add(command.stopDay);
                ret.Add(command.stopHourLocalTime);
                ret.Add(command.stopMinuteLocalTime);
                ret.Add(command.stopSecondLocalTime);
                return ret.ToArray();
            }
        }
        public class METER_TBL_STATUS_DEPTH_GET
        {
            public const byte ID = 0x09;
            public byte statusEventLogDepth;
            public static implicit operator METER_TBL_STATUS_DEPTH_GET(byte[] data)
            {
                METER_TBL_STATUS_DEPTH_GET ret = new METER_TBL_STATUS_DEPTH_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.statusEventLogDepth = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_STATUS_DEPTH_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.statusEventLogDepth);
                return ret.ToArray();
            }
        }
        public class METER_TBL_STATUS_SUPPORTED_GET
        {
            public const byte ID = 0x07;
            public static implicit operator METER_TBL_STATUS_SUPPORTED_GET(byte[] data)
            {
                METER_TBL_STATUS_SUPPORTED_GET ret = new METER_TBL_STATUS_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](METER_TBL_STATUS_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_TBL_STATUS_SUPPORTED_REPORT
        {
            public const byte ID = 0x08;
            public byte[] supportedOperatingStatus = new byte[3];
            public byte statusEventLogDepth;
            public static implicit operator METER_TBL_STATUS_SUPPORTED_REPORT(byte[] data)
            {
                METER_TBL_STATUS_SUPPORTED_REPORT ret = new METER_TBL_STATUS_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedOperatingStatus = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.statusEventLogDepth = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_STATUS_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.supportedOperatingStatus[0]);
                ret.Add(command.supportedOperatingStatus[1]);
                ret.Add(command.supportedOperatingStatus[2]);
                ret.Add(command.statusEventLogDepth);
                return ret.ToArray();
            }
        }
        public class METER_TBL_CURRENT_DATA_GET
        {
            public const byte ID = 0x0C;
            public byte[] datasetRequested = new byte[3];
            public static implicit operator METER_TBL_CURRENT_DATA_GET(byte[] data)
            {
                METER_TBL_CURRENT_DATA_GET ret = new METER_TBL_CURRENT_DATA_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.datasetRequested = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_CURRENT_DATA_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.datasetRequested[0]);
                ret.Add(command.datasetRequested[1]);
                ret.Add(command.datasetRequested[2]);
                return ret.ToArray();
            }
        }
        public class METER_TBL_CURRENT_DATA_REPORT
        {
            public const byte ID = 0x0D;
            public byte reportsToFollow;
            public struct Tproperties1
            {
                private byte _value;
                public byte rateType
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte reserved
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
            public byte[] dataset = new byte[3];
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourLocalTime;
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public class TVG
            {
                public struct Tproperties1
                {
                    private byte _value;
                    public byte currentScale
                    {
                        get { return (byte)(_value >> 0 & 0x1F); }
                        set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                    }
                    public byte currentPrecision
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
                public byte[] currentValue = new byte[4];
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator METER_TBL_CURRENT_DATA_REPORT(byte[] data)
            {
                METER_TBL_CURRENT_DATA_REPORT ret = new METER_TBL_CURRENT_DATA_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dataset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
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
                    ret.vg = new List<TVG>();
                    for (int j = 0; j < ret.reportsToFollow; j++)
                    {
                        TVG tmp = new TVG();
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.currentValue = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_CURRENT_DATA_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                ret.Add(command.properties1);
                ret.Add(command.dataset[0]);
                ret.Add(command.dataset[1]);
                ret.Add(command.dataset[2]);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourLocalTime);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.properties1);
                        ret.Add(item.currentValue[0]);
                        ret.Add(item.currentValue[1]);
                        ret.Add(item.currentValue[2]);
                        ret.Add(item.currentValue[3]);
                    }
                }
                return ret.ToArray();
            }
        }
        public class METER_TBL_HISTORICAL_DATA_GET
        {
            public const byte ID = 0x0E;
            public byte maximumReports;
            public byte[] historicalDatasetRequested = new byte[3];
            public byte[] startYear = new byte[2];
            public byte startMonth;
            public byte startDay;
            public byte startHourLocalTime;
            public byte startMinuteLocalTime;
            public byte startSecondLocalTime;
            public byte[] stopYear = new byte[2];
            public byte stopMonth;
            public byte stopDay;
            public byte stopHourLocalTime;
            public byte stopMinuteLocalTime;
            public byte stopSecondLocalTime;
            public static implicit operator METER_TBL_HISTORICAL_DATA_GET(byte[] data)
            {
                METER_TBL_HISTORICAL_DATA_GET ret = new METER_TBL_HISTORICAL_DATA_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.maximumReports = data.Length > index ? data[index++] : (byte)0x00;
                    ret.historicalDatasetRequested = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.startYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.startMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startSecondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopYear = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.stopMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHourLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopSecondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_HISTORICAL_DATA_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.maximumReports);
                ret.Add(command.historicalDatasetRequested[0]);
                ret.Add(command.historicalDatasetRequested[1]);
                ret.Add(command.historicalDatasetRequested[2]);
                ret.Add(command.startYear[0]);
                ret.Add(command.startYear[1]);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHourLocalTime);
                ret.Add(command.startMinuteLocalTime);
                ret.Add(command.startSecondLocalTime);
                ret.Add(command.stopYear[0]);
                ret.Add(command.stopYear[1]);
                ret.Add(command.stopMonth);
                ret.Add(command.stopDay);
                ret.Add(command.stopHourLocalTime);
                ret.Add(command.stopMinuteLocalTime);
                ret.Add(command.stopSecondLocalTime);
                return ret.ToArray();
            }
        }
        public class METER_TBL_HISTORICAL_DATA_REPORT
        {
            public const byte ID = 0x0F;
            public byte reportsToFollow;
            public struct Tproperties1
            {
                private byte _value;
                public byte rateType
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte reserved
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
            public byte[] dataset = new byte[3];
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourLocalTime;
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public class TVG
            {
                public struct Tproperties1
                {
                    private byte _value;
                    public byte historicalScale
                    {
                        get { return (byte)(_value >> 0 & 0x1F); }
                        set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                    }
                    public byte historicalPrecision
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
                public byte[] historicalValue = new byte[4];
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator METER_TBL_HISTORICAL_DATA_REPORT(byte[] data)
            {
                METER_TBL_HISTORICAL_DATA_REPORT ret = new METER_TBL_HISTORICAL_DATA_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dataset = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
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
                    ret.vg = new List<TVG>();
                    for (int j = 0; j < ret.reportsToFollow; j++)
                    {
                        TVG tmp = new TVG();
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.historicalValue = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_HISTORICAL_DATA_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                ret.Add(command.properties1);
                ret.Add(command.dataset[0]);
                ret.Add(command.dataset[1]);
                ret.Add(command.dataset[2]);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourLocalTime);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.properties1);
                        ret.Add(item.historicalValue[0]);
                        ret.Add(item.historicalValue[1]);
                        ret.Add(item.historicalValue[2]);
                        ret.Add(item.historicalValue[3]);
                    }
                }
                return ret.ToArray();
            }
        }
        public class METER_TBL_REPORT
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte meterType
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
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
            public struct Tproperties2
            {
                private byte _value;
                public byte payMeter
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
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
            public byte[] datasetSupported = new byte[3];
            public byte[] datasetHistorySupported = new byte[3];
            public byte[] dataHistorySupported = new byte[3];
            public static implicit operator METER_TBL_REPORT(byte[] data)
            {
                METER_TBL_REPORT ret = new METER_TBL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datasetSupported = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.datasetHistorySupported = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.dataHistorySupported = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.datasetSupported[0]);
                ret.Add(command.datasetSupported[1]);
                ret.Add(command.datasetSupported[2]);
                ret.Add(command.datasetHistorySupported[0]);
                ret.Add(command.datasetHistorySupported[1]);
                ret.Add(command.datasetHistorySupported[2]);
                ret.Add(command.dataHistorySupported[0]);
                ret.Add(command.dataHistorySupported[1]);
                ret.Add(command.dataHistorySupported[2]);
                return ret.ToArray();
            }
        }
        public class METER_TBL_TABLE_CAPABILITY_GET
        {
            public const byte ID = 0x05;
            public static implicit operator METER_TBL_TABLE_CAPABILITY_GET(byte[] data)
            {
                METER_TBL_TABLE_CAPABILITY_GET ret = new METER_TBL_TABLE_CAPABILITY_GET();
                return ret;
            }
            public static implicit operator byte[](METER_TBL_TABLE_CAPABILITY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_TBL_TABLE_ID_GET
        {
            public const byte ID = 0x03;
            public static implicit operator METER_TBL_TABLE_ID_GET(byte[] data)
            {
                METER_TBL_TABLE_ID_GET ret = new METER_TBL_TABLE_ID_GET();
                return ret;
            }
            public static implicit operator byte[](METER_TBL_TABLE_ID_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_TBL_TABLE_ID_REPORT
        {
            public const byte ID = 0x04;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfCharacters
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
            public IList<byte> meterIdCharacter = new List<byte>();
            public static implicit operator METER_TBL_TABLE_ID_REPORT(byte[] data)
            {
                METER_TBL_TABLE_ID_REPORT ret = new METER_TBL_TABLE_ID_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.meterIdCharacter = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfCharacters; i++)
                    {
                        ret.meterIdCharacter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_TABLE_ID_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.meterIdCharacter != null)
                {
                    foreach (var tmp in command.meterIdCharacter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class METER_TBL_TABLE_POINT_ADM_NO_GET
        {
            public const byte ID = 0x01;
            public static implicit operator METER_TBL_TABLE_POINT_ADM_NO_GET(byte[] data)
            {
                METER_TBL_TABLE_POINT_ADM_NO_GET ret = new METER_TBL_TABLE_POINT_ADM_NO_GET();
                return ret;
            }
            public static implicit operator byte[](METER_TBL_TABLE_POINT_ADM_NO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_TBL_TABLE_POINT_ADM_NO_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfCharacters
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
            public IList<byte> meterPointAdmNumberCharacter = new List<byte>();
            public static implicit operator METER_TBL_TABLE_POINT_ADM_NO_REPORT(byte[] data)
            {
                METER_TBL_TABLE_POINT_ADM_NO_REPORT ret = new METER_TBL_TABLE_POINT_ADM_NO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.meterPointAdmNumberCharacter = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfCharacters; i++)
                    {
                        ret.meterPointAdmNumberCharacter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_TABLE_POINT_ADM_NO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.meterPointAdmNumberCharacter != null)
                {
                    foreach (var tmp in command.meterPointAdmNumberCharacter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

