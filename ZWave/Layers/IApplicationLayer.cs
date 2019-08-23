using ZWave.Enums;
using ZWave.Layers.Application;

namespace ZWave.Layers
{
    public interface IApplicationLayer
    {
        bool SuppressDebugOutput { get; set; }
        ApiTypes ApiType { get; set; }
        ISessionLayer SessionLayer { get; set; }
        IFrameLayer FrameLayer { get; set; }
        ITransportLayer TransportLayer { get; set; }
        ApplicationClient CreateClient();
    }
}
