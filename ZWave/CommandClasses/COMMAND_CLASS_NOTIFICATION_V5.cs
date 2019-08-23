using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_NOTIFICATION_V5
    {
        public const byte ID = 0x71;
        public const byte VERSION = 5;
        public class NOTIFICATION_GET
        {
            public const byte ID = 0x04;
            public byte v1AlarmType;
            public byte notificationType;
            public byte mevent;
            public static implicit operator NOTIFICATION_GET(byte[] data)
            {
                NOTIFICATION_GET ret = new NOTIFICATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.v1AlarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.notificationType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mevent = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NOTIFICATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
                ret.Add(ID);
                ret.Add(command.v1AlarmType);
                ret.Add(command.notificationType);
                ret.Add(command.mevent);
                return ret.ToArray();
            }
        }
        public class NOTIFICATION_REPORT
        {
            public const byte ID = 0x05;
            public byte v1AlarmType;
            public byte v1AlarmLevel;
            public byte reserved;
            public byte notificationStatus;
            public byte notificationType;
            public byte mevent;
            public struct Tproperties1
            {
                private byte _value;
                public byte eventParametersLength
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved2
                {
                    get { return (byte)(_value >> 5 & 0x03); }
                    set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                }
                public byte sequence
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
            public IList<byte> eventParameter = new List<byte>();
            public byte sequenceNumber;
            public static implicit operator NOTIFICATION_REPORT(byte[] data)
            {
                NOTIFICATION_REPORT ret = new NOTIFICATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.v1AlarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.v1AlarmLevel = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved = data.Length > index ? data[index++] : (byte)0x00;
                    ret.notificationStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.notificationType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mevent = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventParameter = new List<byte>();
                    for (int i = 0; i < ret.properties1.eventParametersLength; i++)
                    {
                        ret.eventParameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NOTIFICATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
                ret.Add(ID);
                ret.Add(command.v1AlarmType);
                ret.Add(command.v1AlarmLevel);
                ret.Add(command.reserved);
                ret.Add(command.notificationStatus);
                ret.Add(command.notificationType);
                ret.Add(command.mevent);
                ret.Add(command.properties1);
                if (command.eventParameter != null)
                {
                    foreach (var tmp in command.eventParameter)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.sequenceNumber);
                return ret.ToArray();
            }
        }
        public class NOTIFICATION_SET
        {
            public const byte ID = 0x06;
            public byte notificationType;
            public byte notificationStatus;
            public static implicit operator NOTIFICATION_SET(byte[] data)
            {
                NOTIFICATION_SET ret = new NOTIFICATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.notificationType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.notificationStatus = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](NOTIFICATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
                ret.Add(ID);
                ret.Add(command.notificationType);
                ret.Add(command.notificationStatus);
                return ret.ToArray();
            }
        }
        public class NOTIFICATION_SUPPORTED_GET
        {
            public const byte ID = 0x07;
            public static implicit operator NOTIFICATION_SUPPORTED_GET(byte[] data)
            {
                NOTIFICATION_SUPPORTED_GET ret = new NOTIFICATION_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](NOTIFICATION_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class NOTIFICATION_SUPPORTED_REPORT
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
            public static implicit operator NOTIFICATION_SUPPORTED_REPORT(byte[] data)
            {
                NOTIFICATION_SUPPORTED_REPORT ret = new NOTIFICATION_SUPPORTED_REPORT();
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
            public static implicit operator byte[](NOTIFICATION_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
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
        public class EVENT_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public byte notificationType;
            public static implicit operator EVENT_SUPPORTED_GET(byte[] data)
            {
                EVENT_SUPPORTED_GET ret = new EVENT_SUPPORTED_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.notificationType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](EVENT_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
                ret.Add(ID);
                ret.Add(command.notificationType);
                return ret.ToArray();
            }
        }
        public class EVENT_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public byte notificationType;
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
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator EVENT_SUPPORTED_REPORT(byte[] data)
            {
                EVENT_SUPPORTED_REPORT ret = new EVENT_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.notificationType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.bitMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfBitMasks; i++)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](EVENT_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NOTIFICATION_V5.ID);
                ret.Add(ID);
                ret.Add(command.notificationType);
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

