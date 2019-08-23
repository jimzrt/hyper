using Utils.UI;

namespace ZWave.Devices
{
    public class EndPoint : EntityBase
    {
        public byte Id { get; set; }
        public EndPoint(byte id)
        {
            Id = id;
        }
    }
}
