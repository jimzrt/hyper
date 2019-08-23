using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SCENE_ACTUATOR_CONF
    {
        public const byte ID = 0x2C;
        public const byte VERSION = 1;
        public class SCENE_ACTUATOR_CONF_GET
        {
            public const byte ID = 0x02;
            public byte sceneId;
            public static implicit operator SCENE_ACTUATOR_CONF_GET(byte[] data)
            {
                SCENE_ACTUATOR_CONF_GET ret = new SCENE_ACTUATOR_CONF_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_ACTUATOR_CONF_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_ACTUATOR_CONF.ID);
                ret.Add(ID);
                ret.Add(command.sceneId);
                return ret.ToArray();
            }
        }
        public class SCENE_ACTUATOR_CONF_REPORT
        {
            public const byte ID = 0x03;
            public byte sceneId;
            public byte level;
            public byte dimmingDuration;
            public static implicit operator SCENE_ACTUATOR_CONF_REPORT(byte[] data)
            {
                SCENE_ACTUATOR_CONF_REPORT ret = new SCENE_ACTUATOR_CONF_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.level = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_ACTUATOR_CONF_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_ACTUATOR_CONF.ID);
                ret.Add(ID);
                ret.Add(command.sceneId);
                ret.Add(command.level);
                ret.Add(command.dimmingDuration);
                return ret.ToArray();
            }
        }
        public class SCENE_ACTUATOR_CONF_SET
        {
            public const byte ID = 0x01;
            public byte sceneId;
            public byte dimmingDuration;
            public struct Tproperties1
            {
                private byte _value;
                public byte reserved
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte moverride
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte level;
            public static implicit operator SCENE_ACTUATOR_CONF_SET(byte[] data)
            {
                SCENE_ACTUATOR_CONF_SET ret = new SCENE_ACTUATOR_CONF_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.dimmingDuration = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.level = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SCENE_ACTUATOR_CONF_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SCENE_ACTUATOR_CONF.ID);
                ret.Add(ID);
                ret.Add(command.sceneId);
                ret.Add(command.dimmingDuration);
                ret.Add(command.properties1);
                ret.Add(command.level);
                return ret.ToArray();
            }
        }
    }
}

