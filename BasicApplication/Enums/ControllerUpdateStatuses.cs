namespace ZWave.BasicApplication.Enums
{
    public enum ControllerUpdateStatuses
    {
        NodeInfoIncludedReceived = 0x86,
        NodeInfoSmartStartReceived = 0x85,
        NodeInfoReceived = 0x84,
        NodeInfoReqDone = 0x82,
        NodeInfoReqFailed = 0x81,
        RoutingPending = 0x80,
        NewIdAssigned = 0x40,
        DeleteDone = 0x20,
        SucId = 0x10
    }
}
