namespace ZWave.BasicApplication.Enums
{
    public enum HeaderTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Unknown = 0x00,
        /// <summary>
        /// HEADER_SOF
        /// </summary>
        StartOfFrame = 0x01,
        /// <summary>
        /// HEADER_ACK
        /// </summary>
        Acknowledge = 0x06,
        /// <summary>
        /// HEADER_NAK
        /// </summary>
        NotAcknowledged = 0x15,
        /// <summary>
        /// HEADER_CAN
        /// </summary>
        Can = 0x18
    }
}
