using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_ZWAVEPLUS_INFO
    {
        public const byte ID = 0x5E;
        public const byte VERSION = 1;
        public class ZWAVEPLUS_INFO_GET
        {
            public const byte ID = 0x01;
            public static implicit operator ZWAVEPLUS_INFO_GET(byte[] data)
            {
                ZWAVEPLUS_INFO_GET ret = new ZWAVEPLUS_INFO_GET();
                return ret;
            }
            public static implicit operator byte[](ZWAVEPLUS_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZWAVEPLUS_INFO.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class ZWAVEPLUS_INFO_REPORT
        {
            public const byte ID = 0x02;
            public byte zWaveVersion;
            public byte roleType;
            public byte nodeType;
            public static implicit operator ZWAVEPLUS_INFO_REPORT(byte[] data)
            {
                ZWAVEPLUS_INFO_REPORT ret = new ZWAVEPLUS_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.zWaveVersion = data.Length > index ? data[index++] : (byte)0x00;
                    ret.roleType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](ZWAVEPLUS_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_ZWAVEPLUS_INFO.ID);
                ret.Add(ID);
                ret.Add(command.zWaveVersion);
                ret.Add(command.roleType);
                ret.Add(command.nodeType);
                return ret.ToArray();
            }
        }
    }
}

