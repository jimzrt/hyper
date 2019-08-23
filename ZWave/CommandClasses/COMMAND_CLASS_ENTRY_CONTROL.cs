using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ENTRY_CONTROL
    {
        public const byte ID = 0x6F;
        public const byte VERSION = 1;
        public class ENTRY_CONTROL_NOTIFICATION
        {
            public const byte ID = 0x01;
            public byte sequenceNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte dataType
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
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
            public byte eventType;
            public byte eventDataLength;
            public IList<byte> eventData = new List<byte>();
            public static implicit operator ENTRY_CONTROL_NOTIFICATION(byte[] data)
            {
                ENTRY_CONTROL_NOTIFICATION ret = new ENTRY_CONTROL_NOTIFICATION();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventDataLength = data.Length > index ? data[index++] : (byte)0x00;
                    if (ret.sequenceNumber > 0)
                    {
                        ret.eventData = new List<byte>();
                        for (int i = 0; i < ret.eventDataLength; i++)
                        {
                            ret.eventData.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_NOTIFICATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.properties1);
                ret.Add(command.eventType);
                ret.Add(command.eventDataLength);
                if (command.sequenceNumber > 0)
                {
                    if (command.eventData != null)
                    {
                        foreach (var tmp in command.eventData)
                        {
                            ret.Add(tmp);
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_KEY_SUPPORTED_GET
        {
            public const byte ID = 0x02;
            public static implicit operator ENTRY_CONTROL_KEY_SUPPORTED_GET(byte[] data)
            {
                ENTRY_CONTROL_KEY_SUPPORTED_GET ret = new ENTRY_CONTROL_KEY_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_KEY_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_KEY_SUPPORTED_REPORT
        {
            public const byte ID = 0x03;
            public byte keySupportedBitMaskLength;
            public IList<byte> keySupportedBitMask = new List<byte>();
            public static implicit operator ENTRY_CONTROL_KEY_SUPPORTED_REPORT(byte[] data)
            {
                ENTRY_CONTROL_KEY_SUPPORTED_REPORT ret = new ENTRY_CONTROL_KEY_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.keySupportedBitMaskLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.keySupportedBitMask = new List<byte>();
                    for (int i = 0; i < ret.keySupportedBitMaskLength; i++)
                    {
                        ret.keySupportedBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_KEY_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.keySupportedBitMaskLength);
                if (command.keySupportedBitMask != null)
                {
                    foreach (var tmp in command.keySupportedBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_EVENT_SUPPORTED_GET
        {
            public const byte ID = 0x04;
            public static implicit operator ENTRY_CONTROL_EVENT_SUPPORTED_GET(byte[] data)
            {
                ENTRY_CONTROL_EVENT_SUPPORTED_GET ret = new ENTRY_CONTROL_EVENT_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_EVENT_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_EVENT_SUPPORTED_REPORT
        {
            public const byte ID = 0x05;
            public struct Tproperties1
            {
                private byte _value;
                public byte dataTypeSupportedBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x03); }
                    set { _value &= 0xFF - 0x03; _value += (byte)(value << 0 & 0x03); }
                }
                public byte reserved1
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
            public IList<byte> dataTypeSupportedBitMask = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte eventSupportedBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved2
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public IList<byte> eventTypeSupportedBitMask = new List<byte>();
            public byte keyCachedSizeSupportedMinimum;
            public byte keyCachedSizeSupportedMaximum;
            public byte keyCachedTimeoutSupportedMinimum;
            public byte keyCachedTimeoutSupportedMaximum;
            public static implicit operator ENTRY_CONTROL_EVENT_SUPPORTED_REPORT(byte[] data)
            {
                ENTRY_CONTROL_EVENT_SUPPORTED_REPORT ret = new ENTRY_CONTROL_EVENT_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dataTypeSupportedBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.dataTypeSupportedBitMaskLength; i++)
                    {
                        ret.dataTypeSupportedBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventTypeSupportedBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties2.eventSupportedBitMaskLength; i++)
                    {
                        ret.eventTypeSupportedBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.keyCachedSizeSupportedMinimum = data.Length > index ? data[index++] : (byte)0x00;
                    ret.keyCachedSizeSupportedMaximum = data.Length > index ? data[index++] : (byte)0x00;
                    ret.keyCachedTimeoutSupportedMinimum = data.Length > index ? data[index++] : (byte)0x00;
                    ret.keyCachedTimeoutSupportedMaximum = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_EVENT_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.dataTypeSupportedBitMask != null)
                {
                    foreach (var tmp in command.dataTypeSupportedBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.eventTypeSupportedBitMask != null)
                {
                    foreach (var tmp in command.eventTypeSupportedBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.keyCachedSizeSupportedMinimum);
                ret.Add(command.keyCachedSizeSupportedMaximum);
                ret.Add(command.keyCachedTimeoutSupportedMinimum);
                ret.Add(command.keyCachedTimeoutSupportedMaximum);
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_CONFIGURATION_SET
        {
            public const byte ID = 0x06;
            public byte keyCacheSize;
            public byte keyCacheTimeout;
            public static implicit operator ENTRY_CONTROL_CONFIGURATION_SET(byte[] data)
            {
                ENTRY_CONTROL_CONFIGURATION_SET ret = new ENTRY_CONTROL_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.keyCacheSize = data.Length > index ? data[index++] : (byte)0x00;
                    ret.keyCacheTimeout = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.keyCacheSize);
                ret.Add(command.keyCacheTimeout);
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_CONFIGURATION_GET
        {
            public const byte ID = 0x07;
            public static implicit operator ENTRY_CONTROL_CONFIGURATION_GET(byte[] data)
            {
                ENTRY_CONTROL_CONFIGURATION_GET ret = new ENTRY_CONTROL_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ENTRY_CONTROL_CONFIGURATION_REPORT
        {
            public const byte ID = 0x08;
            public byte keyCacheSize;
            public byte keyCacheTimeout;
            public static implicit operator ENTRY_CONTROL_CONFIGURATION_REPORT(byte[] data)
            {
                ENTRY_CONTROL_CONFIGURATION_REPORT ret = new ENTRY_CONTROL_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.keyCacheSize = data.Length > index ? data[index++] : (byte)0x00;
                    ret.keyCacheTimeout = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ENTRY_CONTROL_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ENTRY_CONTROL.ID);
                ret.Add(ID);
                ret.Add(command.keyCacheSize);
                ret.Add(command.keyCacheTimeout);
                return ret.ToArray();
            }
        }
    }
}

