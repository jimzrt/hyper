using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ENERGY_PRODUCTION
    {
        public const byte ID = 0x90;
        public const byte VERSION = 1;
        public class ENERGY_PRODUCTION_GET
        {
            public const byte ID = 0x02;
            public byte parameterNumber;
            public static implicit operator ENERGY_PRODUCTION_GET(byte[] data)
            {
                ENERGY_PRODUCTION_GET ret = new ENERGY_PRODUCTION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ENERGY_PRODUCTION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENERGY_PRODUCTION.ID);
                ret.Add(ID);
                ret.Add(command.parameterNumber);
                return ret.ToArray();
            }
        }
        public class ENERGY_PRODUCTION_REPORT
        {
            public const byte ID = 0x03;
            public byte parameterNumber;
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
            public static implicit operator ENERGY_PRODUCTION_REPORT(byte[] data)
            {
                ENERGY_PRODUCTION_REPORT ret = new ENERGY_PRODUCTION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    for (int i = 0; i < ret.properties1.size; i++)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ENERGY_PRODUCTION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENERGY_PRODUCTION.ID);
                ret.Add(ID);
                ret.Add(command.parameterNumber);
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
    }
}

