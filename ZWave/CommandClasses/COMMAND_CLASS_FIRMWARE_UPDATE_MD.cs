using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_FIRMWARE_UPDATE_MD
    {
        public const byte ID = 0x7A;
        public const byte VERSION = 1;
        public class FIRMWARE_MD_GET
        {
            public const byte ID = 0x01;
            public static implicit operator FIRMWARE_MD_GET(byte[] data)
            {
                FIRMWARE_MD_GET ret = new FIRMWARE_MD_GET();
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_MD_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class FIRMWARE_MD_REPORT
        {
            public const byte ID = 0x02;
            public byte[] manufacturerId = new byte[2];
            public byte[] firmwareId = new byte[2];
            public byte[] checksum = new byte[2];
            public static implicit operator FIRMWARE_MD_REPORT(byte[] data)
            {
                FIRMWARE_MD_REPORT ret = new FIRMWARE_MD_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.manufacturerId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.firmwareId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.checksum = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_MD_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                ret.Add(command.manufacturerId[0]);
                ret.Add(command.manufacturerId[1]);
                ret.Add(command.firmwareId[0]);
                ret.Add(command.firmwareId[1]);
                ret.Add(command.checksum[0]);
                ret.Add(command.checksum[1]);
                return ret.ToArray();
            }
        }
        public class FIRMWARE_UPDATE_MD_GET
        {
            public const byte ID = 0x05;
            public byte numberOfReports;
            public struct Tproperties1
            {
                private byte _value;
                public byte reportNumber1
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte zero
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
            public byte reportNumber2;
            public static implicit operator FIRMWARE_UPDATE_MD_GET(byte[] data)
            {
                FIRMWARE_UPDATE_MD_GET ret = new FIRMWARE_UPDATE_MD_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfReports = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reportNumber2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_UPDATE_MD_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                ret.Add(command.numberOfReports);
                ret.Add(command.properties1);
                ret.Add(command.reportNumber2);
                return ret.ToArray();
            }
        }
        public class FIRMWARE_UPDATE_MD_REPORT
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte reportNumber1
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte last
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
            public byte reportNumber2;
            public IList<byte> data = new List<byte>();
            public static implicit operator FIRMWARE_UPDATE_MD_REPORT(byte[] data)
            {
                FIRMWARE_UPDATE_MD_REPORT ret = new FIRMWARE_UPDATE_MD_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.reportNumber2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.data = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.data.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_UPDATE_MD_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.reportNumber2);
                if (command.data != null)
                {
                    foreach (var tmp in command.data)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class FIRMWARE_UPDATE_MD_REQUEST_GET
        {
            public const byte ID = 0x03;
            public byte[] manufacturerId = new byte[2];
            public byte[] firmwareId = new byte[2];
            public byte[] checksum = new byte[2];
            public static implicit operator FIRMWARE_UPDATE_MD_REQUEST_GET(byte[] data)
            {
                FIRMWARE_UPDATE_MD_REQUEST_GET ret = new FIRMWARE_UPDATE_MD_REQUEST_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.manufacturerId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.firmwareId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.checksum = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_UPDATE_MD_REQUEST_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                ret.Add(command.manufacturerId[0]);
                ret.Add(command.manufacturerId[1]);
                ret.Add(command.firmwareId[0]);
                ret.Add(command.firmwareId[1]);
                ret.Add(command.checksum[0]);
                ret.Add(command.checksum[1]);
                return ret.ToArray();
            }
        }
        public class FIRMWARE_UPDATE_MD_REQUEST_REPORT
        {
            public const byte ID = 0x04;
            public byte status;
            public static implicit operator FIRMWARE_UPDATE_MD_REQUEST_REPORT(byte[] data)
            {
                FIRMWARE_UPDATE_MD_REQUEST_REPORT ret = new FIRMWARE_UPDATE_MD_REQUEST_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_UPDATE_MD_REQUEST_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class FIRMWARE_UPDATE_MD_STATUS_REPORT
        {
            public const byte ID = 0x07;
            public byte status;
            public static implicit operator FIRMWARE_UPDATE_MD_STATUS_REPORT(byte[] data)
            {
                FIRMWARE_UPDATE_MD_STATUS_REPORT ret = new FIRMWARE_UPDATE_MD_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](FIRMWARE_UPDATE_MD_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID);
                ret.Add(ID);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
    }
}

