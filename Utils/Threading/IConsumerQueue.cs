using System;

namespace Utils.Threading
{
    public interface IConsumerQueue<T> where T : class
    {
        bool IsOpen { get; }
        string Name { get; }
        void Start(Action<T> consumerCallback);
        void Start(string name, Action<T> consumerCallback);
        void Stop();
        void Stop(Action<T> stopCurrentCallback, Action<T> stopPendingCallback);
        void Add(T item);
        void Reset();
    }
}
