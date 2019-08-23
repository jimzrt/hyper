using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SENSOR_BINARY_V2
    {
        public const byte ID = 0x30;
        public const byte VERSION = 2;
        public class SENSOR_BINARY_GET
        {
            public const byte ID = 0x02;
            public byte sensorType;
            public static implicit operator SENSOR_BINARY_GET(byte[] data)
            {
                SENSOR_BINARY_GET ret = new SENSOR_BINARY_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_BINARY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_BINARY_V2.ID);
                ret.Add(ID);
                ret.Add(command.sensorType);
                return ret.ToArray();
            }
        }
        public class SENSOR_BINARY_REPORT
        {
            public const byte ID = 0x03;
            public byte sensorValue;
            public byte sensorType;
            public static implicit operator SENSOR_BINARY_REPORT(byte[] data)
            {
                SENSOR_BINARY_REPORT ret = new SENSOR_BINARY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sensorValue = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_BINARY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_BINARY_V2.ID);
                ret.Add(ID);
                ret.Add(command.sensorValue);
                ret.Add(command.sensorType);
                return ret.ToArray();
            }
        }
        public class SENSOR_BINARY_SUPPORTED_GET_SENSOR
        {
            public const byte ID = 0x01;
            public static implicit operator SENSOR_BINARY_SUPPORTED_GET_SENSOR(byte[] data)
            {
                SENSOR_BINARY_SUPPORTED_GET_SENSOR ret = new SENSOR_BINARY_SUPPORTED_GET_SENSOR();
                return ret;
            }
            public static implicit operator byte[](SENSOR_BINARY_SUPPORTED_GET_SENSOR command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_BINARY_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SENSOR_BINARY_SUPPORTED_SENSOR_REPORT
        {
            public const byte ID = 0x04;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator SENSOR_BINARY_SUPPORTED_SENSOR_REPORT(byte[] data)
            {
                SENSOR_BINARY_SUPPORTED_SENSOR_REPORT ret = new SENSOR_BINARY_SUPPORTED_SENSOR_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.bitMask = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_BINARY_SUPPORTED_SENSOR_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SENSOR_BINARY_V2.ID);
                ret.Add(ID);
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

