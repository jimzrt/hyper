using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_RATE_TBL_MONITOR
    {
        public const byte ID = 0x49;
        public const byte VERSION = 1;
        public class RATE_TBL_ACTIVE_RATE_GET
        {
            public const byte ID = 0x05;
            public static implicit operator RATE_TBL_ACTIVE_RATE_GET(byte[] data)
            {
                RATE_TBL_ACTIVE_RATE_GET ret = new RATE_TBL_ACTIVE_RATE_GET();
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_ACTIVE_RATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class RATE_TBL_ACTIVE_RATE_REPORT
        {
            public const byte ID = 0x06;
            public byte rateParameterSetId;
            public static implicit operator RATE_TBL_ACTIVE_RATE_REPORT(byte[] data)
            {
                RATE_TBL_ACTIVE_RATE_REPORT ret = new RATE_TBL_ACTIVE_RATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_ACTIVE_RATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                return ret.ToArray();
            }
        }
        public class RATE_TBL_CURRENT_DATA_GET
        {
            public const byte ID = 0x07;
            public byte rateParameterSetId;
            public byte[] datasetRequested = new byte[3];
            public static implicit operator RATE_TBL_CURRENT_DATA_GET(byte[] data)
            {
                RATE_TBL_CURRENT_DATA_GET ret = new RATE_TBL_CURRENT_DATA_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datasetRequested = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_CURRENT_DATA_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                ret.Add(command.datasetRequested[0]);
                ret.Add(command.datasetRequested[1]);
                ret.Add(command.datasetRequested[2]);
                return ret.ToArray();
            }
        }
        public class RATE_TBL_CURRENT_DATA_REPORT
        {
            public const byte ID = 0x08;
            public byte reportsToFollow;
            public byte rateParameterSetId;
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
            public static implicit operator RATE_TBL_CURRENT_DATA_REPORT(byte[] data)
            {
                RATE_TBL_CURRENT_DATA_REPORT ret = new RATE_TBL_CURRENT_DATA_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
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
            public static implicit operator byte[](RATE_TBL_CURRENT_DATA_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                ret.Add(command.rateParameterSetId);
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
        public class RATE_TBL_GET
        {
            public const byte ID = 0x03;
            public byte rateParameterSetId;
            public static implicit operator RATE_TBL_GET(byte[] data)
            {
                RATE_TBL_GET ret = new RATE_TBL_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.rateParameterSetId);
                return ret.ToArray();
            }
        }
        public class RATE_TBL_HISTORICAL_DATA_GET
        {
            public const byte ID = 0x09;
            public byte maximumReports;
            public byte rateParameterSetId;
            public byte[] datasetRequested = new byte[3];
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
            public static implicit operator RATE_TBL_HISTORICAL_DATA_GET(byte[] data)
            {
                RATE_TBL_HISTORICAL_DATA_GET ret = new RATE_TBL_HISTORICAL_DATA_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.maximumReports = data.Length > index ? data[index++] : (byte)0x00;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.datasetRequested = new byte[]
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
            public static implicit operator byte[](RATE_TBL_HISTORICAL_DATA_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.maximumReports);
                ret.Add(command.rateParameterSetId);
                ret.Add(command.datasetRequested[0]);
                ret.Add(command.datasetRequested[1]);
                ret.Add(command.datasetRequested[2]);
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
        public class RATE_TBL_HISTORICAL_DATA_REPORT
        {
            public const byte ID = 0x0A;
            public byte reportsToFollow;
            public byte rateParameterSetId;
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
            public static implicit operator RATE_TBL_HISTORICAL_DATA_REPORT(byte[] data)
            {
                RATE_TBL_HISTORICAL_DATA_REPORT ret = new RATE_TBL_HISTORICAL_DATA_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.rateParameterSetId = data.Length > index ? data[index++] : (byte)0x00;
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
            public static implicit operator byte[](RATE_TBL_HISTORICAL_DATA_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                ret.Add(command.rateParameterSetId);
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
        public class RATE_TBL_REPORT
        {
            public const byte ID = 0x04;
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
            public static implicit operator RATE_TBL_REPORT(byte[] data)
            {
                RATE_TBL_REPORT ret = new RATE_TBL_REPORT();
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
            public static implicit operator byte[](RATE_TBL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
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
        public class RATE_TBL_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator RATE_TBL_SUPPORTED_GET(byte[] data)
            {
                RATE_TBL_SUPPORTED_GET ret = new RATE_TBL_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class RATE_TBL_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public byte ratesSupported;
            public byte[] parameterSetSupportedBitMask = new byte[2];
            public static implicit operator RATE_TBL_SUPPORTED_REPORT(byte[] data)
            {
                RATE_TBL_SUPPORTED_REPORT ret = new RATE_TBL_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.ratesSupported = data.Length > index ? data[index++] : (byte)0x00;
                    ret.parameterSetSupportedBitMask = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](RATE_TBL_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_RATE_TBL_MONITOR.ID);
                ret.Add(ID);
                ret.Add(command.ratesSupported);
                ret.Add(command.parameterSetSupportedBitMask[0]);
                ret.Add(command.parameterSetSupportedBitMask[1]);
                return ret.ToArray();
            }
        }
    }
}

