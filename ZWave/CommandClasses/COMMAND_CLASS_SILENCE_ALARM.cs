using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SILENCE_ALARM
    {
        public const byte ID = 0x9D;
        public const byte VERSION = 1;
        public class SENSOR_ALARM_SET
        {
            public const byte ID = 0x01;
            public byte mode;
            public byte[] seconds = new byte[2];
            public byte numberOfBitMasks;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator SENSOR_ALARM_SET(byte[] data)
            {
                SENSOR_ALARM_SET ret = new SENSOR_ALARM_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.mode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.seconds = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.numberOfBitMasks = data.Length > index ? data[index++] : (byte)0x00;
                    ret.bitMask = new List<byte>();
                    for (int i = 0; i < ret.numberOfBitMasks; i++)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SENSOR_ALARM_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SILENCE_ALARM.ID);
                ret.Add(ID);
                ret.Add(command.mode);
                ret.Add(command.seconds[0]);
                ret.Add(command.seconds[1]);
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

