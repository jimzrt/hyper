using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_WAKE_UP
    {
        public const byte ID = 0x84;
        public const byte VERSION = 1;
        public class WAKE_UP_INTERVAL_GET
        {
            public const byte ID = 0x05;
            public static implicit operator WAKE_UP_INTERVAL_GET(byte[] data)
            {
                WAKE_UP_INTERVAL_GET ret = new WAKE_UP_INTERVAL_GET();
                return ret;
            }
            public static implicit operator byte[](WAKE_UP_INTERVAL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WAKE_UP.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class WAKE_UP_INTERVAL_REPORT
        {
            public const byte ID = 0x06;
            public byte[] seconds = new byte[3];
            public byte nodeid;
            public static implicit operator WAKE_UP_INTERVAL_REPORT(byte[] data)
            {
                WAKE_UP_INTERVAL_REPORT ret = new WAKE_UP_INTERVAL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.seconds = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WAKE_UP_INTERVAL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WAKE_UP.ID);
                ret.Add(ID);
                ret.Add(command.seconds[0]);
                ret.Add(command.seconds[1]);
                ret.Add(command.seconds[2]);
                ret.Add(command.nodeid);
                return ret.ToArray();
            }
        }
        public class WAKE_UP_INTERVAL_SET
        {
            public const byte ID = 0x04;
            public byte[] seconds = new byte[3];
            public byte nodeid;
            public static implicit operator WAKE_UP_INTERVAL_SET(byte[] data)
            {
                WAKE_UP_INTERVAL_SET ret = new WAKE_UP_INTERVAL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.seconds = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](WAKE_UP_INTERVAL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WAKE_UP.ID);
                ret.Add(ID);
                ret.Add(command.seconds[0]);
                ret.Add(command.seconds[1]);
                ret.Add(command.seconds[2]);
                ret.Add(command.nodeid);
                return ret.ToArray();
            }
        }
        public class WAKE_UP_NO_MORE_INFORMATION
        {
            public const byte ID = 0x08;
            public static implicit operator WAKE_UP_NO_MORE_INFORMATION(byte[] data)
            {
                WAKE_UP_NO_MORE_INFORMATION ret = new WAKE_UP_NO_MORE_INFORMATION();
                return ret;
            }
            public static implicit operator byte[](WAKE_UP_NO_MORE_INFORMATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WAKE_UP.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class WAKE_UP_NOTIFICATION
        {
            public const byte ID = 0x07;
            public static implicit operator WAKE_UP_NOTIFICATION(byte[] data)
            {
                WAKE_UP_NOTIFICATION ret = new WAKE_UP_NOTIFICATION();
                return ret;
            }
            public static implicit operator byte[](WAKE_UP_NOTIFICATION command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_WAKE_UP.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
    }
}

