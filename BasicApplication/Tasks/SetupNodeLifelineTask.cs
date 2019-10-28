using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.Tasks
{
    internal class SetupNodeLifelineTask : ActionSerialGroup
    {
        private readonly TransmitOptions _txOptions =
            TransmitOptions.TransmitOptionAcknowledge |
            TransmitOptions.TransmitOptionAutoRoute |
            TransmitOptions.TransmitOptionExplore;

        private readonly NetworkViewPoint _network;
        private readonly NodeInfoTask _requestNodeInfo;
        private readonly RequestDataOperation _requestRoleType;
        private readonly DeleteReturnRouteOperation _deleteReturnRoute;
        private readonly AssignReturnRouteOperation _assignReturnRoute;
        private readonly SendDataOperation _sendAssociationCreate;
        private readonly SendDataOperation _sendMultichannelAssociationCreate;
        private readonly RequestDataOperation _requestWakeUpCapabilities;
        private readonly SendDataOperation _sendWakeUpInterval;

        public int WakeUpInterval { get; set; }
        public bool IsFullSetup { get; set; }
        public byte BasicDeviceType { get; set; }

        private byte _sucNodeId;
        public byte SucNodeId
        {
            get
            {
                return _sucNodeId;
            }
            set
            {
                _sucNodeId = value;
                var associationCmd = new COMMAND_CLASS_ASSOCIATION.ASSOCIATION_SET();
                associationCmd.groupingIdentifier = 0x01;
                associationCmd.nodeId = new List<byte>();
                associationCmd.nodeId.Add(_sucNodeId > 0 ? _sucNodeId : NodeId);
                _sendAssociationCreate.Data = associationCmd;

                var multichannelAssociationCmd = new COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_SET();
                multichannelAssociationCmd.groupingIdentifier = 0x01;
                multichannelAssociationCmd.nodeId = new List<byte>();
                multichannelAssociationCmd.nodeId.Add(_sucNodeId > 0 ? _sucNodeId : NodeId);
                _sendMultichannelAssociationCreate.Data = multichannelAssociationCmd;
            }
        }

        private byte _nodeId;
        public byte NodeId
        {
            get
            {
                return _nodeId;
            }
            set
            {
                _nodeId = value;
                _assignReturnRoute.DestNodeId = _nodeId;
            }
        }

        private byte _targetNodeId;
        public byte TargetNodeId
        {
            get
            {
                return _targetNodeId;
            }
            set
            {
                _targetNodeId = value;
                _requestNodeInfo.NodeId = _targetNodeId;
                _requestRoleType.DestNodeId = _targetNodeId;
                _deleteReturnRoute.NodeId = _targetNodeId;
                _assignReturnRoute.SrcNodeId = _targetNodeId;
                _sendAssociationCreate.NodeId = _targetNodeId;
                _sendMultichannelAssociationCreate.NodeId = _targetNodeId;
                _requestWakeUpCapabilities.DestNodeId = _targetNodeId;
                _sendWakeUpInterval.NodeId = _targetNodeId;
            }
        }

        public SetupNodeLifelineTask(NetworkViewPoint network) :
            base()
        {
            _network = network;
            WakeUpInterval = 5 * 60; // In Seconds.
            IsFullSetup = true;

            _requestNodeInfo = new NodeInfoTask(_network, 0);
            _requestRoleType = new RequestDataOperation(0, 0,
               new COMMAND_CLASS_ZWAVEPLUS_INFO_V2.ZWAVEPLUS_INFO_GET(), _txOptions,
               new COMMAND_CLASS_ZWAVEPLUS_INFO_V2.ZWAVEPLUS_INFO_REPORT(), 2,
               10000)
            { Name = "ReguestData (ZWavePlus)" };
            _deleteReturnRoute = new DeleteReturnRouteOperation(0);
            _assignReturnRoute = new AssignReturnRouteOperation(0, 0);
            _sendAssociationCreate = new SendDataOperation(0, null, _txOptions);
            _sendMultichannelAssociationCreate = new SendDataOperation(0, null, _txOptions);
            _requestWakeUpCapabilities = new RequestDataOperation(0, 0,
               new COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_GET(), _txOptions,
               new COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_REPORT(), 2,
               2000)
            { Name = "ReguestData (WakeUpCapabilites)" };
            _sendWakeUpInterval = new SendDataOperation(0, null, _txOptions);

            SpecificResult.NodeInfo = _requestNodeInfo.SpecificResult;
            SpecificResult.SetWakeUpInterval = _sendWakeUpInterval.SpecificResult;
            SpecificResult.RequestRoleType = _requestRoleType.SpecificResult;

            Actions = new ActionBase[] {
                _requestNodeInfo,
                _requestRoleType,
                _deleteReturnRoute,
                _assignReturnRoute,
                _sendAssociationCreate,
                _sendMultichannelAssociationCreate,
                _requestWakeUpCapabilities,
                _sendWakeUpInterval
            };

            _onActionCompleted = OnActionCompleted;
        }

        private void OnActionCompleted(ActionBase action, ActionResult result)
        {
            if (result is NodeInfoResult && result)
            {
                if (!IsFullSetup)
                {
                    _deleteReturnRoute.Token.SetCancelled();
                    _assignReturnRoute.Token.SetCancelled();
                    _sendAssociationCreate.Token.SetCancelled();
                    _sendMultichannelAssociationCreate.Token.SetCancelled();
                    _requestWakeUpCapabilities.Token.SetCancelled();
                    _sendWakeUpInterval.Token.SetCancelled();
                }

                if (_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses.Contains(COMMAND_CLASS_ZWAVEPLUS_INFO_V2.ID))
                {
                    _requestRoleType.Token.SetCancelled();
                }

                if (BasicDeviceType == 0)
                {
                    BasicDeviceType = _requestNodeInfo.SpecificResult.RequestNodeInfo.Basic;
                }

                if (BasicDeviceType != 0x04) //BASIC_TYPE_ROUTING_SLAVE
                {
                    _deleteReturnRoute.Token.SetCancelled();
                    _assignReturnRoute.Token.SetCancelled();
                }

                if ((_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses.Contains(COMMAND_CLASS_ASSOCIATION.ID)) &&
                    (_requestNodeInfo.SpecificResult.RequestNodeInfo.SecureCommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.SecureCommandClasses.Contains(COMMAND_CLASS_ASSOCIATION.ID)))
                {
                    _sendAssociationCreate.Token.SetCancelled();
                }

                if ((_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses.Contains(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID)) &&
                    (_requestNodeInfo.SpecificResult.RequestNodeInfo.SecureCommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.SecureCommandClasses.Contains(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID)))
                {
                    _sendMultichannelAssociationCreate.Token.SetCancelled();
                }
                else
                {
                    _sendAssociationCreate.Token.SetCancelled();
                }

                if ((_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.CommandClasses.Contains(COMMAND_CLASS_WAKE_UP_V2.ID)) &&
                    (_requestNodeInfo.SpecificResult.RequestNodeInfo.SecureCommandClasses == null ||
                    !_requestNodeInfo.SpecificResult.RequestNodeInfo.SecureCommandClasses.Contains(COMMAND_CLASS_WAKE_UP_V2.ID)))
                {
                    _requestWakeUpCapabilities.Token.SetCancelled();
                    _sendWakeUpInterval.Token.SetCancelled();
                }
            }
            else if (result is RequestDataResult)
            {
                var requestRes = (RequestDataResult)result;
                if (result)
                {
                    if (requestRes.Command != null && requestRes.Command.Length > 2)
                    {
                        if (requestRes.Command[0] == COMMAND_CLASS_WAKE_UP_V2.ID
                            && requestRes.Command[1] == COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_REPORT.ID)
                        {
                            var wakeUpCapabilitiesReport = (COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_INTERVAL_CAPABILITIES_REPORT)requestRes.Command;
                            int minVal = Tools.GetInt32(wakeUpCapabilitiesReport.minimumWakeUpIntervalSeconds);
                            if (minVal > WakeUpInterval)
                            {
                                WakeUpInterval = minVal;
                            }

                            COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_SET cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_SET();
                            cmd.seconds = Tools.GetBytes(WakeUpInterval).Skip(1).ToArray();
                            cmd.nodeid = NodeId;
                            _sendWakeUpInterval.Data = cmd;
                        }
                    }
                }
                else
                {
                    COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_SET cmd = new COMMAND_CLASS_WAKE_UP.WAKE_UP_INTERVAL_SET();
                    cmd.seconds = Tools.GetBytes(WakeUpInterval).Skip(1).ToArray();
                    cmd.nodeid = NodeId;
                    _sendWakeUpInterval.Data = cmd;
                }
            }
        }

        public SetupNodeLifelineResult SpecificResult
        {
            get { return (SetupNodeLifelineResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SetupNodeLifelineResult();
        }
    }

    public class SetupNodeLifelineResult : ActionResult
    {
        public NodeInfoResult NodeInfo { get; set; }
        public SendDataResult SetWakeUpInterval { get; set; }
        public RequestDataResult RequestRoleType { get; set; }
    }
}
