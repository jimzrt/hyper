using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZIP_GATEWAY
    {
        public const byte ID = 0x5F;
        public const byte VERSION = 1;
        public class GATEWAY_MODE_SET
        {
            public const byte ID = 0x01;
            public byte mode;
            public static implicit operator GATEWAY_MODE_SET(byte[] data)
            {
                GATEWAY_MODE_SET ret = new GATEWAY_MODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_MODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class GATEWAY_MODE_GET
        {
            public const byte ID = 0x02;
            public static implicit operator GATEWAY_MODE_GET(byte[] data)
            {
                GATEWAY_MODE_GET ret = new GATEWAY_MODE_GET();
                return ret;
            }
            public static implicit operator byte[](GATEWAY_MODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class GATEWAY_MODE_REPORT
        {
            public const byte ID = 0x03;
            public byte mode;
            public static implicit operator GATEWAY_MODE_REPORT(byte[] data)
            {
                GATEWAY_MODE_REPORT ret = new GATEWAY_MODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_MODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                return ret.ToArray();
            }
        }
        public class GATEWAY_PEER_SET
        {
            public const byte ID = 0x04;
            public byte peerProfile;
            public byte[] ipv6Address = new byte[16];
            public byte[] port = new byte[2];
            public struct Tproperties1
            {
                private byte _value;
                public byte peerNameLength
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
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
            public IList<byte> peerName = new List<byte>();
            public static implicit operator GATEWAY_PEER_SET(byte[] data)
            {
                GATEWAY_PEER_SET ret = new GATEWAY_PEER_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.peerProfile = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ipv6Address = new byte[]
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
                    ret.port = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.peerName = new List<byte>();
                    for (int i = 0; i < ret.properties1.peerNameLength; i++)
                    {
                        ret.peerName.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_PEER_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                ret.Add(command.peerProfile);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.port[0]);
                ret.Add(command.port[1]);
                ret.Add(command.properties1);
                if (command.peerName != null)
                {
                    foreach (var tmp in command.peerName)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class GATEWAY_PEER_GET
        {
            public const byte ID = 0x05;
            public byte peerProfile;
            public static implicit operator GATEWAY_PEER_GET(byte[] data)
            {
                GATEWAY_PEER_GET ret = new GATEWAY_PEER_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.peerProfile = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_PEER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                ret.Add(command.peerProfile);
                return ret.ToArray();
            }
        }
        public class GATEWAY_PEER_REPORT
        {
            public const byte ID = 0x06;
            public byte peerProfile;
            public byte peerCount;
            public byte[] ipv6Address = new byte[16];
            public byte[] port = new byte[2];
            public struct Tproperties1
            {
                private byte _value;
                public byte peerNameLength
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
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
            public IList<byte> peerName = new List<byte>();
            public static implicit operator GATEWAY_PEER_REPORT(byte[] data)
            {
                GATEWAY_PEER_REPORT ret = new GATEWAY_PEER_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.peerProfile = data.Length > index ? data[index++] : (byte)0x00;
                    ret.peerCount = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ipv6Address = new byte[]
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
                    ret.port = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.peerName = new List<byte>();
                    for (int i = 0; i < ret.properties1.peerNameLength; i++)
                    {
                        ret.peerName.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_PEER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                ret.Add(command.peerProfile);
                ret.Add(command.peerCount);
                if (command.ipv6Address != null)
                {
                    foreach (var tmp in command.ipv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.port[0]);
                ret.Add(command.port[1]);
                ret.Add(command.properties1);
                if (command.peerName != null)
                {
                    foreach (var tmp in command.peerName)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class GATEWAY_LOCK_SET
        {
            public const byte ID = 0x07;
            public struct Tproperties1
            {
                private byte _value;
                public byte mlock
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte show
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
            public static implicit operator GATEWAY_LOCK_SET(byte[] data)
            {
                GATEWAY_LOCK_SET ret = new GATEWAY_LOCK_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GATEWAY_LOCK_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class UNSOLICITED_DESTINATION_SET
        {
            public const byte ID = 0x08;
            public byte[] unsolicitedIpv6Destination = new byte[16];
            public byte[] unsolicitedDestinationPort = new byte[2];
            public static implicit operator UNSOLICITED_DESTINATION_SET(byte[] data)
            {
                UNSOLICITED_DESTINATION_SET ret = new UNSOLICITED_DESTINATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.unsolicitedIpv6Destination = new byte[]
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
                    ret.unsolicitedDestinationPort = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](UNSOLICITED_DESTINATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                if (command.unsolicitedIpv6Destination != null)
                {
                    foreach (var tmp in command.unsolicitedIpv6Destination)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.unsolicitedDestinationPort[0]);
                ret.Add(command.unsolicitedDestinationPort[1]);
                return ret.ToArray();
            }
        }
        public class UNSOLICITED_DESTINATION_GET
        {
            public const byte ID = 0x09;
            public static implicit operator UNSOLICITED_DESTINATION_GET(byte[] data)
            {
                UNSOLICITED_DESTINATION_GET ret = new UNSOLICITED_DESTINATION_GET();
                return ret;
            }
            public static implicit operator byte[](UNSOLICITED_DESTINATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class UNSOLICITED_DESTINATION_REPORT
        {
            public const byte ID = 0x0A;
            public byte[] unsolicitedIpv6Destination = new byte[16];
            public byte[] unsolicitedDestinationPort = new byte[2];
            public static implicit operator UNSOLICITED_DESTINATION_REPORT(byte[] data)
            {
                UNSOLICITED_DESTINATION_REPORT ret = new UNSOLICITED_DESTINATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.unsolicitedIpv6Destination = new byte[]
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
                    ret.unsolicitedDestinationPort = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](UNSOLICITED_DESTINATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                if (command.unsolicitedIpv6Destination != null)
                {
                    foreach (var tmp in command.unsolicitedIpv6Destination)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.unsolicitedDestinationPort[0]);
                ret.Add(command.unsolicitedDestinationPort[1]);
                return ret.ToArray();
            }
        }
        public class COMMAND_APPLICATION_NODE_INFO_SET
        {
            public const byte ID = 0x0B;
            public IList<byte> nonSecureCommandClass = new List<byte>();
            private byte[] securityScheme0Mark = { 0xF1, 0x00 };
            public IList<byte> securityScheme0CommandClass = new List<byte>();
            public static implicit operator COMMAND_APPLICATION_NODE_INFO_SET(byte[] data)
            {
                COMMAND_APPLICATION_NODE_INFO_SET ret = new COMMAND_APPLICATION_NODE_INFO_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.nonSecureCommandClass = new List<byte>();
                    while (data.Length - 0 > index && (data.Length - 2 < index || data[index + 0] != ret.securityScheme0Mark[0] || data[index + 1] != ret.securityScheme0Mark[1]))
                    {
                        ret.nonSecureCommandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    index++; //Marker
                    index++; //Marker
                    ret.securityScheme0CommandClass = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.securityScheme0CommandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_APPLICATION_NODE_INFO_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                if (command.nonSecureCommandClass != null)
                {
                    foreach (var tmp in command.nonSecureCommandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.securityScheme0Mark[0]);
                ret.Add(command.securityScheme0Mark[1]);
                if (command.securityScheme0CommandClass != null)
                {
                    foreach (var tmp in command.securityScheme0CommandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class COMMAND_APPLICATION_NODE_INFO_GET
        {
            public const byte ID = 0x0C;
            public static implicit operator COMMAND_APPLICATION_NODE_INFO_GET(byte[] data)
            {
                COMMAND_APPLICATION_NODE_INFO_GET ret = new COMMAND_APPLICATION_NODE_INFO_GET();
                return ret;
            }
            public static implicit operator byte[](COMMAND_APPLICATION_NODE_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class COMMAND_APPLICATION_NODE_INFO_REPORT
        {
            public const byte ID = 0x0D;
            public IList<byte> nonSecureCommandClass = new List<byte>();
            private byte[] securityScheme0Mark = { 0xF1, 0x00 };
            public IList<byte> securityScheme0CommandClass = new List<byte>();
            public static implicit operator COMMAND_APPLICATION_NODE_INFO_REPORT(byte[] data)
            {
                COMMAND_APPLICATION_NODE_INFO_REPORT ret = new COMMAND_APPLICATION_NODE_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.nonSecureCommandClass = new List<byte>();
                    while (data.Length - 0 > index && (data.Length - 2 < index || data[index + 0] != ret.securityScheme0Mark[0] || data[index + 1] != ret.securityScheme0Mark[1]))
                    {
                        ret.nonSecureCommandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    index++; //Marker
                    index++; //Marker
                    ret.securityScheme0CommandClass = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.securityScheme0CommandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_APPLICATION_NODE_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZIP_GATEWAY.ID);
                ret.Add(ID);
                if (command.nonSecureCommandClass != null)
                {
                    foreach (var tmp in command.nonSecureCommandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.securityScheme0Mark[0]);
                ret.Add(command.securityScheme0Mark[1]);
                if (command.securityScheme0CommandClass != null)
                {
                    foreach (var tmp in command.securityScheme0CommandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

