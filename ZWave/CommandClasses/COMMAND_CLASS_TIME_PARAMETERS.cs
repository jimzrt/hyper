using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_TIME_PARAMETERS
    {
        public const byte ID = 0x8B;
        public const byte VERSION = 1;
        public class TIME_PARAMETERS_GET
        {
            public const byte ID = 0x02;
            public static implicit operator TIME_PARAMETERS_GET(byte[] data)
            {
                TIME_PARAMETERS_GET ret = new TIME_PARAMETERS_GET();
                return ret;
            }
            public static implicit operator byte[](TIME_PARAMETERS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_PARAMETERS.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class TIME_PARAMETERS_REPORT
        {
            public const byte ID = 0x03;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourUtc;
            public byte minuteUtc;
            public byte secondUtc;
            public static implicit operator TIME_PARAMETERS_REPORT(byte[] data)
            {
                TIME_PARAMETERS_REPORT ret = new TIME_PARAMETERS_REPORT();
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
                    ret.hourUtc = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteUtc = data.Length > index ? data[index++] : (byte)0x00;
                    ret.secondUtc = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TIME_PARAMETERS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_PARAMETERS.ID);
                ret.Add(ID);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourUtc);
                ret.Add(command.minuteUtc);
                ret.Add(command.secondUtc);
                return ret.ToArray();
            }
        }
        public class TIME_PARAMETERS_SET
        {
            public const byte ID = 0x01;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public byte hourUtc;
            public byte minuteUtc;
            public byte secondUtc;
            public static implicit operator TIME_PARAMETERS_SET(byte[] data)
            {
                TIME_PARAMETERS_SET ret = new TIME_PARAMETERS_SET();
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
                    ret.hourUtc = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteUtc = data.Length > index ? data[index++] : (byte)0x00;
                    ret.secondUtc = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](TIME_PARAMETERS_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_TIME_PARAMETERS.ID);
                ret.Add(ID);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.hourUtc);
                ret.Add(command.minuteUtc);
                ret.Add(command.secondUtc);
                return ret.ToArray();
            }
        }
    }
}

