using System;
using System.Linq;
using Utils;
using ZWave.BasicApplication.CommandClasses;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.Tasks
{
    public class InclusionTask : ActionParallelGroup
    {
        private const byte PROXY_INCLUSION = 0x01;
        private const byte PROXY_INCLUSION_REPLACE = 0x03;

        public static int TIMEOUT = 60000;

        private readonly NetworkViewPoint _network;
        private readonly Modes _mode;
        private readonly int _timeoutMs;
        private readonly Action<NodeStatuses> _nodeStatusCallback;
        private readonly TransmitOptions _txOptions =
            TransmitOptions.TransmitOptionAcknowledge |
            TransmitOptions.TransmitOptionAutoRoute |
            TransmitOptions.TransmitOptionExplore;

        private readonly FilterAchOperation _peerFilter;
        private readonly IAddRemoveNode _addNode;
        private readonly MemoryGetIdOperation _memoryGetId;
        private readonly GetSucNodeIdOperation _getSucNodeId;

        private readonly IsFailedNodeOperation _isFailedSucNodeOperation;
        private readonly RequestNodeInfoOperation _getSucNodeInfo;
        private readonly ActionSerialGroup _requestSucNodeInfoGroup;

        private readonly InclusionController.Initiate _requestInclusionController;
        private readonly SerialApiGetInitDataOperation _serialApiGetInitData;
        private readonly SetupNodeLifelineTask _setupNodeLifelineTask;

        private IsFailedNodeOperation _isFailedNodeOperation;
        private RemoveFailedNodeIdOperation _removeFailedNodeIdOperation;
        private readonly bool _isSmartStart = false;

        public InclusionTask(NetworkViewPoint network, IAddRemoveNode addRemoveNode)
            : this(network, addRemoveNode, false)
        {
        }

        public InclusionTask(NetworkViewPoint network, IAddRemoveNode addRemoveNode, bool IsSmartStart)
            : base(false, null)
        {
            _network = network;
            _isSmartStart = IsSmartStart;
            if (addRemoveNode is AddNodeOperation)
            {
                _mode = (addRemoveNode as AddNodeOperation).InitMode;
                _nodeStatusCallback = (addRemoveNode as AddNodeOperation).NodeStatusCallback;
                _timeoutMs = (addRemoveNode as AddNodeOperation).TimeoutMs;
            }
            else
            {
                _mode = Modes.NodeAny;
                _timeoutMs = TIMEOUT;
            }

            _peerFilter = new FilterAchOperation();
            _peerFilter.SetFilterNodeId(0xFF);
            _addNode = addRemoveNode;
            _addNode.NodeStatusCallback = OnNodeStatus;

            _memoryGetId = new MemoryGetIdOperation();

            _getSucNodeId = new GetSucNodeIdOperation();
            _isFailedSucNodeOperation = new IsFailedNodeOperation(0);
            _getSucNodeInfo = new RequestNodeInfoOperation(0);
            _requestSucNodeInfoGroup = new ActionSerialGroup(OnSucNodeInfoCompletedGroup, _getSucNodeId, _isFailedSucNodeOperation, _getSucNodeInfo)
            {
                Name = "RequestSucNodeInfoGroup (InclusionController)",
                CompletedCallback = OnRequestSucNodeInfoGroupCompleted
            };

            _setupNodeLifelineTask = new SetupNodeLifelineTask(_network);

            _requestInclusionController = new InclusionController.Initiate(0, 0, _txOptions, 240000) { Name = "ReguestData (InclusionController)" };
            _serialApiGetInitData = new SerialApiGetInitDataOperation();

            SpecificResult.AddRemoveNode = _addNode.SpecificResult;
            SpecificResult.MemoryGetId = _memoryGetId.SpecificResult;
            SpecificResult.GetSucNodeId = _getSucNodeId.SpecificResult;
            SpecificResult.NodeInfo = _setupNodeLifelineTask.SpecificResult.NodeInfo;
            SpecificResult.SetWakeUpInterval = _setupNodeLifelineTask.SpecificResult.SetWakeUpInterval;

            _isFailedNodeOperation = new IsFailedNodeOperation(_addNode.SpecificResult.Id);
            _removeFailedNodeIdOperation = new RemoveFailedNodeIdOperation(_addNode.SpecificResult.Id);
            if (!_isSmartStart)
            {
                _isFailedNodeOperation.Token.SetCancelled();
                _removeFailedNodeIdOperation.Token.SetCancelled();
            }

            Actions = new ActionBase[]
            {
                _peerFilter,
                new ActionSerialGroup(OnActionCompleted,
                _memoryGetId,
                _requestSucNodeInfoGroup,
                _serialApiGetInitData,
                (ActionBase)_addNode,
                _isFailedNodeOperation,
                _removeFailedNodeIdOperation,
                _requestInclusionController,
                _setupNodeLifelineTask
                ) { Name = "Inclusion (Group)" }
            };
        }

        private void OnRequestSucNodeInfoGroupCompleted(IActionItem obj)
        {
            var requestSucNodeInfoGroup = (ActionSerialGroup)obj;
            if (requestSucNodeInfoGroup.IsStateCompleted)
            {
                var result = requestSucNodeInfoGroup.Result.InnerResults.Last() as RequestNodeInfoResult;
                if (result != null && result)
                {
                    byte[] supported = result.CommandClasses;
                    byte[] secureSupported = result.SecureCommandClasses;
                    if ((supported != null && supported.Contains(COMMAND_CLASS_INCLUSION_CONTROLLER.ID)) ||
                        (secureSupported != null && secureSupported.Contains(COMMAND_CLASS_INCLUSION_CONTROLLER.ID)))
                    {
                        (_addNode as ApiOperation).SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                        _requestInclusionController.DestNodeId = _getSucNodeId.SpecificResult.SucNodeId;
                        return;
                    }
                }
            }
            _requestInclusionController.Token.SetCancelled();
        }

        private void OnSucNodeInfoCompletedGroup(ActionBase action, ActionResult result)
        {
            if (result is GetSucNodeIdResult && result)
            {
                _setupNodeLifelineTask.SucNodeId = _getSucNodeId.SpecificResult.SucNodeId;
                if (_getSucNodeId.SpecificResult.SucNodeId > 0 && _getSucNodeId.SpecificResult.SucNodeId != _memoryGetId.SpecificResult.NodeId)
                {
                    _isFailedSucNodeOperation.NodeId = _getSucNodeId.SpecificResult.SucNodeId;
                    _getSucNodeInfo.NodeId = _getSucNodeId.SpecificResult.SucNodeId;
                    _peerFilter.SetFilterSucNodeId(_getSucNodeId.SpecificResult.SucNodeId);
                    _setupNodeLifelineTask.IsFullSetup = false;
                }
                else
                {
                    _requestSucNodeInfoGroup.Token.SetCancelled();
                    _requestInclusionController.Token.SetCancelled();
                }
            }
            else if (result is IsFailedNodeResult && result)
            {
                if (((IsFailedNodeResult)result).RetValue) // SUC failed.
                {
                    _requestSucNodeInfoGroup.Token.SetCancelled();
                }
            }
        }

        private void OnNodeStatus(NodeStatuses nodeStatus)
        {
            if (nodeStatus == NodeStatuses.AddingRemovingController || nodeStatus == NodeStatuses.AddingRemovingSlave || nodeStatus == NodeStatuses.Done)
            {
                if (_addNode is ReplaceFailedNodeOperation)
                {
                    _addNode.SpecificResult.AddRemoveNodeStatus = AddRemoveNodeStatuses.Replaced;
                }
                _peerFilter.SetFilterNodeId(_addNode.SpecificResult.Id);
            }
            if (_nodeStatusCallback != null)
                _nodeStatusCallback(nodeStatus);
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            base.SetStateCompleted(ou);
            if (SpecificResult.AddRemoveNode.State == ActionStates.Completed)
            {
                if (_setupNodeLifelineTask.SpecificResult.RequestRoleType &&
                    _setupNodeLifelineTask.SpecificResult.RequestRoleType.Command != null &&
                    _setupNodeLifelineTask.SpecificResult.RequestRoleType.Command.Length > 2)
                {
                    COMMAND_CLASS_ZWAVEPLUS_INFO_V2.ZWAVEPLUS_INFO_REPORT cmd = _setupNodeLifelineTask.SpecificResult.RequestRoleType.Command;
                    SpecificResult.AddRemoveNode.RoleType = cmd.roleType;
                }
            }
        }

        private void OnActionCompleted(ActionBase action, ActionResult result)
        {
            if (result is MemoryGetIdResult && result)
            {
                _setupNodeLifelineTask.NodeId = _memoryGetId.SpecificResult.NodeId;
            }
            else if (result is SerialApiGetInitDataResult && result)
            {
                if (_addNode is AddNodeOperation)
                {
                    (_addNode as AddNodeOperation).NodesBefore = (result as SerialApiGetInitDataResult).IncludedNodes;
                }
            }
            else if (result is AddRemoveNodeResult)
            {
                if (result)
                {
                    _setupNodeLifelineTask.TargetNodeId = _addNode.SpecificResult.Id;
                    _setupNodeLifelineTask.BasicDeviceType = _addNode.SpecificResult.Basic;

                    if (_addNode.SpecificResult.Id == _getSucNodeId.SpecificResult.SucNodeId)
                    {
                        _requestInclusionController.Token.SetCancelled();
                    }
                    else
                    {
                        if (_addNode.SpecificResult.AddRemoveNodeStatus == AddRemoveNodeStatuses.Replicated)
                        {
                            _setupNodeLifelineTask.Token.SetCancelled();
                            _requestInclusionController.Token.SetCancelled();
                        }
                        else
                        {
                            byte stepId = _addNode.SpecificResult.AddRemoveNodeStatus == AddRemoveNodeStatuses.Replaced ? PROXY_INCLUSION_REPLACE : PROXY_INCLUSION;
                            _requestInclusionController.SetCommandParameters(_addNode.SpecificResult.Id, stepId);
                        }
                    }

                    if ((result as AddRemoveNodeResult).SubstituteStatus == SubstituteStatuses.Failed && _isSmartStart)
                    {
                        if (_isFailedNodeOperation != null)
                        {
                            _isFailedNodeOperation.NodeId = _addNode.SpecificResult.Id;
                        }
                        else
                        {
                            _isFailedNodeOperation = new IsFailedNodeOperation(_addNode.SpecificResult.Id);
                        }
                    }
                    else
                    {
                        _isFailedNodeOperation.Token.SetCancelled();
                        _removeFailedNodeIdOperation.Token.SetCancelled();
                    }

                }
                else
                {
                    //this.Token.Result.TraceLog.Add(result.TraceLog);
                    _setupNodeLifelineTask.Token.SetCancelled();
                    _requestInclusionController.Token.SetCancelled();
                    //if (result.State == ActionStates.Failed)
                    //{
                    //    SetStateFailed(null);
                    //}
                    //else if (result.State == ActionStates.Expired)
                    //{
                    //    SetStateExpired(null);
                    //}
                }
            }
            else if (result is RequestDataResult && result)
            {
                var requestRes = (RequestDataResult)result;
                if (requestRes.Command != null && requestRes.Command.Length > 2 &&
                    requestRes.Command[0] == COMMAND_CLASS_INCLUSION_CONTROLLER.ID &&
                    requestRes.Command[1] == COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE.ID)
                {
                    COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE cmd = requestRes.Command;
                    if (cmd.status == 0x01)
                    {
                        SpecificResult.AddRemoveNode.SubstituteStatus = SubstituteStatuses.Done;
                    }
                }
            }
            else if (result is IsFailedNodeResult)
            {
                if ((result as IsFailedNodeResult).RetValue)
                {
                    _setupNodeLifelineTask.Token.SetCancelled();
                    SpecificResult.AddRemoveNode.AddRemoveNodeStatus = AddRemoveNodeStatuses.None;
                    if (_removeFailedNodeIdOperation != null)
                    {
                        _removeFailedNodeIdOperation.ReplacedNodeId = _addNode.SpecificResult.Id;
                    }
                    else
                    {
                        _removeFailedNodeIdOperation = new RemoveFailedNodeIdOperation(_addNode.SpecificResult.Id);
                    }
                }
                else
                {
                    _removeFailedNodeIdOperation.Token.SetCancelled();
                }
            }
        }

        public InclusionResult SpecificResult
        {
            get { return (InclusionResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new InclusionResult();
        }
    }

    public class InclusionResult : ActionResult
    {
        public AddRemoveNodeResult AddRemoveNode { get; set; }
        public MemoryGetIdResult MemoryGetId { get; set; }
        public GetSucNodeIdResult GetSucNodeId { get; set; }
        public NodeInfoResult NodeInfo { get; set; }
        public SendDataResult SetWakeUpInterval { get; set; }
    }
}