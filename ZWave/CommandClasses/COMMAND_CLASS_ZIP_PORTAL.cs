using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZIP_PORTAL
    {
        public const byte ID = 0x61;
        public const byte VERSION = 1;
        public class GATEWAY_CONFIGURATION_SET
        {
            public const byte ID = 0x01;
            public byte[] lanIpv6Address = new byte[16];
            public byte lanIpv6PrefixLength;
            public byte[] portalIpv6Prefix = new byte[16];
            public byte portalIpv6PrefixLength;
            public byte[] defaultGatewayIpv6Address = new byte[16];
            public byte[] panIpv6Prefix = new byte[16];
            public static implicit operator GATEWAY_CONFIGURATION_SET(byte[] data)
            {
                GATEWAY_CONFIGURATION_SET ret = new GATEWAY_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.lanIpv6Address = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.lanIpv6PrefixLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.portalIpv6Prefix = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.portalIpv6PrefixLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.defaultGatewayIpv6Address = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.panIpv6Prefix = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_PORTAL.ID);
                ret.Add(ID);
                if (command.lanIpv6Address != null)
                {
                    foreach (var tmp in command.lanIpv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.lanIpv6PrefixLength);
                if (command.portalIpv6Prefix != null)
                {
                    foreach (var tmp in command.portalIpv6Prefix)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.portalIpv6PrefixLength);
                if (command.defaultGatewayIpv6Address != null)
                {
                    foreach (var tmp in command.defaultGatewayIpv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                if (command.panIpv6Prefix != null)
                {
                    foreach (var tmp in command.panIpv6Prefix)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class GATEWAY_CONFIGURATION_STATUS
        {
            public const byte ID = 0x02;
            public byte status;
            public static implicit operator GATEWAY_CONFIGURATION_STATUS(byte[] data)
            {
                GATEWAY_CONFIGURATION_STATUS ret = new GATEWAY_CONFIGURATION_STATUS();
                if (data != null)
                {
                    int index = 2;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_CONFIGURATION_STATUS command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_PORTAL.ID);
                ret.Add(ID);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
        public class GATEWAY_CONFIGURATION_GET
        {
            public const byte ID = 0x03;
            public static implicit operator GATEWAY_CONFIGURATION_GET(byte[] data)
            {
                GATEWAY_CONFIGURATION_GET ret = new GATEWAY_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](GATEWAY_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_PORTAL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class GATEWAY_CONFIGURATION_REPORT
        {
            public const byte ID = 0x04;
            public byte[] lanIpv6Address = new byte[16];
            public byte lanIpv6PrefixLength;
            public byte[] portalIpv6Prefix = new byte[16];
            public byte portalIpv6PrefixLength;
            public byte[] defaultGatewayIpv6Address = new byte[16];
            public byte[] panIpv6Prefix = new byte[16];
            public static implicit operator GATEWAY_CONFIGURATION_REPORT(byte[] data)
            {
                GATEWAY_CONFIGURATION_REPORT ret = new GATEWAY_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.lanIpv6Address = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.lanIpv6PrefixLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.portalIpv6Prefix = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.portalIpv6PrefixLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.defaultGatewayIpv6Address = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.panIpv6Prefix = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_PORTAL.ID);
                ret.Add(ID);
                if (command.lanIpv6Address != null)
                {
                    foreach (var tmp in command.lanIpv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.lanIpv6PrefixLength);
                if (command.portalIpv6Prefix != null)
                {
                    foreach (var tmp in command.portalIpv6Prefix)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.portalIpv6PrefixLength);
                if (command.defaultGatewayIpv6Address != null)
                {
                    foreach (var tmp in command.defaultGatewayIpv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                if (command.panIpv6Prefix != null)
                {
                    foreach (var tmp in command.panIpv6Prefix)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

