using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_DCP_CONFIG
    {
        public const byte ID = 0x3A;
        public const byte VERSION = 1;
        public class DCP_LIST_REMOVE
        {
            public const byte ID = 0x04;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourLocalTime;
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public static implicit operator DCP_LIST_REMOVE(byte[] data)
            {
                DCP_LIST_REMOVE ret = new DCP_LIST_REMOVE();
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
                }
                return ret;
            }
            public static implicit operator byte[](DCP_LIST_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DCP_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourLocalTime);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                return ret.ToArray();
            }
        }
        public class DCP_LIST_SET
        {
            public const byte ID = 0x03;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourLocalTime;
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public byte dcpRateId;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfDc
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
            public class TVG1
            {
                public byte genericDeviceClass;
                public byte specificDeviceClass;
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public byte[] startYear = new byte[2];
            public byte startMonth;
            public byte startDay;
            public byte startHourLocalTime;
            public byte startMinuteLocalTime;
            public byte startSecondLocalTime;
            public byte durationHourTime;
            public byte durationMinuteTime;
            public byte durationSecondTime;
            public byte eventPriority;
            public byte loadShedding;
            public byte startAssociationGroup;
            public byte stopAssociationGroup;
            public byte randomizationInterval;
            public static implicit operator DCP_LIST_SET(byte[] data)
            {
                DCP_LIST_SET ret = new DCP_LIST_SET();
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
                    ret.dcpRateId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.properties1.numberOfDc; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.genericDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.specificDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
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
                    ret.durationHourTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationMinuteTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationSecondTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventPriority = data.Length > index ? data[index++] : (byte)0x00;
                    ret.loadShedding = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startAssociationGroup = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopAssociationGroup = data.Length > index ? data[index++] : (byte)0x00;
                    ret.randomizationInterval = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DCP_LIST_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DCP_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourLocalTime);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                ret.Add(command.dcpRateId);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.genericDeviceClass);
                        ret.Add(item.specificDeviceClass);
                    }
                }
                ret.Add(command.startYear[0]);
                ret.Add(command.startYear[1]);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHourLocalTime);
                ret.Add(command.startMinuteLocalTime);
                ret.Add(command.startSecondLocalTime);
                ret.Add(command.durationHourTime);
                ret.Add(command.durationMinuteTime);
                ret.Add(command.durationSecondTime);
                ret.Add(command.eventPriority);
                ret.Add(command.loadShedding);
                ret.Add(command.startAssociationGroup);
                ret.Add(command.stopAssociationGroup);
                ret.Add(command.randomizationInterval);
                return ret.ToArray();
            }
        }
        public class DCP_LIST_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator DCP_LIST_SUPPORTED_GET(byte[] data)
            {
                DCP_LIST_SUPPORTED_GET ret = new DCP_LIST_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](DCP_LIST_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DCP_CONFIG.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DCP_LIST_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public byte dcpListSize;
            public byte freeDcpListEntries;
            public static implicit operator DCP_LIST_SUPPORTED_REPORT(byte[] data)
            {
                DCP_LIST_SUPPORTED_REPORT ret = new DCP_LIST_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.dcpListSize = data.Length > index ? data[index++] : (byte)0x00;
                    ret.freeDcpListEntries = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DCP_LIST_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DCP_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.dcpListSize);
                ret.Add(command.freeDcpListEntries);
                return ret.ToArray();
            }
        }
    }
}

