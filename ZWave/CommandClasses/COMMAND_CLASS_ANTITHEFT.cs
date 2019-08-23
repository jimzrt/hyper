using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ANTITHEFT
    {
        public const byte ID = 0x5D;
        public const byte VERSION = 1;
        public class ANTITHEFT_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfMagicCodeBytes
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte enable
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
            public IList<byte> magicCode = new List<byte>();
            public byte[] manufacturerId = new byte[2];
            public byte antiTheftHintNumberBytes;
            public IList<byte> antiTheftHintByte = new List<byte>();
            public static implicit operator ANTITHEFT_SET(byte[] data)
            {
                ANTITHEFT_SET ret = new ANTITHEFT_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.magicCode = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfMagicCodeBytes; i++)
                    {
                        ret.magicCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.manufacturerId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.antiTheftHintNumberBytes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.antiTheftHintByte = new List<byte>();
                    for (int i = 0; i < ret.antiTheftHintNumberBytes; i++)
                    {
                        ret.antiTheftHintByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ANTITHEFT_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ANTITHEFT.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.magicCode != null)
                {
                    foreach (var tmp in command.magicCode)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.manufacturerId[0]);
                ret.Add(command.manufacturerId[1]);
                ret.Add(command.antiTheftHintNumberBytes);
                if (command.antiTheftHintByte != null)
                {
                    foreach (var tmp in command.antiTheftHintByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ANTITHEFT_GET
        {
            public const byte ID = 0x02;
            public static implicit operator ANTITHEFT_GET(byte[] data)
            {
                ANTITHEFT_GET ret = new ANTITHEFT_GET();
                return ret;
            }
            public static implicit operator byte[](ANTITHEFT_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ANTITHEFT.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ANTITHEFT_REPORT
        {
            public const byte ID = 0x03;
            public byte antiTheftProtectionStatus;
            public byte[] manufacturerId = new byte[2];
            public byte antiTheftHintNumberBytes;
            public IList<byte> antiTheftHintByte = new List<byte>();
            public static implicit operator ANTITHEFT_REPORT(byte[] data)
            {
                ANTITHEFT_REPORT ret = new ANTITHEFT_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.antiTheftProtectionStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.manufacturerId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.antiTheftHintNumberBytes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.antiTheftHintByte = new List<byte>();
                    for (int i = 0; i < ret.antiTheftHintNumberBytes; i++)
                    {
                        ret.antiTheftHintByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ANTITHEFT_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ANTITHEFT.ID);
                ret.Add(ID);
                ret.Add(command.antiTheftProtectionStatus);
                ret.Add(command.manufacturerId[0]);
                ret.Add(command.manufacturerId[1]);
                ret.Add(command.antiTheftHintNumberBytes);
                if (command.antiTheftHintByte != null)
                {
                    foreach (var tmp in command.antiTheftHintByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

