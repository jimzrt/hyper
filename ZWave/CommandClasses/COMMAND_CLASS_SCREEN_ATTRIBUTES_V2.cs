using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SCREEN_ATTRIBUTES_V2
    {
        public const byte ID = 0x93;
        public const byte VERSION = 2;
        public class SCREEN_ATTRIBUTES_GET
        {
            public const byte ID = 0x01;
            public static implicit operator SCREEN_ATTRIBUTES_GET(byte[] data)
            {
                SCREEN_ATTRIBUTES_GET ret = new SCREEN_ATTRIBUTES_GET();
                return ret;
            }
            public static implicit operator byte[](SCREEN_ATTRIBUTES_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCREEN_ATTRIBUTES_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SCREEN_ATTRIBUTES_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfLines
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte escapeSequence
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte reserved
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
            public byte numberOfCharactersPerLine;
            public byte sizeOfLineBuffer;
            public byte numericalPresentationOfACharacter;
            public byte screenTimeout;
            public static implicit operator SCREEN_ATTRIBUTES_REPORT(byte[] data)
            {
                SCREEN_ATTRIBUTES_REPORT ret = new SCREEN_ATTRIBUTES_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfCharactersPerLine = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sizeOfLineBuffer = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numericalPresentationOfACharacter = data.Length > index ? data[index++] : (byte)0x00;
                    ret.screenTimeout = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCREEN_ATTRIBUTES_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCREEN_ATTRIBUTES_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.numberOfCharactersPerLine);
                ret.Add(command.sizeOfLineBuffer);
                ret.Add(command.numericalPresentationOfACharacter);
                ret.Add(command.screenTimeout);
                return ret.ToArray();
            }
        }
        public class SCREEN_ATTRIBUTES_REPORT_LEGACY
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfLines
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte escapeSequence
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte reserved
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
            public byte numberOfCharactersPerLine;
            public byte sizeOfLineBuffer;
            public byte numericalPresentationOfACharacter;
            public byte screenTimeout;
            public static implicit operator SCREEN_ATTRIBUTES_REPORT_LEGACY(byte[] data)
            {
                SCREEN_ATTRIBUTES_REPORT_LEGACY ret = new SCREEN_ATTRIBUTES_REPORT_LEGACY();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfCharactersPerLine = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sizeOfLineBuffer = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numericalPresentationOfACharacter = data.Length > index ? data[index++] : (byte)0x00;
                    ret.screenTimeout = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCREEN_ATTRIBUTES_REPORT_LEGACY command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCREEN_ATTRIBUTES_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.numberOfCharactersPerLine);
                ret.Add(command.sizeOfLineBuffer);
                ret.Add(command.numericalPresentationOfACharacter);
                ret.Add(command.screenTimeout);
                return ret.ToArray();
            }
        }
    }
}

