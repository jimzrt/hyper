using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_DOOR_LOCK_V2
    {
        public const byte ID = 0x62;
        public const byte VERSION = 2;
        public class DOOR_LOCK_CONFIGURATION_GET
        {
            public const byte ID = 0x05;
            public static implicit operator DOOR_LOCK_CONFIGURATION_GET(byte[] data)
            {
                DOOR_LOCK_CONFIGURATION_GET ret = new DOOR_LOCK_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_CONFIGURATION_REPORT
        {
            public const byte ID = 0x06;
            public byte operationType;
            public struct Tproperties1
            {
                private byte _value;
                public byte insideDoorHandlesState
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte outsideDoorHandlesState
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public byte lockTimeoutMinutes;
            public byte lockTimeoutSeconds;
            public static implicit operator DOOR_LOCK_CONFIGURATION_REPORT(byte[] data)
            {
                DOOR_LOCK_CONFIGURATION_REPORT ret = new DOOR_LOCK_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.operationType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutSeconds = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V2.ID);
                ret.Add(ID);
                ret.Add(command.operationType);
                ret.Add(command.properties1);
                ret.Add(command.lockTimeoutMinutes);
                ret.Add(command.lockTimeoutSeconds);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_CONFIGURATION_SET
        {
            public const byte ID = 0x04;
            public byte operationType;
            public struct Tproperties1
            {
                private byte _value;
                public byte insideDoorHandlesState
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte outsideDoorHandlesState
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public byte lockTimeoutMinutes;
            public byte lockTimeoutSeconds;
            public static implicit operator DOOR_LOCK_CONFIGURATION_SET(byte[] data)
            {
                DOOR_LOCK_CONFIGURATION_SET ret = new DOOR_LOCK_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.operationType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutSeconds = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V2.ID);
                ret.Add(ID);
                ret.Add(command.operationType);
                ret.Add(command.properties1);
                ret.Add(command.lockTimeoutMinutes);
                ret.Add(command.lockTimeoutSeconds);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_OPERATION_GET
        {
            public const byte ID = 0x02;
            public static implicit operator DOOR_LOCK_OPERATION_GET(byte[] data)
            {
                DOOR_LOCK_OPERATION_GET ret = new DOOR_LOCK_OPERATION_GET();
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_OPERATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V2.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_OPERATION_REPORT
        {
            public const byte ID = 0x03;
            public byte doorLockMode;
            public struct Tproperties1
            {
                private byte _value;
                public byte insideDoorHandlesMode
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte outsideDoorHandlesMode
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public byte doorCondition;
            public byte lockTimeoutMinutes;
            public byte lockTimeoutSeconds;
            public static implicit operator DOOR_LOCK_OPERATION_REPORT(byte[] data)
            {
                DOOR_LOCK_OPERATION_REPORT ret = new DOOR_LOCK_OPERATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.doorLockMode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.doorCondition = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutSeconds = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_OPERATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V2.ID);
                ret.Add(ID);
                ret.Add(command.doorLockMode);
                ret.Add(command.properties1);
                ret.Add(command.doorCondition);
                ret.Add(command.lockTimeoutMinutes);
                ret.Add(command.lockTimeoutSeconds);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_OPERATION_SET
        {
            public const byte ID = 0x01;
            public byte doorLockMode;
            public static implicit operator DOOR_LOCK_OPERATION_SET(byte[] data)
            {
                DOOR_LOCK_OPERATION_SET ret = new DOOR_LOCK_OPERATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.doorLockMode = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_OPERATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V2.ID);
                ret.Add(ID);
                ret.Add(command.doorLockMode);
                return ret.ToArray();
            }
        }
    }
}

