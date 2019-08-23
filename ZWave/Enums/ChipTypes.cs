using System.ComponentModel;

namespace ZWave.Enums
{
    public enum ChipTypes
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        [Description("Unknown")]
        UNKNOWN = 0x00,
        /// <summary>
        /// ZW010x chipt type (100 series).
        /// </summary>
        [Description("ZW010x")]
        ZW010x = 0x01,
        /// <summary>
        /// ZW020x chipt type (200 series).
        /// </summary>
        [Description("ZW020x")]
        ZW020x = 0x02,
        /// <summary>
        /// ZW030x chipt type (300 series).
        /// </summary>
        [Description("ZW030x")]
        ZW030x = 0x03,
        /// <summary>
        /// ZW040x chipt type (400 series).
        /// </summary>
        [Description("ZW040x")]
        ZW040x = 0x04,
        /// <summary>
        /// ZW050x chipt type (500 series).
        /// </summary>
        [Description("ZW050x")]
        ZW050x = 0x05,
        /// <summary>
        /// ZW070x chipt type (700 series).
        /// </summary>
        [Description("ZW070x")]
        ZW070x = 0x07
    }
}
