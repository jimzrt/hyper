using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3
    {
        public const byte ID = 0x4E;
        public const byte VERSION = 3;
        public class SCHEDULE_ENTRY_LOCK_ENABLE_ALL_SET
        {
            public const byte ID = 0x02;
            public byte enabled;
            public static implicit operator SCHEDULE_ENTRY_LOCK_ENABLE_ALL_SET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_ENABLE_ALL_SET ret = new SCHEDULE_ENTRY_LOCK_ENABLE_ALL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.enabled = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_ENABLE_ALL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.enabled);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_ENABLE_SET
        {
            public const byte ID = 0x01;
            public byte userIdentifier;
            public byte enabled;
            public static implicit operator SCHEDULE_ENTRY_LOCK_ENABLE_SET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_ENABLE_SET ret = new SCHEDULE_ENTRY_LOCK_ENABLE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.enabled = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_ENABLE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.enabled);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_TIME_OFFSET_GET
        {
            public const byte ID = 0x0B;
            public static implicit operator SCHEDULE_ENTRY_LOCK_TIME_OFFSET_GET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_TIME_OFFSET_GET ret = new SCHEDULE_ENTRY_LOCK_TIME_OFFSET_GET();
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_TIME_OFFSET_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_TIME_OFFSET_REPORT
        {
            public const byte ID = 0x0C;
            public struct Tproperties1
            {
                private byte _value;
                public byte hourTzo
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signTzo
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
            public byte minuteTzo;
            public struct Tproperties2
            {
                private byte _value;
                public byte minuteOffsetDst
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signOffsetDst
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
            public static implicit operator SCHEDULE_ENTRY_LOCK_TIME_OFFSET_REPORT(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_TIME_OFFSET_REPORT ret = new SCHEDULE_ENTRY_LOCK_TIME_OFFSET_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteTzo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_TIME_OFFSET_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minuteTzo);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_TIME_OFFSET_SET
        {
            public const byte ID = 0x0D;
            public struct Tproperties1
            {
                private byte _value;
                public byte hourTzo
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signTzo
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
            public byte minuteTzo;
            public struct Tproperties2
            {
                private byte _value;
                public byte minuteOffsetDst
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signOffsetDst
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
            public static implicit operator SCHEDULE_ENTRY_LOCK_TIME_OFFSET_SET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_TIME_OFFSET_SET ret = new SCHEDULE_ENTRY_LOCK_TIME_OFFSET_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteTzo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_TIME_OFFSET_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minuteTzo);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_WEEK_DAY_GET
        {
            public const byte ID = 0x04;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public static implicit operator SCHEDULE_ENTRY_LOCK_WEEK_DAY_GET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_WEEK_DAY_GET ret = new SCHEDULE_ENTRY_LOCK_WEEK_DAY_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_WEEK_DAY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_WEEK_DAY_REPORT
        {
            public const byte ID = 0x05;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public byte dayOfWeek;
            public byte startHour;
            public byte startMinute;
            public byte stopHour;
            public byte stopMinute;
            public static implicit operator SCHEDULE_ENTRY_LOCK_WEEK_DAY_REPORT(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_WEEK_DAY_REPORT ret = new SCHEDULE_ENTRY_LOCK_WEEK_DAY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dayOfWeek = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinute = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_WEEK_DAY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                ret.Add(command.dayOfWeek);
                ret.Add(command.startHour);
                ret.Add(command.startMinute);
                ret.Add(command.stopHour);
                ret.Add(command.stopMinute);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_WEEK_DAY_SET
        {
            public const byte ID = 0x03;
            public byte setAction;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public byte dayOfWeek;
            public byte startHour;
            public byte startMinute;
            public byte stopHour;
            public byte stopMinute;
            public static implicit operator SCHEDULE_ENTRY_LOCK_WEEK_DAY_SET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_WEEK_DAY_SET ret = new SCHEDULE_ENTRY_LOCK_WEEK_DAY_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.setAction = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dayOfWeek = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinute = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_WEEK_DAY_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.setAction);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                ret.Add(command.dayOfWeek);
                ret.Add(command.startHour);
                ret.Add(command.startMinute);
                ret.Add(command.stopHour);
                ret.Add(command.stopMinute);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_YEAR_DAY_GET
        {
            public const byte ID = 0x07;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public static implicit operator SCHEDULE_ENTRY_LOCK_YEAR_DAY_GET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_YEAR_DAY_GET ret = new SCHEDULE_ENTRY_LOCK_YEAR_DAY_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_YEAR_DAY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_YEAR_DAY_REPORT
        {
            public const byte ID = 0x08;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public byte startYear;
            public byte startMonth;
            public byte startDay;
            public byte startHour;
            public byte startMinute;
            public byte stopYear;
            public byte stopMonth;
            public byte stopDay;
            public byte stopHour;
            public byte stopMinute;
            public static implicit operator SCHEDULE_ENTRY_LOCK_YEAR_DAY_REPORT(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_YEAR_DAY_REPORT ret = new SCHEDULE_ENTRY_LOCK_YEAR_DAY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinute = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopYear = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_YEAR_DAY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                ret.Add(command.startYear);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHour);
                ret.Add(command.startMinute);
                ret.Add(command.stopYear);
                ret.Add(command.stopMonth);
                ret.Add(command.stopDay);
                ret.Add(command.stopHour);
                ret.Add(command.stopMinute);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_YEAR_DAY_SET
        {
            public const byte ID = 0x06;
            public byte setAction;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public byte startYear;
            public byte startMonth;
            public byte startDay;
            public byte startHour;
            public byte startMinute;
            public byte stopYear;
            public byte stopMonth;
            public byte stopDay;
            public byte stopHour;
            public byte stopMinute;
            public static implicit operator SCHEDULE_ENTRY_LOCK_YEAR_DAY_SET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_YEAR_DAY_SET ret = new SCHEDULE_ENTRY_LOCK_YEAR_DAY_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.setAction = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinute = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopYear = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMonth = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopMinute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_YEAR_DAY_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.setAction);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                ret.Add(command.startYear);
                ret.Add(command.startMonth);
                ret.Add(command.startDay);
                ret.Add(command.startHour);
                ret.Add(command.startMinute);
                ret.Add(command.stopYear);
                ret.Add(command.stopMonth);
                ret.Add(command.stopDay);
                ret.Add(command.stopHour);
                ret.Add(command.stopMinute);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_TYPE_SUPPORTED_GET
        {
            public const byte ID = 0x09;
            public static implicit operator SCHEDULE_ENTRY_TYPE_SUPPORTED_GET(byte[] data)
            {
                SCHEDULE_ENTRY_TYPE_SUPPORTED_GET ret = new SCHEDULE_ENTRY_TYPE_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_TYPE_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_TYPE_SUPPORTED_REPORT
        {
            public const byte ID = 0x0A;
            public byte numberOfSlotsWeekDay;
            public byte numberOfSlotsYearDay;
            public byte numberOfSlotsDailyRepeating;
            public static implicit operator SCHEDULE_ENTRY_TYPE_SUPPORTED_REPORT(byte[] data)
            {
                SCHEDULE_ENTRY_TYPE_SUPPORTED_REPORT ret = new SCHEDULE_ENTRY_TYPE_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfSlotsWeekDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfSlotsYearDay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfSlotsDailyRepeating = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_TYPE_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.numberOfSlotsWeekDay);
                ret.Add(command.numberOfSlotsYearDay);
                ret.Add(command.numberOfSlotsDailyRepeating);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_GET
        {
            public const byte ID = 0x0E;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public static implicit operator SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_GET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_GET ret = new SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_REPORT
        {
            public const byte ID = 0x0F;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public byte weekDayBitmask;
            public byte startHour;
            public byte startMinute;
            public byte durationHour;
            public byte durationMinute;
            public static implicit operator SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_REPORT(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_REPORT ret = new SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.weekDayBitmask = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinute = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationMinute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                ret.Add(command.weekDayBitmask);
                ret.Add(command.startHour);
                ret.Add(command.startMinute);
                ret.Add(command.durationHour);
                ret.Add(command.durationMinute);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_SET
        {
            public const byte ID = 0x10;
            public byte setAction;
            public byte userIdentifier;
            public byte scheduleSlotId;
            public byte weekDayBitmask;
            public byte startHour;
            public byte startMinute;
            public byte durationHour;
            public byte durationMinute;
            public static implicit operator SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_SET(byte[] data)
            {
                SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_SET ret = new SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.setAction = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleSlotId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.weekDayBitmask = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startMinute = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationHour = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationMinute = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_ENTRY_LOCK_DAILY_REPEATING_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE_ENTRY_LOCK_V3.ID);
                ret.Add(ID);
                ret.Add(command.setAction);
                ret.Add(command.userIdentifier);
                ret.Add(command.scheduleSlotId);
                ret.Add(command.weekDayBitmask);
                ret.Add(command.startHour);
                ret.Add(command.startMinute);
                ret.Add(command.durationHour);
                ret.Add(command.durationMinute);
                return ret.ToArray();
            }
        }
    }
}

