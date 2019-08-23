using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_IP_CONFIGURATION
    {
        public const byte ID = 0x9A;
        public const byte VERSION = 1;
        public class IP_CONFIGURATION_GET
        {
            public const byte ID = 0x02;
            public static implicit operator IP_CONFIGURATION_GET(byte[] data)
            {
                IP_CONFIGURATION_GET ret = new IP_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](IP_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_CONFIGURATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class IP_CONFIGURATION_RELEASE
        {
            public const byte ID = 0x04;
            public static implicit operator IP_CONFIGURATION_RELEASE(byte[] data)
            {
                IP_CONFIGURATION_RELEASE ret = new IP_CONFIGURATION_RELEASE();
                return ret;
            }
            public static implicit operator byte[](IP_CONFIGURATION_RELEASE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_CONFIGURATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class IP_CONFIGURATION_RENEW
        {
            public const byte ID = 0x05;
            public static implicit operator IP_CONFIGURATION_RENEW(byte[] data)
            {
                IP_CONFIGURATION_RENEW ret = new IP_CONFIGURATION_RENEW();
                return ret;
            }
            public static implicit operator byte[](IP_CONFIGURATION_RENEW command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_CONFIGURATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class IP_CONFIGURATION_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte autoDns
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte autoIp
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
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
            public byte[] ipAddress = new byte[4];
            public byte[] subnetMask = new byte[4];
            public byte[] gateway = new byte[4];
            public byte[] dns1 = new byte[4];
            public byte[] dns2 = new byte[4];
            public byte[] leasetime = new byte[4];
            public static implicit operator IP_CONFIGURATION_REPORT(byte[] data)
            {
                IP_CONFIGURATION_REPORT ret = new IP_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ipAddress = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.subnetMask = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.gateway = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.dns1 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.dns2 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.leasetime = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](IP_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.ipAddress[0]);
                ret.Add(command.ipAddress[1]);
                ret.Add(command.ipAddress[2]);
                ret.Add(command.ipAddress[3]);
                ret.Add(command.subnetMask[0]);
                ret.Add(command.subnetMask[1]);
                ret.Add(command.subnetMask[2]);
                ret.Add(command.subnetMask[3]);
                ret.Add(command.gateway[0]);
                ret.Add(command.gateway[1]);
                ret.Add(command.gateway[2]);
                ret.Add(command.gateway[3]);
                ret.Add(command.dns1[0]);
                ret.Add(command.dns1[1]);
                ret.Add(command.dns1[2]);
                ret.Add(command.dns1[3]);
                ret.Add(command.dns2[0]);
                ret.Add(command.dns2[1]);
                ret.Add(command.dns2[2]);
                ret.Add(command.dns2[3]);
                ret.Add(command.leasetime[0]);
                ret.Add(command.leasetime[1]);
                ret.Add(command.leasetime[2]);
                ret.Add(command.leasetime[3]);
                return ret.ToArray();
            }
        }
        public class IP_CONFIGURATION_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte autoDns
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte autoIp
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
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
            public byte[] ipAddress = new byte[4];
            public byte[] subnetMask = new byte[4];
            public byte[] gateway = new byte[4];
            public byte[] dns1 = new byte[4];
            public byte[] dns2 = new byte[4];
            public static implicit operator IP_CONFIGURATION_SET(byte[] data)
            {
                IP_CONFIGURATION_SET ret = new IP_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ipAddress = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.subnetMask = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.gateway = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.dns1 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.dns2 = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](IP_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IP_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.ipAddress[0]);
                ret.Add(command.ipAddress[1]);
                ret.Add(command.ipAddress[2]);
                ret.Add(command.ipAddress[3]);
                ret.Add(command.subnetMask[0]);
                ret.Add(command.subnetMask[1]);
                ret.Add(command.subnetMask[2]);
                ret.Add(command.subnetMask[3]);
                ret.Add(command.gateway[0]);
                ret.Add(command.gateway[1]);
                ret.Add(command.gateway[2]);
                ret.Add(command.gateway[3]);
                ret.Add(command.dns1[0]);
                ret.Add(command.dns1[1]);
                ret.Add(command.dns1[2]);
                ret.Add(command.dns1[3]);
                ret.Add(command.dns2[0]);
                ret.Add(command.dns2[1]);
                ret.Add(command.dns2[2]);
                ret.Add(command.dns2[3]);
                return ret.ToArray();
            }
        }
    }
}

