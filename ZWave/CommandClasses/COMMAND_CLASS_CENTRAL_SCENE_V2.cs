using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CENTRAL_SCENE_V2
    {
        public const byte ID = 0x5B;
        public const byte VERSION = 2;
        public class CENTRAL_SCENE_SUPPORTED_GET
        {
            public const byte ID = 0x01;
            public static implicit operator CENTRAL_SCENE_SUPPORTED_GET(byte[] data)
            {
                CENTRAL_SCENE_SUPPORTED_GET ret = new CENTRAL_SCENE_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](CENTRAL_SCENE_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CENTRAL_SCENE_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class CENTRAL_SCENE_SUPPORTED_REPORT
        {
            public const byte ID = 0x02;
            public byte supportedScenes;
            public struct Tproperties1
            {
                private byte _value;
                public byte identical
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte numberOfBitMaskBytes
                {
                    get { return (byte)(_value >> 1 & 0x03); }
                    set { _value &= 0xFF - 0x06; _value += (byte)(value << 1 & 0x06); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public class TVG1
            {
                public IList<byte> supportedKeyAttributesForScene = new List<byte>();
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator CENTRAL_SCENE_SUPPORTED_REPORT(byte[] data)
            {
                CENTRAL_SCENE_SUPPORTED_REPORT ret = new CENTRAL_SCENE_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedScenes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    for (int j = 0; j < ret.supportedScenes; j++)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.supportedKeyAttributesForScene = new List<byte>();
                        for (int i = 0; i < ret.properties1.numberOfBitMaskBytes; i++)
                        {
                            tmp.supportedKeyAttributesForScene.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CENTRAL_SCENE_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CENTRAL_SCENE_V2.ID);
                ret.Add(ID);
                ret.Add(command.supportedScenes);
                ret.Add(command.properties1);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        if (item.supportedKeyAttributesForScene != null)
                        {
                            foreach (var tmp in item.supportedKeyAttributesForScene)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class CENTRAL_SCENE_NOTIFICATION
        {
            public const byte ID = 0x03;
            public byte sequenceNumber;
            public struct Tproperties1
            {
                private byte _value;
                public byte keyAttributes
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 3 & 0x1F); }
                    set { _value &= 0xFF - 0xF8; _value += (byte)(value << 3 & 0xF8); }
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
            public byte sceneNumber;
            public static implicit operator CENTRAL_SCENE_NOTIFICATION(byte[] data)
            {
                CENTRAL_SCENE_NOTIFICATION ret = new CENTRAL_SCENE_NOTIFICATION();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sceneNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CENTRAL_SCENE_NOTIFICATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CENTRAL_SCENE_V2.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.properties1);
                ret.Add(command.sceneNumber);
                return ret.ToArray();
            }
        }
    }
}

