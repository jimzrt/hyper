using ZWave.Enums;

namespace ZWave.BasicApplication.Security
{
    public class RetransmissionRecord
    {
        public byte[] Data { get; set; }
        public byte Counter { get; set; }
        public SubstituteSettings SubstituteSettings { get; set; }
        public SecuritySchemes SecurityScheme { get; set; }
        public RetransmissionRecord(byte[] data)
        {
            Data = data;
        }
    }
}
