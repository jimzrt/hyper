using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SENSOR_ALARM
    {
        public const byte ID = 0x9C;
        public const byte VERSION = 1;
        public class SENSOR_ALARM_GET
        {
            public const byte ID = 0x01;
            public byte sensorType;
            public static implicit operator SENSOR_ALARM_GET(byte[] data)
            {
                SENSOR_ALARM_GET ret = new SENSOR_ALARM_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_ALARM_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_ALARM.ID);
                ret.Add(ID);
                ret.Add(command.sensorType);
                return ret.ToArray();
            }
        }
        public class SENSOR_ALARM_REPORT
        {
            public const byte ID = 0x02;
            public byte sourceNodeId;
            public byte sensorType;
            public byte sensorState;
            public byte[] seconds = new byte[2];
            public static implicit operator SENSOR_ALARM_REPORT(byte[] data)
            {
                SENSOR_ALARM_REPORT ret = new SENSOR_ALARM_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sourceNodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorState = data.Length > index ? data[index++] : (byte)0x00;
                    ret.seconds = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_ALARM_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_ALARM.ID);
                ret.Add(ID);
                ret.Add(command.sourceNodeId);
                ret.Add(command.sensorType);
                ret.Add(command.sensorState);
                ret.Add(command.seconds[0]);
                ret.Add(command.seconds[1]);
                return ret.ToArray();
            }
        }
        public class SENSOR_ALARM_SUPPORTED_GET
        {
            public const byte ID = 0x03;
            public static implicit operator SENSOR_ALARM_SUPPORTED_GET(byte[] data)
            {
                SENSOR_ALARM_SUPPORTED_GET ret = new SENSOR_ALARM_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](SENSOR_ALARM_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_ALARM.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SENSOR_ALARM_SUPPORTED_REPORT
        {
            public const byte ID = 0x04;
            public byte numberOfBitMasks;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator SENSOR_ALARM_SUPPORTED_REPORT(byte[] data)
            {
                SENSOR_ALARM_SUPPORTED_REPORT ret = new SENSOR_ALARM_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.numberOfBitMasks = data.Length > index ? data[index++] : (byte)0x00;
                    ret.bitMask = new List<byte>();
                    for (int i = 0; i < ret.numberOfBitMasks; i++)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_ALARM_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_ALARM.ID);
                ret.Add(ID);
                ret.Add(command.numberOfBitMasks);
                if (command.bitMask != null)
                {
                    foreach (var tmp in command.bitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

