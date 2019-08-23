using System;
using System.Text;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Application;
using ZWave.TextApplication.Operations;

namespace ZWave.TextApplication
{
    public class TextDevice : ApplicationClient
    {
        public Encoding TextEncoding { get; set; }
        internal TextDevice(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(ApiTypes.Text, sessionId, sc, fc, tc)
        {
            TextEncoding = Encoding.ASCII;
        }

        public void Stop(Type taskType)
        {
            SessionClient.Cancel(taskType);
            //SessionLayer.WaitForCompletedSignal(SessionId);
        }

        public void ExecuteAsync(ActionBase action, Action<IActionItem> completedCallback)
        {
            action.CompletedCallback = completedCallback;
            action.Token.LogEntryPointCategory = "Text";
            action.Token.LogEntryPointSource = DataSource == null ? "" : DataSource.SourceName;
            SessionClient.ExecuteAsync(action);
        }

        public ActionResult Execute(ActionBase action)
        {
            action.Token.LogEntryPointCategory = "Text";
            action.Token.LogEntryPointSource = DataSource == null ? "" : DataSource.SourceName;
            var token = SessionClient.ExecuteAsync(action);
            token.WaitCompletedSignal();
            return token.Result;
        }

        public void Send(string text)
        {
            Execute(new SendOperation(text, TextEncoding));
        }

        public RequestResult Request(string text, int timeoutMs)
        {
            var res = (RequestResult)Execute(new RequestOperation(text, TextEncoding, timeoutMs));
            return res;
        }

        public ReceiveResult ReceiveData(int timeoutMs)
        {
            var res = (ReceiveResult)Execute(new ReceiveOperation(TextEncoding, timeoutMs));
            return res;
        }
    }
}
