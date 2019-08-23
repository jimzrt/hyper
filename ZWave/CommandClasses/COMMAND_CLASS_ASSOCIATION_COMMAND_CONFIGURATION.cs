using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION
    {
        public const byte ID = 0x9B;
        public const byte VERSION = 1;
        public class COMMAND_CONFIGURATION_GET
        {
            public const byte ID = 0x04;
            public byte groupingIdentifier;
            public byte nodeId;
            public static implicit operator COMMAND_CONFIGURATION_GET(byte[] data)
            {
                COMMAND_CONFIGURATION_GET ret = new COMMAND_CONFIGURATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class COMMAND_CONFIGURATION_REPORT
        {
            public const byte ID = 0x05;
            public byte groupingIdentifier;
            public byte nodeId;
            public struct Tproperties1
            {
                private byte _value;
                public byte reportsToFollow
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x07); }
                    set { _value &= 0xFF - 0x70; _value += (byte)(value << 4 & 0x70); }
                }
                public byte first
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
            public byte commandLength;
            public byte commandClassIdentifier;
            public byte commandIdentifier;
            public IList<byte> commandByte = new List<byte>();
            public static implicit operator COMMAND_CONFIGURATION_REPORT(byte[] data)
            {
                COMMAND_CONFIGURATION_REPORT ret = new COMMAND_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClassIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandByte = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.commandByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.nodeId);
                ret.Add(command.properties1);
                ret.Add(command.commandLength);
                ret.Add(command.commandClassIdentifier);
                ret.Add(command.commandIdentifier);
                if (command.commandByte != null)
                {
                    foreach (var tmp in command.commandByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class COMMAND_CONFIGURATION_SET
        {
            public const byte ID = 0x03;
            public byte groupingIdentifier;
            public byte nodeId;
            public byte commandLength;
            public byte commandClassIdentifier;
            public byte commandIdentifier;
            public IList<byte> commandByte = new List<byte>();
            public static implicit operator COMMAND_CONFIGURATION_SET(byte[] data)
            {
                COMMAND_CONFIGURATION_SET ret = new COMMAND_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClassIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandByte = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.commandByte.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.groupingIdentifier);
                ret.Add(command.nodeId);
                ret.Add(command.commandLength);
                ret.Add(command.commandClassIdentifier);
                ret.Add(command.commandIdentifier);
                if (command.commandByte != null)
                {
                    foreach (var tmp in command.commandByte)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class COMMAND_RECORDS_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator COMMAND_RECORDS_SUPPORTED_GET(byte[] data)
            {
                COMMAND_RECORDS_SUPPORTED_GET ret = new COMMAND_RECORDS_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](COMMAND_RECORDS_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class COMMAND_RECORDS_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte confCmd
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte vC
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte maxCommandLength
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
            public byte[] freeCommandRecords = new byte[2];
            public byte[] maxCommandRecords = new byte[2];
            public static implicit operator COMMAND_RECORDS_SUPPORTED_REPORT(byte[] data)
            {
                COMMAND_RECORDS_SUPPORTED_REPORT ret = new COMMAND_RECORDS_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.freeCommandRecords = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.maxCommandRecords = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_RECORDS_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.freeCommandRecords[0]);
                ret.Add(command.freeCommandRecords[1]);
                ret.Add(command.maxCommandRecords[0]);
                ret.Add(command.maxCommandRecords[1]);
                return ret.ToArray();
            }
        }
    }
}

