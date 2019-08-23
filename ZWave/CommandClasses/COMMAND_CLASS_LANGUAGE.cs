using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_LANGUAGE
    {
        public const byte ID = 0x89;
        public const byte VERSION = 1;
        public class LANGUAGE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator LANGUAGE_GET(byte[] data)
            {
                LANGUAGE_GET ret = new LANGUAGE_GET();
                return ret;
            }
            public static implicit operator byte[](LANGUAGE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_LANGUAGE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class LANGUAGE_REPORT
        {
            public const byte ID = 0x03;
            public byte[] language = new byte[3];
            public byte[] country = new byte[2];
            public static implicit operator LANGUAGE_REPORT(byte[] data)
            {
                LANGUAGE_REPORT ret = new LANGUAGE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.language = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.country = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](LANGUAGE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_LANGUAGE.ID);
                ret.Add(ID);
                ret.Add(command.language[0]);
                ret.Add(command.language[1]);
                ret.Add(command.language[2]);
                ret.Add(command.country[0]);
                ret.Add(command.country[1]);
                return ret.ToArray();
            }
        }
        public class LANGUAGE_SET
        {
            public const byte ID = 0x01;
            public byte[] language = new byte[3];
            public byte[] country = new byte[2];
            public static implicit operator LANGUAGE_SET(byte[] data)
            {
                LANGUAGE_SET ret = new LANGUAGE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.language = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.country = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](LANGUAGE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_LANGUAGE.ID);
                ret.Add(ID);
                ret.Add(command.language[0]);
                ret.Add(command.language[1]);
                ret.Add(command.language[2]);
                ret.Add(command.country[0]);
                ret.Add(command.country[1]);
                return ret.ToArray();
            }
        }
    }
}

