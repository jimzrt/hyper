using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_POWERLEVEL
    {
        public const byte ID = 0x73;
        public const byte VERSION = 1;
        public class POWERLEVEL_GET
        {
            public const byte ID = 0x02;
            public static implicit operator POWERLEVEL_GET(byte[] data)
            {
                POWERLEVEL_GET ret = new POWERLEVEL_GET();
                return ret;
            }
            public static implicit operator byte[](POWERLEVEL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_POWERLEVEL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class POWERLEVEL_REPORT
        {
            public const byte ID = 0x03;
            public byte powerLevel;
            public byte timeout;
            public static implicit operator POWERLEVEL_REPORT(byte[] data)
            {
                POWERLEVEL_REPORT ret = new POWERLEVEL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.powerLevel = data.Length > index ? data[index++] : (byte)0x00;
                    ret.timeout = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](POWERLEVEL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_POWERLEVEL.ID);
                ret.Add(ID);
                ret.Add(command.powerLevel);
                ret.Add(command.timeout);
                return ret.ToArray();
            }
        }
        public class POWERLEVEL_SET
        {
            public const byte ID = 0x01;
            public byte powerLevel;
            public byte timeout;
            public static implicit operator POWERLEVEL_SET(byte[] data)
            {
                POWERLEVEL_SET ret = new POWERLEVEL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.powerLevel = data.Length > index ? data[index++] : (byte)0x00;
                    ret.timeout = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](POWERLEVEL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_POWERLEVEL.ID);
                ret.Add(ID);
                ret.Add(command.powerLevel);
                ret.Add(command.timeout);
                return ret.ToArray();
            }
        }
        public class POWERLEVEL_TEST_NODE_GET
        {
            public const byte ID = 0x05;
            public static implicit operator POWERLEVEL_TEST_NODE_GET(byte[] data)
            {
                POWERLEVEL_TEST_NODE_GET ret = new POWERLEVEL_TEST_NODE_GET();
                return ret;
            }
            public static implicit operator byte[](POWERLEVEL_TEST_NODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_POWERLEVEL.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class POWERLEVEL_TEST_NODE_REPORT
        {
            public const byte ID = 0x06;
            public byte testNodeid;
            public byte statusOfOperation;
            public byte[] testFrameCount = new byte[2];
            public static implicit operator POWERLEVEL_TEST_NODE_REPORT(byte[] data)
            {
                POWERLEVEL_TEST_NODE_REPORT ret = new POWERLEVEL_TEST_NODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.testNodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.statusOfOperation = data.Length > index ? data[index++] : (byte)0x00;
                    ret.testFrameCount = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](POWERLEVEL_TEST_NODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_POWERLEVEL.ID);
                ret.Add(ID);
                ret.Add(command.testNodeid);
                ret.Add(command.statusOfOperation);
                ret.Add(command.testFrameCount[0]);
                ret.Add(command.testFrameCount[1]);
                return ret.ToArray();
            }
        }
        public class POWERLEVEL_TEST_NODE_SET
        {
            public const byte ID = 0x04;
            public byte testNodeid;
            public byte powerLevel;
            public byte[] testFrameCount = new byte[2];
            public static implicit operator POWERLEVEL_TEST_NODE_SET(byte[] data)
            {
                POWERLEVEL_TEST_NODE_SET ret = new POWERLEVEL_TEST_NODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.testNodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.powerLevel = data.Length > index ? data[index++] : (byte)0x00;
                    ret.testFrameCount = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](POWERLEVEL_TEST_NODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_POWERLEVEL.ID);
                ret.Add(ID);
                ret.Add(command.testNodeid);
                ret.Add(command.powerLevel);
                ret.Add(command.testFrameCount[0]);
                ret.Add(command.testFrameCount[1]);
                return ret.ToArray();
            }
        }
    }
}

