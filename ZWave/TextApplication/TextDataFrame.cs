using System;
using System.Text;
using ZWave.Enums;
using ZWave.Layers.Frame;

namespace ZWave.TextApplication
{
    public class TextDataFrame : CustomDataFrame
    {
        private const int MAX_LENGTH = 4000;

        public TextDataFrame(byte sessionId, DataFrameTypes type, bool isHandled, bool isOutcome, DateTime timeStamp)
            : base(sessionId, type, isHandled, isOutcome, timeStamp)
        {
            ApiType = ApiTypes.Text;
        }

        protected override int GetMaxLength()
        {
            return MAX_LENGTH;
        }

        /// <summary>
        /// Returns buffer
        /// </summary>
        /// <returns></returns>
        protected override byte[] RefreshData()
        {
            return Buffer;
        }

        /// <summary>
        /// Returns payload
        /// </summary>
        /// <returns></returns>
        protected override byte[] RefreshPayload()
        {
            return Buffer;
        }

        public override String ToString()
        {
            return Encoding.ASCII.GetString(Buffer);
        }
    }
}
