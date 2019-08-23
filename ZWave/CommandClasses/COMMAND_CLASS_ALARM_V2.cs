using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ALARM_V2
    {
        public const byte ID = 0x71;
        public const byte VERSION = 2;
        public class ALARM_GET
        {
            public const byte ID = 0x04;
            public byte alarmType;
            public byte zwaveAlarmType;
            public static implicit operator ALARM_GET(byte[] data)
            {
                ALARM_GET ret = new ALARM_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.alarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ALARM_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM_V2.ID);
                ret.Add(ID);
                ret.Add(command.alarmType);
                ret.Add(command.zwaveAlarmType);
                return ret.ToArray();
            }
        }
        public class ALARM_REPORT
        {
            public const byte ID = 0x05;
            public byte alarmType;
            public byte alarmLevel;
            public byte zensorNetSourceNodeId;
            public byte zwaveAlarmStatus;
            public byte zwaveAlarmType;
            public byte zwaveAlarmEvent;
            public byte numberOfEventParameters;
            public IList<byte> eventParameter = new List<byte>();
            public static implicit operator ALARM_REPORT(byte[] data)
            {
                ALARM_REPORT ret = new ALARM_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.alarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.alarmLevel = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zensorNetSourceNodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmEvent = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfEventParameters = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventParameter = new List<byte>();
                    for (int i = 0; i < ret.numberOfEventParameters; i++)
                    {
                        ret.eventParameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ALARM_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM_V2.ID);
                ret.Add(ID);
                ret.Add(command.alarmType);
                ret.Add(command.alarmLevel);
                ret.Add(command.zensorNetSourceNodeId);
                ret.Add(command.zwaveAlarmStatus);
                ret.Add(command.zwaveAlarmType);
                ret.Add(command.zwaveAlarmEvent);
                ret.Add(command.numberOfEventParameters);
                if (command.eventParameter != null)
                {
                    foreach (var tmp in command.eventParameter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ALARM_SET
        {
            public const byte ID = 0x06;
            public byte zwaveAlarmType;
            public byte zwaveAlarmStatus;
            public static implicit operator ALARM_SET(byte[] data)
            {
                ALARM_SET ret = new ALARM_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.zwaveAlarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmStatus = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ALARM_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM_V2.ID);
                ret.Add(ID);
                ret.Add(command.zwaveAlarmType);
                ret.Add(command.zwaveAlarmStatus);
                return ret.ToArray();
            }
        }
        public class ALARM_TYPE_SUPPORTED_GET
        {
            public const byte ID = 0x07;
            public static implicit operator ALARM_TYPE_SUPPORTED_GET(byte[] data)
            {
                ALARM_TYPE_SUPPORTED_GET ret = new ALARM_TYPE_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](ALARM_TYPE_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ALARM_TYPE_SUPPORTED_REPORT
        {
            public const byte ID = 0x08;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfBitMasks
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x03); }
                    set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                }
                public byte v1Alarm
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
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator ALARM_TYPE_SUPPORTED_REPORT(byte[] data)
            {
                ALARM_TYPE_SUPPORTED_REPORT ret = new ALARM_TYPE_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.bitMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfBitMasks; i++)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ALARM_TYPE_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
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
    }
}

