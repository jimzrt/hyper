using System;
using System.Net;
using System.Threading;

namespace ZWave.Layers.Transport.TcpToSerialBridge
{
    public class TcpToSerialServer
    {
        private static readonly int MAX_THREADS = 1;

        private readonly RunningTask[] _tasks = new RunningTask[MAX_THREADS];
        private RunningTask[] Tasks { get { return _tasks; } }

        private readonly object _taskLocker = new object();
        private bool _isRunning;
        public bool IsRunning
        {
            get
            {
                lock (_taskLocker)
                {
                    return _isRunning;
                }
            }
        }

        public void Start(string address, int portNo, SerialPortTransportClient transportClient, Action<string> logOutput, Action<Exception> handleError)
        {
            if (!_isRunning)
            {
                lock (_taskLocker)
                {
                    if (!_isRunning)
                    {
                        IPAddress ipAddress;
                        if (!IPAddress.TryParse(address, out ipAddress))
                        {
                            throw new ArgumentException("address");
                        }

                        if (portNo <= 0 || portNo > ushort.MaxValue)
                        {
                            throw new ArgumentException("portNo");
                        }

                        _tasks[0] = new RunningTask();

                        _tasks[0].RunTaskForHandler(new TcpToSerialBridge(ipAddress.GetAddressBytes(), portNo, transportClient, logOutput), handleError);
                        _isRunning = true;
                    }
                }
            }
        }

        public void Stop()
        {
            if (_isRunning)
            {
                lock (_taskLocker)
                {
                    if (_isRunning)
                    {
                        _tasks[0].StopTask();
                        _isRunning = false;
                    }
                }
            }
        }
    }

    internal class RunningTask
    {
        private TcpToSerialBridge _handler;
        public TcpToSerialBridge Handler { get { return _handler; } }

        private Thread _task;
        public Thread Task { get { return _task; } }

        public void RunTaskForHandler(TcpToSerialBridge handler, Action<Exception> handleError)
        {
            if (_handler == null || _task != null && !IsThreadActive(_task))
            {
                _handler = handler;
                _task = new Thread(() => SafeThreadRun(() => _handler.Run(), handleError));
                _task.Start();
            }
        }

        private void SafeThreadRun(Action start, Action<Exception> handleError)
        {
            try
            {
                start.Invoke();
            }
            catch (Exception e)
            {
                handleError(e);
            }
        }

        private bool IsThreadActive(Thread thread)
        {
            return thread.ThreadState == ThreadState.Running ||
                thread.ThreadState == ThreadState.Background ||
                thread.ThreadState == ThreadState.WaitSleepJoin;
        }

        public void StopTask()
        {
            if (_handler == null)
            {
                return;
            }
            _handler.Stop();
            _task.Join();
            _handler = null;
            _task = null;
        }
    }
}
