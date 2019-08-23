using System.Collections.Generic;

namespace Utils.Threading
{
    public class ConcurrentQueue<T>
    {
        public int MaxCapacity { get; set; }
        private readonly Queue<T> _innerQueue;
        private readonly object _lockObject = new object();

        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return _innerQueue.Count;
                }
            }
        }

        public ConcurrentQueue() :
            this(100000)
        { }

        public ConcurrentQueue(int maxCapacity)
        {
            MaxCapacity = maxCapacity;
            _innerQueue = new Queue<T>(MaxCapacity);
        }

        public void Enqueue(T item)
        {
            lock (_lockObject)
            {
                _innerQueue.Enqueue(item);
            }
        }

        public bool IsAddAllowed
        {
            get
            {
                lock (_lockObject)
                {
                    return _innerQueue.Count < MaxCapacity;
                }
            }
        }

        public T Dequeue()
        {
            T ret = default(T);
            lock (_lockObject)
            {
                if (_innerQueue.Count > 0)
                {
                    ret = _innerQueue.Dequeue();
                }
            }
            return ret;
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _innerQueue.Clear();
            }
        }
    }
}
