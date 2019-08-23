using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE
    {
        public const byte ID = 0x46;
        public const byte VERSION = 1;
        public class SCHEDULE_CHANGED_GET
        {
            public const byte ID = 0x04;
            public static implicit operator SCHEDULE_CHANGED_GET(byte[] data)
            {
                SCHEDULE_CHANGED_GET ret = new SCHEDULE_CHANGED_GET();
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_CHANGED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_CHANGED_REPORT
        {
            public const byte ID = 0x05;
            public byte changecounter;
            public static implicit operator SCHEDULE_CHANGED_REPORT(byte[] data)
            {
                SCHEDULE_CHANGED_REPORT ret = new SCHEDULE_CHANGED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.changecounter = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_CHANGED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.changecounter);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_GET
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte weekday
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public static implicit operator SCHEDULE_GET(byte[] data)
            {
                SCHEDULE_GET ret = new SCHEDULE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_OVERRIDE_GET
        {
            public const byte ID = 0x07;
            public static implicit operator SCHEDULE_OVERRIDE_GET(byte[] data)
            {
                SCHEDULE_OVERRIDE_GET ret = new SCHEDULE_OVERRIDE_GET();
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_OVERRIDE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_OVERRIDE_REPORT
        {
            public const byte ID = 0x08;
            public struct Tproperties1
            {
                private byte _value;
                public byte overrideType
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
            public byte overrideState;
            public static implicit operator SCHEDULE_OVERRIDE_REPORT(byte[] data)
            {
                SCHEDULE_OVERRIDE_REPORT ret = new SCHEDULE_OVERRIDE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.overrideState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_OVERRIDE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.overrideState);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_OVERRIDE_SET
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte overrideType
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
            public byte overrideState;
            public static implicit operator SCHEDULE_OVERRIDE_SET(byte[] data)
            {
                SCHEDULE_OVERRIDE_SET ret = new SCHEDULE_OVERRIDE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.overrideState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_OVERRIDE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.overrideState);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte weekday
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public byte[] switchpoint0 = new byte[3];
            public byte[] switchpoint1 = new byte[3];
            public byte[] switchpoint2 = new byte[3];
            public byte[] switchpoint3 = new byte[3];
            public byte[] switchpoint4 = new byte[3];
            public byte[] switchpoint5 = new byte[3];
            public byte[] switchpoint6 = new byte[3];
            public byte[] switchpoint7 = new byte[3];
            public byte[] switchpoint8 = new byte[3];
            public static implicit operator SCHEDULE_REPORT(byte[] data)
            {
                SCHEDULE_REPORT ret = new SCHEDULE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.switchpoint0 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint1 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint2 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint3 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint4 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint5 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint6 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint7 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint8 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.switchpoint0[0]);
                ret.Add(command.switchpoint0[1]);
                ret.Add(command.switchpoint0[2]);
                ret.Add(command.switchpoint1[0]);
                ret.Add(command.switchpoint1[1]);
                ret.Add(command.switchpoint1[2]);
                ret.Add(command.switchpoint2[0]);
                ret.Add(command.switchpoint2[1]);
                ret.Add(command.switchpoint2[2]);
                ret.Add(command.switchpoint3[0]);
                ret.Add(command.switchpoint3[1]);
                ret.Add(command.switchpoint3[2]);
                ret.Add(command.switchpoint4[0]);
                ret.Add(command.switchpoint4[1]);
                ret.Add(command.switchpoint4[2]);
                ret.Add(command.switchpoint5[0]);
                ret.Add(command.switchpoint5[1]);
                ret.Add(command.switchpoint5[2]);
                ret.Add(command.switchpoint6[0]);
                ret.Add(command.switchpoint6[1]);
                ret.Add(command.switchpoint6[2]);
                ret.Add(command.switchpoint7[0]);
                ret.Add(command.switchpoint7[1]);
                ret.Add(command.switchpoint7[2]);
                ret.Add(command.switchpoint8[0]);
                ret.Add(command.switchpoint8[1]);
                ret.Add(command.switchpoint8[2]);
                return ret.ToArray();
            }
        }
        public class SCHEDULE_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte weekday
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public byte[] switchpoint0 = new byte[3];
            public byte[] switchpoint1 = new byte[3];
            public byte[] switchpoint2 = new byte[3];
            public byte[] switchpoint3 = new byte[3];
            public byte[] switchpoint4 = new byte[3];
            public byte[] switchpoint5 = new byte[3];
            public byte[] switchpoint6 = new byte[3];
            public byte[] switchpoint7 = new byte[3];
            public byte[] switchpoint8 = new byte[3];
            public static implicit operator SCHEDULE_SET(byte[] data)
            {
                SCHEDULE_SET ret = new SCHEDULE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.switchpoint0 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint1 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint2 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint3 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint4 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint5 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint6 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint7 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.switchpoint8 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](SCHEDULE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.switchpoint0[0]);
                ret.Add(command.switchpoint0[1]);
                ret.Add(command.switchpoint0[2]);
                ret.Add(command.switchpoint1[0]);
                ret.Add(command.switchpoint1[1]);
                ret.Add(command.switchpoint1[2]);
                ret.Add(command.switchpoint2[0]);
                ret.Add(command.switchpoint2[1]);
                ret.Add(command.switchpoint2[2]);
                ret.Add(command.switchpoint3[0]);
                ret.Add(command.switchpoint3[1]);
                ret.Add(command.switchpoint3[2]);
                ret.Add(command.switchpoint4[0]);
                ret.Add(command.switchpoint4[1]);
                ret.Add(command.switchpoint4[2]);
                ret.Add(command.switchpoint5[0]);
                ret.Add(command.switchpoint5[1]);
                ret.Add(command.switchpoint5[2]);
                ret.Add(command.switchpoint6[0]);
                ret.Add(command.switchpoint6[1]);
                ret.Add(command.switchpoint6[2]);
                ret.Add(command.switchpoint7[0]);
                ret.Add(command.switchpoint7[1]);
                ret.Add(command.switchpoint7[2]);
                ret.Add(command.switchpoint8[0]);
                ret.Add(command.switchpoint8[1]);
                ret.Add(command.switchpoint8[2]);
                return ret.ToArray();
            }
        }
    }
}

