using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utils;
using Utils.Threading;
using ZWave.Enums;

namespace ZWave
{
    public abstract class ActionBase : IActionCase, IActionItem
    {
        public bool IsHandled { get; set; }
        public byte SessionId { get; set; }
        public int Id { get; set; }

        public bool IsFirstPriority { get; set; }
        public bool IsExclusive { get; set; }

        public ActionToken Token { get; set; }
        public Action<IActionItem> CompletedCallback { get; set; }
        private ActionBase _parentAction;

        public ActionBase ParentAction
        {
            get { return _parentAction; }
            set
            {
                _parentAction = value;
                Token.IsChildAction = _parentAction != null;
            }
        }

        public byte SequenceNumber { get; set; }
        public bool IsSequenceNumberRequired { get; set; }
        public ActionUnit ActionUnitStop { get; set; }
        protected List<ActionUnit> ActionUnits { get; set; }

        public bool IsStateCompleted
        {
            get { return Token.Result.State == ActionStates.Completed; }
        }

        public ActionResult Result
        {
            get { return Token.Result; }
        }

        public ActionBase(bool isExclusive)
        {
            IsExclusive = isExclusive;
            Id = NextId();
            ActionUnits = new List<ActionUnit>();
            Token = new ActionToken(GetType(), Id, 0, CreateOperationResult());
        }

        public virtual void NewToken(bool isNextId)
        {
            if (isNextId)
            {
                var newId = NextId();
                "{0}->{1}"._DLOG(Id, newId);
                Id = newId;
            }
            var oldTimeout = Token.TimeoutMs;
            Token = new ActionToken(GetType(), Id, oldTimeout, CreateOperationResult());
            Token.TimeoutMs = oldTimeout;
            Token.Name = Name;
            ActionUnits.Clear();
            CompletedCallback = null;
        }

        public virtual void NewToken()
        {
            NewToken(false);
        }

        public string AboutMeSafe()
        {
            string ret;
            try
            {
                ret = AboutMe();
            }
            catch
            {
                ret = "### not valid";
            }
            return ret;
        }

        public virtual string AboutMe()
        {
            return string.Empty;
        }

        protected abstract void CreateWorkflow();
        protected abstract void CreateInstance();
        protected virtual ActionResult CreateOperationResult()
        {
            return new ActionResult();
        }

        internal ActionHandlerResult Start()
        {
            ActionHandlerResult ret = null;
            Token.Name = Name;
            CreateInstance();
            CreateWorkflow();
            Token.SetRunning();
            if (ActionUnits.Count > 0)
            {
                ActionUnit ou = ActionUnits.FirstOrDefault(x => x is StartActionUnit);
                ret = Handle(ou);
                if (ou != null)
                    Token.Reset(ou.TimeoutMs);
            }
            return ret;
        }

        internal void FixStates()
        {
            if (Token.State == ActionStates.Completing)
                Token.SetCompleted();
            else if (Token.State == ActionStates.Expiring)
                Token.SetExpired();
            else if (Token.State == ActionStates.Cancelling)
                Token.SetCancelled();
            else if (Token.State == ActionStates.Failing)
                Token.SetFailed();
        }

        private ActionHandlerResult Handle(ActionUnit actionUnit)
        {
            ActionHandlerResult ret = null;
            if (actionUnit != null)
            {
                ret = new ActionHandlerResult(this);
                {
                    ActionUnit ou = actionUnit;
                    if (actionUnit is DataReceivedUnit)
                    {
                        DataReceivedUnit dou = (DataReceivedUnit)actionUnit;
                        if (dou.DataFrame != null && dou.DataFrame.Data != null)
                            AddTraceLogItem(DateTime.Now, dou.DataFrame.Data, false);
                    }
                    if (ou.Func != null)
                    {
                        ou.Func(ou);
                    }
                    if (Token.IsStateActive)
                    {
                        if (ou.ActionItems != null && ou.ActionItems.Count > 0)
                        {
                            foreach (var item in ou.ActionItems)
                            {
                                item.ParentAction = this;
                                ret.NextActions.Add(item);
                            }
                        }
                        else
                        {
                            FixStates();
                        }
                    }
                }
            }
            return ret;
        }

        internal ActionHandlerResult TryHandle(IActionCase actionCase)
        {
            ActionHandlerResult ret = null;
            if (Token.IsStateActive)
            {
                if (ActionUnits.Count > 0)
                {
                    ActionUnit ou = ActionUnits.FirstOrDefault(x => x.TryHandle(actionCase));
                    ret = Handle(ou);
                    if (ou != null && ou.TimeoutMs > 0)
                        Token.Reset(ou.TimeoutMs);
                }
            }
            return ret;
        }

        internal ActionHandlerResult TryHandleStopped()
        {
            ActionHandlerResult ret = new ActionHandlerResult(this);
            ActionUnit ou = ActionUnitStop;
            if (ou != null)
                ret = Handle(ou);
            return ret;
        }

        private static int _IdCounter;
        private int NextId()
        {
            return Interlocked.Increment(ref _IdCounter);
        }

        protected virtual void SetStateCompleted(ActionUnit ou)
        {
            Token.SetCompleted();
        }

        protected virtual void SetStateFailed(ActionUnit ou)
        {
            Token.SetFailed();
        }

        protected virtual void SetStateCancelled(ActionUnit ou)
        {
            Token.SetCancelled();
        }

        protected virtual void SetStateExpired(ActionUnit ou)
        {
            Token.SetExpired();
        }

        protected virtual void SetStateCompleting(ActionUnit ou)
        {
            Token.SetCompleting();
        }

        protected virtual void SetStateFailing(ActionUnit ou)
        {
            Token.SetFailing();
        }

        private string _typeName = null;
        public string Name
        {
            get
            {
                if (_typeName == null)
                {
                    StringBuilder sbType = new StringBuilder();
                    string type = GetType().Name;
                    if (type.EndsWith("Operation"))
                    {
                        type = type.Replace("Operation", "");
                    }
                    sbType.Append(type);
                    ActionBase tmp = ParentAction;
                    while (tmp != null)
                    {
                        type = tmp.GetType().Name;
                        if (type.EndsWith("Operation"))
                        {
                            type = type.Replace("Operation", "");
                        }
                        sbType.AppendFormat(".{0}", type);
                        tmp = tmp.ParentAction;
                    }
                    _typeName = sbType.ToString();
                }
                return _typeName;
            }
            set
            {
                _typeName = value;
            }
        }

        private string GetId(ActionBase action)
        {
            string ret = action.Id.ToString();
            if (action.ParentAction != null)
            {
                ret += "." + GetId(action.ParentAction);
            }
            return ret;
        }

        public string GetName()
        {
            return string.Format("{0} {1} (Id={2}, IsExclusive={3}, Timeout={4})", Token.Result.State, Name, GetId(this), IsExclusive, Token.TimeoutMs);
        }

        public void AddTraceLogItem(DateTime dateTime, byte[] data, bool isOutcome)
        {
            Result.AddTraceLogItem(dateTime, data, isOutcome);
        }

        public void AddTraceLogItems(ConcurrentList<TraceLogItem> traceLog)
        {
            // Added To Array to prevent modification of traceLog in foreach loop itself.
            foreach (var item in traceLog.ToArray())
            {
                Result.TraceLog.Add(item);
            }
        }

        public override string ToString()
        {
            return Tools.FormatStr("{0}={1}({2})", GetType().Name, Token.Result.State, Token.Result.RetryCount);
        }
    }

    public class SubstituteSettings
    {
        public SubstituteFlags SubstituteFlags { get; set; }
        public int MaxBytesPerFrameSize { get; set; }

        public SubstituteSettings() { }

        public SubstituteSettings(SubstituteFlags substituteFlags, int maxBytesPerFrameSize)
        {
            SubstituteFlags = substituteFlags;
            MaxBytesPerFrameSize = maxBytesPerFrameSize;
        }

        public bool HasFlag(SubstituteFlags flag)
        {
            return (SubstituteFlags & flag) == flag;
        }

        public void SetFlag(SubstituteFlags flag)
        {
            SubstituteFlags |= flag;
        }

        public void ClearFlag(SubstituteFlags flag)
        {
            SubstituteFlags &= ~flag;
        }
    }
}
