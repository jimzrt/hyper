using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Utils;
using Utils.Threading;
using ZWave.Enums;
using ZWave.Layers.Frame;

namespace ZWave.Layers.Session
{
    public class SessionClient : ISessionClient
    {
        private AutoResetEvent _actionStartSignal = new AutoResetEvent(false);
        private byte _funcIdCounter = 0;
        public byte SessionId { get; set; }
        public bool SuppressDebugOutput { get; set; }
        public string LogEntryPointClass { get; set; }
        public Func<ActionHandlerResult, bool> SendFramesCallback { get; set; }
        public Func<ActionBase, ActionBase> PostSubstituteAction { get; set; }
        public ConcurrentLinkedList<ActionBase> RunningActions { get; set; }
        public ConcurrentQueue<ActionBase> PendingExclusiveActions { get; set; }

        private ITimeoutManager _timeoutManager;
        public ITimeoutManager TimeoutManager
        {
            get { return _timeoutManager; }
            set
            {
                if (_timeoutManager != null)
                {
                    _timeoutManager.Stop();
                }
                _timeoutManager = value;
            }
        }

        private IConsumerQueue<IActionItem> _callbackBufferBlock;
        public IConsumerQueue<IActionItem> CallbackBufferBlock
        {
            get { return _callbackBufferBlock; }
            set
            {
                if (_callbackBufferBlock != null)
                {
                    _callbackBufferBlock.Stop();
                }
                _callbackBufferBlock = value;
            }
        }

        private IConsumerQueue<IActionCase> _actionCaseConsumer;
        public IConsumerQueue<IActionCase> ActionCaseConsumer
        {
            get { return _actionCaseConsumer; }
            set
            {
                if (_actionCaseConsumer != null)
                {
                    _actionCaseConsumer.Stop();
                }
                _actionCaseConsumer = value;
            }
        }

        private List<ISubstituteManager> _substituteManagers = new List<ISubstituteManager>();
        private Dictionary<SubstituteIncomingFlags, ISubstituteManager> _substituteManagersDictionary = new Dictionary<SubstituteIncomingFlags, ISubstituteManager>();
        private Action<ActionToken> _actionChangeCallback;
        internal ApiTypes ApiType { get; set; }
        public SessionClient(Action<ActionToken> actionChangeCallback)
        {
            _actionChangeCallback = actionChangeCallback;
            IsHandleFrameEnabled = true;
            RunningActions = new ConcurrentLinkedList<ActionBase>();
            PendingExclusiveActions = new ConcurrentQueue<ActionBase>();
        }

        public void RunComponents()
        {
            TimeoutManager.Start(this);
            CallbackBufferBlock.Start("CallbackBufferBlock", action =>
            {
                if (action.CompletedCallback != null)
                {
                    action.CompletedCallback(action);
                }
            });
            ActionCaseConsumer.Start("ActionCaseConsumer", HandleActionCaseInner);
        }

        public void RunComponentsDefault()
        {
            TimeoutManager = new TimeoutManager();
            CallbackBufferBlock = new ConsumerQueue<IActionItem>();
            ActionCaseConsumer = new ConsumerQueue<IActionCase>();
            RunComponents();
        }

        #region Private

        private void ProcessCompleted(ActionBase action)
        {
            if (action.Token.IsStateFinished)
            {
                if (RunningActions.Remove(action))
                {
                    action.Token.Name = action.Name + ": " + action.AboutMeSafe();
                    if (_actionChangeCallback != null)
                        _actionChangeCallback(action.Token);

                    action.Token.SetCompletedSignal();
                    if (!SuppressDebugOutput)
                    {
                        "{0:X2} {1}"._DLOG(SessionId, action.GetName() + action.AboutMeSafe());
                    }
                    CallbackBufferBlock.Add(action);
                    ActionCaseConsumer.Add(action);
                }
            }
        }

        private object _actionStateLock = new object();
        public void ProcessNext(ActionHandlerResult ahResult)
        {
            if (ahResult != null && ahResult.NextActions != null)
            {
                var sendFrames = ahResult.NextActions.Where(x => x is CommandMessage);
                var timeIntervals = ahResult.NextActions.Where(x => x is TimeInterval);
                var actions = ahResult.NextActions.Where(x => x is ActionBase);
                if (sendFrames.Any())
                {
                    if (SendFramesCallback != null)
                    {
                        var isTransmitted = SendFramesCallback(ahResult);
                        if (isTransmitted)
                        {
                            ahResult.Parent.FixStates();
                        }
                        else
                        {
                            ahResult.Parent.Token.SetFailed();
                        }
                        if (ahResult.Parent.Token.IsStateFinished)
                        {
                            ActionCaseConsumer.Add(ahResult.Parent);
                        }
                    }
                }

                if (timeIntervals.Any())
                {
                    foreach (TimeInterval item in timeIntervals)
                    {
                        TimeoutManager.AddTimer(item);
                    }
                }

                if (actions.Any())
                {
                    foreach (ActionBase item in actions)
                    {
                        ActionCaseConsumer.Add(item);
                    }
                }
            }
        }

        private void ProcessSubstituteManagers(CustomDataFrame dataFrame, List<ActionHandlerResult> ahResults, Dictionary<SubstituteIncomingFlags, CustomDataFrame> substitutedDataFrames)
        {
            // 32 bits in int
            for (int i = 31; i >= 0; i--)
            {
                int mask = 1 << i;
                if ((dataFrame.SubstituteIncomingFlags & (SubstituteIncomingFlags)mask) > 0 && _substituteManagersDictionary.ContainsKey((SubstituteIncomingFlags)mask))
                {
                    var smgr = _substituteManagersDictionary[(SubstituteIncomingFlags)mask];
                    var dataFrameOri = substitutedDataFrames[(SubstituteIncomingFlags)mask];
                    smgr.OnIncomingSubstituted(dataFrameOri, dataFrame, ahResults);
                }
            }
        }

        public void SetFuncId(byte value)
        {
            _funcIdCounter = value;
        }

        private byte NextFuncId()
        {
            _funcIdCounter++;
            while (_funcIdCounter == 0 || _funcIdCounter > 126)
                _funcIdCounter++;
            return _funcIdCounter;
        }
        #endregion

        public void TokenExpired(ActionToken actionToken)
        {
            RunningActions.ForEach(action =>
            {
                if (action.Token == actionToken)
                {
                    action.Token.SetExpiring();
                    ActionCaseConsumer.Add(action);
                }
                return false;
            });
        }

        public bool IsHandleFrameEnabled { get; set; }

        public void HandleActionCase(IActionCase actionCase)
        {
            var dataFrame = actionCase as CustomDataFrame;
            var timeInterval = actionCase as TimeInterval;

            if (dataFrame != null && !dataFrame.IsOutcome && !dataFrame.IsHandled)
            {
                if (IsHandleFrameEnabled)
                {
                    ActionCaseConsumer.Add(dataFrame);
                }
            }

            if (timeInterval != null && !timeInterval.IsHandled)
            {
                ActionCaseConsumer.Add(timeInterval);
            }
        }

        private ActionBase SubstituteAction(ActionBase action)
        {
            lock (_actionStateLock)
            {
                foreach (var item in _substituteManagers)
                {
                    var tmp = item.SubstituteAction(action);
                    if (tmp != null)
                    {
                        action = tmp;
                        if (action.IsSequenceNumberRequired)
                            action.SequenceNumber = NextFuncId();
                    }
                }
            }
            if (PostSubstituteAction != null)
            {
                var tmp = PostSubstituteAction(action);
                if (tmp != null)
                {
                    action = tmp;
                    if (action.IsSequenceNumberRequired)
                        action.SequenceNumber = NextFuncId();
                }
            }
            return action;
        }

        private CustomDataFrame SubstituteIncoming(CustomDataFrame dataFrame, List<ActionHandlerResult> ahResults, Dictionary<SubstituteIncomingFlags, CustomDataFrame> substitutedDataFrames)
        {
            ActionBase additionalAction;
            ActionBase completeAction;
            lock (_actionStateLock)
            {
                foreach (var item in Enumerable.Reverse(_substituteManagers))
                {
                    substitutedDataFrames.Add(item.Id, dataFrame);
                    dataFrame = item.SubstituteIncoming(dataFrame, out additionalAction, out completeAction);
                    if (additionalAction != null)
                    {
                        var ahRes = new ActionHandlerResult(null);
                        ahRes.NextActions.Add(additionalAction);
                        ahResults.Add(ahRes);
                    }
                    if (completeAction != null)
                    {
                        ProcessCompleted(completeAction);
                    }
                }
                return dataFrame;
            }
        }
        private volatile bool _isExclusiveBusy = false;
        private void HandleActionCaseInner(IActionCase actionCase)
        {
            var customDataFrame = actionCase as CustomDataFrame;
            var timeInterval = actionCase as TimeInterval;
            var action = actionCase as ActionBase;
            if (customDataFrame != null)
            {
                CustomDataFrame dataFrameOri = customDataFrame;
                var ahResults = new List<ActionHandlerResult>();
                Dictionary<SubstituteIncomingFlags, CustomDataFrame> substitutedDataFrames = new Dictionary<SubstituteIncomingFlags, CustomDataFrame>();
                var dataFrame = SubstituteIncoming(dataFrameOri, ahResults, substitutedDataFrames);
                if (dataFrame != null)
                {
                    TryHandleDataFrame(dataFrame, ahResults);
                    if (dataFrame.Parent != null)
                    {
                        TryHandleDataFrame(dataFrame.Parent, ahResults);
                    }
                    ProcessSubstituteManagers(dataFrame, ahResults, substitutedDataFrames);
                    foreach (var ahResult in ahResults)
                    {
                        ProcessNext(ahResult);
                        if (ahResult.Parent != null)
                        {
                            ProcessCompleted(ahResult.Parent);
                        }
                    }
                }
            }
            else if (timeInterval != null)
            {
                if (timeInterval.ParentAction != null)
                {
                    ProcessNext(timeInterval.ParentAction.TryHandle(actionCase));
                    if (timeInterval.ParentAction != null)
                    {
                        ProcessCompleted(timeInterval.ParentAction);
                    }
                }
            }
            else if (action != null)
            {
                if (action.Token.IsStateFinished)
                {
                    ProcessCompleted(action);
                    if (action.IsExclusive)
                    {
                        _isExclusiveBusy = false;
                        var pendingAction = PendingExclusiveActions.Dequeue();
                        if (pendingAction != null)
                        {
                            _actionCaseConsumer.Add(pendingAction);
                        }
                    }
                    RunningActions.ForEach(x =>
                    {
                        if (x.ParentAction == action)
                        {
                            if (x.ParentAction.Token.State == ActionStates.Expired || x.ParentAction.Token.State == ActionStates.Cancelled)
                            {
                                x.Token.SetCancelling();
                                var ahResult = x.TryHandleStopped();
                                ProcessNext(ahResult);
                                x.Token.SetCancelled();
                                ProcessCompleted(x);
                            }
                            else
                            {
                                x.Token.SetCancelled();
                                ProcessCompleted(x);
                            }
                        }
                        return false;
                    });
                    if (action.ParentAction != null && action.ParentAction.Token.IsStateActive)
                    {
                        var ahResult = action.ParentAction.TryHandle(action);
                        ProcessNext(ahResult);
                        action.ParentAction.FixStates();
                        ProcessCompleted(action.ParentAction);
                    }
                }
                else
                {
                    if (action.Token.State == ActionStates.None)
                    {
                        var actionStartSignal = action.Token.StartSignal;
                        action.SessionId = SessionId;
                        if (action.IsSequenceNumberRequired)
                            action.SequenceNumber = NextFuncId();
                        action = SubstituteAction(action);
                        if (action.IsExclusive)
                        {
                            if (!SuppressDebugOutput)
                                "{0:X2} (W){1}"._DLOG(SessionId, action.GetName() + action.AboutMeSafe());
                            if (_isExclusiveBusy)
                            {
                                PendingExclusiveActions.Enqueue(action);
                                return;
                            }
                            else
                            {
                                _isExclusiveBusy = true;
                            }
                            //CheckPoint.Pass(action);
                        }

                        var ahResult = action.Start();

                        if (_actionChangeCallback != null)
                            _actionChangeCallback(action.Token);

                        if (action.IsFirstPriority)
                        {
                            RunningActions.AddFirst(action);
                        }
                        else
                        {
                            RunningActions.AddLast(action);
                        }
                        if (!SuppressDebugOutput)
                            "{0:X2} {1}"._DLOG(SessionId, action.GetName() + action.AboutMeSafe());

                        ProcessNext(ahResult);
                        if (action.Token.TimeoutMs > 0)
                            TimeoutManager.AddTimer(action.Token);
                        if (actionStartSignal != null && !isDisposing)
                        {
                            actionStartSignal.Set();
                        }
                    }
                    else if (action.Token.Result.State == ActionStates.Cancelling)
                    {
                        var ahResult = action.TryHandleStopped();
                        ProcessNext(ahResult);
                    }
                    else if (action.Token.Result.State == ActionStates.Expiring)
                    {
                        var ahResult = action.TryHandleStopped();
                        ProcessNext(ahResult);
                    }
                    action.FixStates();
                    ProcessCompleted(action);
                }
            }
        }

        private void TryHandleDataFrame(CustomDataFrame actionCase, List<ActionHandlerResult> ahResults)
        {
            lock (_actionStateLock)
            {
                RunningActions.ForEach(x =>
                {
                    if (!actionCase.IsHandled)
                    {
                        var ahr = x.TryHandle(actionCase);
                        if (ahr != null)
                        {
                            ahResults.Add(ahr);
                        }
                    }
                    return false;
                });
            }
        }

        Stopwatch sw = new Stopwatch();
        public ActionToken ExecuteAsync(IActionItem actionCase)
        {
            var action = actionCase as ActionBase;
            ActionToken ret = null;
            if (action != null)
            {
                if (!SuppressDebugOutput)
                {
                    "{0:X2} (E){1}"._DLOG(SessionId, action.GetName() + action.AboutMeSafe());
                }
                if (action.Token != null)
                {
                    action.Token.HandleMe = (x) => OnHandleToken(x);
                    action.Token.LogEntryPointClassLineNumber = GetLineNumberInTheEntryPointClass(LogEntryPointClass);
                    action.Token.StartSignal = _actionStartSignal;
                }
                ret = action.Token;
                ActionCaseConsumer.Add(action);
                if (!isDisposing)
                {
                    _actionStartSignal.WaitOne();
                    if (!SuppressDebugOutput)
                    {
                        "{0:X2} (R){1}"._DLOG(SessionId, action.GetName() + action.AboutMeSafe());
                    }
                }
            }
            return ret;
        }

        public static int GetLineNumberInTheEntryPointClass(string logEntryPointClass)
        {
            int ret = -1;
            try
            {
                if (!string.IsNullOrEmpty(logEntryPointClass))
                {
                    StackTrace st = new StackTrace(true);
                    StackFrame sfTC = null;
                    StackFrame sfMTC = null;
                    for (int i = 4; i < st.FrameCount; i++)
                    {
                        StackFrame sf = st.GetFrame(i);
                        string fileName = sf.GetFileName();
                        if (fileName != null && fileName.Contains(logEntryPointClass))
                        {
                            sfTC = st.GetFrame(i - 1);
                            sfMTC = st.GetFrame(i - 2);
                            break;
                        }
                    }
                    if (sfTC != null)
                    {
                        ret = sfTC.GetFileLineNumber();
                    }
                }
            }
            catch { }
            return ret;
        }

        public void Cancel(Type actionType)
        {
            RunningActions.ForEach(type =>
            {
                if (type.GetType() == actionType)
                {
                    type.Token.SetCancelling();
                    ActionCaseConsumer.Add(type);
                }
                return false;
            });
        }

        public void Cancel(ActionToken actionToken)
        {
            if (actionToken != null && !actionToken.IsStateFinished)
            {
                RunningActions.ForEach(action =>
                {
                    if (action.Token == actionToken)
                    {
                        action.Token.SetCancelling();
                        ActionCaseConsumer.Add(action);
                    }
                    return false;
                });
            }
        }

        public void OnHandleToken(ActionToken actionToken)
        {
            RunningActions.ForEach(action =>
            {
                if (action.Token == actionToken)
                {
                    ActionCaseConsumer.Add(action);
                }
                return false;
            });
        }

        public ISubstituteManager GetSubstituteManager(Type type)
        {
            lock (_actionStateLock)
            {
                return _substituteManagers.FirstOrDefault(x => type.IsInstanceOfType(x));
            }
        }

        public List<ISubstituteManager> GetSubstituteManagers()
        {
            lock (_actionStateLock)
            {
                return _substituteManagers;
            }
        }

        public void ClearSubstituteManagers()
        {
            lock (_actionStateLock)
            {
                foreach (var item in _substituteManagers)
                {
                    var runningTokens = item.GetRunningActionTokens();
                    if (runningTokens != null)
                    {
                        foreach (var token in runningTokens)
                        {
                            Cancel(token);
                        }
                    }
                };
                _substituteManagers.Clear();
                _substituteManagersDictionary.Clear();
            }
        }

        public void AddSubstituteManager(ISubstituteManager sm, params ActionBase[] actions)
        {
            lock (_actionStateLock)
            {
                _substituteManagers.Add(sm);
                _substituteManagersDictionary.Add(sm.Id, sm);
            }
            if (actions != null)
            {
                foreach (var item in actions)
                {
                    ExecuteAsync(item);
                    sm.AddRunningActionToken(item.Token);
                }
            }
        }

        #region IDisposable Members



        public void DisposeAA()
        {

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private volatile bool isDisposing;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    isDisposing = true;
                    // TODO: dispose managed state (managed objects).
                    _actionStartSignal.Set();
                    _actionStartSignal.Close();
                    CallbackBufferBlock.Stop();
                    TimeoutManager.Stop();
                    ActionCaseConsumer.Stop();
                    RunningActions.ForEach(action =>
                    {
                        action.Token.SetCancelled();
                        action.Token.SetCompletedSignal();
                        return false;
                    });
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SessionClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
