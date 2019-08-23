using System;
using System.IO;
using Utils;
using ZWave.Enums;
using ZWave.Exceptions;

namespace ZWave.Layers
{
    /// <summary>
    /// Provides data structure for logging communication on TransportLayer
    /// </summary>
    public class DataChunk
    {
        /// <summary>
        /// Gets or sets the position in the file storage.
        /// </summary>
        /// <value>The position.</value>
        public long Position { get; set; }
        /// <summary>
        /// The maximum length of the data buffer.
        /// </summary>
        private const int MAX_DATA_BUFFER_LENGTH = int.MaxValue;
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// Is incoming frame, otherwise frame is outgoing
        /// </summary>
        /// <value>The type.</value>
        public bool IsOutcome { get; set; }
        /// <summary>
        /// Gets or sets the Session Id
        /// </summary>
        public byte SessionId { get; set; }
        /// <summary>
        /// Gets or sets the length of the data buffer.
        /// </summary>
        /// <value>The length of the data buffer.</value>
        public int DataBufferLength { get; set; }

        public int TotalBytes
        {
            get { return 14 + DataBufferLength; }
        }

        protected byte[] DataBuffer { get; set; }
        /// <summary>
        /// Gets or sets the data buffer.
        /// </summary>
        /// <value>The data buffer.</value>
        public byte[] GetDataBuffer()
        {
            return DataBuffer;
        }
        /// <summary>
        /// Gets or sets the API type
        /// 0 - Zniffer API; (FE - 0 = FE)
        /// 1 - Basic API; (FE - 1 = FD)
        /// 2 - Programmer API. (FE - 2 = FC)
        /// 3 - Zip API. (FE - 3 = FB)
        /// 7 - Text (FE - 7 = F7)
        /// </summary>
        public ApiTypes ApiType
        {
            get { return (ApiTypes)(byte)(0xFE - EOD); }
            set { EOD = (byte)(0xFE - (byte)value); }
        }

        public byte EOD { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataChunk"/> class.
        /// </summary>
        private DataChunk()
        {
        }

        public override string ToString()
        {
            return Tools.GetHex(ToByteArray());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataChunk"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public DataChunk(byte[] data, byte sessionId, bool isOutcome, ApiTypes apiType)
        {
            IsOutcome = isOutcome;
            SessionId = sessionId;
            TimeStamp = Tools.CurrentDateTime;
            DataBufferLength = data != null ? data.Length : 0;
            DataBuffer = data;
            ApiType = apiType;
        }

        /// <summary>
        /// Toes the byte array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            DataBufferLength = OnToByteArray();
            byte[] array = new byte[14 + DataBufferLength];
            Array.Copy(BitConverter.GetBytes(TimeStamp.ToBinary()), array, 8);
            // IsOutcome: 7 bit, SessionId: 0-6 bits
            if (SessionId >= 0x80)
                throw new ZWaveException("SessionId can't be greater or equal 0x80");
            array[8] = IsOutcome ? (byte)(0x80 + SessionId) : SessionId;
            Array.Copy(BitConverter.GetBytes(DataBufferLength), 0, array, 9, 4);
            for (int i = 0; i < DataBufferLength; i++)
            {
                array[i + 13] = DataBuffer[i];
            }
            array[13 + DataBufferLength] = EOD; // see ApiType
            return array;
        }

        public virtual int OnToByteArray()
        {
            int ret = DataBufferLength;
            return ret;
        }

        private static DateTime defaultDateTime = new DateTime(2010, 1, 1);
        /// <summary>
        /// Reads DataChunk from stream. Returns null if not enough data to read.
        /// </summary>
        /// <param name="bReader"></param>
        /// <returns></returns>
        public static DataChunk ReadDataChunk(BinaryReader bReader)
        {
            return ReadDataChunk(bReader, MAX_DATA_BUFFER_LENGTH);
        }

        public static DataChunk ReadDataChunk(BinaryReader bReader, int maxLength)
        {
            DataChunk dc = new DataChunk();
            dc.Position = bReader.BaseStream.Position;
            byte[] tmpBuffer = bReader.ReadBytes(8);
            if (tmpBuffer.Length != 8)
                return null;
            try
            {
                var ticks = BitConverter.ToInt64(tmpBuffer, 0);
                dc.TimeStamp = DateTime.FromBinary(ticks);
            }
            catch (ArgumentException)
            {
                dc.TimeStamp = defaultDateTime;
                //return null;
            }

            tmpBuffer = bReader.ReadBytes(1);
            if (tmpBuffer.Length != 1)
                return null;

            dc.IsOutcome = tmpBuffer[0] >= 0x80;
            dc.SessionId = (byte)(tmpBuffer[0] & 0x7F);

            tmpBuffer = bReader.ReadBytes(4);
            if (tmpBuffer.Length != 4)
                return null;

            dc.DataBufferLength = BitConverter.ToInt32(tmpBuffer, 0);

            if (dc.DataBufferLength >= 0 && dc.DataBufferLength < maxLength)
            {
                tmpBuffer = bReader.ReadBytes(dc.DataBufferLength);
                if (tmpBuffer.Length != dc.DataBufferLength)
                    return null;

                dc.DataBuffer = new byte[dc.DataBufferLength];
                Array.Copy(tmpBuffer, dc.GetDataBuffer(), dc.DataBufferLength);
            }
            else
                return null;

            tmpBuffer = bReader.ReadBytes(1);
            if (tmpBuffer.Length != 1)
                return null;

            dc.EOD = tmpBuffer[0];
            return dc;
        }
    }
}
