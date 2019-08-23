using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Utils;
using ZWave.Exceptions;

namespace ZWave
{
    public class ActionToken
    {
        public static int DefaultTimeout = 5 * 60000;
        public static bool ThrowExceptionOnDefaultTimeoutExpired = false;
        private AutoResetEvent _completedSignal { get; set; }
        public int ActionId { get; set; }
        public ActionResult Result { get; set; }
        public int TimeoutMs;
        public DateTime ExpireDateTime = DateTime.MinValue;
        public int LogEntryPointClassLineNumber { get; set; }
        public string LogEntryPointSource { get; set; }
        public string LogEntryPointCategory { get; set; }
        public string Name { get; set; }
        public Type ParentType { get; set; }
        public bool IsChildAction { get; set; }
        public object CustomObject { get; set; }
        public Action<ActionToken> HandleMe { get; set; }

        private bool _waitCompletedSignalReceived;
        internal bool IsStateFinished
        {
            get
            {
                return State == ActionStates.Cancelled ||
                    State == ActionStates.Completed ||
                    State == ActionStates.Expired ||
                    State == ActionStates.Failed;
            }
        }

        internal ActionToken(Type actionType, int actionId, int timeoutMs, ActionResult result)
        {
            _completedSignal = new AutoResetEvent(false);
            ParentType = actionType;
            ActionId = actionId;
            TimeoutMs = timeoutMs;
            Result = result;
            Result.StartTimestamp = DateTime.Now;
        }

        public AutoResetEvent StartSignal { get; set; }

        public ActionStates State
        {
            get { return Result.State; }
        }

        public bool IsStateActive
        {
            get
            {
                return Result.State != ActionStates.Completed &&
                    Result.State != ActionStates.Expired &&
                    Result.State != ActionStates.Failed &&
                    Result.State != ActionStates.Cancelled;
            }
        }

        public void Complete()
        {
            SetCompleted();
            if (HandleMe != null)
            {
                HandleMe(this);
            }
        }

        [DebuggerHidden]
        public bool CheckExpired(DateTime currentDate)
        {
            bool ret = false;
            if (ExpireDateTime < currentDate)
            {
                ret = true;
            }
            return ret;
        }

        [DebuggerHidden]
        public void Reset(int timeoutMs)
        {
            TimeoutMs = timeoutMs;
            ExpireDateTime = DateTime.Now + TimeSpan.FromMilliseconds(timeoutMs);
        }

        [DebuggerHidden]
        public void SetCompleted()
        {
            Result.State = ActionStates.Completed;
            Result.StopTimestamp = DateTime.Now;
        }

        [DebuggerHidden]
        public void SetFailed()
        {
            Result.State = ActionStates.Failed;
            Result.StopTimestamp = DateTime.Now;
        }

        [DebuggerHidden]
        public void SetCancelled()
        {
            Result.State = ActionStates.Cancelled;
            Result.StopTimestamp = DateTime.Now;
        }

        [DebuggerHidden]
        public void SetExpired()
        {
            Result.State = ActionStates.Expired;
            Result.StopTimestamp = DateTime.Now;
        }

        [DebuggerHidden]
        internal void SetExpiring()
        {
            Result.State = ActionStates.Expiring;
        }

        [DebuggerHidden]
        internal void SetCompleting()
        {
            Result.State = ActionStates.Completing;
        }

        [DebuggerHidden]
        internal void SetFailing()
        {
            Result.State = ActionStates.Failing;
        }

        [DebuggerHidden]
        internal void SetRunning()
        {
            Result.State = ActionStates.Running;
            Result.StartTimestamp = DateTime.Now;
        }

        [DebuggerHidden]
        internal void SetCancelling()
        {
            Result.State = ActionStates.Cancelling;
        }

        [DebuggerHidden]
        public ActionResult WaitCompletedSignal()
        {
            return WaitCompletedSignal(0);
        }

        [DebuggerHidden]
        public ActionResult WaitCompletedSignal(int timeoutMs)
        {
            if (timeoutMs > 0)
            {
                Reset(timeoutMs);
            }
            if (!_waitCompletedSignalReceived && !_completedSignal.WaitOne(/*DefaultTimeout*/))
            {
                "{0} UNEXPECTED Default Timeout"._DLOG(ParentType);
                if (ThrowExceptionOnDefaultTimeoutExpired)
                {
                    OperationException.Throw("UNEXPECTED Default Timeout");
                }
            }
            _waitCompletedSignalReceived = true;

            return Result;
        }

        [DebuggerHidden]
        public void SetCompletedSignal()
        {
            _completedSignal.Set();
        }

        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (LogEntryPointClassLineNumber > 0)
                    sb.AppendFormat(" Ln {0}", LogEntryPointClassLineNumber);
                if (!string.IsNullOrEmpty(LogEntryPointCategory))
                    sb.AppendFormat(" {0}", LogEntryPointCategory);
                if (!string.IsNullOrEmpty(LogEntryPointSource))
                    sb.AppendFormat(" {0}", LogEntryPointSource);
                return sb.ToString();
            }
        }
    }
}
