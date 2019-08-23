using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_DOOR_LOCK_V4
    {
        public const byte ID = 0x62;
        public const byte VERSION = 4;
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
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
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
            public byte[] autoRelockTime = new byte[2];
            public byte[] holdAndReleaseTime = new byte[2];
            public struct Tproperties2
            {
                private byte _value;
                public byte ta
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte btb
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 2 & 0x3F); }
                    set { _value &= 0xFF - 0xFC; _value += (byte)(value << 2 & 0xFC); }
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
                    ret.autoRelockTime = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.holdAndReleaseTime = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
                ret.Add(ID);
                ret.Add(command.operationType);
                ret.Add(command.properties1);
                ret.Add(command.lockTimeoutMinutes);
                ret.Add(command.lockTimeoutSeconds);
                ret.Add(command.autoRelockTime[0]);
                ret.Add(command.autoRelockTime[1]);
                ret.Add(command.holdAndReleaseTime[0]);
                ret.Add(command.holdAndReleaseTime[1]);
                ret.Add(command.properties2);
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
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
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
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_OPERATION_REPORT
        {
            public const byte ID = 0x03;
            public byte currentDoorLockMode;
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
            public byte targetDoorLockMode;
            public byte duration;
            public static implicit operator DOOR_LOCK_OPERATION_REPORT(byte[] data)
            {
                DOOR_LOCK_OPERATION_REPORT ret = new DOOR_LOCK_OPERATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.currentDoorLockMode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.doorCondition = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutMinutes = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lockTimeoutSeconds = data.Length > index ? data[index++] : (byte)0x00;
                    ret.targetDoorLockMode = data.Length > index ? data[index++] : (byte)0x00;
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_OPERATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
                ret.Add(ID);
                ret.Add(command.currentDoorLockMode);
                ret.Add(command.properties1);
                ret.Add(command.doorCondition);
                ret.Add(command.lockTimeoutMinutes);
                ret.Add(command.lockTimeoutSeconds);
                ret.Add(command.targetDoorLockMode);
                ret.Add(command.duration);
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
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
                ret.Add(ID);
                ret.Add(command.doorLockMode);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_CAPABILITIES_GET
        {
            public const byte ID = 0x07;
            public static implicit operator DOOR_LOCK_CAPABILITIES_GET(byte[] data)
            {
                DOOR_LOCK_CAPABILITIES_GET ret = new DOOR_LOCK_CAPABILITIES_GET();
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_CAPABILITIES_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class DOOR_LOCK_CAPABILITIES_REPORT
        {
            public const byte ID = 0x08;
            public struct Tproperties1
            {
                private byte _value;
                public byte supportedOperationTypeBitMaskLength
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
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
            public IList<byte> supportedOperationTypeBitMask = new List<byte>();
            public byte supportedDoorLockModeListLength;
            public IList<byte> supportedDoorLockMode = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte supportedInsideHandleModesBitmask
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte supportedOutsideHandleModesBitmask
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
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
            public byte supportedDoorComponents;
            public struct Tproperties3
            {
                private byte _value;
                public byte btbs
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte tas
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte hrs
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte ars
                {
                    get { return (byte)(_value >> 3 & 0x01); }
                    set { _value &= 0xFF - 0x08; _value += (byte)(value << 3 & 0x08); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
                }
                public static implicit operator Tproperties3(byte data)
                {
                    Tproperties3 ret = new Tproperties3();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties3 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties3 properties3;
            public static implicit operator DOOR_LOCK_CAPABILITIES_REPORT(byte[] data)
            {
                DOOR_LOCK_CAPABILITIES_REPORT ret = new DOOR_LOCK_CAPABILITIES_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedOperationTypeBitMask = new List<byte>();
                    for (int i = 0; i < ret.properties1.supportedOperationTypeBitMaskLength; i++)
                    {
                        ret.supportedOperationTypeBitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.supportedDoorLockModeListLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedDoorLockMode = new List<byte>();
                    for (int i = 0; i < ret.supportedDoorLockModeListLength; i++)
                    {
                        ret.supportedDoorLockMode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.supportedDoorComponents = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](DOOR_LOCK_CAPABILITIES_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_DOOR_LOCK_V4.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.supportedOperationTypeBitMask != null)
                {
                    foreach (var tmp in command.supportedOperationTypeBitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.supportedDoorLockModeListLength);
                if (command.supportedDoorLockMode != null)
                {
                    foreach (var tmp in command.supportedDoorLockMode)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                ret.Add(command.supportedDoorComponents);
                ret.Add(command.properties3);
                return ret.ToArray();
            }
        }
    }
}

