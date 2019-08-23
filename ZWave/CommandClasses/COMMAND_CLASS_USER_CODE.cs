using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_USER_CODE
    {
        public const byte ID = 0x63;
        public const byte VERSION = 1;
        public class USER_CODE_GET
        {
            public const byte ID = 0x02;
            public byte userIdentifier;
            public static implicit operator USER_CODE_GET(byte[] data)
            {
                USER_CODE_GET ret = new USER_CODE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                return ret.ToArray();
            }
        }
        public class USER_CODE_REPORT
        {
            public const byte ID = 0x03;
            public byte userIdentifier;
            public byte userIdStatus;
            public IList<byte> userCode = new List<byte>();
            public static implicit operator USER_CODE_REPORT(byte[] data)
            {
                USER_CODE_REPORT ret = new USER_CODE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userCode = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.userIdStatus);
                if (command.userCode != null)
                {
                    foreach (var tmp in command.userCode)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class USER_CODE_SET
        {
            public const byte ID = 0x01;
            public byte userIdentifier;
            public byte userIdStatus;
            public IList<byte> userCode = new List<byte>();
            public static implicit operator USER_CODE_SET(byte[] data)
            {
                USER_CODE_SET ret = new USER_CODE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.userIdentifier = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userIdStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.userCode = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.userCode.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](USER_CODE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE.ID);
                ret.Add(ID);
                ret.Add(command.userIdentifier);
                ret.Add(command.userIdStatus);
                if (command.userCode != null)
                {
                    foreach (var tmp in command.userCode)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class USERS_NUMBER_GET
        {
            public const byte ID = 0x04;
            public static implicit operator USERS_NUMBER_GET(byte[] data)
            {
                USERS_NUMBER_GET ret = new USERS_NUMBER_GET();
                return ret;
            }
            public static implicit operator byte[](USERS_NUMBER_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class USERS_NUMBER_REPORT
        {
            public const byte ID = 0x05;
            public byte supportedUsers;
            public static implicit operator USERS_NUMBER_REPORT(byte[] data)
            {
                USERS_NUMBER_REPORT ret = new USERS_NUMBER_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.supportedUsers = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](USERS_NUMBER_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_USER_CODE.ID);
                ret.Add(ID);
                ret.Add(command.supportedUsers);
                return ret.ToArray();
            }
        }
    }
}

