using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MAILBOX
    {
        public const byte ID = 0x69;
        public const byte VERSION = 1;
        public class MAILBOX_CONFIGURATION_GET
        {
            public const byte ID = 0x01;
            public static implicit operator MAILBOX_CONFIGURATION_GET(byte[] data)
            {
                MAILBOX_CONFIGURATION_GET ret = new MAILBOX_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](MAILBOX_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MAILBOX.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MAILBOX_CONFIGURATION_SET
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte mode
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
            public byte[] forwardingDestinationIpv6Address = new byte[16];
            public byte[] udpPortNumber = new byte[2];
            public static implicit operator MAILBOX_CONFIGURATION_SET(byte[] data)
            {
                MAILBOX_CONFIGURATION_SET ret = new MAILBOX_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.forwardingDestinationIpv6Address = new byte[]
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
                    ret.udpPortNumber = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](MAILBOX_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MAILBOX.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.forwardingDestinationIpv6Address != null)
                {
                    foreach (var tmp in command.forwardingDestinationIpv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.udpPortNumber[0]);
                ret.Add(command.udpPortNumber[1]);
                return ret.ToArray();
            }
        }
        public class MAILBOX_CONFIGURATION_REPORT
        {
            public const byte ID = 0x03;
            public struct Tproperties1
            {
                private byte _value;
                public byte mode
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte supportedModes
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public byte[] mailboxCapacity = new byte[2];
            public byte[] forwardingDestinationIpv6Address = new byte[16];
            public byte[] udpPortNumber = new byte[2];
            public static implicit operator MAILBOX_CONFIGURATION_REPORT(byte[] data)
            {
                MAILBOX_CONFIGURATION_REPORT ret = new MAILBOX_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mailboxCapacity = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.forwardingDestinationIpv6Address = new byte[]
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
                    ret.udpPortNumber = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](MAILBOX_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MAILBOX.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.mailboxCapacity[0]);
                ret.Add(command.mailboxCapacity[1]);
                if (command.forwardingDestinationIpv6Address != null)
                {
                    foreach (var tmp in command.forwardingDestinationIpv6Address)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.udpPortNumber[0]);
                ret.Add(command.udpPortNumber[1]);
                return ret.ToArray();
            }
        }
        public class MAILBOX_QUEUE
        {
            public const byte ID = 0x04;
            public byte sequenceNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte mode
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte last
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
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
            public byte queueHandle;
            public IList<byte> mailboxEntry = new List<byte>();
            public static implicit operator MAILBOX_QUEUE(byte[] data)
            {
                MAILBOX_QUEUE ret = new MAILBOX_QUEUE();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.queueHandle = data.Length > index ? data[index++] : (byte)0x00;
                    ret.mailboxEntry = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.mailboxEntry.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MAILBOX_QUEUE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MAILBOX.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.properties1);
                ret.Add(command.queueHandle);
                if (command.mailboxEntry != null)
                {
                    foreach (var tmp in command.mailboxEntry)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MAILBOX_WAKEUP_NOTIFICATION
        {
            public const byte ID = 0x05;
            public byte queueHandle;
            public static implicit operator MAILBOX_WAKEUP_NOTIFICATION(byte[] data)
            {
                MAILBOX_WAKEUP_NOTIFICATION ret = new MAILBOX_WAKEUP_NOTIFICATION();
                if (data != null)
                {
                    int index = 2;
                    ret.queueHandle = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MAILBOX_WAKEUP_NOTIFICATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MAILBOX.ID);
                ret.Add(ID);
                ret.Add(command.queueHandle);
                return ret.ToArray();
            }
        }
        public class MAILBOX_NODE_FAILING
        {
            public const byte ID = 0x06;
            public byte queueHandle;
            public static implicit operator MAILBOX_NODE_FAILING(byte[] data)
            {
                MAILBOX_NODE_FAILING ret = new MAILBOX_NODE_FAILING();
                if (data != null)
                {
                    int index = 2;
                    ret.queueHandle = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MAILBOX_NODE_FAILING command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MAILBOX.ID);
                ret.Add(ID);
                ret.Add(command.queueHandle);
                return ret.ToArray();
            }
        }
    }
}

