using System;

namespace ZWave.Enums
{
    public enum Modes
    {
        None = 0,
        NodeAny = 1,
        NodeController = 2,
        [Obsolete]
        NodeSlave = 3,
        [Obsolete]
        NodeExisting = 4,
        NodeStop = 5,
        NodeStopFailed = 6,
        NodeAnyS2 = 7,
        NodeHomeId = 8,
        NodeSmartStart = 9,
        NodeOptionNetworkWide = 0x40, // Flag
        NodeOptionHighPower = 0x80, // Flag
        FlagModes = NodeOptionNetworkWide | NodeOptionHighPower
    }
}

