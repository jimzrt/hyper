using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_HRV_STATUS
    {
        public const byte ID = 0x37;
        public const byte VERSION = 1;
        public class HRV_STATUS_GET
        {
            public const byte ID = 0x01;
            public byte statusParameter;
            public static implicit operator HRV_STATUS_GET(byte[] data)
            {
                HRV_STATUS_GET ret = new HRV_STATUS_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.statusParameter = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](HRV_STATUS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_HRV_STATUS.ID);
                ret.Add(ID);
                ret.Add(command.statusParameter);
                return ret.ToArray();
            }
        }
        public class HRV_STATUS_REPORT
        {
            public const byte ID = 0x02;
            public byte statusParameter;
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
            public static implicit operator HRV_STATUS_REPORT(byte[] data)
            {
                HRV_STATUS_REPORT ret = new HRV_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.statusParameter = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.value = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](HRV_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_HRV_STATUS.ID);
                ret.Add(ID);
                ret.Add(command.statusParameter);
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
        public class HRV_STATUS_SUPPORTED_GET
        {
            public const byte ID = 0x03;
            public static implicit operator HRV_STATUS_SUPPORTED_GET(byte[] data)
            {
                HRV_STATUS_SUPPORTED_GET ret = new HRV_STATUS_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](HRV_STATUS_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_HRV_STATUS.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class HRV_STATUS_SUPPORTED_REPORT
        {
            public const byte ID = 0x04;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator HRV_STATUS_SUPPORTED_REPORT(byte[] data)
            {
                HRV_STATUS_SUPPORTED_REPORT ret = new HRV_STATUS_SUPPORTED_REPORT();
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
            public static implicit operator byte[](HRV_STATUS_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_HRV_STATUS.ID);
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

