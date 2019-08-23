using System;
using ZWave.Layers;

namespace ZWave
{
    public interface ITimeoutManager : IDisposable
    {
        void Start(ISessionClient sessionClient);
        void Stop();
        void AddTimer(TimeInterval timeInterval);
        void AddTimer(ActionToken token);
    }
}
