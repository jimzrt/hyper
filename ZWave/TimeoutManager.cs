using System;
using System.Threading;
using Utils.Threading;
using ZWave.Layers;

namespace ZWave
{
    public sealed class TimeoutManager : ITimeoutManager
    {
        private ISessionClient _sessionClient;
        private Timer _timer;
        public static int TICK = 100;
        private const int CAPACITY = 1000;
        private readonly ConcurrentList<ActionToken> _values = new ConcurrentList<ActionToken>(CAPACITY);
        private readonly ConcurrentList<TimeInterval> _timeIntervals = new ConcurrentList<TimeInterval>(CAPACITY);

        public void Stop()
        {
            Dispose();
        }

        public void Start(ISessionClient sessionClient)
        {
            _sessionClient = sessionClient;
            if (_timer == null)
            {
                _timer = new Timer(TimerCallback, null, TICK, Timeout.Infinite);
            }
            else
            {
                _timer.Change(TICK, Timeout.Infinite);
            }
        }

        public void AddTimer(TimeInterval timeInterval)
        {
            if (timeInterval == null)
            {
                throw new Exceptions.OperationException();
            }

            timeInterval.ExpireDateTime = DateTime.Now + TimeSpan.FromMilliseconds(timeInterval.IntervalMs);
            _timeIntervals.Add(timeInterval);
        }

        public void AddTimer(ActionToken token)
        {
            if (token == null)
            {
                throw new Exceptions.OperationException();
            }
            var dt = DateTime.Now;
            token.ExpireDateTime = dt + TimeSpan.FromMilliseconds(token.TimeoutMs);
            _values.Add(token);
        }

        private void TimerCallback(object state)
        {
            DateTime dt = DateTime.Now;
            for (int i = _values.Count - 1; i >= 0; i--)
            {
                ActionToken at = _values[i];
                if (at != null)
                {
                    if (!at.IsStateFinished)
                    {
                        if (at.CheckExpired(dt))
                        {
                            _sessionClient.TokenExpired(at);
                        }
                    }
                    else
                    {
                        _values.RemoveAt(i);
                    }
                }
            }
            for (int i = _timeIntervals.Count - 1; i >= 0; i--)
            {
                TimeInterval ti = _timeIntervals[i];
                if (ti != null)
                {
                    if (ti.CheckExpired(dt))
                    {
                        _sessionClient.HandleActionCase(ti);
                        _timeIntervals.RemoveAt(i);
                    }
                }
            }
            lock (mLockObject)
            {
                if (_timer != null)
                {
                    _timer.Change(TICK, Timeout.Infinite);
                }
            }
        }

        #region IDisposable Members
        private readonly object mLockObject = new object();
        public void Dispose()
        {
            lock (mLockObject)
            {
                if (_timer != null)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        #endregion
    }
}
