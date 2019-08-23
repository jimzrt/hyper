namespace ZWave.BasicApplication.Enums
{
    public enum ProtocolStatuses
    {
        /// <summary>
        /// Protocol is idle
        /// </summary>
        Idle = 0x00,
        /// <summary>
        /// Protocol is analyzing the routing table
        /// </summary>
        Routing = 0x01,
        /// <summary>
        /// SUC sends pending updates
        /// </summary>
        SUC = 0x02
    }
}
