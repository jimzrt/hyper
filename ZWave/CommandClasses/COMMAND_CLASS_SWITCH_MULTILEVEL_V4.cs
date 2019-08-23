using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SWITCH_MULTILEVEL_V4
    {
        public const byte ID = 0x26;
        public const byte VERSION = 4;
        public class SWITCH_MULTILEVEL_GET
        {
            public const byte ID = 0x02;
            public static implicit operator SWITCH_MULTILEVEL_GET(byte[] data)
            {
                SWITCH_MULTILEVEL_GET ret = new SWITCH_MULTILEVEL_GET();
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SWITCH_MULTILEVEL_REPORT
        {
            public const byte ID = 0x03;
            public byte currentValue;
            public byte targetValue;
            public byte duration;
            public static implicit operator SWITCH_MULTILEVEL_REPORT(byte[] data)
            {
                SWITCH_MULTILEVEL_REPORT ret = new SWITCH_MULTILEVEL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.currentValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.targetValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                ret.Add(command.currentValue);
                ret.Add(command.targetValue);
                ret.Add(command.duration);
                return ret.ToArray();
            }
        }
        public class SWITCH_MULTILEVEL_SET
        {
            public const byte ID = 0x01;
            public byte value;
            public byte dimmingDuration;
            public static implicit operator SWITCH_MULTILEVEL_SET(byte[] data)
            {
                SWITCH_MULTILEVEL_SET ret = new SWITCH_MULTILEVEL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.value = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                ret.Add(command.value);
                ret.Add(command.dimmingDuration);
                return ret.ToArray();
            }
        }
        public class SWITCH_MULTILEVEL_START_LEVEL_CHANGE
        {
            public const byte ID = 0x04;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte incDec
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte ignoreStartLevel
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte upDown
                {
                    get { return (byte)(_value >> 6 & 0x03); }
                    set { _value &= 0xFF - 0xC0; _value += (byte)(value << 6 & 0xC0); }
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
            public byte startLevel;
            public byte dimmingDuration;
            public byte stepSize;
            public static implicit operator SWITCH_MULTILEVEL_START_LEVEL_CHANGE(byte[] data)
            {
                SWITCH_MULTILEVEL_START_LEVEL_CHANGE ret = new SWITCH_MULTILEVEL_START_LEVEL_CHANGE();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.startLevel = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stepSize = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_START_LEVEL_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.startLevel);
                ret.Add(command.dimmingDuration);
                ret.Add(command.stepSize);
                return ret.ToArray();
            }
        }
        public class SWITCH_MULTILEVEL_STOP_LEVEL_CHANGE
        {
            public const byte ID = 0x05;
            public static implicit operator SWITCH_MULTILEVEL_STOP_LEVEL_CHANGE(byte[] data)
            {
                SWITCH_MULTILEVEL_STOP_LEVEL_CHANGE ret = new SWITCH_MULTILEVEL_STOP_LEVEL_CHANGE();
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_STOP_LEVEL_CHANGE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SWITCH_MULTILEVEL_SUPPORTED_GET
        {
            public const byte ID = 0x06;
            public static implicit operator SWITCH_MULTILEVEL_SUPPORTED_GET(byte[] data)
            {
                SWITCH_MULTILEVEL_SUPPORTED_GET ret = new SWITCH_MULTILEVEL_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SWITCH_MULTILEVEL_SUPPORTED_REPORT
        {
            public const byte ID = 0x07;
            public struct Tproperties1
            {
                private byte _value;
                public byte primarySwitchType
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved1
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
            public struct Tproperties2
            {
                private byte _value;
                public byte secondarySwitchType
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved2
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
            public static implicit operator SWITCH_MULTILEVEL_SUPPORTED_REPORT(byte[] data)
            {
                SWITCH_MULTILEVEL_SUPPORTED_REPORT ret = new SWITCH_MULTILEVEL_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SWITCH_MULTILEVEL_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SWITCH_MULTILEVEL_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
    }
}

