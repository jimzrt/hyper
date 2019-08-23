namespace ZWave.Layers.Transport
{
    public class TextTransportLayer : TransportLayer
    {
        public override ITransportListener Listener { get; set; }
        public override ITransportClient CreateClient(byte sessionId)
        {
            TextTransportClient ret = new TextTransportClient(TransmitCallback)
            {
                SuppressDebugOutput = SuppressDebugOutput,
                SessionId = sessionId
            };
            return ret;
        }
    }
}
