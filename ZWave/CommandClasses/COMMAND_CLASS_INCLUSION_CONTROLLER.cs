using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_INCLUSION_CONTROLLER
    {
        public const byte ID = 0x74;
        public const byte VERSION = 1;
        public class INITIATE
        {
            public const byte ID = 0x01;
            public byte nodeId;
            public byte stepId;
            public static implicit operator INITIATE(byte[] data)
            {
                INITIATE ret = new INITIATE();
                if (data != null)
                {
                    int index = 2;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.stepId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](INITIATE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INCLUSION_CONTROLLER.ID);
                ret.Add(ID);
                ret.Add(command.nodeId);
                ret.Add(command.stepId);
                return ret.ToArray();
            }
        }
        public class COMPLETE
        {
            public const byte ID = 0x02;
            public byte stepId;
            public byte status;
            public static implicit operator COMPLETE(byte[] data)
            {
                COMPLETE ret = new COMPLETE();
                if (data != null)
                {
                    int index = 2;
                    ret.stepId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.status = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](COMPLETE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_INCLUSION_CONTROLLER.ID);
                ret.Add(ID);
                ret.Add(command.stepId);
                ret.Add(command.status);
                return ret.ToArray();
            }
        }
    }
}

