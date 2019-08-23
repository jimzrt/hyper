using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CHIMNEY_FAN
    {
        public const byte ID = 0x2A;
        public const byte VERSION = 1;
        public class CHIMNEY_FAN_ALARM_LOG_GET
        {
            public const byte ID = 0x20;
            public static implicit operator CHIMNEY_FAN_ALARM_LOG_GET(byte[] data)
            {
                CHIMNEY_FAN_ALARM_LOG_GET ret = new CHIMNEY_FAN_ALARM_LOG_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_LOG_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_LOG_REPORT
        {
            public const byte ID = 0x21;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved11
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm1
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError1
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded1
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte reserved12
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte alarmStillActive1
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
                public byte reserved21
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm2
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError2
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded2
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte reserved22
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte alarmStillActive2
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
            public struct Tproperties3
            {
                private byte _value;
                public byte reserved31
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm3
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError3
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded3
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte reserved32
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte alarmStillActive3
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
                public byte reserved41
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm4
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError4
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded4
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte reserved42
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte alarmStillActive4
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
                public byte reserved51
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm5
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError5
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded5
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte reserved52
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte alarmStillActive5
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public static implicit operator CHIMNEY_FAN_ALARM_LOG_REPORT(byte[] data)
            {
                CHIMNEY_FAN_ALARM_LOG_REPORT ret = new CHIMNEY_FAN_ALARM_LOG_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties5 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_LOG_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.properties3);
                ret.Add(command.properties4);
                ret.Add(command.properties5);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_LOG_SET
        {
            public const byte ID = 0x1F;
            public byte message;
            public static implicit operator CHIMNEY_FAN_ALARM_LOG_SET(byte[] data)
            {
                CHIMNEY_FAN_ALARM_LOG_SET ret = new CHIMNEY_FAN_ALARM_LOG_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.message = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_LOG_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.message);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_STATUS_GET
        {
            public const byte ID = 0x23;
            public static implicit operator CHIMNEY_FAN_ALARM_STATUS_GET(byte[] data)
            {
                CHIMNEY_FAN_ALARM_STATUS_GET ret = new CHIMNEY_FAN_ALARM_STATUS_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_STATUS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_STATUS_REPORT
        {
            public const byte ID = 0x24;
            public struct Tproperties1
            {
                private byte _value;
                public byte service
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte notUsed
                {
                    get { return (byte)(_value >> 4 & 0x03); }
                    set { _value &= 0xFF - 0x30; _value += (byte)(value << 4 & 0x30); }
                }
                public byte speedChangeEnable
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte startTemperatureExceeded
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
            public static implicit operator CHIMNEY_FAN_ALARM_STATUS_REPORT(byte[] data)
            {
                CHIMNEY_FAN_ALARM_STATUS_REPORT ret = new CHIMNEY_FAN_ALARM_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_STATUS_SET
        {
            public const byte ID = 0x22;
            public struct Tproperties1
            {
                private byte _value;
                public byte notUsed1
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte acknowledgeExternalAlarm
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte acknowledgeSensorError
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte acknowledgeAlarmTemperatureExceeded
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte notUsed2
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
            public static implicit operator CHIMNEY_FAN_ALARM_STATUS_SET(byte[] data)
            {
                CHIMNEY_FAN_ALARM_STATUS_SET ret = new CHIMNEY_FAN_ALARM_STATUS_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_STATUS_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_TEMP_GET
        {
            public const byte ID = 0x0E;
            public static implicit operator CHIMNEY_FAN_ALARM_TEMP_GET(byte[] data)
            {
                CHIMNEY_FAN_ALARM_TEMP_GET ret = new CHIMNEY_FAN_ALARM_TEMP_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_TEMP_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_TEMP_REPORT
        {
            public const byte ID = 0x0F;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_ALARM_TEMP_REPORT(byte[] data)
            {
                CHIMNEY_FAN_ALARM_TEMP_REPORT ret = new CHIMNEY_FAN_ALARM_TEMP_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_TEMP_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_ALARM_TEMP_SET
        {
            public const byte ID = 0x0D;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_ALARM_TEMP_SET(byte[] data)
            {
                CHIMNEY_FAN_ALARM_TEMP_SET ret = new CHIMNEY_FAN_ALARM_TEMP_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_ALARM_TEMP_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_BOOST_TIME_GET
        {
            public const byte ID = 0x11;
            public static implicit operator CHIMNEY_FAN_BOOST_TIME_GET(byte[] data)
            {
                CHIMNEY_FAN_BOOST_TIME_GET ret = new CHIMNEY_FAN_BOOST_TIME_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_BOOST_TIME_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_BOOST_TIME_REPORT
        {
            public const byte ID = 0x12;
            public byte time;
            public static implicit operator CHIMNEY_FAN_BOOST_TIME_REPORT(byte[] data)
            {
                CHIMNEY_FAN_BOOST_TIME_REPORT ret = new CHIMNEY_FAN_BOOST_TIME_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.time = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_BOOST_TIME_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.time);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_BOOST_TIME_SET
        {
            public const byte ID = 0x10;
            public byte time;
            public static implicit operator CHIMNEY_FAN_BOOST_TIME_SET(byte[] data)
            {
                CHIMNEY_FAN_BOOST_TIME_SET ret = new CHIMNEY_FAN_BOOST_TIME_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.time = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_BOOST_TIME_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.time);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_DEFAULT_SET
        {
            public const byte ID = 0x28;
            public static implicit operator CHIMNEY_FAN_DEFAULT_SET(byte[] data)
            {
                CHIMNEY_FAN_DEFAULT_SET ret = new CHIMNEY_FAN_DEFAULT_SET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_DEFAULT_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_MIN_SPEED_GET
        {
            public const byte ID = 0x26;
            public static implicit operator CHIMNEY_FAN_MIN_SPEED_GET(byte[] data)
            {
                CHIMNEY_FAN_MIN_SPEED_GET ret = new CHIMNEY_FAN_MIN_SPEED_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_MIN_SPEED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_MIN_SPEED_REPORT
        {
            public const byte ID = 0x27;
            public byte minSpeed;
            public static implicit operator CHIMNEY_FAN_MIN_SPEED_REPORT(byte[] data)
            {
                CHIMNEY_FAN_MIN_SPEED_REPORT ret = new CHIMNEY_FAN_MIN_SPEED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.minSpeed = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_MIN_SPEED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.minSpeed);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_MIN_SPEED_SET
        {
            public const byte ID = 0x25;
            public byte minSpeed;
            public static implicit operator CHIMNEY_FAN_MIN_SPEED_SET(byte[] data)
            {
                CHIMNEY_FAN_MIN_SPEED_SET ret = new CHIMNEY_FAN_MIN_SPEED_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.minSpeed = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_MIN_SPEED_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.minSpeed);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_MODE_GET
        {
            public const byte ID = 0x17;
            public static implicit operator CHIMNEY_FAN_MODE_GET(byte[] data)
            {
                CHIMNEY_FAN_MODE_GET ret = new CHIMNEY_FAN_MODE_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_MODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_MODE_REPORT
        {
            public const byte ID = 0x18;
            public byte mode;
            public static implicit operator CHIMNEY_FAN_MODE_REPORT(byte[] data)
            {
                CHIMNEY_FAN_MODE_REPORT ret = new CHIMNEY_FAN_MODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_MODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_MODE_SET
        {
            public const byte ID = 0x16;
            public byte mode;
            public static implicit operator CHIMNEY_FAN_MODE_SET(byte[] data)
            {
                CHIMNEY_FAN_MODE_SET ret = new CHIMNEY_FAN_MODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_MODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_SETUP_GET
        {
            public const byte ID = 0x1A;
            public static implicit operator CHIMNEY_FAN_SETUP_GET(byte[] data)
            {
                CHIMNEY_FAN_SETUP_GET ret = new CHIMNEY_FAN_SETUP_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_SETUP_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_SETUP_REPORT
        {
            public const byte ID = 0x1B;
            public byte mode;
            public byte boostTime;
            public byte stopTime;
            public byte minSpeed;
            public struct Tproperties1
            {
                private byte _value;
                public byte size1
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale1
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision1
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
            public IList<byte> startTemperature = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte size2
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale2
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision2
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
            public IList<byte> stopTemperature = new List<byte>();
            public struct Tproperties3
            {
                private byte _value;
                public byte size3
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale3
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision3
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
            public IList<byte> alarmTemperatureValue = new List<byte>();
            public static implicit operator CHIMNEY_FAN_SETUP_REPORT(byte[] data)
            {
                CHIMNEY_FAN_SETUP_REPORT ret = new CHIMNEY_FAN_SETUP_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.boostTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minSpeed = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startTemperature = new List<byte>();
                    for (int i = 0; i < ret.properties1.size1; i++)
                    {
                        ret.startTemperature.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopTemperature = new List<byte>();
                    for (int i = 0; i < ret.properties2.size2; i++)
                    {
                        ret.stopTemperature.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.alarmTemperatureValue = new List<byte>();
                    for (int i = 0; i < ret.properties3.size3; i++)
                    {
                        ret.alarmTemperatureValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_SETUP_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                ret.Add(command.boostTime);
                ret.Add(command.stopTime);
                ret.Add(command.minSpeed);
                ret.Add(command.properties1);
                if (command.startTemperature != null)
                {
                    foreach (var tmp in command.startTemperature)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.stopTemperature != null)
                {
                    foreach (var tmp in command.stopTemperature)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties3);
                if (command.alarmTemperatureValue != null)
                {
                    foreach (var tmp in command.alarmTemperatureValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_SETUP_SET
        {
            public const byte ID = 0x19;
            public byte mode;
            public byte boostTime;
            public byte stopTime;
            public byte minSpeed;
            public struct Tproperties1
            {
                private byte _value;
                public byte size1
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale1
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision1
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
            public IList<byte> startTemperature = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte size2
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale2
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision2
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
            public IList<byte> stopTemperature = new List<byte>();
            public struct Tproperties3
            {
                private byte _value;
                public byte size3
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale3
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision3
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
            public IList<byte> alarmTemperatureValue = new List<byte>();
            public static implicit operator CHIMNEY_FAN_SETUP_SET(byte[] data)
            {
                CHIMNEY_FAN_SETUP_SET ret = new CHIMNEY_FAN_SETUP_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.boostTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minSpeed = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startTemperature = new List<byte>();
                    for (int i = 0; i < ret.properties1.size1; i++)
                    {
                        ret.startTemperature.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stopTemperature = new List<byte>();
                    for (int i = 0; i < ret.properties2.size2; i++)
                    {
                        ret.stopTemperature.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.alarmTemperatureValue = new List<byte>();
                    for (int i = 0; i < ret.properties3.size3; i++)
                    {
                        ret.alarmTemperatureValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_SETUP_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                ret.Add(command.boostTime);
                ret.Add(command.stopTime);
                ret.Add(command.minSpeed);
                ret.Add(command.properties1);
                if (command.startTemperature != null)
                {
                    foreach (var tmp in command.startTemperature)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.stopTemperature != null)
                {
                    foreach (var tmp in command.stopTemperature)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties3);
                if (command.alarmTemperatureValue != null)
                {
                    foreach (var tmp in command.alarmTemperatureValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_SPEED_GET
        {
            public const byte ID = 0x05;
            public static implicit operator CHIMNEY_FAN_SPEED_GET(byte[] data)
            {
                CHIMNEY_FAN_SPEED_GET ret = new CHIMNEY_FAN_SPEED_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_SPEED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_SPEED_REPORT
        {
            public const byte ID = 0x06;
            public byte speed;
            public static implicit operator CHIMNEY_FAN_SPEED_REPORT(byte[] data)
            {
                CHIMNEY_FAN_SPEED_REPORT ret = new CHIMNEY_FAN_SPEED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.speed = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_SPEED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.speed);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_SPEED_SET
        {
            public const byte ID = 0x04;
            public byte speed;
            public static implicit operator CHIMNEY_FAN_SPEED_SET(byte[] data)
            {
                CHIMNEY_FAN_SPEED_SET ret = new CHIMNEY_FAN_SPEED_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.speed = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_SPEED_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.speed);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_START_TEMP_GET
        {
            public const byte ID = 0x08;
            public static implicit operator CHIMNEY_FAN_START_TEMP_GET(byte[] data)
            {
                CHIMNEY_FAN_START_TEMP_GET ret = new CHIMNEY_FAN_START_TEMP_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_START_TEMP_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_START_TEMP_REPORT
        {
            public const byte ID = 0x09;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_START_TEMP_REPORT(byte[] data)
            {
                CHIMNEY_FAN_START_TEMP_REPORT ret = new CHIMNEY_FAN_START_TEMP_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_START_TEMP_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_START_TEMP_SET
        {
            public const byte ID = 0x07;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_START_TEMP_SET(byte[] data)
            {
                CHIMNEY_FAN_START_TEMP_SET ret = new CHIMNEY_FAN_START_TEMP_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_START_TEMP_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STATE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator CHIMNEY_FAN_STATE_GET(byte[] data)
            {
                CHIMNEY_FAN_STATE_GET ret = new CHIMNEY_FAN_STATE_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STATE_REPORT
        {
            public const byte ID = 0x03;
            public byte state;
            public static implicit operator CHIMNEY_FAN_STATE_REPORT(byte[] data)
            {
                CHIMNEY_FAN_STATE_REPORT ret = new CHIMNEY_FAN_STATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.state = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.state);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STATE_SET
        {
            public const byte ID = 0x01;
            public byte state;
            public static implicit operator CHIMNEY_FAN_STATE_SET(byte[] data)
            {
                CHIMNEY_FAN_STATE_SET ret = new CHIMNEY_FAN_STATE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.state = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STATE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.state);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STATUS_GET
        {
            public const byte ID = 0x1D;
            public static implicit operator CHIMNEY_FAN_STATUS_GET(byte[] data)
            {
                CHIMNEY_FAN_STATUS_GET ret = new CHIMNEY_FAN_STATUS_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STATUS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STATUS_REPORT
        {
            public const byte ID = 0x1E;
            public byte state;
            public byte speed;
            public struct Tproperties1
            {
                private byte _value;
                public byte service
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte externalAlarm
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte sensorError
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte alarmTemperatureExceeded
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte notUsed
                {
                    get { return (byte)(_value >> 4 & 0x03); }
                    set { _value &= 0xFF - 0x30; _value += (byte)(value << 4 & 0x30); }
                }
                public byte speedChangeEnable
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte startTemperatureExceeded
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
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_STATUS_REPORT(byte[] data)
            {
                CHIMNEY_FAN_STATUS_REPORT ret = new CHIMNEY_FAN_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.state = data.Length > index ? data[index++] : (byte)0x00;
                    ret.speed = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties2.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.state);
                ret.Add(command.speed);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STOP_TEMP_GET
        {
            public const byte ID = 0x0B;
            public static implicit operator CHIMNEY_FAN_STOP_TEMP_GET(byte[] data)
            {
                CHIMNEY_FAN_STOP_TEMP_GET ret = new CHIMNEY_FAN_STOP_TEMP_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STOP_TEMP_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STOP_TEMP_REPORT
        {
            public const byte ID = 0x0C;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_STOP_TEMP_REPORT(byte[] data)
            {
                CHIMNEY_FAN_STOP_TEMP_REPORT ret = new CHIMNEY_FAN_STOP_TEMP_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STOP_TEMP_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STOP_TEMP_SET
        {
            public const byte ID = 0x0A;
            public struct Tproperties1
            {
                private byte _value;
                public byte size
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte scale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte precision
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
            public IList<byte> value = new List<byte>();
            public static implicit operator CHIMNEY_FAN_STOP_TEMP_SET(byte[] data)
            {
                CHIMNEY_FAN_STOP_TEMP_SET ret = new CHIMNEY_FAN_STOP_TEMP_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STOP_TEMP_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.value != null)
                {
                    foreach (var tmp in command.value)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STOP_TIME_GET
        {
            public const byte ID = 0x14;
            public static implicit operator CHIMNEY_FAN_STOP_TIME_GET(byte[] data)
            {
                CHIMNEY_FAN_STOP_TIME_GET ret = new CHIMNEY_FAN_STOP_TIME_GET();
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STOP_TIME_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STOP_TIME_REPORT
        {
            public const byte ID = 0x15;
            public byte time;
            public static implicit operator CHIMNEY_FAN_STOP_TIME_REPORT(byte[] data)
            {
                CHIMNEY_FAN_STOP_TIME_REPORT ret = new CHIMNEY_FAN_STOP_TIME_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.time = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STOP_TIME_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.time);
                return ret.ToArray();
            }
        }
        public class CHIMNEY_FAN_STOP_TIME_SET
        {
            public const byte ID = 0x13;
            public byte time;
            public static implicit operator CHIMNEY_FAN_STOP_TIME_SET(byte[] data)
            {
                CHIMNEY_FAN_STOP_TIME_SET ret = new CHIMNEY_FAN_STOP_TIME_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.time = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CHIMNEY_FAN_STOP_TIME_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CHIMNEY_FAN.ID);
                ret.Add(ID);
                ret.Add(command.time);
                return ret.ToArray();
            }
        }
    }
}

