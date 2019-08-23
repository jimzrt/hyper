using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_REMOTE_ASSOCIATION
    {
        public const byte ID = 0x7D;
        public const byte VERSION = 1;
        public class REMOTE_ASSOCIATION_CONFIGURATION_GET
        {
            public const byte ID = 0x02;
            public byte localGroupingIdentifier;
            public static implicit operator REMOTE_ASSOCIATION_CONFIGURATION_GET(byte[] data)
            {
                REMOTE_ASSOCIATION_CONFIGURATION_GET ret = new REMOTE_ASSOCIATION_CONFIGURATION_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.localGroupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](REMOTE_ASSOCIATION_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_REMOTE_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.localGroupingIdentifier);
                return ret.ToArray();
            }
        }
        public class REMOTE_ASSOCIATION_CONFIGURATION_REPORT
        {
            public const byte ID = 0x03;
            public byte localGroupingIdentifier;
            public byte remoteNodeid;
            public byte remoteGroupingIdentifier;
            public static implicit operator REMOTE_ASSOCIATION_CONFIGURATION_REPORT(byte[] data)
            {
                REMOTE_ASSOCIATION_CONFIGURATION_REPORT ret = new REMOTE_ASSOCIATION_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.localGroupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.remoteNodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.remoteGroupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](REMOTE_ASSOCIATION_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_REMOTE_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.localGroupingIdentifier);
                ret.Add(command.remoteNodeid);
                ret.Add(command.remoteGroupingIdentifier);
                return ret.ToArray();
            }
        }
        public class REMOTE_ASSOCIATION_CONFIGURATION_SET
        {
            public const byte ID = 0x01;
            public byte localGroupingIdentifier;
            public byte remoteNodeid;
            public byte remoteGroupingIdentifier;
            public static implicit operator REMOTE_ASSOCIATION_CONFIGURATION_SET(byte[] data)
            {
                REMOTE_ASSOCIATION_CONFIGURATION_SET ret = new REMOTE_ASSOCIATION_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.localGroupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.remoteNodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.remoteGroupingIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](REMOTE_ASSOCIATION_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_REMOTE_ASSOCIATION.ID);
                ret.Add(ID);
                ret.Add(command.localGroupingIdentifier);
                ret.Add(command.remoteNodeid);
                ret.Add(command.remoteGroupingIdentifier);
                return ret.ToArray();
            }
        }
    }
}

