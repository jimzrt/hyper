using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetListenBeforeTalkThresholdOperation : ControlApiOperation
    {
        private byte Channel { get; set; }
        private byte Threshhold { get; set; }
        public SetListenBeforeTalkThresholdOperation(byte channel, byte threshhold)
            : base(CommandTypes.CmdSetListenBeforeTalkThreshold, false)
        {
            Channel = channel;
            Threshhold = threshhold;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { Channel, Threshhold };
        }
    }
}
