using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4
    {
        public const byte ID = 0x44;
        public const byte VERSION = 4;
        public class THERMOSTAT_FAN_MODE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator THERMOSTAT_FAN_MODE_GET(byte[] data)
            {
                THERMOSTAT_FAN_MODE_GET ret = new THERMOSTAT_FAN_MODE_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_FAN_MODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_FAN_MODE_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte fanMode
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte off
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
            public static implicit operator THERMOSTAT_FAN_MODE_REPORT(byte[] data)
            {
                THERMOSTAT_FAN_MODE_REPORT ret = new THERMOSTAT_FAN_MODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_FAN_MODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_FAN_MODE_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte fanMode
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte off
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
            public static implicit operator THERMOSTAT_FAN_MODE_SET(byte[] data)
            {
                THERMOSTAT_FAN_MODE_SET ret = new THERMOSTAT_FAN_MODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_FAN_MODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_FAN_MODE_SUPPORTED_GET
        {
            public const byte ID = 0x04;
            public static implicit operator THERMOSTAT_FAN_MODE_SUPPORTED_GET(byte[] data)
            {
                THERMOSTAT_FAN_MODE_SUPPORTED_GET ret = new THERMOSTAT_FAN_MODE_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_FAN_MODE_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_FAN_MODE_SUPPORTED_REPORT
        {
            public const byte ID = 0x05;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator THERMOSTAT_FAN_MODE_SUPPORTED_REPORT(byte[] data)
            {
                THERMOSTAT_FAN_MODE_SUPPORTED_REPORT ret = new THERMOSTAT_FAN_MODE_SUPPORTED_REPORT();
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
            public static implicit operator byte[](THERMOSTAT_FAN_MODE_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_FAN_MODE_V4.ID);
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
    }
}

