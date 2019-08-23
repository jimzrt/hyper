using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_DOOR_LOCK_LOGGING
    {
        public const byte ID = 0x4C;
        public const byte VERSION = 1;
        public class DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_GET(byte[] data)
            {
                DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_GET ret = new DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_LOGGING.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public byte maxRecordsStored;
            public static implicit operator DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_REPORT(byte[] data)
            {
                DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_REPORT ret = new DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.maxRecordsStored = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_LOGGING_RECORDS_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_LOGGING.ID);
                ret.Add(ID);
                ret.Add(command.maxRecordsStored);
                return ret.ToArray();
            }
        }
        public class RECORD_GET
        {
            public const byte ID = 0x03;
            public byte recordNumber;
            public static implicit operator RECORD_GET(byte[] data)
            {
                RECORD_GET ret = new RECORD_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.recordNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](RECORD_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_LOGGING.ID);
                ret.Add(ID);
                ret.Add(command.recordNumber);
                return ret.ToArray();
            }
        }
        public class RECORD_REPORT
        {
            public const byte ID = 0x04;
            public byte recordNumber;
            public byte[] year = new byte[2];
            public byte month;
            public byte day;
            public struct Tproperties1
            {
                private byte _value;
                public byte hourLocalTime
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte recordStatus
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
            public byte minuteLocalTime;
            public byte secondLocalTime;
            public byte eventType;
            public byte userIdentifier;
            public byte userCodeLength;
            public IList<byte> userCode = new List<byte>();
            public static implicit operator RECORD_REPORT(byte[] data)
            {
                RECORD_REPORT ret = new RECORD_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.recordNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.year = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.month = data.Length > index ? data[index++] : (byte)0x00;
                    ret.day = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.minuteLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.secondLocalTime = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userCodeLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userCode = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](RECORD_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_LOGGING.ID);
                ret.Add(ID);
                ret.Add(command.recordNumber);
                ret.Add(command.year[0]);
                ret.Add(command.year[1]);
                ret.Add(command.month);
                ret.Add(command.day);
                ret.Add(command.properties1);
                ret.Add(command.minuteLocalTime);
                ret.Add(command.secondLocalTime);
                ret.Add(command.eventType);
                ret.Add(command.userIdentifier);
                ret.Add(command.userCodeLength);
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
    }
}

