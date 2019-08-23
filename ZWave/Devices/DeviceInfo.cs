namespace ZWave.Devices
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {
        }

        public DeviceInfo(byte nodeId)
        {
            NodeTag = new NodeTag(nodeId, 0);
        }

        public DeviceInfo(NodeTag nodeId)
        {
            NodeTag = nodeId;
        }

        public byte Id
        {
            get { return NodeTag.Id; }
        }

        public byte EndPointId
        {
            get { return NodeTag.EndPointId; }
        }

        public NodeTag NodeTag { get; }
    }
}
