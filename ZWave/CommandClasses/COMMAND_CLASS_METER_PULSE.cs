using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_METER_PULSE
    {
        public const byte ID = 0x35;
        public const byte VERSION = 1;
        public class METER_PULSE_GET
        {
            public const byte ID = 0x04;
            public static implicit operator METER_PULSE_GET(byte[] data)
            {
                METER_PULSE_GET ret = new METER_PULSE_GET();
                return ret;
            }
            public static implicit operator byte[](METER_PULSE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_PULSE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class METER_PULSE_REPORT
        {
            public const byte ID = 0x05;
            public byte[] pulseCount = new byte[4];
            public static implicit operator METER_PULSE_REPORT(byte[] data)
            {
                METER_PULSE_REPORT ret = new METER_PULSE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.pulseCount = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](METER_PULSE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_PULSE.ID);
                ret.Add(ID);
                ret.Add(command.pulseCount[0]);
                ret.Add(command.pulseCount[1]);
                ret.Add(command.pulseCount[2]);
                ret.Add(command.pulseCount[3]);
                return ret.ToArray();
            }
        }
    }
}

