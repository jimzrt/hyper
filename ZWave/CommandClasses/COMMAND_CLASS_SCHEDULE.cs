using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SCHEDULE
    {
        public const byte ID = 0x53;
        public const byte VERSION = 1;
        public class SCHEDULE_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator SCHEDULE_SUPPORTED_GET(byte[] data)
            {
                SCHEDULE_SUPPORTED_GET ret = new SCHEDULE_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public byte numberOfSupportedScheduleId;
            public struct Tproperties1
            {
                private byte _value;
                public byte startTimeSupport
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte fallbackSupport
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte supportEnableDisable
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
            public byte numberOfSupportedCc;
            public class TVG1
            {
                public byte supportedCc;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte supportedCommand
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
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public struct Tproperties2
            {
                private byte _value;
                public byte supportedOverrideTypes
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte overrideSupport
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
            public static implicit operator SCHEDULE_SUPPORTED_REPORT(byte[] data)
            {
                SCHEDULE_SUPPORTED_REPORT ret = new SCHEDULE_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfSupportedScheduleId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfSupportedCc = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.numberOfSupportedCc; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.supportedCc = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.numberOfSupportedScheduleId);
                ret.Add(command.properties1);
                ret.Add(command.numberOfSupportedCc);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.supportedCc);
                        ret.Add(item.properties1);
                    }
                }
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
        public class COMMAND_SCHEDULE_SET
        {
            public const byte ID = 0x03;
            public byte scheduleId;
            public byte userIdentifier;
            public byte startYear;
            public struct Tproperties1
            {
                private byte _value;
                public byte startMonth
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved1
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
            public struct Tproperties2
            {
                private byte _value;
                public byte startDayOfMonth
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved2
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
            public struct Tproperties3
            {
                private byte _value;
                public byte startWeekday
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public struct Tproperties4
            {
                private byte _value;
                public byte startHour
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte durationType
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties4(byte data)
                {
                    Tproperties4 ret = new Tproperties4();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties4 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties4 properties4;
            public struct Tproperties5
            {
                private byte _value;
                public byte startMinute
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte reserved3
                {
                    get { return (byte)(_value >> 6 & 0x03); }
                    set { _value &= 0xFF - 0xC0; _value += (byte)(value << 6 & 0xC0); }
                }
                public static implicit operator Tproperties5(byte data)
                {
                    Tproperties5 ret = new Tproperties5();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties5 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties5 properties5;
            public byte[] durationByte = new byte[2];
            public byte reportsToFollow;
            public byte numberOfCmdToFollow;
            public class TVG1
            {
                public byte cmdLength;
                public IList<byte> cmdByte = new List<byte>();
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator COMMAND_SCHEDULE_SET(byte[] data)
            {
                COMMAND_SCHEDULE_SET ret = new COMMAND_SCHEDULE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.scheduleId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties5 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationByte = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfCmdToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.numberOfCmdToFollow; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.cmdLength = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.cmdByte = new List<byte>();
                        for (int i = 0; i < tmp.cmdLength; i++)
                        {
                            tmp.cmdByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_SCHEDULE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.scheduleId);
                ret.Add(command.userIdentifier);
                ret.Add(command.startYear);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.properties3);
                ret.Add(command.properties4);
                ret.Add(command.properties5);
                ret.Add(command.durationByte[0]);
                ret.Add(command.durationByte[1]);
                ret.Add(command.reportsToFollow);
                ret.Add(command.numberOfCmdToFollow);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.cmdLength);
                        if (item.cmdByte != null)
                        {
                            foreach (var tmp in item.cmdByte)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class COMMAND_SCHEDULE_GET
        {
            public const byte ID = 0x04;
            public byte scheduleId;
            public static implicit operator COMMAND_SCHEDULE_GET(byte[] data)
            {
                COMMAND_SCHEDULE_GET ret = new COMMAND_SCHEDULE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.scheduleId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_SCHEDULE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.scheduleId);
                return ret.ToArray();
            }
        }
        public class COMMAND_SCHEDULE_REPORT
        {
            public const byte ID = 0x05;
            public byte scheduleId;
            public byte userIdentifier;
            public byte startYear;
            public struct Tproperties1
            {
                private byte _value;
                public byte startMonth
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte activeId
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
            public struct Tproperties2
            {
                private byte _value;
                public byte startDayOfMonth
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved2
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
            public struct Tproperties3
            {
                private byte _value;
                public byte startWeekday
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public struct Tproperties4
            {
                private byte _value;
                public byte startHour
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte durationType
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties4(byte data)
                {
                    Tproperties4 ret = new Tproperties4();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties4 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties4 properties4;
            public struct Tproperties5
            {
                private byte _value;
                public byte startMinute
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte reserved3
                {
                    get { return (byte)(_value >> 6 & 0x03); }
                    set { _value &= 0xFF - 0xC0; _value += (byte)(value << 6 & 0xC0); }
                }
                public static implicit operator Tproperties5(byte data)
                {
                    Tproperties5 ret = new Tproperties5();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties5 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties5 properties5;
            public byte[] durationByte = new byte[2];
            public byte reportsToFollow;
            public byte numberOfCmdToFollow;
            public class TVG1
            {
                public byte cmdLength;
                public IList<byte> cmdByte = new List<byte>();
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator COMMAND_SCHEDULE_REPORT(byte[] data)
            {
                COMMAND_SCHEDULE_REPORT ret = new COMMAND_SCHEDULE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.scheduleId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startYear = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties5 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.durationByte = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfCmdToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.numberOfCmdToFollow; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.cmdLength = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.cmdByte = new List<byte>();
                        for (int i = 0; i < tmp.cmdLength; i++)
                        {
                            tmp.cmdByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_SCHEDULE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.scheduleId);
                ret.Add(command.userIdentifier);
                ret.Add(command.startYear);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.properties3);
                ret.Add(command.properties4);
                ret.Add(command.properties5);
                ret.Add(command.durationByte[0]);
                ret.Add(command.durationByte[1]);
                ret.Add(command.reportsToFollow);
                ret.Add(command.numberOfCmdToFollow);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.cmdLength);
                        if (item.cmdByte != null)
                        {
                            foreach (var tmp in item.cmdByte)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class SCHEDULE_REMOVE
        {
            public const byte ID = 0x06;
            public byte scheduleId;
            public static implicit operator SCHEDULE_REMOVE(byte[] data)
            {
                SCHEDULE_REMOVE ret = new SCHEDULE_REMOVE();
                if (data != null)
                {
                    int index = 2;
                    ret.scheduleId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_REMOVE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.scheduleId);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_STATE_SET
        {
            public const byte ID = 0x07;
            public byte scheduleId;
            public byte scheduleState;
            public static implicit operator SCHEDULE_STATE_SET(byte[] data)
            {
                SCHEDULE_STATE_SET ret = new SCHEDULE_STATE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.scheduleId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.scheduleState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_STATE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.scheduleId);
                ret.Add(command.scheduleState);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_STATE_GET
        {
            public const byte ID = 0x08;
            public static implicit operator SCHEDULE_STATE_GET(byte[] data)
            {
                SCHEDULE_STATE_GET ret = new SCHEDULE_STATE_GET();
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_STATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_STATE_REPORT
        {
            public const byte ID = 0x09;
            public byte numberOfSupportedScheduleId;
            public struct Tproperties1
            {
                private byte _value;
                public byte moverride
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reportsToFollow
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
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
                public struct Tproperties1
                {
                    private byte _value;
                    public byte activeId1
                    {
                        get { return (byte)(_value >> 0 & 0x0F); }
                        set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                    }
                    public byte activeId2
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
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator SCHEDULE_STATE_REPORT(byte[] data)
            {
                SCHEDULE_STATE_REPORT ret = new SCHEDULE_STATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfSupportedScheduleId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    while (data.Length - 0 > index)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_STATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.numberOfSupportedScheduleId);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.properties1);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

