namespace ZWave.Enums
{
    public enum ControllerChangeModes
    {
        Start = Modes.NodeController | Modes.NodeOptionHighPower,
        Stop = Modes.NodeStop,
        StopFailed = Modes.NodeStopFailed
    }
}
