using System.Collections;
using ZWave.Enums;

namespace ZWave.Layers.Application
{
    public abstract class ApplicationLayer : IApplicationLayer
    {
        private const int MAX_SESSIONS = 0x7F;
        private BitArray SessionIds = new BitArray(MAX_SESSIONS);
        public bool SuppressDebugOutput { get; set; }
        public ISessionLayer SessionLayer { get; set; }
        public IFrameLayer FrameLayer { get; set; }
        public ITransportLayer TransportLayer { get; set; }
        public ApiTypes ApiType { get; set; }
        private ApplicationLayer()
        {
        }

        public ApplicationLayer(ApiTypes apiType, ISessionLayer sessionLayer, IFrameLayer frameLayer, ITransportLayer transportLayer)
        {
            ApiType = apiType;
            SessionLayer = sessionLayer;
            FrameLayer = frameLayer;
            TransportLayer = transportLayer;
        }

        public void ResetSessionIdCounter()
        {
            SessionIds = new BitArray(MAX_SESSIONS);
        }

        virtual public byte NextSessionId()
        {
            byte ret = 0;
            for (int i = 0; i < SessionIds.Length; i++)
            {
                if (SessionIds[i] == false)
                {
                    SessionIds[i] = true;
                    break;
                }
                ret++;
            }
            return ret;
        }

        public void ReleaseSessionId(byte sessionId)
        {
            if (sessionId < SessionIds.Length)
                SessionIds[sessionId] = false;
        }

        public ApplicationClient CreateClient()
        {
            var sessionId = NextSessionId();
            ApplicationClient ret = new ApplicationClient(ApiType, sessionId, SessionLayer.CreateClient(), FrameLayer.CreateClient(), TransportLayer.CreateClient(sessionId));
            return ret;
        }
    }
}
