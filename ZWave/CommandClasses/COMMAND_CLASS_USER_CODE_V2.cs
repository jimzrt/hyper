using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_USER_CODE_V2
    {
        public const byte ID = 0x63;
        public const byte VERSION = 2;
        public class USER_CODE_GET
        {
            public const byte ID = 0x02;
            public byte userIdentifier;
            public static implicit operator USER_CODE_GET(byte[] data)
            {
                USER_CODE_GET ret = new USER_CODE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                return ret.ToArray();
            }
        }
        public class USER_CODE_REPORT
        {
            public const byte ID = 0x03;
            public byte userIdentifier;
            public byte userIdStatus;
            public IList<byte> userCode = new List<byte>();
            public static implicit operator USER_CODE_REPORT(byte[] data)
            {
                USER_CODE_REPORT ret = new USER_CODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userCode = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.userIdStatus);
                if (command.userCode != null)
                {
                    foreach (var tmp in command.userCode)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class USER_CODE_SET
        {
            public const byte ID = 0x01;
            public byte userIdentifier;
            public byte userIdStatus;
            public IList<byte> userCode = new List<byte>();
            public static implicit operator USER_CODE_SET(byte[] data)
            {
                USER_CODE_SET ret = new USER_CODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userCode = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.userIdStatus);
                if (command.userCode != null)
                {
                    foreach (var tmp in command.userCode)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class USERS_NUMBER_GET
        {
            public const byte ID = 0x04;
            public static implicit operator USERS_NUMBER_GET(byte[] data)
            {
                USERS_NUMBER_GET ret = new USERS_NUMBER_GET();
                return ret;
            }
            public static implicit operator byte[](USERS_NUMBER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class USERS_NUMBER_REPORT
        {
            public const byte ID = 0x05;
            public byte supportedUsers;
            public byte[] extendedSupportedUsers = new byte[2];
            public static implicit operator USERS_NUMBER_REPORT(byte[] data)
            {
                USERS_NUMBER_REPORT ret = new USERS_NUMBER_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedUsers = data.Length > index ? data[index++] : (byte)0x00;
                    ret.extendedSupportedUsers = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](USERS_NUMBER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.supportedUsers);
                ret.Add(command.extendedSupportedUsers[0]);
                ret.Add(command.extendedSupportedUsers[1]);
                return ret.ToArray();
            }
        }
        public class EXTENDED_USER_CODE_SET
        {
            public const byte ID = 0x0B;
            public byte numberOfUserCodes;
            public class TVG1
            {
                public byte[] userIdentifier = new byte[2];
                public byte userIdStatus;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte userCodeLength
                    {
                        get { return (byte)(_value >> 0 & 0x0F); }
                        set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                    }
                    public byte reserved
                    {
                        get { return (byte)(_value >> 4 & 0x0F); }
                        set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
                public IList<byte> userCode = new List<byte>();
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator EXTENDED_USER_CODE_SET(byte[] data)
            {
                EXTENDED_USER_CODE_SET ret = new EXTENDED_USER_CODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfUserCodes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.numberOfUserCodes; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.userIdentifier = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        tmp.userIdStatus = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.userCode = new List<byte>();
                        for (int i = 0; i < tmp.properties1.userCodeLength; i++)
                        {
                            tmp.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](EXTENDED_USER_CODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.numberOfUserCodes);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.userIdentifier[0]);
                        ret.Add(item.userIdentifier[1]);
                        ret.Add(item.userIdStatus);
                        ret.Add(item.properties1);
                        if (item.userCode != null)
                        {
                            foreach (var tmp in item.userCode)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class EXTENDED_USER_CODE_GET
        {
            public const byte ID = 0x0C;
            public byte[] userIdentifier = new byte[2];
            public struct Tproperties1
            {
                private byte _value;
                public byte reportMore
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
            public static implicit operator EXTENDED_USER_CODE_GET(byte[] data)
            {
                EXTENDED_USER_CODE_GET ret = new EXTENDED_USER_CODE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](EXTENDED_USER_CODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier[0]);
                ret.Add(command.userIdentifier[1]);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class EXTENDED_USER_CODE_REPORT
        {
            public const byte ID = 0x0D;
            public byte numberOfUserCodes;
            public class TVG1
            {
                public byte[] userIdentifier = new byte[2];
                public byte userIdStatus;
                public struct Tproperties1
                {
                    private byte _value;
                    public byte userCodeLength
                    {
                        get { return (byte)(_value >> 0 & 0x0F); }
                        set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                    }
                    public byte reserved
                    {
                        get { return (byte)(_value >> 4 & 0x0F); }
                        set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
                public IList<byte> userCode = new List<byte>();
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public byte[] nextUserIdentifier = new byte[2];
            public static implicit operator EXTENDED_USER_CODE_REPORT(byte[] data)
            {
                EXTENDED_USER_CODE_REPORT ret = new EXTENDED_USER_CODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfUserCodes = data.Length > index ? data[index++] : (byte)0x00;
                    if (ret.numberOfUserCodes > 0)
                    {
                        ret.vg1 = new List<TVG1>();
                        while (data.Length - 2 > index)
                        {
                            TVG1 tmp = new TVG1();
                            tmp.userIdentifier = new byte[]
                            {
                                data.Length > index ? data[index++] : (byte)0x00,
                                data.Length > index ? data[index++] : (byte)0x00
                            };
                            tmp.userIdStatus = data.Length > index ? data[index++] : (byte)0x00;
                            tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                            tmp.userCode = new List<byte>();
                            for (int i = 0; i < tmp.properties1.userCodeLength; i++)
                            {
                                tmp.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                            }
                            ret.vg1.Add(tmp);
                        }
                    }
                    ret.nextUserIdentifier = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](EXTENDED_USER_CODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.numberOfUserCodes);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.userIdentifier[0]);
                        ret.Add(item.userIdentifier[1]);
                        ret.Add(item.userIdStatus);
                        ret.Add(item.properties1);
                        if (item.userCode != null)
                        {
                            foreach (var tmp in item.userCode)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                ret.Add(command.nextUserIdentifier[0]);
                ret.Add(command.nextUserIdentifier[1]);
                return ret.ToArray();
            }
        }
        public class USER_CODE_CAPABILITIES_GET
        {
            public const byte ID = 0x06;
            public static implicit operator USER_CODE_CAPABILITIES_GET(byte[] data)
            {
                USER_CODE_CAPABILITIES_GET ret = new USER_CODE_CAPABILITIES_GET();
                return ret;
            }
            public static implicit operator byte[](USER_CODE_CAPABILITIES_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class USER_CODE_CAPABILITIES_REPORT
        {
            public const byte ID = 0x07;
            public struct Tproperties1
            {
                private byte _value;
                public byte supportedUserIdStatusBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte mcdSupport
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte mcSupport
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
            public IList<byte> supportedUserIdStatusBitMask = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte supportedKeypadModesBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte mucsSupport
                {
                    get { return (byte)(_value >> 5 & 0x01); }
                    set { _value &= 0xFF - 0x20; _value += (byte)(value << 5 & 0x20); }
                }
                public byte mucrSupport
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte uccSupport
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> supportedKeypadModesBitMask = new List<byte>();
            public struct Tproperties3
            {
                private byte _value;
                public byte supportedKeysBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
                }
                public static implicit operator Tproperties3(byte data)
                {
                    Tproperties3 ret = new Tproperties3();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties3 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties3 properties3;
            public IList<byte> supportedKeysBitMask = new List<byte>();
            public static implicit operator USER_CODE_CAPABILITIES_REPORT(byte[] data)
            {
                USER_CODE_CAPABILITIES_REPORT ret = new USER_CODE_CAPABILITIES_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedUserIdStatusBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.supportedUserIdStatusBitMaskLength; i++)
                    {
                        ret.supportedUserIdStatusBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedKeypadModesBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties2.supportedKeypadModesBitMaskLength; i++)
                    {
                        ret.supportedKeypadModesBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedKeysBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties3.supportedKeysBitMaskLength; i++)
                    {
                        ret.supportedKeysBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_CAPABILITIES_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.supportedUserIdStatusBitMask != null)
                {
                    foreach (var tmp in command.supportedUserIdStatusBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.supportedKeypadModesBitMask != null)
                {
                    foreach (var tmp in command.supportedKeypadModesBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties3);
                if (command.supportedKeysBitMask != null)
                {
                    foreach (var tmp in command.supportedKeysBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class USER_CODE_KEYPAD_MODE_SET
        {
            public const byte ID = 0x08;
            public byte keypadMode;
            public static implicit operator USER_CODE_KEYPAD_MODE_SET(byte[] data)
            {
                USER_CODE_KEYPAD_MODE_SET ret = new USER_CODE_KEYPAD_MODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.keypadMode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_KEYPAD_MODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.keypadMode);
                return ret.ToArray();
            }
        }
        public class USER_CODE_KEYPAD_MODE_GET
        {
            public const byte ID = 0x09;
            public static implicit operator USER_CODE_KEYPAD_MODE_GET(byte[] data)
            {
                USER_CODE_KEYPAD_MODE_GET ret = new USER_CODE_KEYPAD_MODE_GET();
                return ret;
            }
            public static implicit operator byte[](USER_CODE_KEYPAD_MODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class USER_CODE_KEYPAD_MODE_REPORT
        {
            public const byte ID = 0x0A;
            public byte keypadMode;
            public static implicit operator USER_CODE_KEYPAD_MODE_REPORT(byte[] data)
            {
                USER_CODE_KEYPAD_MODE_REPORT ret = new USER_CODE_KEYPAD_MODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.keypadMode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_KEYPAD_MODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.keypadMode);
                return ret.ToArray();
            }
        }
        public class MASTER_CODE_SET
        {
            public const byte ID = 0x0E;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterCodeLength
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public IList<byte> masterCode = new List<byte>();
            public static implicit operator MASTER_CODE_SET(byte[] data)
            {
                MASTER_CODE_SET ret = new MASTER_CODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.masterCode = new List<byte>();
                    for (int i = 0; i < ret.properties1.masterCodeLength; i++)
                    {
                        ret.masterCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MASTER_CODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.masterCode != null)
                {
                    foreach (var tmp in command.masterCode)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MASTER_CODE_GET
        {
            public const byte ID = 0x0F;
            public static implicit operator MASTER_CODE_GET(byte[] data)
            {
                MASTER_CODE_GET ret = new MASTER_CODE_GET();
                return ret;
            }
            public static implicit operator byte[](MASTER_CODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MASTER_CODE_REPORT
        {
            public const byte ID = 0x10;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterCodeLength
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public IList<byte> masterCode = new List<byte>();
            public static implicit operator MASTER_CODE_REPORT(byte[] data)
            {
                MASTER_CODE_REPORT ret = new MASTER_CODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.masterCode = new List<byte>();
                    for (int i = 0; i < ret.properties1.masterCodeLength; i++)
                    {
                        ret.masterCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MASTER_CODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.masterCode != null)
                {
                    foreach (var tmp in command.masterCode)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class USER_CODE_CHECKSUM_GET
        {
            public const byte ID = 0x11;
            public static implicit operator USER_CODE_CHECKSUM_GET(byte[] data)
            {
                USER_CODE_CHECKSUM_GET ret = new USER_CODE_CHECKSUM_GET();
                return ret;
            }
            public static implicit operator byte[](USER_CODE_CHECKSUM_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class USER_CODE_CHECKSUM_REPORT
        {
            public const byte ID = 0x12;
            public byte[] userCodeChecksum = new byte[2];
            public static implicit operator USER_CODE_CHECKSUM_REPORT(byte[] data)
            {
                USER_CODE_CHECKSUM_REPORT ret = new USER_CODE_CHECKSUM_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.userCodeChecksum = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_CHECKSUM_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE_V2.ID);
                ret.Add(ID);
                ret.Add(command.userCodeChecksum[0]);
                ret.Add(command.userCodeChecksum[1]);
                return ret.ToArray();
            }
        }
    }
}

