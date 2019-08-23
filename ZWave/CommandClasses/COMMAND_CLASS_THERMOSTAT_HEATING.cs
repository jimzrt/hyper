using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_THERMOSTAT_HEATING
    {
        public const byte ID = 0x38;
        public const byte VERSION = 1;
        public class THERMOSTAT_HEATING_STATUS_REPORT
        {
            public const byte ID = 0x0D;
            public byte status;
            public static implicit operator THERMOSTAT_HEATING_STATUS_REPORT(byte[] data)
            {
                THERMOSTAT_HEATING_STATUS_REPORT ret = new THERMOSTAT_HEATING_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_MODE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator THERMOSTAT_HEATING_MODE_GET(byte[] data)
            {
                THERMOSTAT_HEATING_MODE_GET ret = new THERMOSTAT_HEATING_MODE_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_MODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_MODE_REPORT
        {
            public const byte ID = 0x03;
            public byte mode;
            public static implicit operator THERMOSTAT_HEATING_MODE_REPORT(byte[] data)
            {
                THERMOSTAT_HEATING_MODE_REPORT ret = new THERMOSTAT_HEATING_MODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_MODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_MODE_SET
        {
            public const byte ID = 0x01;
            public byte mode;
            public static implicit operator THERMOSTAT_HEATING_MODE_SET(byte[] data)
            {
                THERMOSTAT_HEATING_MODE_SET ret = new THERMOSTAT_HEATING_MODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_MODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_RELAY_STATUS_GET
        {
            public const byte ID = 0x09;
            public static implicit operator THERMOSTAT_HEATING_RELAY_STATUS_GET(byte[] data)
            {
                THERMOSTAT_HEATING_RELAY_STATUS_GET ret = new THERMOSTAT_HEATING_RELAY_STATUS_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_RELAY_STATUS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_RELAY_STATUS_REPORT
        {
            public const byte ID = 0x0A;
            public byte relayStatus;
            public static implicit operator THERMOSTAT_HEATING_RELAY_STATUS_REPORT(byte[] data)
            {
                THERMOSTAT_HEATING_RELAY_STATUS_REPORT ret = new THERMOSTAT_HEATING_RELAY_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.relayStatus = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_RELAY_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.relayStatus);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_SETPOINT_GET
        {
            public const byte ID = 0x05;
            public byte setpointNr;
            public static implicit operator THERMOSTAT_HEATING_SETPOINT_GET(byte[] data)
            {
                THERMOSTAT_HEATING_SETPOINT_GET ret = new THERMOSTAT_HEATING_SETPOINT_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.setpointNr = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_SETPOINT_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.setpointNr);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_SETPOINT_REPORT
        {
            public const byte ID = 0x06;
            public byte setpointNr;
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
            public static implicit operator THERMOSTAT_HEATING_SETPOINT_REPORT(byte[] data)
            {
                THERMOSTAT_HEATING_SETPOINT_REPORT ret = new THERMOSTAT_HEATING_SETPOINT_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.setpointNr = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_SETPOINT_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.setpointNr);
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
        public class THERMOSTAT_HEATING_SETPOINT_SET
        {
            public const byte ID = 0x04;
            public byte setpointNr;
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
            public static implicit operator THERMOSTAT_HEATING_SETPOINT_SET(byte[] data)
            {
                THERMOSTAT_HEATING_SETPOINT_SET ret = new THERMOSTAT_HEATING_SETPOINT_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.setpointNr = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_SETPOINT_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.setpointNr);
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
        public class THERMOSTAT_HEATING_STATUS_GET
        {
            public const byte ID = 0x0C;
            public static implicit operator THERMOSTAT_HEATING_STATUS_GET(byte[] data)
            {
                THERMOSTAT_HEATING_STATUS_GET ret = new THERMOSTAT_HEATING_STATUS_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_STATUS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_STATUS_SET
        {
            public const byte ID = 0x0B;
            public byte status;
            public static implicit operator THERMOSTAT_HEATING_STATUS_SET(byte[] data)
            {
                THERMOSTAT_HEATING_STATUS_SET ret = new THERMOSTAT_HEATING_STATUS_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_STATUS_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_HEATING_TIMED_OFF_SET
        {
            public const byte ID = 0x11;
            public byte minutes;
            public byte hours;
            public static implicit operator THERMOSTAT_HEATING_TIMED_OFF_SET(byte[] data)
            {
                THERMOSTAT_HEATING_TIMED_OFF_SET ret = new THERMOSTAT_HEATING_TIMED_OFF_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.minutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hours = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_HEATING_TIMED_OFF_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_HEATING.ID);
                ret.Add(ID);
                ret.Add(command.minutes);
                ret.Add(command.hours);
                return ret.ToArray();
            }
        }
    }
}

