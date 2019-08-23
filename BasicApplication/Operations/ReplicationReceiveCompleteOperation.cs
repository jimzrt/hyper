using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ReplicationReceiveCompleteOperation : ControlNApiOperation
    {
        public ReplicationReceiveCompleteOperation()
            : base(CommandTypes.CmdZWaveReplicationCommandComplete)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }
    }
}
