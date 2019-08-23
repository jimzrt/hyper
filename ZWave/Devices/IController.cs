using ZWave.Enums;

namespace ZWave.Devices
{
    public interface IController : IDevice
    {
        ControllerRoles NetworkRole { get; }
        byte[] IncludedNodes { get; set; }
    }
}
