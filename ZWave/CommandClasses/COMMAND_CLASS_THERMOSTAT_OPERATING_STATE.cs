using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_THERMOSTAT_OPERATING_STATE
    {
        public const byte ID = 0x42;
        public const byte VERSION = 1;
        public class THERMOSTAT_OPERATING_STATE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator THERMOSTAT_OPERATING_STATE_GET(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_GET ret = new THERMOSTAT_OPERATING_STATE_GET();
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class THERMOSTAT_OPERATING_STATE_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte operatingState
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
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
            public static implicit operator THERMOSTAT_OPERATING_STATE_REPORT(byte[] data)
            {
                THERMOSTAT_OPERATING_STATE_REPORT ret = new THERMOSTAT_OPERATING_STATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](THERMOSTAT_OPERATING_STATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_THERMOSTAT_OPERATING_STATE.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
    }
}

