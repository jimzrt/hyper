using System;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.BasicApplication.Tasks;
using ZWave.Devices;
using ZWave.Enums;
using ZWave.Layers;

namespace ZWave.BasicApplication.Devices
{
    public class Controller : Device, IController
    {
        private byte mControllerCapability;
        public byte ControllerCapability
        {
            get { return mControllerCapability; }
            set
            {
                mControllerCapability = value;
                Notify("ControllerCapability");
            }
        }

        public ControllerRoles NetworkRole
        {
            get
            {
                ControllerRoles result = ControllerRoles.None;
                if ((ControllerCapability & (byte)ControllerCapabilities.IS_SECONDARY) != 0)
                {
                    result |= ControllerRoles.Secondary;
                }
                if ((ControllerCapability & (byte)ControllerCapabilities.IS_SUC) != 0)
                {
                    result |= ControllerRoles.SUC;
                }
                if ((ControllerCapability & (byte)ControllerCapabilities.ON_OTHER_NETWORK) != 0)
                {
                    result |= ControllerRoles.OtherNetwork;
                }
                if ((ControllerCapability & (byte)ControllerCapabilities.IS_REAL_PRIMARY) != 0)
                {
                    result |= ControllerRoles.RealPrimary;
                }
                if ((ControllerCapability & (byte)ControllerCapabilities.NODEID_SERVER_PRESENT) != 0)
                {
                    result |= ControllerRoles.NodeIdServerPresent;
                }
                if ((result & ControllerRoles.SUC) != 0 && (result & ControllerRoles.NodeIdServerPresent) != 0)
                {
                    result |= ControllerRoles.SIS;
                }
                if ((result & ControllerRoles.SIS) == 0 && (result & ControllerRoles.SUC) == 0 && (result & ControllerRoles.NodeIdServerPresent) != 0)
                {
                    result |= ControllerRoles.Inclusion;
                }
                return result;
            }
        }

        internal Controller(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(sessionId, sc, fc, tc)
        {
        }

        public byte[] IncludedNodes { get; set; }

        public override ActionToken ExecuteAsync(IActionItem actionItem, Action<IActionItem> completedCallback)
        {
            var actionBase = actionItem as ActionBase;
            if (actionBase != null)
            {
                if (actionBase is GetControllerCapabilitiesOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            GetControllerCapabilitiesResult res = (GetControllerCapabilitiesResult)action.Result;
                            if (res)
                                ControllerCapability = res.ControllerCapability;
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is GetSucNodeIdOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            GetSucNodeIdResult res = (GetSucNodeIdResult)action.Result;
                            if (res)
                                SucNodeId = res.SucNodeId;
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is SetLearnModeControllerOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            SetLearnModeResult res = (SetLearnModeResult)action.Result;
                            if (res)
                                Id = res.NodeId;
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else
                    actionBase.CompletedCallback = completedCallback;
            }
            return base.ExecuteAsync(actionBase, actionBase.CompletedCallback);
        }

        public override ActionResult Execute(IActionItem action)
        {
            ActionResult ret = base.Execute(action);
            if (action is GetControllerCapabilitiesOperation)
            {
                GetControllerCapabilitiesResult res = (GetControllerCapabilitiesResult)ret;
                if (res)
                    ControllerCapability = res.ControllerCapability;
            }
            else if (action is GetSucNodeIdOperation)
            {
                GetSucNodeIdResult res = (GetSucNodeIdResult)ret;
                if (res)
                    SucNodeId = res.SucNodeId;
            }
            else if (action is SetLearnModeControllerOperation)
            {
                SetLearnModeResult res = (SetLearnModeResult)ret;
                if (res)
                    Id = res.NodeId;
            }
            return ret;
        }

        #region GetProtocolInfo

        public GetNodeProtocolInfoResult GetProtocolInfo(byte nodeId)
        {
            return (GetNodeProtocolInfoResult)Execute(new GetNodeProtocolInfoOperation(nodeId));
        }

        public ActionToken GetProtocolInfo(byte nodeId, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new GetNodeProtocolInfoOperation(nodeId), completedCallback);
        }

        #endregion

        #region AddNodeToNetwork

        public AddRemoveNodeResult AddNodeToNetwork(Modes mode, int timeoutMs)
        {
            ResetNodeStatusSignals();
            return (AddRemoveNodeResult)Execute(new AddNodeOperation(mode, SetNodeStatusSignal, timeoutMs));
        }

        public ActionToken AddNodeToNetworkNonSecure(Modes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            return ExecuteAsync(new AddNodeOperation(mode, SetNodeStatusSignal, timeoutMs)
            {
                SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
            }, completedCallback);
        }

        public ActionToken AddNodeToNetwork(Modes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            return ExecuteAsync(new AddNodeOperation(mode, SetNodeStatusSignal, timeoutMs), completedCallback);
        }

        public ActionToken AddNodeToNetwork(Modes mode, bool isModeStopEnabled, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            return ExecuteAsync(new AddNodeOperation(mode, SetNodeStatusSignal, isModeStopEnabled, timeoutMs), completedCallback);
        }

        #endregion

        #region IncludeNode

        public InclusionResult IncludeNode(Modes mode, int timeoutMs)
        {
            ResetNodeStatusSignals();
            AddNodeOperation AddNodeOperation = new AddNodeOperation(mode, SetNodeStatusSignal, timeoutMs);
            return (InclusionResult)Execute(new InclusionTask(Network, AddNodeOperation));
        }

        public ActionToken IncludeNode(Modes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            AddNodeOperation AddNodeOperation = new AddNodeOperation(mode, SetNodeStatusSignal, timeoutMs);
            return ExecuteAsync(new InclusionTask(Network, AddNodeOperation), completedCallback);
        }

        #endregion

        #region RemoveNodeFromNetwork

        public AddRemoveNodeResult RemoveNodeFromNetwork(Modes mode, int timeoutMs)
        {
            ResetNodeStatusSignals();
            return (AddRemoveNodeResult)Execute(new RemoveNodeOperation(mode, SetNodeStatusSignal, timeoutMs));
        }

        public ActionToken RemoveNodeFromNetwork(Modes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new RemoveNodeOperation(mode, SetNodeStatusSignal, timeoutMs), completedCallback);
        }

        #endregion

        #region ExcludeNode

        public ExclusionResult ExcludeNode(Modes mode, int timeoutMs)
        {
            ResetNodeStatusSignals();
            return (ExclusionResult)Execute(new ExclusionTask(mode, SetNodeStatusSignal, timeoutMs));
        }

        public ActionToken ExcludeNode(Modes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            return ExecuteAsync(new ExclusionTask(mode, SetNodeStatusSignal, timeoutMs), completedCallback);
        }

        #endregion

        #region RemoveNodeIdFromNetwork

        public AddRemoveNodeResult RemoveNodeIdFromNetwork(Modes mode, byte nodeId, int timeoutMs)
        {
            return (AddRemoveNodeResult)Execute(new RemoveNodeIdOperation(mode, nodeId, SetNodeStatusSignal, timeoutMs));
        }

        public ActionToken RemoveNodeIdFromNetwork(Modes mode, byte nodeId, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new RemoveNodeIdOperation(mode, nodeId, SetNodeStatusSignal, timeoutMs), completedCallback);
        }

        #endregion

        #region SetLearnMode

        public override SetLearnModeResult SetLearnMode(LearnModes mode, bool isSubstituteDenied, int timeoutMs)
        {
            SetLearnModeResult ret = null;
            ResetNodeStatusSignals();
            var action = new SetLearnModeControllerOperation(mode, SetNodeStatusSignal, timeoutMs);
            if (isSubstituteDenied)
            {
                action.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            }
            ret = (SetLearnModeResult)Execute(action);
            return ret;
        }

        public override SetLearnModeResult SetLearnMode(LearnModes mode, int timeoutMs)
        {
            return SetLearnMode(mode, false, timeoutMs);
        }

        public override ActionToken SetLearnMode(LearnModes mode, bool isSubstituteDenied, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ActionToken ret = null;
            ResetNodeStatusSignals();
            SetLearnModeControllerOperation action = new SetLearnModeControllerOperation(mode, SetNodeStatusSignal, timeoutMs);
            if (isSubstituteDenied)
            {
                action.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            }
            learnModeOperation = action;
            ret = ExecuteAsync(action, completedCallback);
            return ret;
        }

        public override ActionToken SetLearnMode(LearnModes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return SetLearnMode(mode, false, timeoutMs, completedCallback);
        }

        #endregion

        public ActionResult EnableSUC(bool isEnable, byte capabilities)
        {
            return Execute(new EnableSucOperation(Convert.ToByte(isEnable), capabilities));
        }

        public SetSucNodeIdResult SetSucNodeID(byte nodeId, bool isEnable, bool isLowPower, byte capabilities)
        {
            if (nodeId != Id)
            {
                var res = (SetSucNodeIdResult)Execute(new SetSucNodeIdOperation(
                    nodeId,
                    Convert.ToByte(isEnable),
                    isLowPower,
                    capabilities));
                if (res)
                {
                    SucNodeId = nodeId;
                }
                return res;
            }
            else
            {
                var ret = (ReturnValueResult)Execute(new SetSucSelfOperation(
                    nodeId,
                    Convert.ToByte(isEnable),
                    isLowPower,
                    capabilities));
                if (ret)
                {
                    SucNodeId = nodeId;
                }
                return new SetSucNodeIdResult(ret)
                {
                    RetVal = SetSucReturnValues.SucSetSucceeded
                };
            }
        }

        public ReturnValueResult AreNodesNeighbours(byte nodeId1, byte nodeId2)
        {
            return (ReturnValueResult)Execute(new AreNodesNeighboursOperation(nodeId1, nodeId2));
        }

        public AssignReturnRouteResult AssignReturnRoute(byte nodeId1, byte nodeId2, out ActionToken token)
        {
            token = ExecuteAsync(new AssignReturnRouteOperation(nodeId1, nodeId2), null);
            return (AssignReturnRouteResult)token.WaitCompletedSignal();
        }

        public AssignReturnRouteResult AssignPriorityReturnRoute(byte source, byte destination,
            byte priorityRoute0, byte priorityRoute1, byte priorityRoute2, byte priorityRoute3, byte routespeed, out ActionToken token)
        {
            token = ExecuteAsync(new AssignPriorityReturnRouteOperation(source, destination,
                priorityRoute0, priorityRoute1, priorityRoute2, priorityRoute3, routespeed), null);
            return (AssignReturnRouteResult)token.WaitCompletedSignal();
        }

        public ActionResult AssignSucReturnRoute(byte nodeId, out ActionToken token)
        {
            token = ExecuteAsync(new AssignSucReturnRouteOperation(nodeId), null);
            return token.WaitCompletedSignal();
        }

        public ActionResult AssignPrioritySucReturnRoute(byte source, byte repeater0, byte repeater1, byte repeater2, byte repeater3, byte routespeed, out ActionToken token)
        {
            token = ExecuteAsync(new AssignPrioritySucReturnRouteOperation(source, repeater0, repeater1, repeater2, repeater3, routespeed), null);
            return token.WaitCompletedSignal();
        }

        public ActionResult DeleteReturnRoute(byte nodeId, out ActionToken token)
        {
            token = ExecuteAsync(new DeleteReturnRouteOperation(nodeId), null);
            return token.WaitCompletedSignal();
        }

        public ActionResult DeleteSucReturnRoute(byte nodeId, out ActionToken token)
        {
            token = ExecuteAsync(new DeleteSucReturnRouteOperation(nodeId), null);
            return token.WaitCompletedSignal();
        }

        public GetControllerCapabilitiesResult GetControllerCapabilities()
        {
            return (GetControllerCapabilitiesResult)Execute(new GetControllerCapabilitiesOperation());
        }

        public ActionToken GetControllerCapabilities(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new GetControllerCapabilitiesOperation(), completedCallback);
        }

        public GetNeighborCountResult GetNeighborCount(byte nodeId)
        {
            return (GetNeighborCountResult)Execute(new GetNeighborCountOperation(nodeId));
        }

        public GetRoutingInfoResult GetRoutingInfo(byte nodeId, byte removeBad, byte removeNonReps)
        {
            return (GetRoutingInfoResult)Execute(new GetRoutingInfoOperation(nodeId, removeBad, removeNonReps));
        }

        public GetSucNodeIdResult GetSucNodeId()
        {
            return (GetSucNodeIdResult)Execute(new GetSucNodeIdOperation());
        }

        public ActionToken GetSucNodeId(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new GetSucNodeIdOperation(), completedCallback);
        }

        public ActionToken IsFailedNode(byte nodeId, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new IsFailedNodeOperation(nodeId), completedCallback);
        }

        public IsFailedNodeResult IsFailedNode(byte nodeId)
        {
            return (IsFailedNodeResult)Execute(new IsFailedNodeOperation(nodeId));
        }

        public ActionResult RemoveFailedNodeId(byte nodeId)
        {
            return Execute(new RemoveFailedNodeIdOperation(nodeId));
        }

        public ActionToken RemoveFailedNodeId(byte nodeId, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new RemoveFailedNodeIdOperation(nodeId), completedCallback);
        }

        public InclusionResult ReplaceFailedNode(byte nodeId)
        {
            ReplaceFailedNodeOperation ReplaceFailedNodeOperation = new ReplaceFailedNodeOperation(nodeId);
            return (InclusionResult)Execute(new InclusionTask(Network, ReplaceFailedNodeOperation));
        }

        public ActionToken ReplaceFailedNode(byte nodeId, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();

            ReplaceFailedNodeOperation ReplaceFailedNodeOperation = new ReplaceFailedNodeOperation(nodeId, SetNodeStatusSignal, 60000, new byte[] { nodeId, 0x01 }); ;
            return ExecuteAsync(new InclusionTask(Network, ReplaceFailedNodeOperation), completedCallback);
        }

        public ActionToken ReplaceFailedNode(byte nodeId, Action<IActionItem> completedCallback, Modes mode, int timeoutMs)
        {
            ResetNodeStatusSignals();

            ReplaceFailedNodeOperation ReplaceFailedNodeOperation = new ReplaceFailedNodeOperation(nodeId, SetNodeStatusSignal, timeoutMs, new byte[] { nodeId, 0x01 });
            return ExecuteAsync(new InclusionTask(Network, ReplaceFailedNodeOperation), completedCallback);
        }

        public ActionResult ReplicationReceiveComplete()
        {
            return Execute(new ReplicationReceiveCompleteOperation());
        }

        public TransmitResult ReplicationSend(byte nodeId, byte[] data, TransmitOptions txOptions)
        {
            return (TransmitResult)Execute(new ReplicationSendOperation(nodeId, data, txOptions));
        }

        public RequestNodeNeighborUpdateResult RequestNodeNeighborUpdate(byte nodeId, int timeoutMs)
        {
            return (RequestNodeNeighborUpdateResult)Execute(new RequestNodeNeighborUpdateOperation(nodeId, timeoutMs));
        }

        public ActionToken RequestNodeNeighborUpdate(byte nodeId, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new RequestNodeNeighborUpdateOperation(nodeId, timeoutMs), completedCallback);
        }

        public RequestNodeNeighborUpdateResult RequestNodeNeighborUpdate(byte nodeId, int timeoutMs, out ActionToken token)
        {
            token = RequestNodeNeighborUpdate(nodeId, timeoutMs, null);
            return (RequestNodeNeighborUpdateResult)token.WaitCompletedSignal();
        }

        public TransmitResult SendSucId(byte nodeId, TransmitOptions txOptions)
        {
            return (TransmitResult)Execute(new SendSucIdOperation(nodeId, txOptions));
        }

        public ActionResult SetRoutingInfo(byte nodeId, byte[] nodeMask)
        {
            return Execute(new SetRoutingInfoOperation(nodeId, nodeMask));
        }

        public SetPriorityRouteResult SetPriorityRoute(byte destination, byte repeater0, byte repeater1, byte repeater2, byte repeater3, byte routespeed)
        {
            return (SetPriorityRouteResult)Execute(new SetPriorityRouteOperation(destination, repeater0, repeater1, repeater2, repeater3, routespeed));
        }

        public GetPriorityRouteResult GetPriorityRoute(byte destination)
        {
            return (GetPriorityRouteResult)Execute(new GetPriorityRouteOperation(destination));
        }

        #region ControllerChange

        public AddRemoveNodeResult ControllerChange(ControllerChangeModes mode, int timeoutMs)
        {
            ResetNodeStatusSignals();
            return (AddRemoveNodeResult)Execute(new ControllerChangeOperation(mode, SetNodeStatusSignal, timeoutMs));
        }

        public ActionToken ControllerChange(ControllerChangeModes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            return ExecuteAsync(new ControllerChangeOperation(mode, SetNodeStatusSignal, timeoutMs), completedCallback);
        }

        public ActionToken ControllerChange(ControllerChangeModes mode, bool isModeStopEnabled, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ResetNodeStatusSignals();
            return ExecuteAsync(new ControllerChangeOperation(mode, SetNodeStatusSignal, isModeStopEnabled, timeoutMs), completedCallback);
        }

        #endregion

        #region CreateNewPrimary

        public AddRemoveNodeResult CreateNewPrimary(CreatePrimaryModes mode, int timeoutMs)
        {
            return (AddRemoveNodeResult)Execute(new CreateNewPrimaryCtrlOperation(mode, SetNodeStatusSignal, timeoutMs));
        }

        public ActionToken CreateNewPrimary(CreatePrimaryModes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new CreateNewPrimaryCtrlOperation(mode, SetNodeStatusSignal, timeoutMs), completedCallback);
        }

        #endregion

        #region ControllerUpdate Handler

        public ActionToken HandleControllerUpdate(Action<ApplicationControllerUpdateResult> updateCallback)
        {
            return ExecuteAsync(new ApplicationControllerUpdateOperation(updateCallback), null);
        }

        #endregion

        #region Allocated for NUNIT test

        public ActionResult NUnitCmd()
        {
            return Execute(new NUnitCmdOperation());
        }
        public ActionResult NUnitInit()
        {
            return Execute(new NUnitInitOperation());
        }
        public ActionResult NUnitList()
        {
            return Execute(new NUnitListOperation());
        }
        public NUnitRunResult NUnitRun(byte scenarioId)
        {
            return (NUnitRunResult)Execute(new NUnitRunOperation(scenarioId));
        }
        public ActionResult NUnitEnd()
        {
            return Execute(new NUnitEndOperation());
        }
        public ActionResult IoPortStatus(byte enable)
        {
            return Execute(new IoPortStatusOperation(enable));
        }
        public ActionResult IoPortPinSet(byte portPin, byte value)
        {
            return null;// Execute(new IoPortOperation(enable));
        }

        #endregion

        public ActionResult SetSmartStartMode(bool isStart)
        {
            return Execute(new SetSmartStartAction(isStart));
        }

        public ActionToken SmartStartSupport(Func<byte[], Tuple<byte, byte[], int>> dskNeededCallback, Action<bool, byte[], ActionResult> busyCallback, int inclusionTimeoutMs)
        {
            return ExecuteAsync(new SmartStartSupport(Network, SetNodeStatusSignal, dskNeededCallback, busyCallback, inclusionTimeoutMs), null);
        }
    }
}
