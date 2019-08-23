using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_TIME_V2
    {
        public const byte ID = 0x8A;
        public const byte VERSION = 2;
        public class DATE_GET
        {
            public const byte ID = 0x03;
            public static implicit operator DATE_GET(byte[] data)
            {
                DATE_GET ret = new DATE_GET();
                return ret;
            }
            public static implicit operator byte[](DATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DATE_REPORT
        {
            public const byte ID = 0x04;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public static implicit operator DATE_REPORT(byte[] data)
            {
                DATE_REPORT ret = new DATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.year = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.month = data.Length > index ? data[index++] : (byte)0x00;
                    ret.day = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                return ret.ToArray();
            }
        }
        public class TIME_GET
        {
            public const byte ID = 0x01;
            public static implicit operator TIME_GET(byte[] data)
            {
                TIME_GET ret = new TIME_GET();
                return ret;
            }
            public static implicit operator byte[](TIME_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class TIME_OFFSET_GET
        {
            public const byte ID = 0x06;
            public static implicit operator TIME_OFFSET_GET(byte[] data)
            {
                TIME_OFFSET_GET ret = new TIME_OFFSET_GET();
                return ret;
            }
            public static implicit operator byte[](TIME_OFFSET_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class TIME_OFFSET_REPORT
        {
            public const byte ID = 0x07;
            public struct Tproperties1
            {
                private byte _value;
                public byte hourTzo
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signTzo
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
            public byte minuteTzo;
            public struct Tproperties2
            {
                private byte _value;
                public byte minuteOffsetDst
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signOffsetDst
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public byte monthStartDst;
            public byte dayStartDst;
            public byte hourStartDst;
            public byte monthEndDst;
            public byte dayEndDst;
            public byte hourEndDst;
            public static implicit operator TIME_OFFSET_REPORT(byte[] data)
            {
                TIME_OFFSET_REPORT ret = new TIME_OFFSET_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteTzo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.monthStartDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dayStartDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hourStartDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.monthEndDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dayEndDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hourEndDst = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TIME_OFFSET_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minuteTzo);
                ret.Add(command.properties2);
                ret.Add(command.monthStartDst);
                ret.Add(command.dayStartDst);
                ret.Add(command.hourStartDst);
                ret.Add(command.monthEndDst);
                ret.Add(command.dayEndDst);
                ret.Add(command.hourEndDst);
                return ret.ToArray();
            }
        }
        public class TIME_OFFSET_SET
        {
            public const byte ID = 0x05;
            public struct Tproperties1
            {
                private byte _value;
                public byte hourTzo
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signTzo
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
            public byte minuteTzo;
            public struct Tproperties2
            {
                private byte _value;
                public byte minuteOffsetDst
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte signOffsetDst
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public byte monthStartDst;
            public byte dayStartDst;
            public byte hourStartDst;
            public byte monthEndDst;
            public byte dayEndDst;
            public byte hourEndDst;
            public static implicit operator TIME_OFFSET_SET(byte[] data)
            {
                TIME_OFFSET_SET ret = new TIME_OFFSET_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteTzo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.monthStartDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dayStartDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hourStartDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.monthEndDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dayEndDst = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hourEndDst = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TIME_OFFSET_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minuteTzo);
                ret.Add(command.properties2);
                ret.Add(command.monthStartDst);
                ret.Add(command.dayStartDst);
                ret.Add(command.hourStartDst);
                ret.Add(command.monthEndDst);
                ret.Add(command.dayEndDst);
                ret.Add(command.hourEndDst);
                return ret.ToArray();
            }
        }
        public class TIME_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte hourLocalTime
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x03); }
                    set { _value &= 0xFF - 0x60; _value += (byte)(value << 5 & 0x60); }
                }
                public byte rtcFailure
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
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public static implicit operator TIME_REPORT(byte[] data)
            {
                TIME_REPORT ret = new TIME_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.secondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TIME_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                return ret.ToArray();
            }
        }
    }
}

