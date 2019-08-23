using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class SetLearnModeS2Operation : ApiOperation
    {
        private SecurityManagerInfo _securityManagerInfo;
        private readonly ISecurityTestSettingsService _securityTestSettingsService;
        internal byte NodeId { get; set; }
        internal byte VirtualNodeId { get; set; }
        internal SetLearnModeS2Operation(SecurityManagerInfo securityManagerInfo)
            : base(false, null, false)
        {
            _securityManagerInfo = securityManagerInfo;
            _prevHomeId = _securityManagerInfo.Network.HomeId;
            _securityTestSettingsService = new SecurityTestSettingsService(_securityManagerInfo, true);
        }

        COMMAND_CLASS_SECURITY_2.KEX_SET _KEX_SET = null;
        DateTime _PkReportReceivedTimeStamp = DateTime.MinValue;
        private bool _isClientSideAuthRequested = false;
        private bool _isClientSideAuthGranted = false;
        private InvariantPeerNodeId _peerNodeId;
        private bool _isWaitingForKexReportEcho = false;

        private RequestDataOperation _KEXReportKEXSet;
        private RequestDataOperation _PKReportPKReport; // Receive and load tmp key.
        private RequestDataOperation _KEXSetEchoKEXReportEcho;
        private RequestDataOperation _NKGetNKReport;
        private RequestDataOperation _NKVerifyTransferEnd; // Send and load tmp key.
        private SendDataExOperation _TransferEnd; // Send and load real key.
        private SendDataExOperation _KexFail;
        private SendDataExOperation _KexFailCancel;
        private ResponseDataOperation _KexFailReceived;
        private ResponseDataOperation _SecurityMessageReceived;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnKEXGet, 0, _KEXReportKEXSet, _KexFailReceived, _SecurityMessageReceived));
            ActionUnits.Add(new ActionCompletedUnit(_KEXReportKEXSet, OnKEXSet, _PKReportPKReport));
            ActionUnits.Add(new ActionCompletedUnit(_PKReportPKReport, OnPKReport, _KEXSetEchoKEXReportEcho));
            ActionUnits.Add(new ActionCompletedUnit(_KEXSetEchoKEXReportEcho, OnKEXReportEcho));
            ActionUnits.Add(new ActionCompletedUnit(_NKGetNKReport, OnNKReport, _NKVerifyTransferEnd));
            ActionUnits.Add(new ActionCompletedUnit(_NKVerifyTransferEnd, OnTransferEnd));
            ActionUnits.Add(new ActionCompletedUnit(_TransferEnd, OnSendTransferEnd));
            ActionUnits.Add(new ActionCompletedUnit(_KexFail, OnKexFail));
            ActionUnits.Add(new ActionCompletedUnit(_KexFailReceived, OnKexFailReceived));
            ActionUnits.Add(new ActionCompletedUnit(_SecurityMessageReceived, OnSecurityMessageReceived));
            ActionUnitStop = new ActionUnit(_KexFailCancel);
        }

        protected override void CreateInstance()
        {
            _KexFailReceived = new ResponseDataOperation(OnKexFailCallback, TransmitOptions.TransmitOptionNone, 0, COMMAND_CLASS_SECURITY_2.ID, COMMAND_CLASS_SECURITY_2.KEX_FAIL.ID);
            _SecurityMessageReceived = new ResponseDataOperation(OnSecurityMessageCallback, TransmitOptions.TransmitOptionNone, 0,
                COMMAND_CLASS_SECURITY_2.ID, COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.ID);

            _KEXReportKEXSet = new RequestDataOperation(0, 0, null, _securityManagerInfo.TxOptions, new COMMAND_CLASS_SECURITY_2.KEX_SET(), 2, InclusionS2TimeoutConstants.Joining.KexSet);
            _KEXReportKEXSet.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            _KEXReportKEXSet.Name = "RequestData KEX_REPORT/KEX_SET";
            _KEXReportKEXSet.IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad;

            _PKReportPKReport = new RequestDataOperation(0, 0, null,
                _securityManagerInfo.TxOptions, new COMMAND_CLASS_SECURITY_2.PUBLIC_KEY_REPORT(), 2, InclusionS2TimeoutConstants.Joining.PublicKeyReport);
            _PKReportPKReport.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            _PKReportPKReport.Name = "RequestData PUBLIC_KEY_REPORT/PUBLIC_KEY_REPORT";
            _PKReportPKReport.IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad;

            _KEXSetEchoKEXReportEcho = new RequestDataOperation(0, 0, null,
              _securityManagerInfo.TxOptions, new COMMAND_CLASS_SECURITY_2.KEX_REPORT(), 2, InclusionS2TimeoutConstants.Joining.PublicKeyReport);
            _KEXSetEchoKEXReportEcho.Name = "RequestData KEX_SET(echo)/KEX_REPORT(echo)";
            _KEXSetEchoKEXReportEcho.SubstituteSettings.SetFlag(SubstituteFlags.UseSecurity);
            _KEXSetEchoKEXReportEcho.IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad;

            _NKGetNKReport = new RequestDataOperation(0, 0, null,
             _securityManagerInfo.TxOptions, new COMMAND_CLASS_SECURITY_2.SECURITY_2_NETWORK_KEY_REPORT(), 2, InclusionS2TimeoutConstants.Joining.NetworkKeyReport);
            _NKGetNKReport.Name = "RequestData NETWORK_KEY_GET/NETWORK_KEY_REPORT";
            _NKGetNKReport.SubstituteSettings.SetFlag(SubstituteFlags.UseSecurity);
            _NKGetNKReport.IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad;

            _NKVerifyTransferEnd = new RequestDataOperation(0, 0, null,
             _securityManagerInfo.TxOptions, new COMMAND_CLASS_SECURITY_2.SECURITY_2_TRANSFER_END(), 2, InclusionS2TimeoutConstants.Joining.TransferEnd);
            _NKVerifyTransferEnd.Name = "RequestData NETWORK_KEY_VERIFY/TRANSFER_END";
            _NKVerifyTransferEnd.SubstituteSettings.SetFlag(SubstituteFlags.UseSecurity);
            _NKVerifyTransferEnd.IgnoreRxStatuses = ReceiveStatuses.TypeMulti | ReceiveStatuses.TypeBroad;

            _TransferEnd = new SendDataExOperation(0, 0, new COMMAND_CLASS_SECURITY_2.SECURITY_2_TRANSFER_END(), _securityManagerInfo.TxOptions, SecuritySchemes.S2_TEMP);
            _TransferEnd.SubstituteSettings.SetFlag(SubstituteFlags.UseSecurity);
            _TransferEnd.Name = "SendData TRANSFER_END";

            _KexFail = new SendDataExOperation(0, 0, null, _securityManagerInfo.TxOptions, SecuritySchemes.S2_TEMP);
            _KexFail.Name = "RequestData KEX_FAIL";
            _KexFail.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);

            _KexFailCancel = new SendDataExOperation(0, 0, null, _securityManagerInfo.TxOptions, SecuritySchemes.NONE);
            _KexFailCancel.Name = "SendData KEX_FAIL_CANCEL";
            _KexFailCancel.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            _KexFailCancel.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL() { kexFailType = 0x06 };
        }

        private void OnKexFail(ActionCompletedUnit ou)
        {
            SetStateCompletedSecurityFailed(ou);
        }

        private List<byte[]> OnKexFailCallback(AchData achData)
        {
            _KexFailReceived.Token.SetCompleted();
            return null;
        }

        private List<byte[]> OnSecurityMessageCallback(AchData achData)
        {
            if (((SubstituteIncomingFlags)achData.SubstituteIncomingFlags).HasFlag(SubstituteIncomingFlags.SecurityFailed))
            {
                _SecurityMessageReceived.Token.SetCompleted();
            }
            return null;
        }

        protected void OnKexFailReceived(ActionUnit ou)
        {
            if (_KexFailReceived.Result)
            {
                SetStateCompletedSecurityFailed(ou);
            }
        }

        protected void OnSecurityMessageReceived(ActionCompletedUnit ou)
        {
            if (_SecurityMessageReceived.Result)
            {
                if (_isWaitingForKexReportEcho)
                {
                    Thread.Sleep(3000);
                    _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                }
                _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x05 };
                _KexFail.NodeId = NodeId;
                _KexFail.BridgeNodeId = VirtualNodeId;
                ou.SetNextActionItems(_KexFail);
            }
        }

        protected void SetStateCompletedSecurityFailed(ActionUnit ou)
        {
            if (VirtualNodeId == 0)
            {
                _securityManagerInfo.Network.ResetSecuritySchemes();
                _securityManagerInfo.Network.ResetSecuritySchemes(NodeId);
            }
            _securityManagerInfo.IsInclusion = false;
            SetStateCompleted(ou);
        }

        protected void SetStateCompletedSecurityDone(ActionUnit ou)
        {
            SpecificResult.SubstituteStatus = SubstituteStatuses.Done;
            if (VirtualNodeId == 0)
            {
                _securityManagerInfo.Network.SetSecuritySchemes(_grantedSchemes.ToArray());
                _securityManagerInfo.Network.SetSecuritySchemes(NodeId, _grantedSchemes.ToArray());
                if (_grantedSchemes.Contains(SecuritySchemes.S2_ACCESS))
                {
                    _securityManagerInfo.ActivateNetworkKeyS2ForNode(_peerNodeId, SecuritySchemes.S2_ACCESS);
                }
                else if (_grantedSchemes.Contains(SecuritySchemes.S2_AUTHENTICATED))
                {
                    _securityManagerInfo.ActivateNetworkKeyS2ForNode(_peerNodeId, SecuritySchemes.S2_AUTHENTICATED);
                }
                else if (_grantedSchemes.Contains(SecuritySchemes.S2_UNAUTHENTICATED))
                {
                    _securityManagerInfo.ActivateNetworkKeyS2ForNode(_peerNodeId, SecuritySchemes.S2_UNAUTHENTICATED);
                }
            }
            _securityManagerInfo.IsInclusion = false;
            SetStateCompleted(ou);
        }

        protected void SetStateCompletedSecurityNone(ActionUnit ou)
        {
            SpecificResult.SubstituteStatus = SubstituteStatuses.None;
            if (VirtualNodeId == 0)
            {
                _securityManagerInfo.Network.ResetSecuritySchemes();
                _securityManagerInfo.Network.ResetSecuritySchemes(NodeId);
            }
            _securityManagerInfo.IsInclusion = false;
            SetStateCompleted(ou);
        }

        protected override void SetStateFailed(ActionUnit ou)
        {
            if (VirtualNodeId == 0)
            {
                _securityManagerInfo.Network.ResetSecuritySchemes();
                _securityManagerInfo.Network.ResetSecuritySchemes(NodeId);
            }
            _securityManagerInfo.IsInclusion = false;
            base.SetStateFailed(ou);
        }

        private byte[] _prevHomeId = null;
        DateTime kexReportStart;
        private void OnKEXGet(StartActionUnit ou)
        {
            kexReportStart = DateTime.Now;
            SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
            _securityManagerInfo.IsInclusion = true;

            _KexFailCancel.NodeId = NodeId;
            _KexFailCancel.BridgeNodeId = VirtualNodeId;

            _KEXReportKEXSet.DestNodeId = NodeId;
            _KEXReportKEXSet.SrcNodeId = VirtualNodeId;
            if (VirtualNodeId == 0)
            {
                _peerNodeId = new InvariantPeerNodeId(_securityManagerInfo.Network.NodeId, NodeId);
            }
            else
            {
                _peerNodeId = new InvariantPeerNodeId(VirtualNodeId, NodeId);
            }
            var cmd = new COMMAND_CLASS_SECURITY_2.KEX_REPORT();

            _isClientSideAuthRequested = _securityManagerInfo.TestEnableClientSideAuthS2;
            cmd.properties1 = new COMMAND_CLASS_SECURITY_2.KEX_REPORT.Tproperties1()
            {
                requestCsa = _isClientSideAuthRequested ? (byte)1 : (byte)0
            };
            byte keysToRequest = (byte)(NetworkKeyS2Flags.S2Class0 |
                NetworkKeyS2Flags.S2Class1 |
                NetworkKeyS2Flags.S2Class2 |
                NetworkKeyS2Flags.S0);

            if (!_securityManagerInfo.Network.IsEnabledS0)
            {
                keysToRequest = (byte)(keysToRequest & ~(byte)(NetworkKeyS2Flags.S0));
            }
            if (!_securityManagerInfo.Network.IsEnabledS2_UNAUTHENTICATED)
            {
                keysToRequest = (byte)(keysToRequest & ~(byte)(NetworkKeyS2Flags.S2Class0));
            }
            if (!_securityManagerInfo.Network.IsEnabledS2_AUTHENTICATED)
            {
                keysToRequest = (byte)(keysToRequest & ~(byte)(NetworkKeyS2Flags.S2Class1));
            }
            if (!_securityManagerInfo.Network.IsEnabledS2_ACCESS)
            {
                keysToRequest = (byte)(keysToRequest & ~(byte)(NetworkKeyS2Flags.S2Class2));
            }

            cmd.requestedKeys = keysToRequest;
            cmd.supportedEcdhProfiles = 1;
            cmd.supportedKexSchemes = 2;
            _KEXReportKEXSet.Data = cmd;
            #region KEXReport
            if (_securityManagerInfo.TestFramesS2.ContainsKey(SecurityS2TestFrames.KEXReport))
            {
                var testFrame = _securityManagerInfo.TestFramesS2[SecurityS2TestFrames.KEXReport];
                if (testFrame.Command != null && testFrame.Command.Length > 5 &&
                    testFrame.Command[0] == COMMAND_CLASS_SECURITY_2.ID && testFrame.Command[1] == COMMAND_CLASS_SECURITY_2.KEX_REPORT.ID)
                {
                    COMMAND_CLASS_SECURITY_2.KEX_REPORT tmp = testFrame.Command;
                    keysToRequest = tmp.requestedKeys;
                }
                _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.KEXReport, _KEXReportKEXSet);
            }
            #endregion
            var duration = (int)(DateTime.Now - kexReportStart).TotalMilliseconds;
            if (duration > InclusionS2TimeoutConstants.Joining.PublicKeyReport)
            {
                _KEXReportKEXSet.SetNewExpectTimeout(100);
            }
        }

        private Queue<NetworkKeyS2Flags> _grantedKeys = new Queue<NetworkKeyS2Flags>();
        private List<SecuritySchemes> _grantedSchemes = new List<SecuritySchemes>();
        private void OnKEXSet(ActionCompletedUnit ou)
        {
            if (_KEXReportKEXSet.Result &&
                _KEXReportKEXSet.Result.State == ActionStates.Completed &&
                _KEXReportKEXSet.SpecificResult.Command != null &&
                _KEXReportKEXSet.SpecificResult.Command.Length > 2)
            {
                _grantedKeys.Clear();
                _KEX_SET = _KEXReportKEXSet.SpecificResult.Command;
                if (_KEX_SET.properties1.echo == 0 && _KEX_SET.selectedKexScheme == 0x02 && _KEX_SET.selectedEcdhProfile == 0x01)
                {
                    if (CheckIsCsaCorrectlyGranted() && ValidateKexSetKeys())
                    {
                        bool hasHighKeys = false;
                        NetworkKeyS2Flags grantedKeysMask = (NetworkKeyS2Flags)_KEX_SET.grantedKeys;
                        if (grantedKeysMask.HasFlag(NetworkKeyS2Flags.S2Class2))
                        {
                            _grantedKeys.Enqueue(NetworkKeyS2Flags.S2Class2);
                            hasHighKeys = true;
                        }
                        if (grantedKeysMask.HasFlag(NetworkKeyS2Flags.S2Class1))
                        {
                            _grantedKeys.Enqueue(NetworkKeyS2Flags.S2Class1);
                            hasHighKeys = true;
                        }
                        if (grantedKeysMask.HasFlag(NetworkKeyS2Flags.S2Class0))
                        {
                            _grantedKeys.Enqueue(NetworkKeyS2Flags.S2Class0);
                        }
                        if (grantedKeysMask.HasFlag(NetworkKeyS2Flags.S0))
                        {
                            _grantedKeys.Enqueue(NetworkKeyS2Flags.S0);
                        }

                        _PKReportPKReport.DestNodeId = NodeId;
                        _PKReportPKReport.SrcNodeId = VirtualNodeId;
                        var cmd = new COMMAND_CLASS_SECURITY_2.PUBLIC_KEY_REPORT();
                        cmd.properties1 = 0;
                        cmd.ecdhPublicKey = new List<byte>(_securityManagerInfo.GetJoinPublicKeyS2());
                        _isClientSideAuthGranted = _KEX_SET.properties1.requestCsa > 0 ? true : false;
                        if (hasHighKeys && !_isClientSideAuthGranted && cmd.ecdhPublicKey.Count > 1)
                        {
                            cmd.ecdhPublicKey[0] = 0;
                            cmd.ecdhPublicKey[1] = 0;
                            if (_securityManagerInfo.DskPinCallback != null)
                            {
                                _securityManagerInfo.DskPinCallback();
                            }
                        }
                        _PKReportPKReport.Data = cmd;
                        #region PublicKeyReportB
                        _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.PublicKeyReportB, _PKReportPKReport);
                        #endregion
                    }
                    else
                    {
                        _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x01 };
                        _KexFail.NodeId = NodeId;
                        _KexFail.BridgeNodeId = VirtualNodeId;
                        ou.SetNextActionItems(_KexFail);
                    }
                }
                else
                {
                    byte currentKexFailType = _KEX_SET.selectedKexScheme != 0x02 ? (byte)0x02 : (byte)0x00;
                    if (currentKexFailType == 0x00)
                    {
                        currentKexFailType = _KEX_SET.selectedEcdhProfile != 0x01 ? (byte)0x03 : (byte)0x00;
                    }

                    _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = currentKexFailType };
                    _KexFail.NodeId = NodeId;
                    _KexFail.BridgeNodeId = VirtualNodeId;
                    ou.SetNextActionItems(_KexFail);
                }
            }
            else
                SetStateCompletedSecurityFailed(ou);
        }

        private bool ValidateKexSetKeys()
        {
            bool ret = true;
            byte tmpFlags = _KEX_SET.grantedKeys;
            tmpFlags = (byte)(tmpFlags & ~(byte)NetworkKeyS2Flags.S0);
            tmpFlags = (byte)(tmpFlags & ~(byte)NetworkKeyS2Flags.S2Class0);
            tmpFlags = (byte)(tmpFlags & ~(byte)NetworkKeyS2Flags.S2Class1);
            tmpFlags = (byte)(tmpFlags & ~(byte)NetworkKeyS2Flags.S2Class2);

            if (tmpFlags != 0)
            {
                ret = false;
            }
            return ret;
        }

        private bool CheckIsCsaCorrectlyGranted()
        {
            bool ret = true;
            if (!_isClientSideAuthRequested && _KEX_SET.properties1.requestCsa == 1)
            {
                ret = false;
            }
            return ret;
        }

        private void OnPKReport(ActionCompletedUnit ou)
        {
            _PkReportReceivedTimeStamp = DateTime.Now;
            if (_PKReportPKReport.Result.State == ActionStates.Completed &&
                _PKReportPKReport.SpecificResult.Command != null &&
                _PKReportPKReport.SpecificResult.Command.Length > 2)
            {
                COMMAND_CLASS_SECURITY_2.PUBLIC_KEY_REPORT rpt = _PKReportPKReport.SpecificResult.Command;
                if (rpt.properties1.includingNode == 1)
                {
                    var receiverPublicKey = ((List<byte>)rpt.ecdhPublicKey).ToArray();
                    if (receiverPublicKey != null && receiverPublicKey.Length == 32 && ValidatePublicKeyReport(receiverPublicKey))
                    {
                        var senderPublicKey = _securityManagerInfo.GetJoinPublicKeyS2();
                        if (_isClientSideAuthRequested && _KEX_SET.properties1.requestCsa > 0 && _securityManagerInfo.DSKVerificationOnReceiverCallback != null)
                        {
                            byte[] senderDSK = _securityManagerInfo.DSKVerificationOnReceiverCallback();
                            if (senderDSK != null && senderDSK.Length == 4)
                            {
                                Array.Copy(senderDSK, 0, receiverPublicKey, 0, 4);
                            }
                        }
                        _securityManagerInfo.SetNetworkKeyS2Temp(_securityManagerInfo.CalculateTempNetworkKeyS2(receiverPublicKey, false));
                        _securityManagerInfo.ActivateNetworkKeyS2TempForNode(_peerNodeId);

                        var kexSetEcho = _KEX_SET;
                        kexSetEcho.properties1.echo = 1;
                        _KEXSetEchoKEXReportEcho.DestNodeId = NodeId;
                        _KEXSetEchoKEXReportEcho.SrcNodeId = VirtualNodeId;
                        _KEXSetEchoKEXReportEcho.Data = kexSetEcho;

                        _isWaitingForKexReportEcho = true;
                        #region KEXSetEcho
                        _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.KEXSetEcho, _KEXSetEchoKEXReportEcho);
                        #endregion
                    }
                    else
                    {
                        _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x00 };
                        _KexFail.NodeId = NodeId;
                        _KexFail.BridgeNodeId = VirtualNodeId;
                        ou.SetNextActionItems(_KexFail);
                    }
                }
                else
                    SetStateCompletedSecurityFailed(ou);
            }
            else
                SetStateCompletedSecurityFailed(ou);
        }

        private bool ValidatePublicKeyReport(byte[] receiverPublicKey)
        {
            bool ret = false;
            if (_isClientSideAuthRequested && _KEX_SET.properties1.requestCsa > 0)
            {
                ret = receiverPublicKey.Take(4).SequenceEqual(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            }
            else
            {
                ret = true;
            }
            return ret;
        }

        private void OnKEXReportEcho(ActionCompletedUnit ou)
        {
            if (_KEXSetEchoKEXReportEcho.Result)
            {
                _isWaitingForKexReportEcho = false;
                if (_KEXSetEchoKEXReportEcho.SpecificResult.RxSecurityScheme == SecuritySchemes.S2_TEMP)
                {
                    if (ValidateKexReportEcho(_KEXSetEchoKEXReportEcho.SpecificResult.Command))
                    {
                        _NKGetNKReport.DestNodeId = NodeId;
                        _NKGetNKReport.SrcNodeId = VirtualNodeId;
                        if (_grantedKeys.Count > 0)
                        {
                            var cmd = new COMMAND_CLASS_SECURITY_2.SECURITY_2_NETWORK_KEY_GET();
                            var key = _grantedKeys.Dequeue();
                            cmd.requestedKey = (byte)key;
                            _NKGetNKReport.Data = cmd;

                            ou.SetNextActionItems(_NKGetNKReport);

                            #region TestFrame Section
                            var scheme = SecurityManagerInfo.ConvertToSecurityScheme(key);
                            switch (scheme)
                            {
                                case SecuritySchemes.S0:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S0, _NKGetNKReport);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_UNAUTHENTICATED:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S2Unauthenticated, _NKGetNKReport);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_AUTHENTICATED:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S2Authenticated, _NKGetNKReport);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_ACCESS:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S2Access, _NKGetNKReport);
                                    #endregion
                                    break;
                            }
                            #endregion
                        }
                        else
                        {
                            _TransferEnd.NodeId = NodeId;
                            _TransferEnd.BridgeNodeId = VirtualNodeId;
                            var cmd = new COMMAND_CLASS_SECURITY_2.SECURITY_2_TRANSFER_END();
                            cmd.properties1.keyRequestComplete = 1;
                            cmd.properties1.keyVerified = 0;
                            _TransferEnd.Data = cmd;

                            #region TransferEndB
                            _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.TransferEndB, _TransferEnd);
                            #endregion
                            ou.SetNextActionItems(_TransferEnd);
                        }
                    }
                    else
                    {
                        _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                        _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x07 }; // KEX_FAIL_AUTH
                        _KexFail.NodeId = NodeId;
                        _KexFail.BridgeNodeId = VirtualNodeId;
                        ou.SetNextActionItems(_KexFail);
                    }
                }
                else
                {
                    _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                    _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x07 }; // KEX_FAIL_AUTH
                    _KexFail.NodeId = NodeId;
                    _KexFail.BridgeNodeId = VirtualNodeId;
                    ou.SetNextActionItems(_KexFail);
                }
            }
            else if (_isClientSideAuthGranted)
            {
                _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x06 }; // KEX_FAIL_CANCEL
                _KexFail.NodeId = NodeId;
                _KexFail.BridgeNodeId = VirtualNodeId;
                ou.SetNextActionItems(_KexFail);
            }
            else if ((DateTime.Now - _PkReportReceivedTimeStamp).TotalMilliseconds < InclusionS2TimeoutConstants.Including.UserInputDsk)
            {
                _KEXSetEchoKEXReportEcho.NewToken();
                ou.SetNextActionItems(_KEXSetEchoKEXReportEcho);
            }
            else
            {
                SetStateCompletedSecurityFailed(ou);
            }
        }

        private bool ValidateKexReportEcho(byte[] echoCmd)
        {
            bool ret = false;
            if (_KEXReportKEXSet.Data != null && echoCmd != null)
            {
                COMMAND_CLASS_SECURITY_2.KEX_REPORT kexReport = _KEXReportKEXSet.Data;
                kexReport.properties1.echo = 1;
                ret = echoCmd.SequenceEqual((byte[])kexReport);
            }
            return ret;
        }

        private void OnNKReport(ActionCompletedUnit ou)
        {
            if (_NKGetNKReport.Result)
            {
                if (_NKGetNKReport.SpecificResult.RxSecurityScheme == SecuritySchemes.S2_TEMP)
                {
                    COMMAND_CLASS_SECURITY_2.SECURITY_2_NETWORK_KEY_GET get = _NKGetNKReport.Data;
                    COMMAND_CLASS_SECURITY_2.SECURITY_2_NETWORK_KEY_REPORT rpt = _NKGetNKReport.SpecificResult.Command;
                    if (rpt.grantedKey == get.requestedKey && rpt.networkKey != null && rpt.networkKey.Length == 16)
                    {
                        NetworkKeyS2Flags verifyKey = (NetworkKeyS2Flags)rpt.grantedKey;
                        var scheme = SecurityManagerInfo.ConvertToSecurityScheme(verifyKey);
                        if (scheme != SecuritySchemes.NONE)
                        {
                            _grantedSchemes.Add(scheme);
                            SpecificResult.SecuritySchemes = _grantedSchemes.ToArray();
                            _securityManagerInfo.SetNetworkKey(rpt.networkKey, scheme);
                            if (scheme == SecuritySchemes.S0)
                            {
                                _securityManagerInfo.ActivateNetworkKeyS0();
                            }
                            _securityManagerInfo.ActivateNetworkKeyS2ForNode(_peerNodeId, scheme);

                            _NKVerifyTransferEnd.NewToken();
                            _NKVerifyTransferEnd.DestNodeId = NodeId;
                            _NKVerifyTransferEnd.SrcNodeId = VirtualNodeId;
                            var cmd = new COMMAND_CLASS_SECURITY_2.SECURITY_2_NETWORK_KEY_VERIFY();
                            _NKVerifyTransferEnd.Data = cmd;

                            #region TestFrame Section
                            switch (scheme)
                            {
                                case SecuritySchemes.S0:
                                    #region NetworkKeyVerify_S0
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyVerify_S0, _NKVerifyTransferEnd);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_UNAUTHENTICATED:
                                    #region NetworkKeyVerify_S2Unauthenticated
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyVerify_S2Unauthenticated, _NKVerifyTransferEnd);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_AUTHENTICATED:
                                    #region NetworkKeyVerify_S2Authenticated
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyVerify_S2Authenticated, _NKVerifyTransferEnd);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_ACCESS:
                                    #region NetworkKeyVerify_S2Access
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyVerify_S2Access, _NKVerifyTransferEnd);
                                    #endregion
                                    break;
                            }
                            #endregion
                            _NKVerifyTransferEnd.SendDataSubstituteCallback = () =>
                            {
                                _securityManagerInfo.ActivateNetworkKeyS2TempForNode(_peerNodeId);
                            };
                        }
                        else
                        {
                            _securityManagerInfo.ActivateNetworkKeyS2TempForNode(_peerNodeId);
                            _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                            _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x0A }; // KEX_FAIL_KEY_REPORT
                            _KexFail.NodeId = NodeId;
                            _KexFail.BridgeNodeId = VirtualNodeId;
                            ou.SetNextActionItems(_KexFail);
                        }
                    }
                    else
                    {
                        _securityManagerInfo.ActivateNetworkKeyS2TempForNode(_peerNodeId);
                        _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                        _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x0A }; // KEX_FAIL_KEY_REPORT
                        _KexFail.NodeId = NodeId;
                        _KexFail.BridgeNodeId = VirtualNodeId;
                        ou.SetNextActionItems(_KexFail);
                    }
                }
                else
                {
                    _securityManagerInfo.ActivateNetworkKeyS2TempForNode(_peerNodeId);
                    _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                    _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x07 }; // KEX_FAIL_AUTH
                    _KexFail.NodeId = NodeId;
                    _KexFail.BridgeNodeId = VirtualNodeId;
                    ou.SetNextActionItems(_KexFail);
                }
            }
            else
            {
                SetStateCompletedSecurityFailed(ou);
            }
        }

        private void OnTransferEnd(ActionCompletedUnit ou)
        {
            if (_NKVerifyTransferEnd.Result)
            {
                if (_NKVerifyTransferEnd.SpecificResult.RxSecurityScheme == SecuritySchemes.S2_TEMP)
                {
                    COMMAND_CLASS_SECURITY_2.SECURITY_2_TRANSFER_END rpt = _NKVerifyTransferEnd.SpecificResult.Command;
                    if (rpt.properties1.keyVerified > 0)
                    {
                        if (_grantedKeys.Count > 0)
                        {
                            _NKGetNKReport.NewToken();
                            var cmd = new COMMAND_CLASS_SECURITY_2.SECURITY_2_NETWORK_KEY_GET();
                            var key = _grantedKeys.Dequeue();
                            cmd.requestedKey = (byte)key;
                            _NKGetNKReport.Data = cmd;

                            #region TestFrame Section
                            var scheme = SecurityManagerInfo.ConvertToSecurityScheme(key);
                            switch (scheme)
                            {
                                case SecuritySchemes.S0:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S0, _NKGetNKReport);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_UNAUTHENTICATED:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S2Unauthenticated, _NKGetNKReport);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_AUTHENTICATED:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S2Authenticated, _NKGetNKReport);
                                    #endregion
                                    break;
                                case SecuritySchemes.S2_ACCESS:
                                    #region NetworkKeyGet
                                    _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NetworkKeyGet_S2Access, _NKGetNKReport);
                                    #endregion
                                    break;
                            }
                            #endregion

                            ou.SetNextActionItems(_NKGetNKReport);
                        }
                        else
                        {
                            _TransferEnd.NodeId = NodeId;
                            _TransferEnd.BridgeNodeId = VirtualNodeId;
                            var cmd = new COMMAND_CLASS_SECURITY_2.SECURITY_2_TRANSFER_END();
                            cmd.properties1.keyRequestComplete = 1;
                            cmd.properties1.keyVerified = 0;
                            _TransferEnd.Data = cmd;
                            #region TransferEndB
                            _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.TransferEndB, _TransferEnd);
                            #endregion
                            ou.SetNextActionItems(_TransferEnd);
                        }
                    }
                    else
                    {
                        SetStateCompletedSecurityFailed(ou);
                    }
                }
                else
                {
                    _securityManagerInfo.ActivateNetworkKeyS2TempForNode(_peerNodeId);
                    _KexFail.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                    _KexFail.Data = new COMMAND_CLASS_SECURITY_2.KEX_FAIL { kexFailType = 0x07 }; // KEX_FAIL_AUTH
                    _KexFail.NodeId = NodeId;
                    _KexFail.BridgeNodeId = VirtualNodeId;
                    ou.SetNextActionItems(_KexFail);
                }
            }
            else
            {
                SetStateCompletedSecurityFailed(ou);
            }
        }

        private void OnSendTransferEnd(ActionCompletedUnit tu)
        {
            if (tu.Action.Result.State == ActionStates.Completed)
            {
                if (_grantedSchemes.Count > 0)
                {
                    SetStateCompletedSecurityDone(tu);
                }
                else
                {
                    SetStateCompletedSecurityNone(tu);
                }
            }
            else
            {
                SetStateCompletedSecurityFailed(tu);
            }
        }

        public override string AboutMe()
        {
            if (ParentAction != null)
                return string.Format("Id={0}, Security={1}", SpecificResult.NodeId, SpecificResult.SubstituteStatus);
            else
                return "";
        }

        public SetLearnModeResult SpecificResult
        {
            get { return (SetLearnModeResult)ParentAction.Result; }
        }
    }
}
