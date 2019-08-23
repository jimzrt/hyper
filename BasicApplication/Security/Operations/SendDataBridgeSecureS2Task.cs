using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class SendDataBridgeSecureS2Task : ApiOperation
    {
        #region Timeouts
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        public static int NONCE_REQUEST_INCLUSION_TIMER = 10000;
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        public static int NONCE_REQUEST_TIMER = 20000;
        #endregion

        protected TransmitOptions TxOptions { get; set; }
        internal byte[] CommandToSecureSend { get; private set; }
        internal int DataDelay { get; set; }
        internal byte DestNodeId { get; private set; }
        internal byte? TestNodeId { get; set; }
        internal byte SrcNodeId { get; private set; }
        private InvariantPeerNodeId _peerNodeId;

        //public Action<ActionUnit> OnHandledCallback { get; set; }

        private SecurityManagerInfo _securityManagerInfo;
        private SecurityS2CryptoProvider _securityS2CryptoProvider;
        private SpanTable _spanTable;
        private MpanTable _mpanTable;
        private SinglecastKey _sckey;
        private RequestDataOperation _requestNonce;
        private SendDataBridgeOperation _sendEncData;
        private readonly ISecurityTestSettingsService _securityTestSettingsService;
        public Action SubstituteCallback { get; set; }
        public Extensions ExtensionsToAdd { get; set; }
        public SubstituteSettings SubstituteSettingsForRetransmission { get; set; }
        internal SendDataBridgeSecureS2Task(SecurityManagerInfo securityManagerInfo,
            SecurityS2CryptoProvider securityS2CryptoProvider, SinglecastKey sckey, SpanTable spanTable, MpanTable mpanTable,
            byte srcNodeId,
            byte destNodeId,
            byte[] data,
            TransmitOptions txOptions)
            : base(false, null, false)
        {
            _securityManagerInfo = securityManagerInfo;
            _securityS2CryptoProvider = securityS2CryptoProvider;
            _mpanTable = mpanTable;
            _spanTable = spanTable;
            _sckey = sckey;
            SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            CommandToSecureSend = data;
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
            TxOptions = txOptions;
            _peerNodeId = new InvariantPeerNodeId(SrcNodeId, DestNodeId);
            _securityTestSettingsService = new SecurityTestSettingsService(_securityManagerInfo, false);
        }


        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnStart, 0, _requestNonce));
            ActionUnits.Add(new ActionCompletedUnit(_requestNonce, OnNonceReport, _sendEncData));
            ActionUnits.Add(new ActionCompletedUnit(_sendEncData, OnSendEncData));
        }

        protected override void CreateInstance()
        {
            _spanTable.UpdateTxSequenceNumber(_peerNodeId);
            _requestNonce = new RequestDataOperation(SrcNodeId,
                DestNodeId,
                new COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_GET()
                {
                    sequenceNumber = _spanTable.GetTxSequenceNumber(_peerNodeId)
                },
                TxOptions,
                new COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT(), 2,
                NONCE_REQUEST_TIMER);
            if (_securityManagerInfo.IsInclusion)
                _requestNonce.Token.Reset(NONCE_REQUEST_INCLUSION_TIMER);
            _requestNonce.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            _securityManagerInfo.InitializingNodeId = DestNodeId;

            _sendEncData = new SendDataBridgeOperation(SrcNodeId, TestNodeId ?? DestNodeId, null, TxOptions)
            {
                SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
            };
        }

        private void OnStart(StartActionUnit taskUnit)
        {
            #region NonceGet
            _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NonceGet, _requestNonce);
            #endregion
        }

        private void OnNonceReport(ActionCompletedUnit ou)
        {
            var encryptSucceeded = false;
            AddTraceLogItems(_requestNonce.SpecificResult.TraceLog);
            if (_requestNonce.Result.State == ActionStates.Completed)
            {
                COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT cmd = _requestNonce.SpecificResult.Command;
                if (cmd.receiversEntropyInput != null && cmd.receiversEntropyInput.Count == 16 && cmd.properties1.sos == 1 /* SOS flag */)
                {
                    _spanTable.AddOrReplace(_peerNodeId,
                        cmd.receiversEntropyInput.ToArray(), _spanTable.GetTxSequenceNumber(_peerNodeId), cmd.sequenceNumber);
                    _securityManagerInfo.InitializingNodeId = 0;
                    Extensions extensions = null;
                    if (cmd.properties1.mos == 1)
                    {
                        var groupId = _securityS2CryptoProvider.LastSentMulticastGroupId;
                        var nodeGroupId = new NodeGroupId(_securityManagerInfo.Network.NodeId, groupId);
                        if (groupId != 0 && _mpanTable.CheckMpanExists(nodeGroupId))
                        {
                            extensions = new Extensions();
                            extensions.AddMpanExtension(
                                _mpanTable.GetContainer(nodeGroupId).MpanState,
                                groupId
                                );
                        };
                    }
                    var cryptedData = _securityS2CryptoProvider.EncryptSinglecastCommand(
                        _sckey,
                        _spanTable, SrcNodeId, DestNodeId, _securityManagerInfo.Network.HomeId, CommandToSecureSend, extensions, new SubstituteSettings());
                    if (cryptedData != null)
                    {
                        if (SubstituteCallback != null)
                            SubstituteCallback();
                        encryptSucceeded = true;
                        _sendEncData.Data = cryptedData;
                        //_sendEncData.OnHandledCallback = OnHandledCallback;

                        #region MessageEncapsulation
                        if (_securityManagerInfo.TestFramesS2.ContainsKey(SecurityS2TestFrames.MessageEncapsulation))
                        {
                            var testFrame = _securityManagerInfo.TestFramesS2[SecurityS2TestFrames.MessageEncapsulation];
                            if (testFrame.IsEncryptedSpecified)
                            {
                                if (testFrame.IsEncrypted)
                                {
                                    if (testFrame.IsTemp)
                                    {
                                        if (testFrame.NetworkKey != null)
                                        {
                                            _securityManagerInfo.ActivateNetworkKeyS2CustomForNode(_peerNodeId, testFrame.IsTemp, testFrame.NetworkKey);
                                        }
                                        else
                                        {
                                            _securityManagerInfo.ActivateNetworkKeyS2CustomForNode(_peerNodeId, testFrame.IsTemp, _securityManagerInfo.GetActualNetworkKeyS2Temp());
                                        }
                                    }
                                    else
                                    {
                                        if (testFrame.NetworkKey != null)
                                        {
                                            _securityManagerInfo.ActivateNetworkKeyS2CustomForNode(_peerNodeId, testFrame.IsTemp, testFrame.NetworkKey);
                                        }
                                    }
                                }
                                else
                                {
                                    var msgEncapCryptedData = (COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION)cryptedData;
                                    msgEncapCryptedData.ccmCiphertextObject = new List<byte>(CommandToSecureSend);
                                    _sendEncData.Data = msgEncapCryptedData;
                                }
                            }
                        }
                        #endregion
                    }
                }
            }

            if (!encryptSucceeded)
                SetStateFailed(ou);
        }

        private void OnSendEncData(ActionCompletedUnit ou)
        {
            AddTraceLogItems(_sendEncData.SpecificResult.TraceLog);
            if (_sendEncData.Result.State == ActionStates.Completed)
            {
                SpecificResult.SubstituteStatus = SubstituteStatuses.Done;
                SetStateCompleted(ou);
            }
            else
            {
                SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
                SetStateFailed(ou);
            }
        }

        public SendDataResult SpecificResult
        {
            get { return (SendDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SendDataResult();
        }

    }
}
