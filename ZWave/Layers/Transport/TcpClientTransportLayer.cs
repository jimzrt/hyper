namespace ZWave.Layers.Transport
{
    public class TcpClientTransportLayer : TransportLayer
    {
        public override ITransportListener Listener { get; set; }

        public override ITransportClient CreateClient(byte sessionId)
        {
            var ret = new TcpClientTransportClient(TransmitCallback)
            {
                SuppressDebugOutput = SuppressDebugOutput,
                SessionId = sessionId
            };
            return ret;
        }
    }
}
