using ZWave.Enums;
using ZWave.Layers;

namespace ZWave.Devices
{
    public interface IDevice : IApplicationClient
    {
        byte Id { get; set; }
        byte[] HomeId { get; set; }
        byte SucNodeId { get; set; }
        string Version { get; set; }
        Libraries Library { get; set; }
        ChipTypes ChipType { get; set; }
        byte ChipRevision { get; set; }
        NetworkViewPoint Network { get; set; }
    }
}
