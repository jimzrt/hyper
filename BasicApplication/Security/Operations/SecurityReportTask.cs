using System.Collections.Generic;
using System.Threading;
using Utils;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class SecurityReportTask : ApiAchOperation
    {
        #region Timeouts
        const int SEND_DATA_TIMER = 2000;
        #endregion

        private SecurityS0CryptoProvider _securityS0CryptoProvider;
        private SecurityManagerInfo _securityManagerInfo;
        internal SecurityReportTask(SecurityManagerInfo securityManagerInfo, SecurityS0CryptoProvider securityS0CryptoProvider)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_SECURITY.ID))
        {
            _securityManagerInfo = securityManagerInfo;
            _securityS0CryptoProvider = securityS0CryptoProvider;
        }

        byte handlingNonceGetFromNode = 0;
        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            if (!ou.DataFrame.IsSkippedSecurity)
            {
                if (_securityManagerInfo.IsActive && _securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                {
                    byte[] command = ReceivedAchData.Command;
                    if (command != null && command.Length > 1)
                    {
                        byte[] dataToSend = null;
                        bool isSubstituteDenied = false;
                        if ((command[1] == COMMAND_CLASS_SECURITY.SECURITY_NONCE_GET.ID ||
                            command[1] == COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET.ID) &&
                            handlingNonceGetFromNode != ReceivedAchData.SrcNodeId)
                        {
                            handlingNonceGetFromNode = ReceivedAchData.SrcNodeId;
                            var destNodeId = ReceivedAchData.DestNodeId > 0 ? ReceivedAchData.DestNodeId : _securityManagerInfo.Network.NodeId;
                            if (_securityManagerInfo.IsSenderNonceS0Disabled)
                            {
                                handlingNonceGetFromNode = 0;
                            }
                            else
                            {
                                dataToSend = _securityS0CryptoProvider.GenerateNonceReport(new OrdinalPeerNodeId(ReceivedAchData.SrcNodeId, destNodeId));
                            }
                            isSubstituteDenied = true;
                            if (_securityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.NonceReport))
                            {
                                Thread.Sleep(_securityManagerInfo.DelaysS0[SecurityS0Delays.NonceReport]);
                            }
                        }
                        else if (command[1] == COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_GET.ID && handlingNonceGetFromNode != ReceivedAchData.SrcNodeId)
                        {
                            handlingNonceGetFromNode = ReceivedAchData.SrcNodeId;
                            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
                            if (scheme == SecuritySchemes.S0)
                            {
                                if (!_securityManagerInfo.Network.IsSecuritySchemesSpecified(ReceivedAchData.SrcNodeId) || _securityManagerInfo.Network.HasSecurityScheme(ReceivedAchData.SrcNodeId, SecuritySchemes.S0))
                                {
                                    var ccReport = new COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_REPORT();
                                    if (!_securityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                                    {
                                        if (ReceivedAchData.DestNodeId > 0 && ReceivedAchData.DestNodeId != _securityManagerInfo.Network.NodeId)
                                        {
                                            ccReport.commandClassSupport = new List<byte>(_securityManagerInfo.Network.GetVirtualSecureCommandClasses());
                                        }
                                        else
                                        {
                                            var secureCommandClasses = _securityManagerInfo.Network.GetSecureCommandClasses();
                                            if (secureCommandClasses != null)
                                            {
                                                ccReport.commandClassSupport = new List<byte>(secureCommandClasses);
                                            }
                                        }
                                        dataToSend = ccReport;
                                    }
                                    else
                                    {
                                        dataToSend = new byte[] { 0x98, 0x03, 0x00 };
                                    }
                                }
                            }
                        }

                        if (dataToSend != null)
                        {
                            ApiOperation sendData = null;

                            if (ReceivedAchData.DestNodeId > 0)
                            {
                                sendData = new SendDataBridgeOperation(ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, dataToSend, _securityManagerInfo.TxOptions);
                            }
                            else
                            {
                                sendData = new SendDataOperation(ReceivedAchData.SrcNodeId, dataToSend, _securityManagerInfo.TxOptions);
                            }


                            if (isSubstituteDenied)
                            {
                                sendData.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                            }
                            sendData.CompletedCallback = (x) =>
                            {
                                var action = x as ActionBase;
                                if (action != null)
                                {
                                    handlingNonceGetFromNode = 0;
                                    SpecificResult.TotalCount++;
                                    if (action.Result.State != ActionStates.Completed)
                                        SpecificResult.FailCount++;
                                }
                            };
                            ou.SetNextActionItems(sendData);
                        }
                        else
                        {
                            ou.SetNextActionItems();
                        }
                    }
                }
                else
                {
                    "REJECT, {0}, {1} (IsNodeSecure[S0]={2}, IsActive={3}"._DLOG(
                       _securityManagerInfo.IsInclusion,
                       _securityManagerInfo.Network.HasSecurityScheme(ReceivedAchData.SrcNodeId, SecuritySchemes.S0),
                       _securityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0),
                       _securityManagerInfo.IsActive);
                }
            }
        }

        public NonceResponseDataResult SpecificResult
        {
            get { return (NonceResponseDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new NonceResponseDataResult();
        }
    }

    public class NonceResponseDataResult : ActionResult
    {
        public int TotalCount { get; set; }
        public int FailCount { get; set; }
    }
}
