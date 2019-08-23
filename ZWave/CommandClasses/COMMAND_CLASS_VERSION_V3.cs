using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_VERSION_V3
    {
        public const byte ID = 0x86;
        public const byte VERSION = 3;
        public class VERSION_COMMAND_CLASS_GET
        {
            public const byte ID = 0x13;
            public byte requestedCommandClass;
            public static implicit operator VERSION_COMMAND_CLASS_GET(byte[] data)
            {
                VERSION_COMMAND_CLASS_GET ret = new VERSION_COMMAND_CLASS_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.requestedCommandClass = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](VERSION_COMMAND_CLASS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                ret.Add(command.requestedCommandClass);
                return ret.ToArray();
            }
        }
        public class VERSION_COMMAND_CLASS_REPORT
        {
            public const byte ID = 0x14;
            public byte requestedCommandClass;
            public byte commandClassVersion;
            public static implicit operator VERSION_COMMAND_CLASS_REPORT(byte[] data)
            {
                VERSION_COMMAND_CLASS_REPORT ret = new VERSION_COMMAND_CLASS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.requestedCommandClass = data.Length > index ? data[index++] : (byte)0x00;
                    ret.commandClassVersion = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](VERSION_COMMAND_CLASS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                ret.Add(command.requestedCommandClass);
                ret.Add(command.commandClassVersion);
                return ret.ToArray();
            }
        }
        public class VERSION_GET
        {
            public const byte ID = 0x11;
            public static implicit operator VERSION_GET(byte[] data)
            {
                VERSION_GET ret = new VERSION_GET();
                return ret;
            }
            public static implicit operator byte[](VERSION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class VERSION_REPORT
        {
            public const byte ID = 0x12;
            public byte zWaveLibraryType;
            public byte zWaveProtocolVersion;
            public byte zWaveProtocolSubVersion;
            public byte firmware0Version;
            public byte firmware0SubVersion;
            public byte hardwareVersion;
            public byte numberOfFirmwareTargets;
            public class TVG
            {
                public byte firmwareVersion;
                public byte firmwareSubVersion;
            }
            public List<TVG> vg = new List<TVG>();
            public static implicit operator VERSION_REPORT(byte[] data)
            {
                VERSION_REPORT ret = new VERSION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.zWaveLibraryType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zWaveProtocolVersion = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zWaveProtocolSubVersion = data.Length > index ? data[index++] : (byte)0x00;
                    ret.firmware0Version = data.Length > index ? data[index++] : (byte)0x00;
                    ret.firmware0SubVersion = data.Length > index ? data[index++] : (byte)0x00;
                    ret.hardwareVersion = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfFirmwareTargets = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg = new List<TVG>();
                    for (int j = 0; j < ret.numberOfFirmwareTargets; j++)
                    {
                        TVG tmp = new TVG();
                        tmp.firmwareVersion = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.firmwareSubVersion = data.Length > index ? data[index++] : (byte)0x00;
                        ret.vg.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](VERSION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                ret.Add(command.zWaveLibraryType);
                ret.Add(command.zWaveProtocolVersion);
                ret.Add(command.zWaveProtocolSubVersion);
                ret.Add(command.firmware0Version);
                ret.Add(command.firmware0SubVersion);
                ret.Add(command.hardwareVersion);
                ret.Add(command.numberOfFirmwareTargets);
                if (command.vg != null)
                {
                    foreach (var item in command.vg)
                    {
                        ret.Add(item.firmwareVersion);
                        ret.Add(item.firmwareSubVersion);
                    }
                }
                return ret.ToArray();
            }
        }
        public class VERSION_CAPABILITIES_GET
        {
            public const byte ID = 0x15;
            public static implicit operator VERSION_CAPABILITIES_GET(byte[] data)
            {
                VERSION_CAPABILITIES_GET ret = new VERSION_CAPABILITIES_GET();
                return ret;
            }
            public static implicit operator byte[](VERSION_CAPABILITIES_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class VERSION_CAPABILITIES_REPORT
        {
            public const byte ID = 0x16;
            public struct Tproperties1
            {
                private byte _value;
                public byte version
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte commandClass
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte zWaveSoftware
                {
                    get { return (byte)(_value >> 2 & 0x01); }
                    set { _value &= 0xFF - 0x04; _value += (byte)(value << 2 & 0x04); }
                }
                public byte reserved1
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
            public static implicit operator VERSION_CAPABILITIES_REPORT(byte[] data)
            {
                VERSION_CAPABILITIES_REPORT ret = new VERSION_CAPABILITIES_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](VERSION_CAPABILITIES_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                return ret.ToArray();
            }
        }
        public class VERSION_ZWAVE_SOFTWARE_GET
        {
            public const byte ID = 0x17;
            public static implicit operator VERSION_ZWAVE_SOFTWARE_GET(byte[] data)
            {
                VERSION_ZWAVE_SOFTWARE_GET ret = new VERSION_ZWAVE_SOFTWARE_GET();
                return ret;
            }
            public static implicit operator byte[](VERSION_ZWAVE_SOFTWARE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class VERSION_ZWAVE_SOFTWARE_REPORT
        {
            public const byte ID = 0x18;
            public byte[] sdkVersion = new byte[3];
            public byte[] applicationFrameworkApiVersion = new byte[3];
            public byte[] applicationFrameworkBuildNumber = new byte[2];
            public byte[] hostInterfaceVersion = new byte[3];
            public byte[] hostInterfaceBuildNumber = new byte[2];
            public byte[] zWaveProtocolVersion = new byte[3];
            public byte[] zWaveProtocolBuildNumber = new byte[2];
            public byte[] applicationVersion = new byte[3];
            public byte[] applicationBuildNumber = new byte[2];
            public static implicit operator VERSION_ZWAVE_SOFTWARE_REPORT(byte[] data)
            {
                VERSION_ZWAVE_SOFTWARE_REPORT ret = new VERSION_ZWAVE_SOFTWARE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.sdkVersion = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.applicationFrameworkApiVersion = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.applicationFrameworkBuildNumber = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.hostInterfaceVersion = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.hostInterfaceBuildNumber = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.zWaveProtocolVersion = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.zWaveProtocolBuildNumber = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.applicationVersion = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.applicationBuildNumber = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](VERSION_ZWAVE_SOFTWARE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_VERSION_V3.ID);
                ret.Add(ID);
                ret.Add(command.sdkVersion[0]);
                ret.Add(command.sdkVersion[1]);
                ret.Add(command.sdkVersion[2]);
                ret.Add(command.applicationFrameworkApiVersion[0]);
                ret.Add(command.applicationFrameworkApiVersion[1]);
                ret.Add(command.applicationFrameworkApiVersion[2]);
                ret.Add(command.applicationFrameworkBuildNumber[0]);
                ret.Add(command.applicationFrameworkBuildNumber[1]);
                ret.Add(command.hostInterfaceVersion[0]);
                ret.Add(command.hostInterfaceVersion[1]);
                ret.Add(command.hostInterfaceVersion[2]);
                ret.Add(command.hostInterfaceBuildNumber[0]);
                ret.Add(command.hostInterfaceBuildNumber[1]);
                ret.Add(command.zWaveProtocolVersion[0]);
                ret.Add(command.zWaveProtocolVersion[1]);
                ret.Add(command.zWaveProtocolVersion[2]);
                ret.Add(command.zWaveProtocolBuildNumber[0]);
                ret.Add(command.zWaveProtocolBuildNumber[1]);
                ret.Add(command.applicationVersion[0]);
                ret.Add(command.applicationVersion[1]);
                ret.Add(command.applicationVersion[2]);
                ret.Add(command.applicationBuildNumber[0]);
                ret.Add(command.applicationBuildNumber[1]);
                return ret.ToArray();
            }
        }
    }
}

