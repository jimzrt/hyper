using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SECURITY_2
    {
        public const byte ID = 0x9F;
        public const byte VERSION = 1;
        public class SECURITY_2_NONCE_GET
        {
            public const byte ID = 0x01;
            public byte sequenceNumber;
            public static implicit operator SECURITY_2_NONCE_GET(byte[] data)
            {
                SECURITY_2_NONCE_GET ret = new SECURITY_2_NONCE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_NONCE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                return ret.ToArray();
            }
        }
        public class SECURITY_2_NONCE_REPORT
        {
            public const byte ID = 0x02;
            public byte sequenceNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte sos
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte mos
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
            public IList<byte> receiversEntropyInput = new List<byte>();
            public static implicit operator SECURITY_2_NONCE_REPORT(byte[] data)
            {
                SECURITY_2_NONCE_REPORT ret = new SECURITY_2_NONCE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.receiversEntropyInput = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.receiversEntropyInput.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_NONCE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.properties1);
                if (command.receiversEntropyInput != null)
                {
                    foreach (var tmp in command.receiversEntropyInput)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_2_MESSAGE_ENCAPSULATION
        {
            public const byte ID = 0x03;
            public byte sequenceNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte extension
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte encryptedExtension
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
            public class TVG1
            {
                public byte extensionLength;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte type
                    {
                        get { return (byte)(_value >> 0 & 0x3F); }
                        set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                    }
                    public byte critical
                    {
                        get { return (byte)(_value >> 6 & 0x01); }
                        set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                    }
                    public byte moreToFollow
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
                public IList<byte> extension = new List<byte>();
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public IList<byte> ccmCiphertextObject = new List<byte>();
            public static implicit operator SECURITY_2_MESSAGE_ENCAPSULATION(byte[] data)
            {
                SECURITY_2_MESSAGE_ENCAPSULATION ret = new SECURITY_2_MESSAGE_ENCAPSULATION();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    if (ret.properties1.extension > 0)
                    {
                        ret.vg1 = new List<TVG1>();
                        while (data.Length - 0 > index)
                        {
                            TVG1 tmp = new TVG1();
                            tmp.extensionLength = data.Length > index ? data[index++] : (byte)0x00;
                            tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                            tmp.extension = new List<byte>();
                            for (int i = 0; i < tmp.extensionLength - 2; i++)
                            {
                                tmp.extension.Add(data.Length > index ? data[index++] : (byte)0x00);
                            }
                            ret.vg1.Add(tmp);
                            if (tmp.properties1.moreToFollow == 0)
                                break;
                        }
                    }
                    ret.ccmCiphertextObject = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.ccmCiphertextObject.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_MESSAGE_ENCAPSULATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.extensionLength);
                        ret.Add(item.properties1);
                        if (item.extension != null)
                        {
                            foreach (var tmp in item.extension)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                if (command.ccmCiphertextObject != null)
                {
                    foreach (var tmp in command.ccmCiphertextObject)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class KEX_GET
        {
            public const byte ID = 0x04;
            public static implicit operator KEX_GET(byte[] data)
            {
                KEX_GET ret = new KEX_GET();
                return ret;
            }
            public static implicit operator byte[](KEX_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class KEX_REPORT
        {
            public const byte ID = 0x05;
            public struct Tproperties1
            {
                private byte _value;
                public byte echo
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte requestCsa
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
            public byte supportedKexSchemes;
            public byte supportedEcdhProfiles;
            public byte requestedKeys;
            public static implicit operator KEX_REPORT(byte[] data)
            {
                KEX_REPORT ret = new KEX_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedKexSchemes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedEcdhProfiles = data.Length > index ? data[index++] : (byte)0x00;
                    ret.requestedKeys = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](KEX_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.supportedKexSchemes);
                ret.Add(command.supportedEcdhProfiles);
                ret.Add(command.requestedKeys);
                return ret.ToArray();
            }
        }
        public class KEX_SET
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte echo
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte requestCsa
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
            public byte selectedKexScheme;
            public byte selectedEcdhProfile;
            public byte grantedKeys;
            public static implicit operator KEX_SET(byte[] data)
            {
                KEX_SET ret = new KEX_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.selectedKexScheme = data.Length > index ? data[index++] : (byte)0x00;
                    ret.selectedEcdhProfile = data.Length > index ? data[index++] : (byte)0x00;
                    ret.grantedKeys = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](KEX_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.selectedKexScheme);
                ret.Add(command.selectedEcdhProfile);
                ret.Add(command.grantedKeys);
                return ret.ToArray();
            }
        }
        public class KEX_FAIL
        {
            public const byte ID = 0x07;
            public byte kexFailType;
            public static implicit operator KEX_FAIL(byte[] data)
            {
                KEX_FAIL ret = new KEX_FAIL();
                if (data != null)
                {
                    int index = 2;
                    ret.kexFailType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](KEX_FAIL command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.kexFailType);
                return ret.ToArray();
            }
        }
        public class PUBLIC_KEY_REPORT
        {
            public const byte ID = 0x08;
            public struct Tproperties1
            {
                private byte _value;
                public byte includingNode
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
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
            public IList<byte> ecdhPublicKey = new List<byte>();
            public static implicit operator PUBLIC_KEY_REPORT(byte[] data)
            {
                PUBLIC_KEY_REPORT ret = new PUBLIC_KEY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.ecdhPublicKey = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.ecdhPublicKey.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](PUBLIC_KEY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.ecdhPublicKey != null)
                {
                    foreach (var tmp in command.ecdhPublicKey)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_2_NETWORK_KEY_GET
        {
            public const byte ID = 0x09;
            public byte requestedKey;
            public static implicit operator SECURITY_2_NETWORK_KEY_GET(byte[] data)
            {
                SECURITY_2_NETWORK_KEY_GET ret = new SECURITY_2_NETWORK_KEY_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.requestedKey = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_NETWORK_KEY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.requestedKey);
                return ret.ToArray();
            }
        }
        public class SECURITY_2_NETWORK_KEY_REPORT
        {
            public const byte ID = 0x0A;
            public byte grantedKey;
            public byte[] networkKey = new byte[16];
            public static implicit operator SECURITY_2_NETWORK_KEY_REPORT(byte[] data)
            {
                SECURITY_2_NETWORK_KEY_REPORT ret = new SECURITY_2_NETWORK_KEY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.grantedKey = data.Length > index ? data[index++] : (byte)0x00;
                    ret.networkKey = new byte[]
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
            public static implicit operator byte[](SECURITY_2_NETWORK_KEY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.grantedKey);
                if (command.networkKey != null)
                {
                    foreach (var tmp in command.networkKey)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SECURITY_2_NETWORK_KEY_VERIFY
        {
            public const byte ID = 0x0B;
            public static implicit operator SECURITY_2_NETWORK_KEY_VERIFY(byte[] data)
            {
                SECURITY_2_NETWORK_KEY_VERIFY ret = new SECURITY_2_NETWORK_KEY_VERIFY();
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_NETWORK_KEY_VERIFY command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SECURITY_2_TRANSFER_END
        {
            public const byte ID = 0x0C;
            public struct Tproperties1
            {
                private byte _value;
                public byte keyRequestComplete
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte keyVerified
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
            public static implicit operator SECURITY_2_TRANSFER_END(byte[] data)
            {
                SECURITY_2_TRANSFER_END ret = new SECURITY_2_TRANSFER_END();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_TRANSFER_END command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class SECURITY_2_COMMANDS_SUPPORTED_GET
        {
            public const byte ID = 0x0D;
            public static implicit operator SECURITY_2_COMMANDS_SUPPORTED_GET(byte[] data)
            {
                SECURITY_2_COMMANDS_SUPPORTED_GET ret = new SECURITY_2_COMMANDS_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_COMMANDS_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SECURITY_2_COMMANDS_SUPPORTED_REPORT
        {
            public const byte ID = 0x0E;
            public IList<byte> commandClass = new List<byte>();
            public static implicit operator SECURITY_2_COMMANDS_SUPPORTED_REPORT(byte[] data)
            {
                SECURITY_2_COMMANDS_SUPPORTED_REPORT ret = new SECURITY_2_COMMANDS_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.commandClass = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.commandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_2_COMMANDS_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                ret.Add(ID);
                if (command.commandClass != null)
                {
                    foreach (var tmp in command.commandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

