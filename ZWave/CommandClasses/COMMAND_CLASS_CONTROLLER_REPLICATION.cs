using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_CONTROLLER_REPLICATION
    {
        public const byte ID = 0x21;
        public const byte VERSION = 1;
        public class CTRL_REPLICATION_TRANSFER_GROUP
        {
            public const byte ID = 0x31;
            public byte sequenceNumber;
            public byte groupId;
            public byte nodeId;
            public static implicit operator CTRL_REPLICATION_TRANSFER_GROUP(byte[] data)
            {
                CTRL_REPLICATION_TRANSFER_GROUP ret = new CTRL_REPLICATION_TRANSFER_GROUP();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.groupId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CTRL_REPLICATION_TRANSFER_GROUP command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONTROLLER_REPLICATION.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.groupId);
                ret.Add(command.nodeId);
                return ret.ToArray();
            }
        }
        public class CTRL_REPLICATION_TRANSFER_GROUP_NAME
        {
            public const byte ID = 0x32;
            public byte sequenceNumber;
            public byte groupId;
            public IList<byte> groupName = new List<byte>();
            public static implicit operator CTRL_REPLICATION_TRANSFER_GROUP_NAME(byte[] data)
            {
                CTRL_REPLICATION_TRANSFER_GROUP_NAME ret = new CTRL_REPLICATION_TRANSFER_GROUP_NAME();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.groupId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.groupName = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.groupName.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CTRL_REPLICATION_TRANSFER_GROUP_NAME command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONTROLLER_REPLICATION.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.groupId);
                if (command.groupName != null)
                {
                    foreach (var tmp in command.groupName)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class CTRL_REPLICATION_TRANSFER_SCENE
        {
            public const byte ID = 0x33;
            public byte sequenceNumber;
            public byte sceneId;
            public byte nodeId;
            public byte level;
            public static implicit operator CTRL_REPLICATION_TRANSFER_SCENE(byte[] data)
            {
                CTRL_REPLICATION_TRANSFER_SCENE ret = new CTRL_REPLICATION_TRANSFER_SCENE();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nodeId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.level = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](CTRL_REPLICATION_TRANSFER_SCENE command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONTROLLER_REPLICATION.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.sceneId);
                ret.Add(command.nodeId);
                ret.Add(command.level);
                return ret.ToArray();
            }
        }
        public class CTRL_REPLICATION_TRANSFER_SCENE_NAME
        {
            public const byte ID = 0x34;
            public byte sequenceNumber;
            public byte sceneId;
            public IList<byte> sceneName = new List<byte>();
            public static implicit operator CTRL_REPLICATION_TRANSFER_SCENE_NAME(byte[] data)
            {
                CTRL_REPLICATION_TRANSFER_SCENE_NAME ret = new CTRL_REPLICATION_TRANSFER_SCENE_NAME();
                if (data != null)
                {
                    int index = 2;
                    ret.sequenceNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sceneId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sceneName = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.sceneName.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](CTRL_REPLICATION_TRANSFER_SCENE_NAME command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_CONTROLLER_REPLICATION.ID);
                ret.Add(ID);
                ret.Add(command.sequenceNumber);
                ret.Add(command.sceneId);
                if (command.sceneName != null)
                {
                    foreach (var tmp in command.sceneName)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

