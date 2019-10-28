using System;
using System.Linq;
using System.Threading;
using Utils;
using ZWave.BasicApplication.CommandClasses;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Security;
using ZWave.BasicApplication.Tasks;
using ZWave.CommandClasses;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class InclusionControllerSecureSupport : ApiAchOperation
    {
        //Inclusion Controller Initiate Step ID
        private const byte PROXY_INCLUSION = 0x01;
        private const byte S0_INCLUSION = 0x02;
        private const byte PROXY_INCLUSION_REPLACE = 0x03;

        //Inclusion Controller Complete Status
        private const byte STEP_OK = 0x01;
        private const byte STEP_USER_REJECTED = 0x02;
        private const byte STEP_FAILED = 0x03;
        private const byte STEP_NOT_SUPPORTED = 0x04;

        public TransmitOptions TxOptions { get; set; }
        public TransmitOptions2 TxOptions2 { get; set; }
        public TransmitSecurityOptions TxSecOptions { get; set; }
        private readonly SecurityManagerInfo _securityManagerInfo;
        private readonly Action<ActionResult> _updateCallback;
        private readonly Action<ActionToken, bool> _inclusionControllerStatusUpdateCallback;

        public InclusionControllerSecureSupport(SecurityManagerInfo securityManagerInfo, Action<ActionResult> updateCallback,
                Action<ActionToken, bool> inclusionControllerStatusUpdateCallback)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_INCLUSION_CONTROLLER.ID))
        {
            _updateCallback = updateCallback;
            _securityManagerInfo = securityManagerInfo;
            _inclusionControllerStatusUpdateCallback = inclusionControllerStatusUpdateCallback;
            TxOptions = _securityManagerInfo.TxOptions;
            TxOptions2 = TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE;
            TxSecOptions = TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY;
        }

        public InclusionControllerSecureSupport(SecurityManagerInfo securityManagerInfo)
            : this(securityManagerInfo, null, null)
        {
        }

        private byte nodeIdToInclude = 0;
        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] command = ReceivedAchData.Command;
            if (command != null && command.Length > 1)
            {
                if (command[1] == COMMAND_CLASS_INCLUSION_CONTROLLER.INITIATE.ID && nodeIdToInclude == 0)
                {
                    var initiateCommand = (COMMAND_CLASS_INCLUSION_CONTROLLER.INITIATE)command;

                    if (initiateCommand.stepId == PROXY_INCLUSION || initiateCommand.stepId == PROXY_INCLUSION_REPLACE)
                    {
                        /*TO# 07220 - Delay before request Node Info*/
                        Thread.Sleep(2000);

                        //if we're not SIS then ignore
                        if (_securityManagerInfo.Network.SucNodeId == _securityManagerInfo.Network.NodeId)
                        {
                            //Start add node for example
                            nodeIdToInclude = initiateCommand.nodeId;

                            var isVirtualNodeOperation = new IsVirtualNodeOperation(nodeIdToInclude);
                            if (ReceivedAchData.CommandType != CommandTypes.CmdApplicationCommandHandler_Bridge)
                            {
                                isVirtualNodeOperation.Token.SetCancelled();
                            }
                            var stepId = initiateCommand.stepId;
                            var sendDataComplete = new InclusionController.Complete(nodeId, TxOptions);
                            sendDataComplete.SetCommandParameters(STEP_OK, stepId);
                            var requestDataStep2 = new InclusionController.Initiate(0, nodeId, TxOptions, 20000);
                            requestDataStep2.SetCommandParameters(nodeIdToInclude, S0_INCLUSION);
                            var sendDataRejectComplete = new InclusionController.Complete(nodeId, TxOptions);
                            sendDataRejectComplete.SetCommandParameters(STEP_USER_REJECTED, stepId);

                            var addNodeOperation = new AddNodeS2Operation(_securityManagerInfo);
                            addNodeOperation.SetInclusionControllerInitiateParameters(nodeIdToInclude);

                            var setupNodeLifelineTask = new SetupNodeLifelineTask(_securityManagerInfo.Network);
                            setupNodeLifelineTask.NodeId = _securityManagerInfo.Network.NodeId;
                            setupNodeLifelineTask.SucNodeId = _securityManagerInfo.Network.NodeId;
                            setupNodeLifelineTask.TargetNodeId = nodeIdToInclude;
                            setupNodeLifelineTask.CompletedCallback = OnNodeInfoCompleted;

                            var serialGroup = new ActionSerialGroup(OnInitiateFlowActionCompleted, isVirtualNodeOperation,
                                new RequestNodeInfoOperation(nodeIdToInclude)
                                {
                                    SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
                                },
                                addNodeOperation, requestDataStep2, setupNodeLifelineTask, sendDataComplete);
                            serialGroup.ActionUnitStop = new ActionUnit(sendDataRejectComplete);
                            //hack
                            serialGroup.Token.Result = new AddRemoveNodeResult();
                            serialGroup.CompletedCallback = OnS2SerialGroupCompleted;

                            if (_inclusionControllerStatusUpdateCallback != null)
                            {
                                _inclusionControllerStatusUpdateCallback(serialGroup.Token, false);
                            }
                            ou.SetNextActionItems(serialGroup);
                        }
                    }
                    else if (initiateCommand.stepId == S0_INCLUSION)
                    {
                        //only if asked from SIS
                        if (_securityManagerInfo.Network.SucNodeId == nodeId)
                        {
                            InclusionController.Complete sendDataComplete = null;
                            //Start S0
                            nodeIdToInclude = initiateCommand.nodeId;

                            if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                            {
                                sendDataComplete = new InclusionController.Complete(nodeId, TxOptions);
                                sendDataComplete.SetCommandParameters(STEP_OK, S0_INCLUSION);

                                var addNodeOperation = new AddNodeS0Operation(_securityManagerInfo);
                                addNodeOperation.SetInclusionControllerInitiateParameters(nodeIdToInclude);

                                var nodeInfoOperation = new RequestNodeInfoOperation(nodeIdToInclude);
                                nodeInfoOperation.CompletedCallback = OnNodeInfoCompleted;

                                var serialGroup = new ActionSerialGroup(new RequestNodeInfoOperation(nodeIdToInclude)
                                {
                                    SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
                                },
                                    addNodeOperation, nodeInfoOperation, sendDataComplete);

                                //hack
                                serialGroup.Token.Result = new AddRemoveNodeResult();

                                ou.SetNextActionItems(serialGroup);
                            }
                            else
                            {
                                sendDataComplete = new InclusionController.Complete(nodeId, TxOptions);
                                sendDataComplete.SetCommandParameters(STEP_NOT_SUPPORTED, S0_INCLUSION);
                                ou.SetNextActionItems(sendDataComplete);
                            }
                        }
                    }
                }
                else if (command[1] == COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE.ID)
                {

                }
            }
        }

        private void OnInitiateFlowActionCompleted(ActionBase actionGroup, ActionBase completedAction)
        {
            if (completedAction is RequestNodeInfoOperation)
            {
                if (completedAction.Result)
                {
                    var requestNodeInfoResult = (RequestNodeInfoResult)completedAction.Result;
                    bool isS2Supported = false;
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2) &&
                        requestNodeInfoResult.CommandClasses.Contains(COMMAND_CLASS_SECURITY_2.ID))
                    {
                        isS2Supported = true;
                        ((ActionSerialGroup)actionGroup).Actions[3].Token.SetCancelled();
                    }
                    else
                    {
                        ((ActionSerialGroup)actionGroup).Actions[2].Token.SetCancelled();
                    }

                    if (!isS2Supported && _securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0) &&
                        requestNodeInfoResult.CommandClasses.Contains(COMMAND_CLASS_SECURITY.ID))
                    {
                        ((ActionSerialGroup)actionGroup).Actions[2].Token.SetCancelled();
                    }
                    else
                    {
                        ((ActionSerialGroup)actionGroup).Actions[3].Token.SetCancelled();
                    }
                }
                else
                {
                    ((ActionSerialGroup)actionGroup).Actions[2].Token.SetCancelled();
                    ((ActionSerialGroup)actionGroup).Actions[3].Token.SetCancelled();
                    ((ActionSerialGroup)actionGroup).Actions[4].Token.SetCancelled();
                }
            }
            else if (completedAction.Result && completedAction is IsVirtualNodeOperation)
            {
                var isVirtualNodeResult = (IsVirtualNodeResult)completedAction.Result;
                if (ReceivedAchData.CommandType == CommandTypes.CmdApplicationCommandHandler_Bridge &&
                    isVirtualNodeResult && isVirtualNodeResult.RetValue)
                {
                    foreach (var item in ((ActionSerialGroup)actionGroup).Actions)
                    {
                        if (!(item is SendDataExOperation))
                        {
                            item.Token.SetCancelled();
                        }
                    }
                }
            }

        }

        private void OnNodeInfoCompleted(IActionItem actionItem)
        {
            nodeIdToInclude = 0;
            if (_updateCallback != null)
            {
                var action = actionItem as ActionBase;
                if (action != null)
                {
                    _updateCallback(action.Result);
                }
            }
        }

        private void OnS2SerialGroupCompleted(IActionItem action)
        {
            nodeIdToInclude = 0;
            if (_inclusionControllerStatusUpdateCallback != null)
            {
                _inclusionControllerStatusUpdateCallback(null, true);
            }
        }
    }

    public class InclusionControllerSecure
    {
        public class Initiate : ApiOperation
        {
            private readonly SecurityManagerInfo _securityManagerInfo;
            private RequestDataOperation _requestInclusionSupport;
            private readonly ISecurityTestSettingsService _securityTestSettingsService;
            public InclusionController.Initiate RequestInclusionSupport { get; set; }

            public Initiate(SecurityManagerInfo securityManagerInfo)
                : base(false, null, false)
            {
                _securityManagerInfo = securityManagerInfo;
                _securityTestSettingsService = new SecurityTestSettingsService(_securityManagerInfo, true);
            }

            protected override void CreateWorkflow()
            {
                ActionUnits.Add(new StartActionUnit(OnStart, 0, _requestInclusionSupport));
                ActionUnits.Add(new ActionCompletedUnit(_requestInclusionSupport, OnComplete));
            }

            protected override void CreateInstance()
            {
                _requestInclusionSupport = RequestInclusionSupport;
                //_requestInclusionSupport = new RequestDataOperation(RequestInclusionSupport.SrcNodeId, RequestInclusionSupport.DestNodeId,
                //    RequestInclusionSupport.Data, RequestInclusionSupport.TxOptions,
                //    new COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE(), 2, 240000);
            }

            private void OnStart(StartActionUnit ou)
            {
                if (RequestInclusionSupport.StepId == 0x01)
                {
                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.InclusionInititate1, _requestInclusionSupport);
                }
                else if (RequestInclusionSupport.StepId == 0x02)
                {
                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.InclusionInititate2, _requestInclusionSupport);
                }
            }

            private void OnComplete(ActionCompletedUnit ou)
            {
                SpecificResult.Command = _requestInclusionSupport.SpecificResult.Command;
                SpecificResult.IsBroadcast = _requestInclusionSupport.SpecificResult.IsBroadcast;
                SpecificResult.NodeId = _requestInclusionSupport.SpecificResult.NodeId;
                SpecificResult.RxRssi = _requestInclusionSupport.SpecificResult.RxRssi;
                SpecificResult.RxSecurityScheme = _requestInclusionSupport.SpecificResult.RxSecurityScheme;
                SpecificResult.RxSubstituteStatus = _requestInclusionSupport.SpecificResult.RxSubstituteStatus;
                SpecificResult.TransmitStatus = _requestInclusionSupport.SpecificResult.TransmitStatus;
                SpecificResult.TxSubstituteStatus = _requestInclusionSupport.SpecificResult.TxSubstituteStatus;

                SetStateCompleted(ou);
            }

            public RequestDataResult SpecificResult
            {
                get { return (RequestDataResult)Result; }
            }

            protected override ActionResult CreateOperationResult()
            {
                return new RequestDataResult();
            }
        }

        public class Complete : ApiOperation
        {
            private readonly SecurityManagerInfo _securityManagerInfo;
            private SendDataOperation _sendInclusionComplete;
            private readonly ISecurityTestSettingsService _securityTestSettingsService;
            public InclusionController.Complete SendInclusionComplete { get; set; }

            public Complete(SecurityManagerInfo securityManagerInfo)
                : base(false, null, false)
            {
                _securityManagerInfo = securityManagerInfo;
                _securityTestSettingsService = new SecurityTestSettingsService(_securityManagerInfo, true);
            }

            protected override void CreateWorkflow()
            {
                ActionUnits.Add(new StartActionUnit(OnStart, 0, _sendInclusionComplete));
                ActionUnits.Add(new ActionCompletedUnit(_sendInclusionComplete, SetStateCompleted));
            }

            protected override void CreateInstance()
            {
                _sendInclusionComplete = new SendDataOperation(SendInclusionComplete.NodeId, SendInclusionComplete.Data,
                    SendInclusionComplete.TxOptions);
            }

            private void OnStart(StartActionUnit ou)
            {
                if (SendInclusionComplete.StepId == 0x01)
                {
                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.InclusionComplete1, _sendInclusionComplete);
                }
                else if (SendInclusionComplete.StepId == 0x02)
                {
                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.InclusionComplete2, _sendInclusionComplete);
                }
            }
        }
    }
}
