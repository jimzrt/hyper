using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SOUND_SWITCH
    {
        public const byte ID = 0x79;
        public const byte VERSION = 1;
        public class SOUND_SWITCH_TONES_NUMBER_GET
        {
            public const byte ID = 0x01;
            public static implicit operator SOUND_SWITCH_TONES_NUMBER_GET(byte[] data)
            {
                SOUND_SWITCH_TONES_NUMBER_GET ret = new SOUND_SWITCH_TONES_NUMBER_GET();
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONES_NUMBER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_TONES_NUMBER_REPORT
        {
            public const byte ID = 0x02;
            public byte supportedTones;
            public static implicit operator SOUND_SWITCH_TONES_NUMBER_REPORT(byte[] data)
            {
                SOUND_SWITCH_TONES_NUMBER_REPORT ret = new SOUND_SWITCH_TONES_NUMBER_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedTones = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONES_NUMBER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.supportedTones);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_TONE_INFO_GET
        {
            public const byte ID = 0x03;
            public byte toneIdentifier;
            public static implicit operator SOUND_SWITCH_TONE_INFO_GET(byte[] data)
            {
                SOUND_SWITCH_TONE_INFO_GET ret = new SOUND_SWITCH_TONE_INFO_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.toneIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONE_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.toneIdentifier);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_TONE_INFO_REPORT
        {
            public const byte ID = 0x04;
            public byte toneIdentifier;
            public byte[] toneDuration = new byte[2];
            public byte nameLength;
            public IList<byte> name = new List<byte>();
            public static implicit operator SOUND_SWITCH_TONE_INFO_REPORT(byte[] data)
            {
                SOUND_SWITCH_TONE_INFO_REPORT ret = new SOUND_SWITCH_TONE_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.toneIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.toneDuration = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.nameLength = data.Length > index ? data[index++] : (byte)0x00;
                    ret.name = new List<byte>();
                    for (int i = 0; i < ret.nameLength; i++)
                    {
                        ret.name.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONE_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.toneIdentifier);
                ret.Add(command.toneDuration[0]);
                ret.Add(command.toneDuration[1]);
                ret.Add(command.nameLength);
                if (command.name != null)
                {
                    foreach (var tmp in command.name)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_CONFIGURATION_SET
        {
            public const byte ID = 0x05;
            public byte volume;
            public byte defaultToneIdentifier;
            public static implicit operator SOUND_SWITCH_CONFIGURATION_SET(byte[] data)
            {
                SOUND_SWITCH_CONFIGURATION_SET ret = new SOUND_SWITCH_CONFIGURATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.volume = data.Length > index ? data[index++] : (byte)0x00;
                    ret.defaultToneIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_CONFIGURATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.volume);
                ret.Add(command.defaultToneIdentifier);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_CONFIGURATION_GET
        {
            public const byte ID = 0x06;
            public static implicit operator SOUND_SWITCH_CONFIGURATION_GET(byte[] data)
            {
                SOUND_SWITCH_CONFIGURATION_GET ret = new SOUND_SWITCH_CONFIGURATION_GET();
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_CONFIGURATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_CONFIGURATION_REPORT
        {
            public const byte ID = 0x07;
            public byte volume;
            public byte defaultToneIdentifer;
            public static implicit operator SOUND_SWITCH_CONFIGURATION_REPORT(byte[] data)
            {
                SOUND_SWITCH_CONFIGURATION_REPORT ret = new SOUND_SWITCH_CONFIGURATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.volume = data.Length > index ? data[index++] : (byte)0x00;
                    ret.defaultToneIdentifer = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_CONFIGURATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.volume);
                ret.Add(command.defaultToneIdentifer);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_TONE_PLAY_SET
        {
            public const byte ID = 0x08;
            public byte toneIdentifier;
            public static implicit operator SOUND_SWITCH_TONE_PLAY_SET(byte[] data)
            {
                SOUND_SWITCH_TONE_PLAY_SET ret = new SOUND_SWITCH_TONE_PLAY_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.toneIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONE_PLAY_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.toneIdentifier);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_TONE_PLAY_GET
        {
            public const byte ID = 0x09;
            public static implicit operator SOUND_SWITCH_TONE_PLAY_GET(byte[] data)
            {
                SOUND_SWITCH_TONE_PLAY_GET ret = new SOUND_SWITCH_TONE_PLAY_GET();
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONE_PLAY_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class SOUND_SWITCH_TONE_PLAY_REPORT
        {
            public const byte ID = 0x0A;
            public byte toneIdentifier;
            public static implicit operator SOUND_SWITCH_TONE_PLAY_REPORT(byte[] data)
            {
                SOUND_SWITCH_TONE_PLAY_REPORT ret = new SOUND_SWITCH_TONE_PLAY_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.toneIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SOUND_SWITCH_TONE_PLAY_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SOUND_SWITCH.ID);
                ret.Add(ID);
                ret.Add(command.toneIdentifier);
                return ret.ToArray();
            }
        }
    }
}

