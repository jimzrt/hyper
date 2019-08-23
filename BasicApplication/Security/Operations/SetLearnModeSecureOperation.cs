using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class SetLearnModeSecureOperation : ApiOperation
    {
        private ApiOperation _learnMode;
        private SecurityManagerInfo _securityManagerInfo;
        private Action _resetSecurityCallback;
        private int _timeoutMs;
        private byte[] _previousHomeId;

        public SetLearnModeSecureOperation(SecurityManagerInfo securityManagerInfo, SetLearnModeSlaveOperation learnMode, Action resetSecurityCallback)
           : this(securityManagerInfo, learnMode, resetSecurityCallback, learnMode.TimeoutMs)
        {
        }

        public SetLearnModeSecureOperation(SecurityManagerInfo securityManagerInfo, SetLearnModeControllerOperation learnMode, Action resetSecurityCallback)
            : this(securityManagerInfo, learnMode, resetSecurityCallback, learnMode.TimeoutMs)
        {
        }

        public SetLearnModeSecureOperation(SecurityManagerInfo securityManagerInfo, SetSlaveLearnModeOperation learnMode, Action resetSecurityCallback)
            : this(securityManagerInfo, learnMode, resetSecurityCallback, learnMode.TimeoutMs)
        {
        }

        private SetLearnModeSecureOperation(SecurityManagerInfo securityManagerInfo, ApiOperation learnMode, Action resetSecurityCallback, int timeoutMs)
            : base(false, learnMode.SerialApiCommands, false)
        {
            _timeoutMs = timeoutMs;
            _securityManagerInfo = securityManagerInfo;
            _learnMode = learnMode;
            _resetSecurityCallback = resetSecurityCallback;
            _previousHomeId = _securityManagerInfo.Network.HomeId;
        }

        private ExpectDataOperation _expectSchemeGet;
        private ExpectDataOperation _expectKexGet;
        private SetLearnModeS0Operation _learnModeS0;
        private SetLearnModeS2Operation _learnModeS2;
        private MemoryGetIdOperation _memoryGetId;
        private SerialApiGetInitDataOperation _serialApiGetInitData;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnStart, 0));
            ActionUnits.Add(new ActionCompletedUnit(_learnMode, null, _memoryGetId));
            ActionUnits.Add(new ActionCompletedUnit(_memoryGetId, OnMemoryGetIdCompleted, _serialApiGetInitData));
            ActionUnits.Add(new ActionCompletedUnit(_serialApiGetInitData, OnLearnModeCompleted));
            ActionUnits.Add(new ActionCompletedUnit(_expectSchemeGet, OnExpectSchemeGetCompleted));
            ActionUnits.Add(new ActionCompletedUnit(_expectKexGet, OnExpectKexGetCompleted));
            ActionUnits.Add(new ActionCompletedUnit(_learnModeS0, OnLearnModeS0Completed));
            ActionUnits.Add(new ActionCompletedUnit(_learnModeS2, OnLearnModeS2Completed));
        }

        protected override void CreateInstance()
        {
            _expectSchemeGet = new ExpectDataOperation(0, 0, new COMMAND_CLASS_SECURITY.SECURITY_SCHEME_GET(), 2, _timeoutMs * 2)
            {
                Name = "Expect SCHEME_GET",
                IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad
            };

            _expectKexGet = new ExpectDataOperation(0, 0, new COMMAND_CLASS_SECURITY_2.KEX_GET(), 2, _timeoutMs * 2)
            {
                Name = "Expect KEX_GET",
                IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad
            };
            _memoryGetId = new MemoryGetIdOperation();
            _serialApiGetInitData = new SerialApiGetInitDataOperation();
            _learnModeS2 = new SetLearnModeS2Operation(_securityManagerInfo);
            _learnModeS0 = new SetLearnModeS0Operation(_securityManagerInfo, 0, 0);
        }

        private void OnStart(StartActionUnit unit)
        {
            List<ActionBase> nextItems = new List<ActionBase>(5);
            if (_securityManagerInfo.Network.IsEnabledS0)
            {
                nextItems.Add(_expectSchemeGet);
            }
            if (_securityManagerInfo.Network.IsEnabledS2_ACCESS ||
                _securityManagerInfo.Network.IsEnabledS2_AUTHENTICATED ||
                _securityManagerInfo.Network.IsEnabledS2_UNAUTHENTICATED)
            {
                nextItems.Add(_expectKexGet);
            }
            nextItems.Add(_learnMode);
            unit.SetNextActionItems(nextItems.ToArray());
        }

        private void OnMemoryGetIdCompleted(ActionCompletedUnit unit)
        {
            if (_memoryGetId.Result)
            {
                var lmc = _learnMode as SetLearnModeControllerOperation;
                var lms = _learnMode as SetLearnModeSlaveOperation;
                var lmv = _learnMode as SetSlaveLearnModeOperation;
                if ((lmc != null && lmc.SpecificResult.NodeId == 0) ||
                    (lms != null && lms.SpecificResult.NodeId == 0) ||
                    (lmv != null && lmv.SpecificResult.NodeId == 0))
                {
                    SpecificResult.LearnModeStatus = LearnModeStatuses.Removed;
                }
                else if (lmc != null && _memoryGetId.SpecificResult.HomeId.SequenceEqual(_previousHomeId))
                {
                    SpecificResult.LearnModeStatus = LearnModeStatuses.Replicated;
                }
                else
                {
                    SpecificResult.LearnModeStatus = LearnModeStatuses.Added;
                }

                if (lmc != null)
                {
                    SpecificResult.NodeId = _memoryGetId.SpecificResult.NodeId;
                    _securityManagerInfo.Network.NodeId = _memoryGetId.SpecificResult.NodeId;
                    _securityManagerInfo.Network.HomeId = _memoryGetId.SpecificResult.HomeId;
                }
                else if (lmv != null)
                {
                    SpecificResult.NodeId = lmv.SpecificResult.NodeId;
                }
            }
        }

        private void OnExpectSchemeGetCompleted(ActionCompletedUnit unit)
        {
            if (_learnMode.Result && _memoryGetId.Result && _serialApiGetInitData.Result)
            {
                if (_expectSchemeGet.Result)
                {
                    _expectKexGet.Token.SetCancelled();
                    _learnModeS0.NodeId = _expectSchemeGet.SpecificResult.SrcNodeId;
                    if (_learnMode is SetSlaveLearnModeOperation)
                    {
                        _learnModeS0.VirtualNodeId = _expectSchemeGet.SpecificResult.DestNodeId;
                    }
                    else
                    {
                        _learnModeS0.IsController = _learnMode is SetLearnModeControllerOperation;
                        _securityManagerInfo.Network.ResetAndEnableAndSelfRestore();
                    }
                    COMMAND_CLASS_SECURITY.SECURITY_SCHEME_GET cmd = _expectSchemeGet.SpecificResult.Command;
                    _learnModeS0.SupportedSecuritySchemes = cmd.supportedSecuritySchemes;
                    unit.SetNextActionItems(_learnModeS0);
                }
                else if (_expectSchemeGet.Result.State == ActionStates.Expired)
                {
                    SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
                    _securityManagerInfo.Network.ResetSecuritySchemes();
                    _securityManagerInfo.Network.ResetSecuritySchemes(_expectSchemeGet.SpecificResult.SrcNodeId);
                    SetStateCompleted(unit);
                }
            }
        }

        private void OnExpectKexGetCompleted(ActionCompletedUnit unit)
        {
            if (_learnMode.Result && _memoryGetId.Result && _serialApiGetInitData.Result)
            {
                if (_expectKexGet.Result)
                {
                    _expectSchemeGet.Token.SetCancelled();
                    _learnModeS2.NodeId = _expectKexGet.SpecificResult.SrcNodeId;
                    if (_learnMode is SetSlaveLearnModeOperation)
                    {
                        _learnModeS2.VirtualNodeId = _expectKexGet.SpecificResult.DestNodeId;
                    }
                    else
                    {
                        _securityManagerInfo.Network.ResetAndEnableAndSelfRestore();
                    }
                    unit.SetNextActionItems(_learnModeS2);
                }
                else if (_expectKexGet.Result.State == ActionStates.Expired)
                {
                    SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
                    _securityManagerInfo.Network.ResetSecuritySchemes();
                    _securityManagerInfo.Network.ResetSecuritySchemes(_expectKexGet.SpecificResult.SrcNodeId);
                    SetStateCompleted(unit);
                }
            }
        }

        private void OnLearnModeCompleted(ActionCompletedUnit unit)
        {
            if (_learnMode.Result && _memoryGetId.Result && _serialApiGetInitData.Result)
            {
                if (_expectSchemeGet.Result)
                {
                    OnExpectSchemeGetCompleted(unit);
                }
                else if (_expectKexGet.Result)
                {
                    OnExpectKexGetCompleted(unit);
                }
                else
                {
                    switch (SpecificResult.LearnModeStatus)
                    {
                        case LearnModeStatuses.None:
                            break;
                        case LearnModeStatuses.Added:
                            if (_securityManagerInfo.Network.IsEnabledS0)
                            {
                                _expectSchemeGet.Token.Reset(InclusionS2TimeoutConstants.Joining.PublicKeyReport);
                            }
                            else if (_securityManagerInfo.Network.IsEnabledS2_ACCESS | _securityManagerInfo.Network.IsEnabledS2_AUTHENTICATED | _securityManagerInfo.Network.IsEnabledS2_UNAUTHENTICATED)
                            {
                                _expectKexGet.Token.Reset(InclusionS2TimeoutConstants.Joining.KexGet);
                            }
                            else
                            {
                                SetStateCompleted(unit);
                            }
                            break;
                        case LearnModeStatuses.Removed:
                            _expectSchemeGet.Token.SetCancelled();
                            _expectKexGet.Token.SetCancelled();
                            if (!(_learnMode is SetSlaveLearnModeOperation))
                            {
                                _resetSecurityCallback();
                            }
                            SetStateCompleted(unit);
                            break;
                        case LearnModeStatuses.Changed:
                            break;
                        case LearnModeStatuses.Replicated:
                            _expectSchemeGet.Token.SetCancelled();
                            _expectKexGet.Token.SetCancelled();
                            SetStateCompleted(unit);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                SetStateFailed(unit);
            }
        }

        private void OnLearnModeS0Completed(ActionCompletedUnit unit)
        {
            SetStateCompleted(unit);
        }

        private void OnLearnModeS2Completed(ActionCompletedUnit unit)
        {
            SetStateCompleted(unit);
        }

        public SetLearnModeResult SpecificResult
        {
            get { return (SetLearnModeResult)Result; }
        }

        public override string AboutMe()
        {
            if (Result as SetLearnModeResult != null)
            {
                return string.Format("Id={0}, Security={1}", SpecificResult.NodeId, SpecificResult.SubstituteStatus);
            }
            else
            {
                return "";
            }
        }
    }
}
