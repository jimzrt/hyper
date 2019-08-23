using System;
using Utils;
using ZWave.Enums;
using ZWave.Operations;

namespace ZWave.Layers
{
    public interface IApplicationClient : IDisposable
    {
        byte SessionId { get; set; }
        ITransportClient TransportClient { get; set; }
        IFrameClient FrameClient { get; set; }
        ISessionClient SessionClient { get; set; }
        IDataSource DataSource { get; set; }
        CommunicationStatuses Connect(IDataSource ds);
        CommunicationStatuses Connect();
        void Disconnect();
        void Cancel(ActionToken token);
        ActionToken Listen(ByteIndex[] mask, Action<byte[]> data);
        ExpectResult Expect(ByteIndex[] mask, int timeoutMs);
        ActionToken Expect(ByteIndex[] mask, int timeoutMs, Action<IActionItem> completedCallback);
        SendResult Send(byte[] data);
        ActionToken Send(byte[] data, Action<IActionItem> completedCallback);
        RequestResult Request(byte[] data, ByteIndex[] mask, int timeoutMs);
        ActionToken Request(byte[] data, ByteIndex[] mask, int timeoutMs, Action<IActionItem> completedCallback);
    }
}
