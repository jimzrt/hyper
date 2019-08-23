using System;

namespace Utils.UI
{
    public interface IDispatch
    {
        bool CheckAccess();
        void BeginInvoke(Action action);
        void Invoke(Action action);
        bool InvokeBackground(Action action, int timeoutMs);
        bool InvokeBackground(Action action);
    }
}
