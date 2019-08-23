using System;
using ZWave.Enums;

namespace ZWave.Layers
{
    /// <summary>
    /// Provides the features required to manipulate frame data.
    /// typically: [SOF, PayloadLength, Payload, Checksum]
    /// </summary>
    public interface IDataFrame
    {
        /// <summary>
        /// 
        /// </summary>
        byte SessionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        ApiTypes ApiType { get; set; }
        /// <summary>
        /// Content of the data frame. 
        /// </summary>
        byte[] Buffer { get; }
        /// <summary>
        /// Content of the data frame. Exclude SOF marker, length and CRC bytes
        /// </summary>
        byte[] Data { get; }
        /// <summary>
        /// Payload
        /// </summary>
        /// <returns></returns>
        byte[] Payload { get; }
        /// <summary>
        /// Timestamp
        /// </summary>
        DateTime SystemTimeStamp { get; }
        /// <summary>
        /// Is outcoming frame, otherwise frame is incoming
        /// </summary>
        bool IsOutcome { get; }
        /// <summary>
        /// Is frame ACK, NAK or CAN
        /// </summary>
        DataFrameTypes DataFrameType { get; }
        /// <summary>
        /// Is frame substituted
        /// </summary>
        bool IsSubstituted { get; set; }
        /// <summary>
        /// Is frame handled completly
        /// </summary>
        bool IsHandled { get; set; }
    }
}
