namespace ZWave.BasicApplication.Enums
{
    public enum FailedNodeStatuses
    {
        ZW_NODE_OK = 0x00,
        ZW_FAILED_NODE_REMOVED = 0x01,
        ZW_FAILED_NODE_NOT_REMOVED = 0x02,
        ZW_FAILED_NODE_REPLACE = 0x03,
        ZW_FAILED_NODE_REPLACE_DONE = 0x04,
        ZW_FAILED_NODE_REPLACE_FAILED = 0x05
    }
}
