using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SECURITY
    {
        public const byte ID = 0x98;
        public const byte VERSION = 1;
        public class NETWORK_KEY_SET
        {
            public const byte ID = 0x06;
            public IList<byte> networkKeyByte = new List<byte>();
            public static implicit operator NETWORK_KEY_SET(byte[] data)
            {
                NETWORK_KEY_SET ret = new NETWORK_KEY_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.networkKeyByte = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.networkKeyByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](NETWORK_KEY_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                if (command.networkKeyByte != null)
                {
                    foreach (var tmp in command.networkKeyByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class NETWORK_KEY_VERIFY
        {
            public const byte ID = 0x07;
            public static implicit operator NETWORK_KEY_VERIFY(byte[] data)
            {
                NETWORK_KEY_VERIFY ret = new NETWORK_KEY_VERIFY();
                return ret;
            }
            public static implicit operator byte[](NETWORK_KEY_VERIFY command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SECURITY_COMMANDS_SUPPORTED_GET
        {
            public const byte ID = 0x02;
            public static implicit operator SECURITY_COMMANDS_SUPPORTED_GET(byte[] data)
            {
                SECURITY_COMMANDS_SUPPORTED_GET ret = new SECURITY_COMMANDS_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](SECURITY_COMMANDS_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SECURITY_COMMANDS_SUPPORTED_REPORT
        {
            public const byte ID = 0x03;
            public byte reportsToFollow;
            public IList<byte> commandClassSupport = new List<byte>();
            private byte[] commandClassMark = { 0xEF };
            public IList<byte> commandClassControl = new List<byte>();
            public static implicit operator SECURITY_COMMANDS_SUPPORTED_REPORT(byte[] data)
            {
                SECURITY_COMMANDS_SUPPORTED_REPORT ret = new SECURITY_COMMANDS_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClassSupport = new List<byte>();
                    while (data.Length - 0 > index && (data.Length - 1 < index || data[index + 0] != ret.commandClassMark[0]))
                    {
                        ret.commandClassSupport.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    index++; //Marker
                    ret.commandClassControl = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.commandClassControl.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_COMMANDS_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                if (command.commandClassSupport != null)
                {
                    foreach (var tmp in command.commandClassSupport)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.commandClassMark[0]);
                if (command.commandClassControl != null)
                {
                    foreach (var tmp in command.commandClassControl)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_MESSAGE_ENCAPSULATION
        {
            public const byte ID = 0x81;
            public byte[] initializationVectorByte = new byte[8];
            public struct Tproperties1
            {
                private byte _value;
                public byte sequenceCounter
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte sequenced
                {
                    get { return (byte)(_value >> 4 & 0x01); }
                    set { _value &= 0xFF - 0x10; _value += (byte)(value << 4 & 0x10); }
                }
                public byte secondFrame
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
            public IList<byte> commandByte = new List<byte>();
            public byte receiversNonceIdentifier;
            public byte[] messageAuthenticationCodeByte = new byte[8];
            public static implicit operator SECURITY_MESSAGE_ENCAPSULATION(byte[] data)
            {
                SECURITY_MESSAGE_ENCAPSULATION ret = new SECURITY_MESSAGE_ENCAPSULATION();
                if (data != null)
                {
                    int index = 2;
                    ret.initializationVectorByte = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandByte = new List<byte>();
                    while (data.Length - 9 > index)
                    {
                        ret.commandByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.receiversNonceIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.messageAuthenticationCodeByte = new byte[]
                    {
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
            public static implicit operator byte[](SECURITY_MESSAGE_ENCAPSULATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                if (command.initializationVectorByte != null)
                {
                    foreach (var tmp in command.initializationVectorByte)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties1);
                if (command.commandByte != null)
                {
                    foreach (var tmp in command.commandByte)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.receiversNonceIdentifier);
                if (command.messageAuthenticationCodeByte != null)
                {
                    foreach (var tmp in command.messageAuthenticationCodeByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET
        {
            public const byte ID = 0xC1;
            public byte[] initializationVectorByte = new byte[8];
            public struct Tproperties1
            {
                private byte _value;
                public byte sequenceCounter
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte sequenced
                {
                    get { return (byte)(_value >> 4 & 0x01); }
                    set { _value &= 0xFF - 0x10; _value += (byte)(value << 4 & 0x10); }
                }
                public byte secondFrame
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
            public IList<byte> commandByte = new List<byte>();
            public byte receiversNonceIdentifier;
            public byte[] messageAuthenticationCodeByte = new byte[8];
            public static implicit operator SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET(byte[] data)
            {
                SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET ret = new SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.initializationVectorByte = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandByte = new List<byte>();
                    while (data.Length - 9 > index)
                    {
                        ret.commandByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.receiversNonceIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.messageAuthenticationCodeByte = new byte[]
                    {
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
            public static implicit operator byte[](SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                if (command.initializationVectorByte != null)
                {
                    foreach (var tmp in command.initializationVectorByte)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties1);
                if (command.commandByte != null)
                {
                    foreach (var tmp in command.commandByte)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.receiversNonceIdentifier);
                if (command.messageAuthenticationCodeByte != null)
                {
                    foreach (var tmp in command.messageAuthenticationCodeByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_NONCE_GET
        {
            public const byte ID = 0x40;
            public static implicit operator SECURITY_NONCE_GET(byte[] data)
            {
                SECURITY_NONCE_GET ret = new SECURITY_NONCE_GET();
                return ret;
            }
            public static implicit operator byte[](SECURITY_NONCE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SECURITY_NONCE_REPORT
        {
            public const byte ID = 0x80;
            public IList<byte> nonceByte = new List<byte>();
            public static implicit operator SECURITY_NONCE_REPORT(byte[] data)
            {
                SECURITY_NONCE_REPORT ret = new SECURITY_NONCE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.nonceByte = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.nonceByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_NONCE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                if (command.nonceByte != null)
                {
                    foreach (var tmp in command.nonceByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_SCHEME_GET
        {
            public const byte ID = 0x04;
            public byte supportedSecuritySchemes;
            public static implicit operator SECURITY_SCHEME_GET(byte[] data)
            {
                SECURITY_SCHEME_GET ret = new SECURITY_SCHEME_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedSecuritySchemes = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_SCHEME_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                ret.Add(command.supportedSecuritySchemes);
                return ret.ToArray();
            }
        }
        public class SECURITY_SCHEME_INHERIT
        {
            public const byte ID = 0x08;
            public byte supportedSecuritySchemes;
            public static implicit operator SECURITY_SCHEME_INHERIT(byte[] data)
            {
                SECURITY_SCHEME_INHERIT ret = new SECURITY_SCHEME_INHERIT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedSecuritySchemes = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_SCHEME_INHERIT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                ret.Add(command.supportedSecuritySchemes);
                return ret.ToArray();
            }
        }
        public class SECURITY_SCHEME_REPORT
        {
            public const byte ID = 0x05;
            public byte supportedSecuritySchemes;
            public static implicit operator SECURITY_SCHEME_REPORT(byte[] data)
            {
                SECURITY_SCHEME_REPORT ret = new SECURITY_SCHEME_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedSecuritySchemes = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_SCHEME_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY.ID);
                ret.Add(ID);
                ret.Add(command.supportedSecuritySchemes);
                return ret.ToArray();
            }
        }
    }
}

