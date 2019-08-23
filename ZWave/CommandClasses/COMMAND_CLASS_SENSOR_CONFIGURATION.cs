using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SENSOR_CONFIGURATION
    {
        public const byte ID = 0x9E;
        public const byte VERSION = 1;
        public class SENSOR_TRIGGER_LEVEL_GET
        {
            public const byte ID = 0x02;
            public static implicit operator SENSOR_TRIGGER_LEVEL_GET(byte[] data)
            {
                SENSOR_TRIGGER_LEVEL_GET ret = new SENSOR_TRIGGER_LEVEL_GET();
                return ret;
            }
            public static implicit operator byte[](SENSOR_TRIGGER_LEVEL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_CONFIGURATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SENSOR_TRIGGER_LEVEL_REPORT
        {
            public const byte ID = 0x03;
            public byte sensorType;
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
            public IList<byte> triggerValue = new List<byte>();
            public static implicit operator SENSOR_TRIGGER_LEVEL_REPORT(byte[] data)
            {
                SENSOR_TRIGGER_LEVEL_REPORT ret = new SENSOR_TRIGGER_LEVEL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.triggerValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.triggerValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_TRIGGER_LEVEL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.sensorType);
                ret.Add(command.properties1);
                if (command.triggerValue != null)
                {
                    foreach (var tmp in command.triggerValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SENSOR_TRIGGER_LEVEL_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte current
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte mdefault
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
            public byte sensorType;
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
            public IList<byte> triggerValue = new List<byte>();
            public static implicit operator SENSOR_TRIGGER_LEVEL_SET(byte[] data)
            {
                SENSOR_TRIGGER_LEVEL_SET ret = new SENSOR_TRIGGER_LEVEL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.triggerValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.size; i++)
                    {
                        ret.triggerValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_TRIGGER_LEVEL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.sensorType);
                ret.Add(command.properties2);
                if (command.triggerValue != null)
                {
                    foreach (var tmp in command.triggerValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

