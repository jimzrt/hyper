using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MULTI_CHANNEL_V3
    {
        public const byte ID = 0x60;
        public const byte VERSION = 3;
        public class MULTI_CHANNEL_CAPABILITY_GET
        {
            public const byte ID = 0x09;
            public struct Tproperties1
            {
                private byte _value;
                public byte endPoint
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res
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
            public static implicit operator MULTI_CHANNEL_CAPABILITY_GET(byte[] data)
            {
                MULTI_CHANNEL_CAPABILITY_GET ret = new MULTI_CHANNEL_CAPABILITY_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_CAPABILITY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_CAPABILITY_REPORT
        {
            public const byte ID = 0x0A;
            public struct Tproperties1
            {
                private byte _value;
                public byte endPoint
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte dynamic
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
            public byte genericDeviceClass;
            public byte specificDeviceClass;
            public IList<byte> commandClass = new List<byte>();
            public static implicit operator MULTI_CHANNEL_CAPABILITY_REPORT(byte[] data)
            {
                MULTI_CHANNEL_CAPABILITY_REPORT ret = new MULTI_CHANNEL_CAPABILITY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.genericDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.specificDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClass = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.commandClass.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_CAPABILITY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.genericDeviceClass);
                ret.Add(command.specificDeviceClass);
                if (command.commandClass != null)
                {
                    foreach (var tmp in command.commandClass)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_CMD_ENCAP
        {
            public const byte ID = 0x0D;
            public struct Tproperties1
            {
                private byte _value;
                public byte sourceEndPoint
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res
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
            public struct Tproperties2
            {
                private byte _value;
                public byte destinationEndPoint
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte bitAddress
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public byte commandClass;
            public byte command;
            public IList<byte> parameter = new List<byte>();
            public static implicit operator MULTI_CHANNEL_CMD_ENCAP(byte[] data)
            {
                MULTI_CHANNEL_CMD_ENCAP ret = new MULTI_CHANNEL_CMD_ENCAP();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.command = data.Length > index ? data[index++] : (byte)0x00;
                    ret.parameter = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.parameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_CMD_ENCAP command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                ret.Add(command.commandClass);
                ret.Add(command.command);
                if (command.parameter != null)
                {
                    foreach (var tmp in command.parameter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_END_POINT_FIND
        {
            public const byte ID = 0x0B;
            public byte genericDeviceClass;
            public byte specificDeviceClass;
            public static implicit operator MULTI_CHANNEL_END_POINT_FIND(byte[] data)
            {
                MULTI_CHANNEL_END_POINT_FIND ret = new MULTI_CHANNEL_END_POINT_FIND();
                if (data != null)
                {
                    int index = 2;
                    ret.genericDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.specificDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_END_POINT_FIND command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.genericDeviceClass);
                ret.Add(command.specificDeviceClass);
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_END_POINT_FIND_REPORT
        {
            public const byte ID = 0x0C;
            public byte reportsToFollow;
            public byte genericDeviceClass;
            public byte specificDeviceClass;
            public class TVG
            {
                public struct Tproperties1
                {
                    private byte _value;
                    public byte endPoint
                    {
                        get { return (byte)(_value >> 0 & 0x7F); }
                        set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                    }
                    public byte res
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
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator MULTI_CHANNEL_END_POINT_FIND_REPORT(byte[] data)
            {
                MULTI_CHANNEL_END_POINT_FIND_REPORT ret = new MULTI_CHANNEL_END_POINT_FIND_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.reportsToFollow = data.Length > index ? data[index++] : (byte)0x00;
                    ret.genericDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.specificDeviceClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg = new List<TVG>();
                    while (data.Length - 0 > index)
                    {
                        TVG tmp = new TVG();
                        tmp.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_END_POINT_FIND_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.reportsToFollow);
                ret.Add(command.genericDeviceClass);
                ret.Add(command.specificDeviceClass);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.properties1);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_END_POINT_GET
        {
            public const byte ID = 0x07;
            public static implicit operator MULTI_CHANNEL_END_POINT_GET(byte[] data)
            {
                MULTI_CHANNEL_END_POINT_GET ret = new MULTI_CHANNEL_END_POINT_GET();
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_END_POINT_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MULTI_CHANNEL_END_POINT_REPORT
        {
            public const byte ID = 0x08;
            public struct Tproperties1
            {
                private byte _value;
                public byte res1
                {
                    get { return (byte)(_value >> 0 & 0x3F); }
                    set { _value &= 0xFF - 0x3F; _value += (byte)(value << 0 & 0x3F); }
                }
                public byte identical
                {
                    get { return (byte)(_value >> 6 & 0x01); }
                    set { _value &= 0xFF - 0x40; _value += (byte)(value << 6 & 0x40); }
                }
                public byte dynamic
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
            public struct Tproperties2
            {
                private byte _value;
                public byte endPoints
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res2
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public static implicit operator MULTI_CHANNEL_END_POINT_REPORT(byte[] data)
            {
                MULTI_CHANNEL_END_POINT_REPORT ret = new MULTI_CHANNEL_END_POINT_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_CHANNEL_END_POINT_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
        public class MULTI_INSTANCE_CMD_ENCAP
        {
            public const byte ID = 0x06;
            public struct Tproperties1
            {
                private byte _value;
                public byte instance
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res
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
            public byte commandClass;
            public byte command;
            public IList<byte> parameter = new List<byte>();
            public static implicit operator MULTI_INSTANCE_CMD_ENCAP(byte[] data)
            {
                MULTI_INSTANCE_CMD_ENCAP ret = new MULTI_INSTANCE_CMD_ENCAP();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.command = data.Length > index ? data[index++] : (byte)0x00;
                    ret.parameter = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.parameter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_INSTANCE_CMD_ENCAP command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.commandClass);
                ret.Add(command.command);
                if (command.parameter != null)
                {
                    foreach (var tmp in command.parameter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class MULTI_INSTANCE_GET
        {
            public const byte ID = 0x04;
            public byte commandClass;
            public static implicit operator MULTI_INSTANCE_GET(byte[] data)
            {
                MULTI_INSTANCE_GET ret = new MULTI_INSTANCE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_INSTANCE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.commandClass);
                return ret.ToArray();
            }
        }
        public class MULTI_INSTANCE_REPORT
        {
            public const byte ID = 0x05;
            public byte commandClass;
            public struct Tproperties1
            {
                private byte _value;
                public byte instances
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte res
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
            public static implicit operator MULTI_INSTANCE_REPORT(byte[] data)
            {
                MULTI_INSTANCE_REPORT ret = new MULTI_INSTANCE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.commandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](MULTI_INSTANCE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MULTI_CHANNEL_V3.ID);
                ret.Add(ID);
                ret.Add(command.commandClass);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
    }
}

