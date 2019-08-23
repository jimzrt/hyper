using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_BARRIER_OPERATOR
    {
        public const byte ID = 0x66;
        public const byte VERSION = 1;
        public class BARRIER_OPERATOR_SET
        {
            public const byte ID = 0x01;
            public byte targetValue;
            public static implicit operator BARRIER_OPERATOR_SET(byte[] data)
            {
                BARRIER_OPERATOR_SET ret = new BARRIER_OPERATOR_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.targetValue = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                ret.Add(command.targetValue);
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_GET
        {
            public const byte ID = 0x02;
            public static implicit operator BARRIER_OPERATOR_GET(byte[] data)
            {
                BARRIER_OPERATOR_GET ret = new BARRIER_OPERATOR_GET();
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_REPORT
        {
            public const byte ID = 0x03;
            public byte state;
            public static implicit operator BARRIER_OPERATOR_REPORT(byte[] data)
            {
                BARRIER_OPERATOR_REPORT ret = new BARRIER_OPERATOR_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.state = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                ret.Add(command.state);
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_SIGNAL_SUPPORTED_GET
        {
            public const byte ID = 0x04;
            public static implicit operator BARRIER_OPERATOR_SIGNAL_SUPPORTED_GET(byte[] data)
            {
                BARRIER_OPERATOR_SIGNAL_SUPPORTED_GET ret = new BARRIER_OPERATOR_SIGNAL_SUPPORTED_GET();
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_SIGNAL_SUPPORTED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_SIGNAL_SUPPORTED_REPORT
        {
            public const byte ID = 0x05;
            public IList<byte> bitMask = new List<byte>();
            public static implicit operator BARRIER_OPERATOR_SIGNAL_SUPPORTED_REPORT(byte[] data)
            {
                BARRIER_OPERATOR_SIGNAL_SUPPORTED_REPORT ret = new BARRIER_OPERATOR_SIGNAL_SUPPORTED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.bitMask = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.bitMask.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_SIGNAL_SUPPORTED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                if (command.bitMask != null)
                {
                    foreach (var tmp in command.bitMask)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_SIGNAL_SET
        {
            public const byte ID = 0x06;
            public byte subsystemType;
            public byte subsystemState;
            public static implicit operator BARRIER_OPERATOR_SIGNAL_SET(byte[] data)
            {
                BARRIER_OPERATOR_SIGNAL_SET ret = new BARRIER_OPERATOR_SIGNAL_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.subsystemType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.subsystemState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_SIGNAL_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                ret.Add(command.subsystemType);
                ret.Add(command.subsystemState);
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_SIGNAL_GET
        {
            public const byte ID = 0x07;
            public byte subsystemType;
            public static implicit operator BARRIER_OPERATOR_SIGNAL_GET(byte[] data)
            {
                BARRIER_OPERATOR_SIGNAL_GET ret = new BARRIER_OPERATOR_SIGNAL_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.subsystemType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_SIGNAL_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                ret.Add(command.subsystemType);
                return ret.ToArray();
            }
        }
        public class BARRIER_OPERATOR_SIGNAL_REPORT
        {
            public const byte ID = 0x08;
            public byte subsystemType;
            public byte subsystemState;
            public static implicit operator BARRIER_OPERATOR_SIGNAL_REPORT(byte[] data)
            {
                BARRIER_OPERATOR_SIGNAL_REPORT ret = new BARRIER_OPERATOR_SIGNAL_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.subsystemType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.subsystemState = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](BARRIER_OPERATOR_SIGNAL_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_BARRIER_OPERATOR.ID);
                ret.Add(ID);
                ret.Add(command.subsystemType);
                ret.Add(command.subsystemState);
                return ret.ToArray();
            }
        }
    }
}

