using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SCENE_ACTIVATION
    {
        public const byte ID = 0x2B;
        public const byte VERSION = 1;
        public class SCENE_ACTIVATION_SET
        {
            public const byte ID = 0x01;
            public byte sceneId;
            public byte dimmingDuration;
            public static implicit operator SCENE_ACTIVATION_SET(byte[] data)
            {
                SCENE_ACTIVATION_SET ret = new SCENE_ACTIVATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_ACTIVATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_ACTIVATION.ID);
                ret.Add(ID);
                ret.Add(command.sceneId);
                ret.Add(command.dimmingDuration);
                return ret.ToArray();
            }
        }
    }
}

