using ZWave.Enums;

namespace ZWave.BasicApplication.Enums
{
    public enum CreatePrimaryModes
    {
        Start = Modes.NodeController,
        Stop = Modes.NodeStop,
        StopFailed = Modes.NodeStopFailed
    }
}
