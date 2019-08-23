using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SENSOR_BINARY
    {
        public const byte ID = 0x30;
        public const byte VERSION = 1;
        public class SENSOR_BINARY_GET
        {
            public const byte ID = 0x02;
            public static implicit operator SENSOR_BINARY_GET(byte[] data)
            {
                SENSOR_BINARY_GET ret = new SENSOR_BINARY_GET();
                return ret;
            }
            public static implicit operator byte[](SENSOR_BINARY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_BINARY.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SENSOR_BINARY_REPORT
        {
            public const byte ID = 0x03;
            public byte sensorValue;
            public static implicit operator SENSOR_BINARY_REPORT(byte[] data)
            {
                SENSOR_BINARY_REPORT ret = new SENSOR_BINARY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sensorValue = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_BINARY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_BINARY.ID);
                ret.Add(ID);
                ret.Add(command.sensorValue);
                return ret.ToArray();
            }
        }
    }
}

