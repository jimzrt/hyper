using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class AreNodesNeighboursOperation : ControlApiOperation
    {
        private byte NodeId1 { get; set; }
        private byte NodeId2 { get; set; }
        public AreNodesNeighboursOperation(byte nodeId1, byte nodeId2)
            : base(CommandTypes.CmdAreNodesNeighbours, false)
        {
            NodeId1 = nodeId1;
            NodeId2 = nodeId2;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { NodeId1, NodeId2 };
        }
    }
}
