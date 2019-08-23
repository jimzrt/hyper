using System;
using ZWave.Layers.Frame;

namespace ZWave.Layers
{
    public interface IFrameClient : IDisposable
    {
        byte SessionId { get; set; }
        Action<CustomDataFrame> ReceiveFrameCallback { get; set; }
        Func<byte[], int> SendDataCallback { get; set; }
        void HandleData(DataChunk dataChunk, bool isFromFile);
        bool SendFrames(ActionHandlerResult frameData);
        void ResetParser();
    }
}
