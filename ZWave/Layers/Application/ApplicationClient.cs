using System;
using System.Threading;
using Utils;
using Utils.UI;
using ZWave.Enums;
using ZWave.Operations;

namespace ZWave.Layers.Application
{
    public class ApplicationClient : EntityBase, IApplicationClient
    {
        public byte SessionId { get; set; }
        public ApiTypes ApiType { get; set; }
        public ITransportClient TransportClient { get; set; }
        public IFrameClient FrameClient { get; set; }
        public ISessionClient SessionClient { get; set; }
        public IDataSource DataSource
        {
            get { return TransportClient.DataSource; }
            set { TransportClient.DataSource = value; }
        }

        public ApplicationClient(ApiTypes apiType, byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
        {
            ApiType = apiType;
            SessionId = sessionId;
            SessionClient = sc;
            FrameClient = fc;
            TransportClient = tc;
            BindLayers();
        }

        private void BindLayers()
        {
            SessionClient.SessionId = SessionId;
            FrameClient.SessionId = SessionId;
            TransportClient.SessionId = SessionId;
            TransportClient.ApiType = ApiType;

            SessionClient.SendFramesCallback = FrameClient.SendFrames;
            FrameClient.ReceiveFrameCallback = SessionClient.HandleActionCase;
            FrameClient.SendDataCallback = TransportClient.WriteData;
            TransportClient.ReceiveDataCallback = FrameClient.HandleData;
        }

        private void UnBindLayers()
        {
            SessionClient.SendFramesCallback = null;
            FrameClient.ReceiveFrameCallback = null;
            FrameClient.SendDataCallback = null;
            TransportClient.ReceiveDataCallback = null;
        }

        #region IDisposable Members
        public void Dispose()
        {
            // stop session (wait any exclusive tokens to complete)
            SessionClient.Dispose();
            Thread.Sleep(42); // Allow frame layer sending acknowledge to last unsolicited frame.
            // stop frame buffer block
            FrameClient.Dispose();
            // stop transport
            TransportClient.Dispose();
            // unsubscribe events
            UnBindLayers();
        }

        #endregion

        #region IApplicationClient Members

        public virtual CommunicationStatuses Connect()
        {
            return TransportClient.Connect();
        }

        public virtual CommunicationStatuses Connect(IDataSource ds)
        {
            return TransportClient.Connect(ds);
        }

        public virtual void Disconnect()
        {
            TransportClient.Disconnect();
        }

        public void Cancel(ActionToken token)
        {
            SessionClient.Cancel(token);
        }

        public ActionToken Listen(ByteIndex[] mask, Action<byte[]> data)
        {
            return SessionClient.ExecuteAsync(new ListenOperation(mask, data));
        }

        public ExpectResult Expect(ByteIndex[] mask, int timeoutMs)
        {
            var token = SessionClient.ExecuteAsync(new ExpectOperation(mask, timeoutMs));
            return (ExpectResult)token.WaitCompletedSignal();
        }

        public ActionToken Expect(ByteIndex[] mask, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return SessionClient.ExecuteAsync(new ExpectOperation(mask, timeoutMs) { CompletedCallback = completedCallback });
        }

        public SendResult Send(byte[] data)
        {
            var token = SessionClient.ExecuteAsync(new SendOperation(data));
            return (SendResult)token.WaitCompletedSignal();
        }

        public ActionToken Send(byte[] data, Action<IActionItem> completedCallback)
        {
            return SessionClient.ExecuteAsync(new SendOperation(data) { CompletedCallback = completedCallback });
        }

        public RequestResult Request(byte[] data, ByteIndex[] mask, int timeoutMs)
        {
            var action = new RequestOperation(data, mask, timeoutMs);
            action.Token.LogEntryPointSource = DataSource == null ? "" : DataSource.SourceName;
            var token = SessionClient.ExecuteAsync(action);
            return (RequestResult)token.WaitCompletedSignal();
        }

        public ActionToken Request(byte[] data, ByteIndex[] mask, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return SessionClient.ExecuteAsync(new RequestOperation(data, mask, timeoutMs) { CompletedCallback = completedCallback });
        }

        #endregion
    }
}
