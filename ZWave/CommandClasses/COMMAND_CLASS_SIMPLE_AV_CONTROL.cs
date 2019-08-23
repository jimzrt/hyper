using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SIMPLE_AV_CONTROL
    {
        public const byte ID = 0x94;
        public const byte VERSION = 1;
        public class SIMPLE_AV_CONTROL_GET
        {
            public const byte ID = 0x02;
            public static implicit operator SIMPLE_AV_CONTROL_GET(byte[] data)
            {
                SIMPLE_AV_CONTROL_GET ret = new SIMPLE_AV_CONTROL_GET();
                return ret;
            }
            public static implicit operator byte[](SIMPLE_AV_CONTROL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SIMPLE_AV_CONTROL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SIMPLE_AV_CONTROL_REPORT
        {
            public const byte ID = 0x03;
            public byte numberOfReports;
            public static implicit operator SIMPLE_AV_CONTROL_REPORT(byte[] data)
            {
                SIMPLE_AV_CONTROL_REPORT ret = new SIMPLE_AV_CONTROL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfReports = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SIMPLE_AV_CONTROL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SIMPLE_AV_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.numberOfReports);
                return ret.ToArray();
            }
        }
        public class SIMPLE_AV_CONTROL_SET
        {
            public const byte ID = 0x01;
            public byte sequenceNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte keyAttributes
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
            public byte[] reserved2 = new byte[2];
            public class TVG
            {
                public byte[] command = new byte[2];
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator SIMPLE_AV_CONTROL_SET(byte[] data)
            {
                SIMPLE_AV_CONTROL_SET ret = new SIMPLE_AV_CONTROL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reserved2 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.vg = new List<TVG>();
                    while (data.Length - 0 > index)
                    {
                        TVG tmp = new TVG();
                        tmp.command = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SIMPLE_AV_CONTROL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SIMPLE_AV_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.properties1);
                ret.Add(command.reserved2[0]);
                ret.Add(command.reserved2[1]);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.command[0]);
                        ret.Add(item.command[1]);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SIMPLE_AV_CONTROL_SUPPORTED_GET
        {
            public const byte ID = 0x04;
            public byte reportNo;
            public static implicit operator SIMPLE_AV_CONTROL_SUPPORTED_GET(byte[] data)
            {
                SIMPLE_AV_CONTROL_SUPPORTED_GET ret = new SIMPLE_AV_CONTROL_SUPPORTED_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.reportNo = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SIMPLE_AV_CONTROL_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SIMPLE_AV_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.reportNo);
                return ret.ToArray();
            }
        }
        public class SIMPLE_AV_CONTROL_SUPPORTED_REPORT
        {
            public const byte ID = 0x05;
            public byte reportNo;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator SIMPLE_AV_CONTROL_SUPPORTED_REPORT(byte[] data)
            {
                SIMPLE_AV_CONTROL_SUPPORTED_REPORT ret = new SIMPLE_AV_CONTROL_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportNo = data.Length > index ? data[index++] : (byte)0x00;
                    ret.bitMask = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SIMPLE_AV_CONTROL_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SIMPLE_AV_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.reportNo);
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

