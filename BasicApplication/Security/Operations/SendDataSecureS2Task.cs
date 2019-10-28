using System;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class SendDataSecureS2Task : ApiOperation
    {
        #region Timeouts
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        public static int NONCE_REQUEST_INCLUSION_TIMER = 10000;
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        public static int NONCE_REQUEST_TIMER = 10000;
        #endregion

        protected TransmitOptions TxOptions { get; set; }
        internal byte[] CommandToSecureSend { get; private set; }
        internal int DataDelay { get; set; }
        internal byte NodeId { get; private set; }
        internal byte? TestNodeId { get; set; }
        private readonly SecurityManagerInfo _securityManagerInfo;
        private readonly SecurityS2CryptoProvider _securityS2CryptoProvider;
        private readonly MpanTable _mpanTable;
        private readonly SpanTable _spanTable;
        private readonly SinglecastKey _sckey;
        private RequestDataOperation _requestNonce;
        private SendDataOperation _sendEncData;
        private InvariantPeerNodeId _peerNodeId;
        private readonly ISecurityTestSettingsService _securityTestSettingsService;
        public Action SubstituteCallback { get; set; }
        public Extensions ExtensionsToAdd { get; set; }
        public SubstituteSettings SubstituteSettingsForRetransmission { get; set; }
        internal SendDataSecureS2Task(SecurityManagerInfo securityManagerInfo,
            SecurityS2CryptoProvider securityS2CryptoProvider, SinglecastKey sckey, SpanTable spanTable, MpanTable mpanTable,
            byte nodeId,
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
            NodeId = nodeId;
            _peerNodeId = new InvariantPeerNodeId(_securityManagerInfo.Network.NodeId, NodeId);
            TxOptions = txOptions;
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
            _requestNonce = new RequestDataOperation(0,
                NodeId,
                new COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_GET()
                {
                    sequenceNumber = _spanTable.GetTxSequenceNumber(_peerNodeId)
                },
                TxOptions,
                new[]
                {
                    new ByteIndex( COMMAND_CLASS_SECURITY_2.ID),
                    new ByteIndex( COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT.ID),
                    ByteIndex.AnyValue,
                    new ByteIndex(0x01, 0x01),
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue,
                    ByteIndex.AnyValue
                },
                NONCE_REQUEST_TIMER);
            if (_securityManagerInfo.IsInclusion)
                _requestNonce.Token.Reset(NONCE_REQUEST_INCLUSION_TIMER);
            _requestNonce.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            _securityManagerInfo.InitializingNodeId = NodeId;

            _sendEncData = new SendDataOperation(TestNodeId ?? NodeId, null, TxOptions);
            _sendEncData.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
        }

        private void OnStart(StartActionUnit taskUnit)
        {
            #region NonceGet
            _securityTestSettingsService.ActivateTestPropertiesForFrame(SecurityS2TestFrames.NonceGet, _requestNonce);
            #endregion
        }

        private void OnNonceReport(ActionCompletedUnit ou)
        {
            AddTraceLogItems(_requestNonce.SpecificResult.TraceLog);
            SpecificResult.TransmitStatus = (_requestNonce.Result as TransmitResult).TransmitStatus;
            if (_requestNonce.Result)
            {
                COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT cmd = _requestNonce.SpecificResult.Command;
                "NONCE REPORT {0}"._DLOG(_requestNonce.SpecificResult.Command.GetHex());
                if (cmd.receiversEntropyInput != null && cmd.receiversEntropyInput.Count == 16 && cmd.properties1.sos == 1 /* SOS flag */)
                {
                    _spanTable.AddOrReplace(_peerNodeId,
                        cmd.receiversEntropyInput.ToArray(), _spanTable.GetTxSequenceNumber(_peerNodeId), cmd.sequenceNumber);
                    _securityManagerInfo.InitializingNodeId = 0;
                    if (cmd.properties1.mos == 1)
                    {
                        var groupId = _securityS2CryptoProvider.LastSentMulticastGroupId;
                        var nodeGroupId = new NodeGroupId(_securityManagerInfo.Network.NodeId, groupId);
                        if (groupId != 0 && _mpanTable.CheckMpanExists(nodeGroupId))
                        {
                            if (ExtensionsToAdd == null)
                            {
                                ExtensionsToAdd = new Extensions();
                            }
                            ExtensionsToAdd.AddMpanExtension(
                                _mpanTable.GetContainer(nodeGroupId).MpanState,
                                groupId
                                );
                        };
                    }

                    var cryptedData = _securityS2CryptoProvider.EncryptSinglecastCommand(_sckey, _spanTable, _securityManagerInfo.Network.NodeId, NodeId, _securityManagerInfo.Network.HomeId, CommandToSecureSend, ExtensionsToAdd, SubstituteSettingsForRetransmission);
                    if (cryptedData != null)
                    {
                        if (SubstituteCallback != null)
                            SubstituteCallback();
                        _securityManagerInfo.LastSendDataBuffer = cryptedData;
                        _sendEncData.Data = cryptedData;

                        #region MessageEncapsulation
                        _sendEncData.Data = _securityManagerInfo.TestOverrideMessageEncapsulation(_sckey, _spanTable, _securityS2CryptoProvider, SubstituteSettings, NodeId, CommandToSecureSend, _peerNodeId, ExtensionsToAdd, cryptedData, _sendEncData.Data);
                        #endregion
                    }
                    else
                    {
                        "No Data to Send"._DLOG();
                        SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
                        SetStateFailed(ou);
                    }
                }
                else
                {
                    "Invalid Nonce {0}"._DLOG(_requestNonce.SpecificResult.Command.GetHex());
                }
            }
            else
            {
                SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
                SetStateFailed(ou);
            }
        }

        private void OnSendEncData(ActionCompletedUnit ou)
        {
            AddTraceLogItems(_sendEncData.SpecificResult.TraceLog);
            if (_sendEncData.Result.State == ActionStates.Completed)
            {
                SpecificResult.TransmitStatus = (_sendEncData.Result as TransmitResult).TransmitStatus;
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
