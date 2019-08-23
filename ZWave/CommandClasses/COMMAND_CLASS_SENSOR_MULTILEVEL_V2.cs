using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SENSOR_MULTILEVEL_V2
    {
        public const byte ID = 0x31;
        public const byte VERSION = 2;
        public class SENSOR_MULTILEVEL_GET
        {
            public const byte ID = 0x04;
            public static implicit operator SENSOR_MULTILEVEL_GET(byte[] data)
            {
                SENSOR_MULTILEVEL_GET ret = new SENSOR_MULTILEVEL_GET();
                return ret;
            }
            public static implicit operator byte[](SENSOR_MULTILEVEL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_MULTILEVEL_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SENSOR_MULTILEVEL_REPORT
        {
            public const byte ID = 0x05;
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
            public IList<byte> sensorValue = new List<byte>();
            public static implicit operator SENSOR_MULTILEVEL_REPORT(byte[] data)
            {
                SENSOR_MULTILEVEL_REPORT ret = new SENSOR_MULTILEVEL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.sensorValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_MULTILEVEL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_MULTILEVEL_V2.ID);
                ret.Add(ID);
                ret.Add(command.sensorType);
                ret.Add(command.properties1);
                if (command.sensorValue != null)
                {
                    foreach (var tmp in command.sensorValue)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

