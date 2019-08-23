using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_THERMOSTAT_SETBACK
    {
        public const byte ID = 0x47;
        public const byte VERSION = 1;
        public class THERMOSTAT_SETBACK_GET
        {
            public const byte ID = 0x02;
            public static implicit operator THERMOSTAT_SETBACK_GET(byte[] data)
            {
                THERMOSTAT_SETBACK_GET ret = new THERMOSTAT_SETBACK_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_SETBACK_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_SETBACK.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_SETBACK_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte setbackType
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
            public byte setbackState;
            public static implicit operator THERMOSTAT_SETBACK_REPORT(byte[] data)
            {
                THERMOSTAT_SETBACK_REPORT ret = new THERMOSTAT_SETBACK_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.setbackState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_SETBACK_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_SETBACK.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.setbackState);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_SETBACK_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte setbackType
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
            public byte setbackState;
            public static implicit operator THERMOSTAT_SETBACK_SET(byte[] data)
            {
                THERMOSTAT_SETBACK_SET ret = new THERMOSTAT_SETBACK_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.setbackState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_SETBACK_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_SETBACK.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.setbackState);
                return ret.ToArray();
            }
        }
    }
}

