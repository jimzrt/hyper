using System.Text;
using Utils;

namespace ZWave.TextApplication.Operations
{
    public class RequestOperation : ActionBase
    {
        private int TimeoutMs { get; set; }
        private Encoding TextEncoding { get; set; }
        private byte[] TextData { get; set; }
        private string Text { get; set; }
        public RequestOperation(byte[] textData, int timeoutMs)
            : base(true)
        {
            TimeoutMs = timeoutMs;
            TextData = textData;
        }

        public RequestOperation(string text, Encoding textEncoding, int timeoutMs)
            : base(true)
        {
            TextEncoding = textEncoding;
            TimeoutMs = timeoutMs;
            Text = text;
            TextData = textEncoding.GetBytes(text);
        }

        private CommandMessage message;
        private CommandHandler handler;
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, TimeoutMs, message));
            ActionUnits.Add(new DataReceivedUnit(handler, OnReceived));
        }

        protected override void CreateInstance()
        {
            message = new TextApiMessage(TextData);
            message.IsNoAck = true;
            handler = new CommandHandler();
            handler.AddConditions(ByteIndex.AnyValue);
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            SpecificResult.ReceivedData = ou.DataFrame.Payload;
            if (TextEncoding != null)
                SpecificResult.ReceivedText = TextEncoding.GetString(SpecificResult.ReceivedData);
            SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            if (Text != null && SpecificResult.ReceivedText != null)
            {
                string tt = Text.Replace("\r", "\\r");
                string rr = SpecificResult.ReceivedText.Replace("\r", "\\r");
                return string.Format("Tx:\"{0}\", Rx:\"{1}\"", tt, rr);
            }
            return Tools.GetHex(TextData);
        }

        public RequestResult SpecificResult
        {
            get { return (RequestResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new RequestResult();
        }
    }

    public class RequestResult : ActionResult
    {
        public byte[] ReceivedData { get; set; }
        public string ReceivedText { get; set; }
    }
}
