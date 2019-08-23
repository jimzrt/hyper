using System.Collections.Generic;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x1B | bNodeID | NodeMask[29]
    /// ZW->HOST: RES | 0x1B | retVal
    /// </summary>
    public class SetRoutingInfoOperation : ControlApiOperation
    {
        private byte NodeId { get; set; }
        private byte[] NodeMask { get; set; }
        public SetRoutingInfoOperation(byte nodeId, byte[] nodeMask)
            : base(CommandTypes.CmdZWaveSetRoutingInfo, false)
        {
            NodeId = nodeId;
            NodeMask = nodeMask;
        }

        protected override byte[] CreateInputParameters()
        {
            List<byte> ret = new List<byte>();
            ret.Add(NodeId);
            ret.AddRange(NodeMask);
            return ret.ToArray();
        }
    }
}
