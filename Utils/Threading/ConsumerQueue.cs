using System;
using System.Threading;

namespace Utils.Threading
{
    public class ConsumerQueue<T> : IConsumerQueue<T>
        where T : class
    {
        #region privates

        private ConcurrentQueue<T> _innerQueue;
        private T _innerCurrent;
        private Thread _workerThread;
        private AutoResetEvent _workSignal;
        private Action<T> _consumerCallback;

        #endregion privates

        public string Name { get; set; }

        private volatile bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        public void Stop()
        {
            Stop(null, null);
        }

        public void Stop(Action<T> stopCurrentCallback, Action<T> stopPendingCallback)
        {
            if (!_isOpen)
            {
                return;
            }

            _isOpen = false;

            if (stopCurrentCallback != null && _innerCurrent != null)
            {
                stopCurrentCallback(_innerCurrent);
            }

            if (stopPendingCallback != null)
            {
                while (_innerQueue.Count > 0)
                {
                    var item = _innerQueue.Dequeue();
                    stopPendingCallback(item);
                }
            }
            else
            {
                _innerQueue.Clear();
            }
            SetWorkerSignal();
            if (Thread.CurrentThread.ManagedThreadId != _workerThread.ManagedThreadId)
            {
                _workerThread.Join();
            }
            _workSignal.Close();
        }

        public void Start(Action<T> consumerCallback)
        {
            Start("", consumerCallback);
        }

        public void Start(string name, Action<T> consumerCallback)
        {
            Name = name;
            _innerCurrent = null;
            if (_isOpen)
            {
                return;
            }

            _isOpen = true;
            _workSignal = new AutoResetEvent(false);
            _innerQueue = new ConcurrentQueue<T>();
            _consumerCallback = consumerCallback;
            _workerThread = new Thread(DoWork)
            {
                Name = "Consumer Queue - " + Name,
                IsBackground = true
            };
            _workerThread.Start();
            SetWorkerSignal();
        }

        public void Add(T item)
        {
            if (_isOpen)
            {
                if (_innerQueue.IsAddAllowed)
                {
                    _innerQueue.Enqueue(item);
                    SetWorkerSignal();
                }
            }
        }

        private void DoWork()
        {
            while (_isOpen)
            {
                WaitWorkerSignal();
                while (_isOpen && _innerQueue.Count > 0)
                {
                    _innerCurrent = _innerQueue.Dequeue();
                    if (_innerCurrent != null)
                    {
                        _consumerCallback(_innerCurrent);
                    }
                }
            }
        }

        private void WaitWorkerSignal()
        {
            _workSignal.WaitOne();
        }

        private void SetWorkerSignal()
        {
            _workSignal.Set();
        }

        public void Reset()
        {
            _innerQueue.Clear();
        }
    }
}
