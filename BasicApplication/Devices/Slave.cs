using System;
using ZWave.BasicApplication.Operations;
using ZWave.Enums;
using ZWave.Layers;

namespace ZWave.BasicApplication.Devices
{
    public class Slave : Device
    {
        internal Slave(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(sessionId, sc, fc, tc)
        { }

        public override ActionToken ExecuteAsync(IActionItem actionItem, Action<IActionItem> completedCallback)
        {
            var actionBase = actionItem as ActionBase;
            if (actionBase != null)
            {
                if (actionBase is SetLearnModeSlaveOperation)
                {
                    actionBase.CompletedCallback = x =>
                    {
                        var action = x as ActionBase;
                        SetLearnModeResult res = (SetLearnModeResult)action.Result;
                        if (res)
                            Id = res.NodeId;
                        if (completedCallback != null)
                            completedCallback(action);
                    };
                }
                else
                {
                    actionBase.CompletedCallback = completedCallback;
                }
            }
            return base.ExecuteAsync(actionItem, actionItem.CompletedCallback);
        }

        public override ActionResult Execute(IActionItem action)
        {
            ActionResult ret = base.Execute(action);
            if (action is SetLearnModeSlaveOperation)
            {
                SetLearnModeResult res = (SetLearnModeResult)ret;
                if (res)
                    Id = res.NodeId;
            }
            return ret;
        }

        #region SetLearnMode
        public override SetLearnModeResult SetLearnMode(LearnModes mode, bool isSubstituteDenied, int timeoutMs)
        {
            SetLearnModeResult res = null;
            res = (SetLearnModeResult)Execute(new SetLearnModeSlaveOperation(mode, SetAssignStatusSignal, timeoutMs));
            return res;
        }

        public override SetLearnModeResult SetLearnMode(LearnModes mode, int timeoutMs)
        {
            return SetLearnMode(mode, false, timeoutMs);
        }

        public override ActionToken SetLearnMode(LearnModes mode, bool isSubstituteDenied, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ActionToken ret = null;
            SetLearnModeSlaveOperation oper = new SetLearnModeSlaveOperation(mode, SetAssignStatusSignal, timeoutMs);
            learnModeOperation = oper;
            ret = ExecuteAsync(oper, completedCallback);
            return ret;
        }

        public override ActionToken SetLearnMode(LearnModes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return SetLearnMode(mode, false, timeoutMs, completedCallback);
        }
        #endregion

        public IsNodeWithinDirectRangeResult IsNodeWithinDirectRange(byte nodeId)
        {
            return (IsNodeWithinDirectRangeResult)Execute(new IsNodeWithinDirectRangeOperation(nodeId));
        }

        public ActionResult RediscoveryNeeded(byte nodeId)
        {
            return Execute(new RediscoveryNeededOperation(nodeId));
        }

        public ActionResult RequestNewRouteDestinations(byte[] destList)
        {
            return Execute(new RequestNewRouteDestinationsOperation(destList));
        }

        public SetLearnModeResult StopLearnMode()
        {
            return SetLearnMode(LearnModes.LearnModeDisable, 0);
        }
    }
}
