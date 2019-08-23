using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SCENE_CONTROLLER_CONF
    {
        public const byte ID = 0x2D;
        public const byte VERSION = 1;
        public class SCENE_CONTROLLER_CONF_GET
        {
            public const byte ID = 0x02;
            public byte groupId;
            public static implicit operator SCENE_CONTROLLER_CONF_GET(byte[] data)
            {
                SCENE_CONTROLLER_CONF_GET ret = new SCENE_CONTROLLER_CONF_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_CONTROLLER_CONF_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_CONTROLLER_CONF.ID);
                ret.Add(ID);
                ret.Add(command.groupId);
                return ret.ToArray();
            }
        }
        public class SCENE_CONTROLLER_CONF_REPORT
        {
            public const byte ID = 0x03;
            public byte groupId;
            public byte sceneId;
            public byte dimmingDuration;
            public static implicit operator SCENE_CONTROLLER_CONF_REPORT(byte[] data)
            {
                SCENE_CONTROLLER_CONF_REPORT ret = new SCENE_CONTROLLER_CONF_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.groupId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_CONTROLLER_CONF_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_CONTROLLER_CONF.ID);
                ret.Add(ID);
                ret.Add(command.groupId);
                ret.Add(command.sceneId);
                ret.Add(command.dimmingDuration);
                return ret.ToArray();
            }
        }
        public class SCENE_CONTROLLER_CONF_SET
        {
            public const byte ID = 0x01;
            public byte groupId;
            public byte sceneId;
            public byte dimmingDuration;
            public static implicit operator SCENE_CONTROLLER_CONF_SET(byte[] data)
            {
                SCENE_CONTROLLER_CONF_SET ret = new SCENE_CONTROLLER_CONF_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.groupId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_CONTROLLER_CONF_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_CONTROLLER_CONF.ID);
                ret.Add(ID);
                ret.Add(command.groupId);
                ret.Add(command.sceneId);
                ret.Add(command.dimmingDuration);
                return ret.ToArray();
            }
        }
    }
}

