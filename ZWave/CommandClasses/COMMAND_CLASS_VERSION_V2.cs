using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_VERSION_V2
    {
        public const byte ID = 0x86;
        public const byte VERSION = 2;
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
                ret.Add(COMMAND_CLASS_VERSION_V2.ID);
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
                ret.Add(COMMAND_CLASS_VERSION_V2.ID);
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
                ret.Add(COMMAND_CLASS_VERSION_V2.ID);
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
                ret.Add(COMMAND_CLASS_VERSION_V2.ID);
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
    }
}

