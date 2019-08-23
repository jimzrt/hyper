using System;

namespace ZWave.Security
{
    public class SecurityManagerBase
    {
    }

    public struct NodeGroupId : IEquatable<NodeGroupId>
    {
        public byte NodeId;
        public byte GroupId;
        public NodeGroupId(byte nodeId, byte groupId)
        {
            NodeId = nodeId;
            GroupId = groupId;
        }

        public NodeGroupId(int value)
        {
            NodeId = (byte)(value >> 8);
            GroupId = (byte)value;
        }

        public bool Equals(NodeGroupId other)
        {
            return NodeId == other.NodeId && GroupId == other.GroupId;
        }
    }

    public struct OrdinalPeerNodeId : IEquatable<OrdinalPeerNodeId>
    {
        public byte NodeId1;
        public byte NodeId2;
        public OrdinalPeerNodeId(byte nodeId1, byte nodeId2)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
        }

        public OrdinalPeerNodeId(int value)
        {
            NodeId1 = (byte)(value >> 8);
            NodeId2 = (byte)value;
        }

        public bool Equals(OrdinalPeerNodeId other)
        {
            return NodeId1 == other.NodeId1 && NodeId2 == other.NodeId2;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", NodeId1, NodeId2);
        }
    }

    public struct InvariantPeerNodeId : IEquatable<InvariantPeerNodeId>
    {
        public byte NodeId1;
        public byte NodeId2;
        public InvariantPeerNodeId(byte nodeId1, byte nodeId2)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
        }

        public InvariantPeerNodeId(int value)
        {
            NodeId1 = (byte)(value >> 8);
            NodeId2 = (byte)value;
        }

        public bool Equals(InvariantPeerNodeId other)
        {
            return
                NodeId1 == other.NodeId1 && NodeId2 == other.NodeId2 ||
                NodeId1 == other.NodeId2 && NodeId2 == other.NodeId1;
        }

        public bool IsEmpty
        {
            get { return NodeId1 == 0 && NodeId2 == 0; }
        }

    }
}
