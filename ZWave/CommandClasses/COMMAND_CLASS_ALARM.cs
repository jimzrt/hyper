using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ALARM
    {
        public const byte ID = 0x71;
        public const byte VERSION = 1;
        public class ALARM_GET
        {
            public const byte ID = 0x04;
            public byte alarmType;
            public static implicit operator ALARM_GET(byte[] data)
            {
                ALARM_GET ret = new ALARM_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.alarmType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ALARM_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM.ID);
                ret.Add(ID);
                ret.Add(command.alarmType);
                return ret.ToArray();
            }
        }
        public class ALARM_REPORT
        {
            public const byte ID = 0x05;
            public byte alarmType;
            public byte alarmLevel;
            public static implicit operator ALARM_REPORT(byte[] data)
            {
                ALARM_REPORT ret = new ALARM_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.alarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.alarmLevel = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ALARM_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ALARM.ID);
                ret.Add(ID);
                ret.Add(command.alarmType);
                ret.Add(command.alarmLevel);
                return ret.ToArray();
            }
        }
    }
}

