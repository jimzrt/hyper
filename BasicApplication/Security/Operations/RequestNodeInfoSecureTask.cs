using System.Collections.Generic;
using System.Linq;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class RequestNodeInfoSecureTask : ActionSerialGroup
    {
        public static int CMD_SUPPORTED = 1000;
        public static int CMD_SUPPORTED_FLIRS = 500;
        public static int START_DELAY = 500;

        private TransmitOptions _txOptions =
           TransmitOptions.TransmitOptionAcknowledge |
           TransmitOptions.TransmitOptionAutoRoute |
           TransmitOptions.TransmitOptionExplore;
        private SecurityManagerInfo _securityManagerInfo;

        private RequestNodeInfoOperation _nodeInfo;
        private RequestDataExOperation _supportedS0;
        private RequestDataExOperation _supportedS2_ACCESS;
        private RequestDataExOperation _supportedS2_AUTHENTICATED;
        private RequestDataExOperation _supportedS2_UNAUTHENTICATED;
        private DelayOperation _delayBeforeStart;
        private bool _isInclusionTask { get; set; }

        public RequestNodeInfoSecureTask(SecurityManagerInfo securityManagerInfo, RequestNodeInfoOperation action, bool isInclusionTask)
        {
            _allowFailed = true;
            _securityManagerInfo = securityManagerInfo;
            _nodeInfo = action;
            _isInclusionTask = isInclusionTask;

            var tm = CMD_SUPPORTED;
            if (securityManagerInfo.Network.IsFlirs(_nodeInfo.NodeId)) //skip only for ENTRY
            {
                tm = CMD_SUPPORTED_FLIRS;
            }

            _delayBeforeStart = new DelayOperation(START_DELAY);

            _supportedS0 = new RequestDataExOperation(0, 0,
                    new COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_GET(), _txOptions,
                     TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, SecuritySchemes.S0, TransmitOptions2.NONE,
                    COMMAND_CLASS_SECURITY.ID, COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_REPORT.ID, tm);

            _supportedS2_ACCESS = new RequestDataExOperation(0, 0,
               new COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_GET(), _txOptions,
               TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, SecuritySchemes.S2_ACCESS, TransmitOptions2.NONE,
               COMMAND_CLASS_SECURITY_2.ID, COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT.ID, tm);

            _supportedS2_AUTHENTICATED = new RequestDataExOperation(0, 0,
                new COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_GET(), _txOptions,
                TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, SecuritySchemes.S2_AUTHENTICATED, TransmitOptions2.NONE,
                COMMAND_CLASS_SECURITY_2.ID, COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT.ID, tm);

            _supportedS2_UNAUTHENTICATED = new RequestDataExOperation(0, 0,
                new COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_GET(), _txOptions,
                TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, SecuritySchemes.S2_UNAUTHENTICATED, TransmitOptions2.NONE,
                COMMAND_CLASS_SECURITY_2.ID, COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT.ID, tm);

            List<ActionBase> list = new List<ActionBase>();
            list.Add(_delayBeforeStart);
            list.Add(_nodeInfo);
            if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
            {
                if (_securityManagerInfo.Network.IsSecuritySchemesSpecified(_nodeInfo.NodeId))
                {
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S2_ACCESS))
                    {
                        if (!_isInclusionTask || _securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S2_ACCESS))
                        {
                            list.Add(_supportedS2_ACCESS);
                        }
                    }
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S2_AUTHENTICATED))
                    {
                        if (!_isInclusionTask || _securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S2_AUTHENTICATED))
                        {
                            list.Add(_supportedS2_AUTHENTICATED);
                        }
                    }
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S2_UNAUTHENTICATED))
                    {
                        if (!_isInclusionTask || _securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S2_UNAUTHENTICATED))
                        {
                            list.Add(_supportedS2_UNAUTHENTICATED);
                        }
                    }
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                    {
                        if (!_isInclusionTask || _securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S0))
                        {
                            if (!list.Contains(_supportedS0))
                            {
                                list.Add(_supportedS0);
                            }
                        }
                    }
                }
                else
                {
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S2_ACCESS))
                    {
                        list.Add(_supportedS2_ACCESS);
                    }
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S2_AUTHENTICATED))
                    {
                        list.Add(_supportedS2_AUTHENTICATED);
                    }
                    if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S2_UNAUTHENTICATED))
                    {
                        list.Add(_supportedS2_UNAUTHENTICATED);
                    }
                }
            }
            if (_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
            {
                if (_securityManagerInfo.Network.IsSecuritySchemesSpecified(_nodeInfo.NodeId))
                {
                    if (_securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S0) &&
                        !_securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemeSet.ALLS2))
                    {
                        if (!list.Contains(_supportedS0))
                        {
                            list.Add(_supportedS0);
                        }
                    }
                }
                else
                {
                    if (!list.Contains(_supportedS0))
                    {
                        list.Add(_supportedS0);
                    }
                }
            }

            Actions = list.ToArray();
        }

        public RequestNodeInfoResult SpecificResult
        {
            get { return (RequestNodeInfoResult)Result; }
        }

        protected override void OnCompletedInternal(ActionCompletedUnit ou)
        {
            if (ou.Action.Result is RequestNodeInfoResult)
            {
                if (ou.Action.Result)
                {
                    _securityManagerInfo.Network.ResetCurrentSecurityScheme(_nodeInfo.NodeId);
                    var res = (RequestNodeInfoResult)ou.Action.Result;
                    _securityManagerInfo.Network.SetCommandClasses(new NodeTag(res.NodeId), res.CommandClasses);
                    res.CopyTo(SpecificResult);
                    _supportedS0.DestNodeId = _nodeInfo.NodeId;
                    _supportedS2_ACCESS.DestNodeId = _nodeInfo.NodeId;
                    _supportedS2_AUTHENTICATED.DestNodeId = _nodeInfo.NodeId;
                    _supportedS2_UNAUTHENTICATED.DestNodeId = _nodeInfo.NodeId;

                    List<SecuritySchemes> schemes = new List<SecuritySchemes>();

                    if (res.CommandClasses == null ||
                        !res.CommandClasses.Contains(COMMAND_CLASS_SECURITY_2.ID))
                    {
                        _supportedS2_ACCESS.Token.SetCancelled();
                        _supportedS2_AUTHENTICATED.Token.SetCancelled();
                        _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
                    }
                    else if (!_isInclusionTask && _securityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                    {
                        _securityManagerInfo.Network.ResetSecuritySchemes(_nodeInfo.NodeId);
                        _securityManagerInfo.Network.SetSecuritySchemes(_nodeInfo.NodeId, SecuritySchemeSet.ALL);
                    }

                    if (res.CommandClasses == null ||
                        !res.CommandClasses.Contains(COMMAND_CLASS_SECURITY.ID))
                    {
                        _supportedS0.Token.SetCancelled();
                    }
                    else if (!_isInclusionTask && _securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                    {
                        var schs = _securityManagerInfo.Network.GetSecuritySchemes(_nodeInfo.NodeId);
                        _securityManagerInfo.Network.ResetSecuritySchemes(_nodeInfo.NodeId);
                        if (schs != null)
                        {
                            _securityManagerInfo.Network.SetSecuritySchemes(_nodeInfo.NodeId, SecuritySchemeSet.ALL);
                        }
                        else
                        {
                            _securityManagerInfo.Network.SetSecuritySchemes(_nodeInfo.NodeId, SecuritySchemeSet.S0);
                        }
                    }
                }
                else
                {
                    _supportedS0.Token.SetCancelled();
                    _supportedS2_ACCESS.Token.SetCancelled();
                    _supportedS2_AUTHENTICATED.Token.SetCancelled();
                    _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
                }
            }
            else if (ou.Action.Result is RequestDataResult)
            {
                var res = (RequestDataResult)ou.Action.Result;
                if (res.State == ActionStates.Completed)
                {
                    if (SpecificResult.SecuritySchemes == null)
                    {
                        SpecificResult.SecuritySchemes = new[] { res.RxSecurityScheme };
                    }
                    else
                    {
                        SpecificResult.SecuritySchemes = SpecificResult.SecuritySchemes.Union(new[] { res.RxSecurityScheme }).ToArray();
                    }
                    if (_isInclusionTask)
                    {
                        switch (res.RxSecurityScheme)
                        {
                            case SecuritySchemes.NONE:
                                break;
                            case SecuritySchemes.S2_UNAUTHENTICATED:
                                _supportedS0.Token.SetCancelled();
                                break;
                            case SecuritySchemes.S2_AUTHENTICATED:
                                _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
                                _supportedS0.Token.SetCancelled();
                                break;
                            case SecuritySchemes.S2_ACCESS:
                                _supportedS2_AUTHENTICATED.Token.SetCancelled();
                                _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
                                _supportedS0.Token.SetCancelled();
                                break;
                            case SecuritySchemes.S0:
                                break;
                            default:
                                break;
                        }
                    }
                }

                var reqAction = ou.Action as RequestDataExOperation;
                if (reqAction != null)
                {
                    if (reqAction.Result.State == ActionStates.Expired && !reqAction.IsSendDataCompleted())
                    {
                        if (_supportedS2_ACCESS.Token.IsStateActive)
                        {
                            _supportedS2_ACCESS.Token.SetCancelled();
                        }
                        if (_supportedS2_AUTHENTICATED.Token.IsStateActive)
                        {
                            _supportedS2_AUTHENTICATED.Token.SetCancelled();
                        }
                        if (_supportedS2_UNAUTHENTICATED.Token.IsStateActive)
                        {
                            _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
                        }
                    }
                }
            }
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            base.SetStateCompleted(ou);
            _securityManagerInfo.Network.SetSecuritySchemesSpecified(_nodeInfo.NodeId);

            List<SecuritySchemes> schemes = new List<SecuritySchemes>();
            byte[] secureCommandClasses = null;
            if (_supportedS2_ACCESS.IsSendDataCompleted())
            {
                if (_supportedS2_ACCESS.Result)
                {
                    if (_supportedS2_ACCESS.SpecificResult.Command != null)
                    {
                        var commandClasses = ((COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT)_supportedS2_ACCESS.SpecificResult.Command).
                            commandClass.TakeWhile(x => x != 0xEF).ToArray();
                        if (commandClasses.Length > 0)
                        {
                            secureCommandClasses = commandClasses;
                        }
                    }
                    schemes.Add(SecuritySchemes.S2_ACCESS);
                }
            }
            else if (Actions.Contains(_supportedS2_ACCESS))
            {
                _supportedS2_AUTHENTICATED.Token.SetCancelled();
                _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
            }

            if (_supportedS2_AUTHENTICATED.IsSendDataCompleted())
            {
                if (_supportedS2_AUTHENTICATED.Result)
                {
                    if (_supportedS2_AUTHENTICATED.SpecificResult.Command != null)
                    {
                        var commandClasses = ((COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT)_supportedS2_AUTHENTICATED.SpecificResult.Command).
                            commandClass.TakeWhile(x => x != 0xEF).ToArray();
                        if (commandClasses.Length > 0)
                        {
                            secureCommandClasses = commandClasses;
                        }
                    }
                    schemes.Add(SecuritySchemes.S2_AUTHENTICATED);
                }
                else if (_supportedS2_AUTHENTICATED.Result.State == ActionStates.Cancelled &&
                        _securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S2_AUTHENTICATED))
                {
                    schemes.Add(SecuritySchemes.S2_AUTHENTICATED);
                }
            }
            else if (Actions.Contains(_supportedS2_AUTHENTICATED))
            {
                _supportedS2_UNAUTHENTICATED.Token.SetCancelled();
            }

            if (_supportedS2_UNAUTHENTICATED.IsSendDataCompleted())
            {
                if (_supportedS2_UNAUTHENTICATED.Result)
                {
                    if (_supportedS2_UNAUTHENTICATED.SpecificResult.Command != null)
                    {
                        var commandClasses = ((COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT)_supportedS2_UNAUTHENTICATED.SpecificResult.Command).
                            commandClass.TakeWhile(x => x != 0xEF).ToArray();
                        if (commandClasses.Length > 0)
                        {
                            secureCommandClasses = commandClasses;
                        }
                    }
                    schemes.Add(SecuritySchemes.S2_UNAUTHENTICATED);
                }
                else if (_supportedS2_UNAUTHENTICATED.Result.State == ActionStates.Cancelled &&
                    _securityManagerInfo.Network.HasSecurityScheme(_nodeInfo.NodeId, SecuritySchemes.S2_UNAUTHENTICATED))
                {
                    schemes.Add(SecuritySchemes.S2_UNAUTHENTICATED);
                }
            }

            if (_supportedS0.IsSendDataCompleted())
            {
                if (_supportedS0.Result)
                {
                    if (_supportedS0.SpecificResult.Command != null)
                    {
                        var commandClasses = ((COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_REPORT)_supportedS0.SpecificResult.Command).
                            commandClassSupport.TakeWhile(x => x != 0xEF).ToArray();
                        if (commandClasses.Length > 0)
                        {
                            secureCommandClasses = commandClasses;
                        }
                    }
                    schemes.Add(SecuritySchemes.S0);
                }
            }

            if (schemes.Count > 0)
            {
                if (!_isInclusionTask)
                {
                    _securityManagerInfo.Network.SetSecuritySchemes(_nodeInfo.NodeId, schemes.ToArray());
                }
                _securityManagerInfo.Network.SetSecureCommandClasses(new NodeTag(_nodeInfo.NodeId), secureCommandClasses);
                SpecificResult.SecureCommandClasses = secureCommandClasses;

                if (SecuritySchemeSet.ALLS2.Contains(schemes[0]))
                {
                    var peerNodeId = new InvariantPeerNodeId(_securityManagerInfo.Network.NodeId, _nodeInfo.NodeId);
                    _securityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, schemes[0]);
                }
            }
            else
            {
                if (!_isInclusionTask)
                {
                    _securityManagerInfo.Network.ResetSecuritySchemes(_nodeInfo.NodeId);
                    SpecificResult.SecureCommandClasses = null;
                }
            }
        }
    }
}
