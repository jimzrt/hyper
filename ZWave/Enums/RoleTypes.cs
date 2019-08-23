namespace ZWave.Enums
{
    public enum RoleTypes
    {
        None = 0xFF,
        CONTROLLER_CENTRAL_STATIC = 0x00,
        CONTROLLER_SUB_STATIC = 0x01,
        CONTROLLER_PORTABLE = 0x02,
        CONTROLLER_PORTABLE_REPORTING = 0x03,
        SLAVE_PORTABLE = 0x04,
        SLAVE_ALWAYS_ON = 0x05,
        SLAVE_SLEEPING_REPORTING = 0x06,
        SLAVE_SLEEPING_LISTENING = 0x07
    }
}
