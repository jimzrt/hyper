using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_METER
    {
        public const byte ID = 0x32;
        public const byte VERSION = 1;
        public class METER_GET
        {
            public const byte ID = 0x01;
            public static implicit operator METER_GET(byte[] data)
            {
                METER_GET ret = new METER_GET();
                return ret;
            }
            public static implicit operator byte[](METER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_REPORT
        {
            public const byte ID = 0x02;
            public byte meterType;
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
            public IList<byte> meterValue = new List<byte>();
            public static implicit operator METER_REPORT(byte[] data)
            {
                METER_REPORT ret = new METER_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.meterType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.meterValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.meterValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER.ID);
                ret.Add(ID);
                ret.Add(command.meterType);
                ret.Add(command.properties1);
                if (command.meterValue != null)
                {
                    foreach (var tmp in command.meterValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

