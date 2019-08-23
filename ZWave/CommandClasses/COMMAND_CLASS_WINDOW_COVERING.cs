using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_WINDOW_COVERING
    {
        public const byte ID = 0x6A;
        public const byte VERSION = 1;
        public class WINDOW_COVERING_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator WINDOW_COVERING_SUPPORTED_GET(byte[] data)
            {
                WINDOW_COVERING_SUPPORTED_GET ret = new WINDOW_COVERING_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class WINDOW_COVERING_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfParameterMaskBytes
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
            public IList<byte> parameterMask = new List<byte>();
            public static implicit operator WINDOW_COVERING_SUPPORTED_REPORT(byte[] data)
            {
                WINDOW_COVERING_SUPPORTED_REPORT ret = new WINDOW_COVERING_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.parameterMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfParameterMaskBytes; i++)
                    {
                        ret.parameterMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.parameterMask != null)
                {
                    foreach (var tmp in command.parameterMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class WINDOW_COVERING_GET
        {
            public const byte ID = 0x03;
            public byte parameterId;
            public static implicit operator WINDOW_COVERING_GET(byte[] data)
            {
                WINDOW_COVERING_GET ret = new WINDOW_COVERING_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.parameterId);
                return ret.ToArray();
            }
        }
        public class WINDOW_COVERING_REPORT
        {
            public const byte ID = 0x04;
            public byte parameterId;
            public byte currentValue;
            public byte targetValue;
            public byte duration;
            public static implicit operator WINDOW_COVERING_REPORT(byte[] data)
            {
                WINDOW_COVERING_REPORT ret = new WINDOW_COVERING_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.currentValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.targetValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.parameterId);
                ret.Add(command.currentValue);
                ret.Add(command.targetValue);
                ret.Add(command.duration);
                return ret.ToArray();
            }
        }
        public class WINDOW_COVERING_SET
        {
            public const byte ID = 0x05;
            public struct Tproperties1
            {
                private byte _value;
                public byte parameterCount
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
            public class TVG1
            {
                public byte parameterId;
                public byte value;
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public byte duration;
            public static implicit operator WINDOW_COVERING_SET(byte[] data)
            {
                WINDOW_COVERING_SET ret = new WINDOW_COVERING_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.properties1.parameterCount; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.parameterId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.value = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg1.Add(tmp);
                    }
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.parameterId);
                        ret.Add(item.value);
                    }
                }
                ret.Add(command.duration);
                return ret.ToArray();
            }
        }
        public class WINDOW_COVERING_START_LEVEL_CHANGE
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte res1
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte upDown
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte res2
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
            public byte parameterId;
            public byte duration;
            public static implicit operator WINDOW_COVERING_START_LEVEL_CHANGE(byte[] data)
            {
                WINDOW_COVERING_START_LEVEL_CHANGE ret = new WINDOW_COVERING_START_LEVEL_CHANGE();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.parameterId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_START_LEVEL_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.parameterId);
                ret.Add(command.duration);
                return ret.ToArray();
            }
        }
        public class WINDOW_COVERING_STOP_LEVEL_CHANGE
        {
            public const byte ID = 0x07;
            public byte parameterId;
            public static implicit operator WINDOW_COVERING_STOP_LEVEL_CHANGE(byte[] data)
            {
                WINDOW_COVERING_STOP_LEVEL_CHANGE ret = new WINDOW_COVERING_STOP_LEVEL_CHANGE();
                if (data != null)
                {
                    int index = 2;
                    ret.parameterId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WINDOW_COVERING_STOP_LEVEL_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WINDOW_COVERING.ID);
                ret.Add(ID);
                ret.Add(command.parameterId);
                return ret.ToArray();
            }
        }
    }
}

