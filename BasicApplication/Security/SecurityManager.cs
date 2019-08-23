using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.BasicApplication.CommandClasses;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.BasicApplication.Security;
using ZWave.BasicApplication.Tasks;
using ZWave.CommandClasses;
using ZWave.Configuration;
using ZWave.Devices;
using ZWave.Enums;
using ZWave.Layers.Frame;
using ZWave.Security;

namespace ZWave.BasicApplication
{
    public class SecurityManager : SubstituteManagerBase
    {
        public static int SC_FOLLOWUP_TIMEOUT = 303;
        private static byte _multGroupCounter = 0;

        protected override SubstituteIncomingFlags GetId()
        {
            return SubstituteIncomingFlags.Security;
        }

        #region Security Fields

        private byte _fragmentSequenceCounter = 1;

        #endregion

        private SecurityS2CryptoProvider _securityS2CryptoProvider;
        private SecurityS0CryptoProvider _securityS0CryptoProvider;
        private byte[] _broadcastSeqNoCache = new byte[0xFF];

        public SecurityManagerInfo SecurityManagerInfo { get; set; }

        public int SerialApiVersion { get; set; }

        public SecurityManager(NetworkViewPoint network, NetworkKey[] networkKeys, byte[] privateKey)
            : this(network, networkKeys, privateKey, null, null)
        {
        }

        public SecurityManager(NetworkViewPoint network, NetworkKey[] networkKeys, byte[] privateKey,
            Func<byte, byte[], bool> sendDataSubstitutionCallback,
            Action<byte, byte[], bool> receiveDataSubstitutionCallback)
        {
            IsActive = true;

            if (networkKeys == null)
            {
                networkKeys = new NetworkKey[8];
            }

            foreach (var scheme in SecuritySchemeSet.ALL)
            {
                var index = SecurityManagerInfo.GetNetworkKeyIndex(scheme);

                if (networkKeys[index] == null)
                {
                    networkKeys[index] = new NetworkKey();
                }
            }

            SecurityManagerInfo = new SecurityManagerInfo(network, networkKeys, privateKey)
            {
                SecuritySchemeInGetS0 = 0,
                SecuritySchemeInReportS0 = 0,
                SecuritySchemeInInheritS0 = 0,
                SendDataSubstitutionCallback = sendDataSubstitutionCallback,
                ReceiveDataSubstitutionCallback = receiveDataSubstitutionCallback,
            };

            _securityS2CryptoProvider = new SecurityS2CryptoProvider(SecurityManagerInfo);
            _securityS0CryptoProvider = new SecurityS0CryptoProvider(SecurityManagerInfo);

            foreach (var scheme in SecuritySchemeSet.ALL)
            {
                var index = SecurityManagerInfo.GetNetworkKeyIndex(scheme);
                if (networkKeys[index].Value == null || Tools.IsEmptyArray(networkKeys[index].Value))
                {
                    SecurityManagerInfo.SetNetworkKey(GenerateNetworkKey(scheme), scheme);
                }
            }
            SecurityManagerInfo.SecretKeyS2 = _securityS2CryptoProvider.GenerateSecretKey();

            SecurityManagerInfo.ActivateNetworkKeyS0();
            SecurityManagerInfo.IsActive = true;
        }

        private byte[] GenerateNetworkKey(SecuritySchemes scheme)
        {
            byte[] ret = null;
            if (scheme == SecuritySchemes.S0 && _securityS0CryptoProvider.PRNG != null)
            {
                ret = SecurityS0Utils.GenerateNetworkKey(_securityS0CryptoProvider.PRNG);
            }
            else
            {
                ret = _securityS2CryptoProvider.GetRandomData();
            }
            return ret;
        }

        private Dictionary<byte, byte[]> _sequencedFirstFrameS0 = new Dictionary<byte, byte[]>();

        public override void OnIncomingSubstituted(CustomDataFrame dataFrameOri, CustomDataFrame dataFrameSub, List<ActionHandlerResult> ahResults)
        {
            if (IsActive)
            {
                bool hasSendData = ahResults.FirstOrDefault(
                    x => x.NextActions != null && x.NextActions.FirstOrDefault(
                        y => y is SendDataOperation || y is SendDataExOperation) != null) != null;
                if (!hasSendData)
                {
                    if (dataFrameSub != null)
                    {
                        byte[] cmdData;
                        byte srcNodeId;
                        byte destNodeId;
                        int lenIndex;
                        if (TryParseCommand(dataFrameOri, out destNodeId, out srcNodeId, out lenIndex, out cmdData))
                        {
                            if (cmdData[0] == COMMAND_CLASS_SECURITY_2.ID && cmdData[1] == COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.ID)
                            {
                                var mpanRecord = SecurityManagerInfo.MpanTable.GetLatestContainerByOwnerId(srcNodeId);
                                byte rxSequenceNumber = cmdData[2];
                                if (mpanRecord != null && mpanRecord.IsMosState)
                                {
                                    InvariantPeerNodeId peerNodeId = new InvariantPeerNodeId(SecurityManagerInfo.Network.NodeId, srcNodeId);
                                    var currentTxSequenceNumber = SecurityManagerInfo.SpanTable.GetTxSequenceNumber(peerNodeId);
                                    SecurityManagerInfo.SpanTable.UpdateTxSequenceNumber(peerNodeId);
                                    COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT dataToSend = new COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT();
                                    dataToSend.sequenceNumber = ++currentTxSequenceNumber;
                                    dataToSend.properties1.mos = 1;
                                    var tempAction = new SendDataOperation(srcNodeId, dataToSend, SecurityManagerInfo.TxOptions);
                                    tempAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                                    mpanRecord.SetMosStateReported();
                                    var ahRes = new ActionHandlerResult(null);
                                    ahRes.NextActions.Add(tempAction);
                                    ahResults.Add(ahRes);
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool _isPrevDecryptFailed = false;
        protected override CustomDataFrame SubstituteIncomingInternal(CustomDataFrame packet, byte destNodeId, byte srcNodeId, byte[] cmdData, int lenIndex, out ActionBase additionalAction, out ActionBase completeAction)
        {
            CustomDataFrame ret = packet;
            Extensions extensions;
            byte[] data = null;
            additionalAction = null;
            completeAction = null;
            if (IsActive)
            {
                if (destNodeId == 0)
                {
                    destNodeId = SecurityManagerInfo.Network.NodeId;
                }

                var receiveStatus = (ReceiveStatuses)packet.Data[2];
                bool isMulticastFrame = receiveStatus.HasFlag(ReceiveStatuses.TypeMulti);
                bool isBroadcastFrame = receiveStatus.HasFlag(ReceiveStatuses.TypeBroad);
                CustomDataFrame encryptedFrame = null;
                if (cmdData[0] == COMMAND_CLASS_SECURITY.ID)
                {
                    if ((cmdData[1] == COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.ID ||
                        cmdData[1] == COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET.ID))
                    {
                        encryptedFrame = IncomeDecryptS0(packet, lenIndex, packet.Data, srcNodeId, cmdData, cmdData[1]);
                    }
                }
                else if (cmdData[0] == COMMAND_CLASS_SECURITY_2.ID)
                {
                    if (cmdData.Length > 2 &&
                        (cmdData[1] == COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_GET.ID ||
                        cmdData[1] == COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT.ID ||
                        cmdData[1] == COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.ID))
                    {
                        int seqNoIndex = lenIndex + 3;
                        byte seqNo = packet.Data[seqNoIndex];
                        bool isDuplicate = CheckDuplicate(packet, seqNo, srcNodeId, destNodeId, isMulticastFrame, isBroadcastFrame);
                        if (isDuplicate)
                        {
                            ret = null;
                        }
                        else
                        {
                            if (cmdData[1] == COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.ID)
                            {
                                if (!ValidateS2MessageExtensions(cmdData))
                                {
                                    ret = null;
                                }
                                else
                                {
                                    if (isMulticastFrame || isBroadcastFrame)
                                    {
                                        encryptedFrame = IncomeMulticastDecryptS2(packet, lenIndex, srcNodeId, cmdData, out data, out extensions);
                                    }
                                    else
                                    {
                                        bool canHandleFrame = true;
                                        if (SecurityManagerInfo.IsInclusion) // Wait for temp key activated on inclusion for a first time.
                                        {
                                            if (SecurityManagerInfo.DSKRequestStatus == DSKRequestStatuses.Started ||
                                                !SecurityManagerInfo.IsTempKeyActivatedOnInclusion)
                                            {
                                                canHandleFrame = false;
                                                ret = null;
                                            }
                                        }
                                        if (canHandleFrame)
                                        {
                                            bool isFollowup;
                                            ApplyS2MessageExtensions(srcNodeId, cmdData, out isFollowup);
                                            encryptedFrame = IncomeSinglecastDecryptS2(packet, lenIndex, packet.Data, srcNodeId, destNodeId, cmdData, out data, out extensions);
                                            if (encryptedFrame != null)
                                            {
                                                packet.IsSkippedSecurity = true;
                                                encryptedFrame.Parent = packet;
                                                _isPrevDecryptFailed = false;
                                                ApplyS2MessageExtensionsOnSuccess(srcNodeId, cmdData, ref additionalAction);
                                                bool hasMpanExtension = false;
                                                if (extensions != null && extensions.EncryptedExtensionsList != null && extensions.EncryptedExtensionsList.Count > 0)
                                                {
                                                    List<COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.TVG1> extensionsList;
                                                    if (ValidateS2MessageDecryptedExtensions(extensions.EncryptedExtensionsList, out extensionsList))
                                                    {
                                                        ApplyS2MessageDecryptedExtensions(seqNo, srcNodeId, extensionsList);
                                                        hasMpanExtension = extensionsList.Any(ext => ext.properties1.type == (byte)ExtensionTypes.Mpan);
                                                    }
                                                    else
                                                    {
                                                        encryptedFrame = null;
                                                        ret = null;
                                                    }
                                                }
                                                if (!isFollowup && !hasMpanExtension) // Spec [CC:009F.01.00.11.099]
                                                {
                                                    var mpanRecord = SecurityManagerInfo.MpanTable.GetLatestContainerByOwnerId(srcNodeId);
                                                    if (mpanRecord != null && mpanRecord.IsMosReportedState)
                                                    {
                                                        SecurityManagerInfo.MpanTable.RemoveRecord(new NodeGroupId(srcNodeId, mpanRecord.NodeGroupId.GroupId));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (_isPrevDecryptFailed)
                                                {
                                                    var incomingFlags = packet.SubstituteIncomingFlags | SubstituteIncomingFlags.SecurityFailed;
                                                    packet.SubstituteIncomingFlags = incomingFlags;
                                                }
                                                _isPrevDecryptFailed = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (SecurityManagerInfo.ReceiveDataSubstitutionCallback != null &&
                           (cmdData[0] != COMMAND_CLASS_SECURITY.ID ||
                           (cmdData[1] != COMMAND_CLASS_SECURITY.SECURITY_NONCE_GET.ID &&
                           cmdData[1] != COMMAND_CLASS_SECURITY.SECURITY_NONCE_REPORT.ID)) &&
                           (cmdData[0] != COMMAND_CLASS_SECURITY_2.ID ||
                           (cmdData[1] != COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_GET.ID &&
                           cmdData[1] != COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_REPORT.ID)))
                {
                    SecurityManagerInfo.ReceiveDataSubstitutionCallback(srcNodeId, data ?? cmdData, encryptedFrame != null);
                }
                if (encryptedFrame != null)
                {
                    ret = encryptedFrame;
                }
            }
            return ret;
        }

        private bool ValidateS2MessageExtensions(byte[] command)
        {
            bool ret = true;
            var validatedCommand = (COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION)command;
            if (validatedCommand.properties1.extension > 0)
            {
                foreach (var vg in validatedCommand.vg1)
                {
                    if (vg.properties1.type == (byte)ExtensionTypes.Span)
                    {
                        //disable isCritical check for known extension type
                        ret &= vg.extensionLength == 0x12;
                    }
                    else if (vg.properties1.type == (byte)ExtensionTypes.MpanGrp)
                    {
                        //disable isCritical check for known extension type
                        ret &= vg.extensionLength == 0x03;
                    }
                    else if (vg.properties1.type == (byte)ExtensionTypes.Mos)
                    {
                        //disable isCritical check for known extension type
                        ret &= vg.extensionLength == 0x02;
                    }
                    else if (vg.properties1.type == (byte)ExtensionTypes.Mpan)
                    {
                        ret = false;
                    }
                    else
                    {
                        if (vg.properties1.critical > 0)
                        {
                            ret = false;
                        }
                    }
                    if (!ret)
                    {
                        break;
                    }
                }
            }
            return ret;
        }

        private void ApplyS2MessageExtensions(byte srcNodeId, byte[] command, out bool isFollowup)
        {
            isFollowup = false;
            var msgEncap = (COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION)command;
            if (msgEncap.properties1.extension == 1)
            {
                foreach (var vg in msgEncap.vg1)
                {
                    if (vg.extensionLength == 3 && // MPAN_GRP extension length.
                        vg.properties1.type == (byte)ExtensionTypes.MpanGrp // MPAN_GRP type.
                        )
                    {
                        isFollowup = true;
                        var nodeGroupId = new NodeGroupId(srcNodeId, vg.extension[0]);
                        if (!SecurityManagerInfo.MpanTable.CheckMpanExists(nodeGroupId))
                        {
                            /* 3.6.5.2.2 and [CC:009F.01.00.12.008] */
                            SecurityManagerInfo.MpanTable.AddOrReplace(nodeGroupId, 0x55, null, _securityS2CryptoProvider.GetRandomData());
                            SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).SetSequenceNumber(msgEncap.sequenceNumber);
                            SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).SetMosState(true);
                            SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).SetReceivedTimeStamp();
                        }
                        else
                        {
                            SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).SetReceivedTimeStamp();
                        }
                    }
                }
            }
        }

        private void ApplyS2MessageExtensionsOnSuccess(byte srcNodeId, byte[] command, ref ActionBase additionalAction)
        {
            var msgEncap = (COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION)command;
            foreach (var vg in msgEncap.vg1)
            {
                if (vg.properties1.type == (byte)ExtensionTypes.Mos)
                {
                    var groupId = _securityS2CryptoProvider.LastSentMulticastGroupId;
                    var extensions = new Extensions();
                    // We're asked for MPAN - so we're the owner.
                    byte ownerId = SecurityManagerInfo.Network.NodeId;
                    var nodeGroupId = new NodeGroupId(ownerId, groupId);
                    if (!SecurityManagerInfo.MpanTable.CheckMpanExists(nodeGroupId))
                    {
                        SecurityManagerInfo.MpanTable.AddOrReplace(nodeGroupId, 0x55, null, _securityS2CryptoProvider.GetRandomData());
                    }
                    extensions.AddMpanExtension(SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).MpanState, groupId);
                    SendDataOperation sendOp = new SendDataOperation(srcNodeId, null, SecurityManagerInfo.TxOptions);
                    sendOp.Extensions = extensions;
                    additionalAction = sendOp;
                }
            }
        }

        private static bool ValidateS2MessageDecryptedExtensions(
            List<COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.TVG1> extensions,
            out List<COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.TVG1> extensionsList)
        {
            extensionsList = new List<COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.TVG1>();
            bool ret = true;
            bool isMoreExpected = true;
            foreach (var encExtension in extensions)
            {
                if (!isMoreExpected)
                {
                    ret = false;
                    break;
                }

                extensionsList.Add(encExtension);

                if (encExtension.properties1.type == (byte)ExtensionTypes.Mpan)
                {
                    // Disable isCritical check for known extension type.
                    ret &= encExtension.extensionLength == 0x13;
                    isMoreExpected = encExtension.properties1.moreToFollow == 0x01;
                }
                else
                {
                    ret = false;
                    isMoreExpected = false;
                }

                if (!ret)
                {
                    break;
                }
            }
            if (isMoreExpected)
            {
                ret = false;
            }
            return ret;
        }

        private void ApplyS2MessageDecryptedExtensions(byte seqNo, byte sourceNodeId, List<COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.TVG1> extensions)
        {
            foreach (var encExtension in extensions)
            {
                if (encExtension.properties1.type == (byte)ExtensionTypes.Mpan)
                {
                    byte groupId = encExtension.extension[0];
                    byte[] mpanState = encExtension.extension.Skip(1).ToArray();
                    var nodeGroupId = new NodeGroupId(sourceNodeId, groupId);
                    SecurityManagerInfo.MpanTable.AddOrReplace(nodeGroupId, seqNo, null, mpanState);
                }
            }
        }

        private bool CheckDuplicate(CustomDataFrame packet, byte seqNo, byte srcNodeId, byte destNodeId, bool isMulticastFrame, bool isBroadcastFrame)
        {
            bool isDuplicate = false;
            InvariantPeerNodeId peerNodeId = new InvariantPeerNodeId(destNodeId, srcNodeId);
            if ((isMulticastFrame || isBroadcastFrame) && _broadcastSeqNoCache[srcNodeId] == seqNo)
            {
                isDuplicate = true;
                "drop multicast duplicate: {0}"._DLOG(packet);
            }
            else if (!SecurityManagerInfo.SpanTable.IsValidRxSequenceNumber(peerNodeId, seqNo))
            {
                isDuplicate = true;
                "drop singlecast duplicate: {0}"._DLOG(packet);
            }
            return isDuplicate;
        }

        public void CheckIncrementMpan(byte sourceNodeId, COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION cmd)
        {
            if (cmd.properties1.extension == 1)
            {
                foreach (var extData in cmd.vg1)
                {
                    if (extData.properties1.type == (byte)ExtensionTypes.MpanGrp && // Indicates this is S2 SC-F frame.
                        extData.extensionLength == 3 &&
                        extData.extension.Count == 1) // Multicast group Id.
                    {
                        var nodeGroupId = new NodeGroupId(sourceNodeId, extData.extension[0]);
                        if (SecurityManagerInfo.McKeys.ContainsKey(nodeGroupId))
                        {
                            SecurityS2CryptoProviderBase.IncrementMpan(SecurityManagerInfo.MpanTable, SecurityManagerInfo.McKeys[nodeGroupId], nodeGroupId, new byte[16]); // Spec [CC:009F.01.00.11.029]
                        }
                        break;
                    }
                }
            }
        }

        private CustomDataFrame IncomeSinglecastDecryptS2(CustomDataFrame packet, int lenIndex, byte[] frameData,
            byte srcNodeId, byte destNodeId, COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION cmd, out byte[] data, out Extensions extensions)
        {
            CustomDataFrame encryptedFrame = null;
            extensions = null;
            data = null;
            InvariantPeerNodeId peerNodeId = new InvariantPeerNodeId(destNodeId, srcNodeId);
            if (SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId))
            {
                bool isDecrypted = false;
                var spanContainer = SecurityManagerInfo.SpanTable.GetContainer(peerNodeId);
                if (frameData[1] == (byte)CommandTypes.CmdApplicationCommandHandler_Bridge)
                {
                    isDecrypted = SecurityS2CryptoProviderBase.DecryptSinglecastFrame(
                        spanContainer,
                        SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId) ? SecurityManagerInfo.ScKeys[peerNodeId] : null,
                        srcNodeId,
                        destNodeId,
                        SecurityManagerInfo.Network.HomeId,
                        cmd,
                        out data,
                        out extensions);
                }
                else
                {
                    isDecrypted = SecurityS2CryptoProviderBase.DecryptSinglecastFrame(
                        spanContainer,
                        SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId) ? SecurityManagerInfo.ScKeys[peerNodeId] : null,
                        srcNodeId,
                        SecurityManagerInfo.Network.NodeId,
                        SecurityManagerInfo.Network.HomeId,
                        cmd,
                        out data,
                        out extensions);
                }
                CheckIncrementMpan(srcNodeId, cmd);
                if (isDecrypted)
                {
                    encryptedFrame = DataFrame.CreateDataFrame(packet, lenIndex, data,
                        (byte)SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme,
                        SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId),
                        (byte)SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme);
                    encryptedFrame.Extensions = extensions;
                }
            }

            if (encryptedFrame == null &&
                SecurityManagerInfo.SpanTable.GetSpanState(peerNodeId) == SpanStates.ReceiversNonce)
            {
                var hasCurrentScheme = SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId);
                var currentScheme = SecuritySchemes.NONE;
                if (hasCurrentScheme)
                {
                    currentScheme = SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme;
                }

                var schemes = new SecuritySchemes[]
                                    {
                                        SecuritySchemes.S0,
                                        SecuritySchemes.S2_UNAUTHENTICATED,
                                        SecuritySchemes.S2_AUTHENTICATED,
                                        SecuritySchemes.S2_ACCESS
                                    };
                if (SecurityManagerInfo.IsInclusion)
                {
                    schemes = new SecuritySchemes[]
                                                {
                                                    SecuritySchemes.S0,
                                                    SecuritySchemes.S2_UNAUTHENTICATED,
                                                    SecuritySchemes.S2_AUTHENTICATED,
                                                    SecuritySchemes.S2_ACCESS,
                                                    SecuritySchemes.S2_TEMP
                                                };
                }

                foreach (var scheme in schemes)
                {
                    var key = scheme == SecuritySchemes.S2_TEMP ? SecurityManagerInfo.GetActualNetworkKeyS2Temp() :
                        SecurityManagerInfo.GetActualNetworkKey(scheme);
                    if (key != null)
                    {
                        SecurityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, scheme);
                        bool isDecrypted = false;
                        var spanContainer = SecurityManagerInfo.SpanTable.GetContainer(peerNodeId);
                        if (frameData[1] == (byte)CommandTypes.CmdApplicationCommandHandler_Bridge)
                        {
                            isDecrypted = SecurityS2CryptoProviderBase.DecryptSinglecastFrame(
                                spanContainer,
                                SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId) ? SecurityManagerInfo.ScKeys[peerNodeId] : null,
                                srcNodeId,
                                destNodeId,
                                SecurityManagerInfo.Network.HomeId,
                                cmd,
                                out data,
                                out extensions);
                        }
                        else
                        {
                            isDecrypted = SecurityS2CryptoProviderBase.DecryptSinglecastFrame(
                                spanContainer,
                                SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId) ? SecurityManagerInfo.ScKeys[peerNodeId] : null,
                                srcNodeId,
                                SecurityManagerInfo.Network.NodeId,
                                SecurityManagerInfo.Network.HomeId,
                                cmd,
                                out data,
                                out extensions);
                        }
                        CheckIncrementMpan(srcNodeId, cmd);
                        if (isDecrypted)
                        {
                            encryptedFrame = DataFrame.CreateDataFrame(packet, lenIndex, data,
                                (byte)SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme,
                                SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId),
                                (byte)SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme);
                            encryptedFrame.Extensions = extensions;
                            break;
                        }
                    }
                }
                if (hasCurrentScheme && encryptedFrame == null)
                {
                    SecurityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, currentScheme);
                }
            }

            if (encryptedFrame != null)
            {
                if (SecurityManagerInfo.RetransmissionTableS2.ContainsKey(peerNodeId))
                {
                    SecurityManagerInfo.RetransmissionTableS2.Remove(peerNodeId);
                }
            }
            return encryptedFrame;
        }

        private CustomDataFrame IncomeMulticastDecryptS2(CustomDataFrame packet, int lenIndex, byte srcNodeId, COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION cmd, out byte[] data, out Extensions extensions)
        {
            CustomDataFrame ret = null;
            bool isDecrypted = false;
            data = null;
            extensions = null;
            if (cmd.vg1 != null)
            {
                var mpanGrp = cmd.vg1.FirstOrDefault(x => x.properties1.type == (byte)ExtensionTypes.MpanGrp);
                if (mpanGrp != null && mpanGrp.extension != null && mpanGrp.extension.Count == 1)
                {
                    var nodeGroupId = new NodeGroupId(srcNodeId, mpanGrp.extension[0]);
                    if (SecurityManagerInfo.McKeys.ContainsKey(nodeGroupId))
                    {
                        isDecrypted = SecurityS2CryptoProviderBase.DecryptMulticastFrame(
                            SecurityManagerInfo.MpanTable,
                            SecurityManagerInfo.McKeys[nodeGroupId],
                            nodeGroupId,
                            SecurityManagerInfo.Network.HomeId,
                            cmd,
                            out data,
                            out extensions);
                    }
                    if (!isDecrypted && !SecurityManagerInfo.IsInclusion && SecurityManagerInfo.MpanTable.CheckMpanExists(nodeGroupId) && !SecurityManagerInfo.MpanTable.IsRecordInMOSState(nodeGroupId))
                    {
                        var schemes = new[]
                        {
                            SecuritySchemes.S0,
                            SecuritySchemes.S2_UNAUTHENTICATED,
                            SecuritySchemes.S2_AUTHENTICATED,
                            SecuritySchemes.S2_ACCESS
                        };
                        foreach (var scheme in schemes)
                        {
                            var key = SecurityManagerInfo.GetActualNetworkKey(scheme);
                            if (key != null)
                            {
                                SecurityManagerInfo.ActivateNetworkKeyS2Multi(nodeGroupId.GroupId, srcNodeId, scheme);
                                isDecrypted = SecurityS2CryptoProviderBase.DecryptMulticastFrame(
                                    SecurityManagerInfo.MpanTable,
                                    SecurityManagerInfo.McKeys[nodeGroupId],
                                    nodeGroupId,
                                    SecurityManagerInfo.Network.HomeId,
                                    cmd,
                                    out data,
                                    out extensions);

                                if (isDecrypted)
                                {
                                    break;
                                }
                            }
                        }

                        if (!isDecrypted) // Set MOS.
                        {
                            SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).SetMosState(true);
                        }
                    }
                }
            }

            // Even if decrypt is failed we MUST ignore it for multicast.
            if (isDecrypted)
            {
            }
            return ret;
        }

        private CustomDataFrame IncomeDecryptS0(CustomDataFrame packet, int lenIndex, byte[] frameData, byte srcNodeId, COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION cmd, byte cmdId)
        {
            CustomDataFrame encryptedFrame = null;
            byte decryptedProperties = 0;
            byte[] data = null;
            bool isDecrypted = false;
            if (frameData[1] == (byte)CommandTypes.CmdApplicationCommandHandler_Bridge)
            {
                byte destNodeId = frameData[lenIndex - 2];
                isDecrypted = _securityS0CryptoProvider.DecryptFrame(srcNodeId, destNodeId, SecurityManagerInfo.Network.HomeId, cmd, cmdId, out data, out decryptedProperties);
            }
            else
            {
                isDecrypted = _securityS0CryptoProvider.DecryptFrame(srcNodeId, SecurityManagerInfo.Network.NodeId, SecurityManagerInfo.Network.HomeId, cmd, cmdId, out data, out decryptedProperties);
            }

            if (isDecrypted)// encryptedFrame != null)
            {
                encryptedFrame = DataFrame.CreateDataFrame(packet, lenIndex, data, (byte)SecuritySchemes.S0, true, (byte)SecuritySchemes.S0);
                var prop = (COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.Tproperties1)decryptedProperties;
                if (prop.sequenced > 0)
                {
                    if (prop.secondFrame > 0)
                    {
                        if (_sequencedFirstFrameS0.ContainsKey(srcNodeId))
                        {
                            MergeFragmentedMessage(lenIndex, ref data, encryptedFrame, _sequencedFirstFrameS0[srcNodeId]);
                            _sequencedFirstFrameS0.Remove(srcNodeId);
                        }
                    }
                    else
                    {
                        if (_sequencedFirstFrameS0.ContainsKey(srcNodeId))
                        {
                            _sequencedFirstFrameS0.Remove(srcNodeId);
                        }
                        _sequencedFirstFrameS0.Add(srcNodeId, encryptedFrame.Data);
                        // Ignore first part of fragmented frame.
                        encryptedFrame = null;
                    }
                }
            }
            return encryptedFrame;
        }

        private void MergeFragmentedMessage(int lenIndex, ref byte[] data, CustomDataFrame encryptedFrame, byte[] prevData)
        {
            var prevCmdLen = prevData[lenIndex];
            var thisCmdLen = encryptedFrame.Data[lenIndex];

            byte[] tmpData = new byte[prevCmdLen + encryptedFrame.Data.Length];
            Array.Copy(encryptedFrame.Data, 0, tmpData, 0, lenIndex + 2);
            Array.Copy(prevData, lenIndex + 1, tmpData, lenIndex + 1, prevCmdLen);
            Array.Copy(encryptedFrame.Data, lenIndex + 1, tmpData, (lenIndex + 1 + prevCmdLen),
                (encryptedFrame.Data.Length - (lenIndex + 1)));
            tmpData[lenIndex] = (byte)(thisCmdLen + prevCmdLen);
            byte[] tmpFrameBuffer = DataFrame.CreateFrameBuffer(tmpData);
            encryptedFrame.SetBuffer(tmpFrameBuffer, 0, tmpFrameBuffer.Length);
            data = new byte[thisCmdLen + prevCmdLen];
            Array.Copy(tmpData, lenIndex + 1, data, 0, thisCmdLen + prevCmdLen);
        }

        public SecurityReportTask CreateSecurityReportTask()
        {
            return new SecurityReportTask(SecurityManagerInfo, _securityS0CryptoProvider);
        }

        public SecurityS2ReportTask CreateSecurityS2ReportTask()
        {
            return new SecurityS2ReportTask(SecurityManagerInfo, _securityS2CryptoProvider, SecurityManagerInfo.SpanTable, SecurityManagerInfo.MpanTable);
        }

        public InclusionControllerSecureSupport CreateInclusionControllerSupportTask()
        {
            return new InclusionControllerSecureSupport(SecurityManagerInfo);
        }

        public InclusionControllerSecureSupport CreateInclusionControllerSupportTask(Action<ActionResult> updateCallback,
            Action<ActionToken, bool> inclusionControllerStatusUpdateCallback)
        {
            return new InclusionControllerSecureSupport(SecurityManagerInfo, updateCallback, inclusionControllerStatusUpdateCallback);
        }

        public override ActionBase SubstituteActionInternal(ApiOperation apiAction)
        {
            ActionBase ret = null;
            if (apiAction is SendDataExOperation)
            {
                SetWasRetransmittedOnS2NonceGet(apiAction);
                if (IsActive && SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALL) && !apiAction.SubstituteSettings.HasFlag(SubstituteFlags.DenySecurity))
                {
                    if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseMulticast))
                    {
                        var sendDataExAction = (SendDataExOperation)apiAction;
                        if (sendDataExAction.BridgeNodeId > 0)
                        {
                            var sendAction = new SendDataBridgeOperation(sendDataExAction.BridgeNodeId, sendDataExAction.NodeId, sendDataExAction.Data, sendDataExAction.TxOptions);
                            sendAction.DataDelay = sendDataExAction.DataDelay;
                            sendAction.SubstituteSettings = sendDataExAction.SubstituteSettings;
                            ret = SaSendDataBridge(sendAction, sendDataExAction.SecurityScheme);
                        }
                        else
                        {
                            var sendAction = new SendDataOperation(sendDataExAction.NodeId, sendDataExAction.Data, sendDataExAction.TxOptions);
                            sendAction.DataDelay = sendDataExAction.DataDelay;
                            sendAction.SubstituteSettings = sendDataExAction.SubstituteSettings;
                            ret = SaSendData(sendAction, sendDataExAction.SecurityScheme);
                        }
                    }
                    else
                    {
                        ret = SaSendDataMulti(apiAction);
                    }
                }
                else
                {
                    if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseMulticast))
                    {
                        var sendDataExAction = (SendDataExOperation)apiAction;
                        if (sendDataExAction.BridgeNodeId > 0)
                        {
                            ret = new SendDataBridgeOperation(sendDataExAction.BridgeNodeId, sendDataExAction.NodeId, sendDataExAction.Data, sendDataExAction.TxOptions);
                            if (apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseBroadcast))
                            {
                                ((SendDataBridgeOperation)ret).DestNodeId = 0xFF;
                            }
                        }
                        else
                        {
                            ret = new SendDataOperation(sendDataExAction.NodeId, sendDataExAction.Data, sendDataExAction.TxOptions);
                            if (apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseBroadcast))
                            {
                                ((SendDataOperation)ret).NodeId = 0xFF;
                            }
                        }
                    }
                    else
                    {
                        var sdxo = (SendDataExOperation)apiAction;
                        TransmitOptions txOptions = sdxo.TxOptions;
                        if (sdxo.SubstituteSettings.HasFlag(SubstituteFlags.DenyFollowup))
                        {
                            txOptions = txOptions & ~TransmitOptions.TransmitOptionAcknowledge;
                        }
                        ret = CreateSendDataMultiOperation(sdxo.BridgeNodeId, new byte[] { sdxo.NodeId }, sdxo.Data, txOptions, sdxo.SubstituteSettings);
                    }
                }
            }
            else if (apiAction is SendDataOperation)
            {
                SetWasRetransmittedOnS2NonceGet(apiAction);
                if (IsActive && SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALL) && !apiAction.SubstituteSettings.HasFlag(SubstituteFlags.DenySecurity))
                {
                    if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseMulticast))
                    {
                        ret = SaSendData((SendDataOperation)apiAction, null);
                    }
                    else
                    {
                        ret = SaSendDataMulti(apiAction);
                    }
                }
                else
                {
                    if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseMulticast))
                    {
                        ret = apiAction;
                        if (apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseBroadcast))
                        {
                            ((SendDataOperation)ret).NodeId = 0xFF;
                        }
                    }
                    else
                    {
                        var sdo = (SendDataOperation)apiAction;
                        TransmitOptions txOptions = sdo.TxOptions;
                        if (sdo.SubstituteSettings.HasFlag(SubstituteFlags.DenyFollowup))
                        {
                            txOptions = txOptions & ~TransmitOptions.TransmitOptionAcknowledge;
                        }
                        byte bridgeNodeId = SecurityManagerInfo.Network.IsBridgeController ? SecurityManagerInfo.Network.NodeId : (byte)0;
                        ret = CreateSendDataMultiOperation(bridgeNodeId, new byte[] { sdo.NodeId }, sdo.Data, txOptions, sdo.SubstituteSettings);
                    }
                }
            }
            else if (apiAction is SendDataBridgeOperation)
            {
                SetWasRetransmittedOnS2NonceGet(apiAction);
                if (IsActive && SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALL) && !apiAction.SubstituteSettings.HasFlag(SubstituteFlags.DenySecurity))
                {
                    if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseMulticast))
                    {
                        ret = SaSendDataBridge((SendDataBridgeOperation)apiAction, null);
                    }
                    else
                    {
                        ret = SaSendDataMulti(apiAction);
                    }
                }
                else
                {
                    if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseMulticast))
                    {
                        ret = apiAction;
                        if (apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseBroadcast))
                        {
                            ((SendDataBridgeOperation)ret).DestNodeId = 0xFF;
                        }
                    }
                    else
                    {
                        var sdbo = (SendDataBridgeOperation)apiAction;
                        TransmitOptions txOptions = sdbo.TxOptions;
                        if (sdbo.SubstituteSettings.HasFlag(SubstituteFlags.DenyFollowup))
                        {
                            txOptions = txOptions & ~TransmitOptions.TransmitOptionAcknowledge;
                        }
                        ret = CreateSendDataMultiOperation(sdbo.SrcNodeId, new byte[] { sdbo.DestNodeId }, sdbo.Data, txOptions, sdbo.SubstituteSettings);
                    }
                }
            }
            else if (apiAction is SendDataMultiExOperation ||
                        apiAction is SendDataMultiOperation ||
                        apiAction is SendDataMultiBridgeOperation)
            {
                if (IsActive && SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALL) && !apiAction.SubstituteSettings.HasFlag(SubstituteFlags.DenySecurity))
                {
                    ret = SaSendDataMulti(apiAction);
                }
            }

            else if (IsActive && !apiAction.SubstituteSettings.HasFlag(SubstituteFlags.DenySecurity))
            {
                if ((IsEnabledS0() || IsEnabledS2()))
                {
                    if (apiAction is SetLearnModeControllerOperation)
                    {
                        ret = SaSetLearnModeController((SetLearnModeControllerOperation)apiAction);
                    }
                    else if (apiAction is SetLearnModeSlaveOperation)
                    {
                        ret = SaSetLearnModeSlave((SetLearnModeSlaveOperation)apiAction);
                    }
                }
                if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALL))
                {
                    if (apiAction is InclusionController.Initiate)
                    {
                        ret = SaInclusionControllerInitiate((InclusionController.Initiate)apiAction);
                    }
                    else if (apiAction is InclusionController.Complete)
                    {
                        ret = SaInclusionControllerComplete((InclusionController.Complete)apiAction);
                    }
                    else if (apiAction is RequestNodeInfoOperation)
                    {
                        ret = SaRequestNodeInfo((RequestNodeInfoOperation)apiAction);
                    }
                    else if (apiAction is AddNodeOperation)
                    {
                        ret = SaAddNode((AddNodeOperation)apiAction);
                    }
                    else if (apiAction is ControllerChangeOperation)
                    {
                        ret = SaControllerChange((ControllerChangeOperation)apiAction);
                    }
                    else if (apiAction is ReplaceFailedNodeOperation)
                    {
                        ret = SaReplaceFailedNode((ReplaceFailedNodeOperation)apiAction);
                    }
                    else if (apiAction is SetSlaveLearnModeOperation)
                    {
                        ret = SaSetSlaveLearnMode((SetSlaveLearnModeOperation)apiAction);
                    }
                }
            }
            return ret;
        }

        private ActionBase SaSendData(SendDataOperation apiAction, SecuritySchemes? sendDataSchemeRequested)
        {
            ActionBase ret = null;
            Extensions extensionsToAdd = apiAction.Extensions as Extensions;
            int maxBytesPerFrameSize = SecurityManagerInfo.MaxFragmentSize;
            if (apiAction.SubstituteSettings.MaxBytesPerFrameSize > 0)
            {
                maxBytesPerFrameSize = apiAction.SubstituteSettings.MaxBytesPerFrameSize;
            }
            byte srcNodeId = SecurityManagerInfo.Network.NodeId;
            byte dstNodeId = apiAction.NodeId;
            byte[] data = apiAction.Data;
            bool isBroadCast = apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseBroadcast);
            if (dstNodeId > 0x00 && dstNodeId < 0xFF && (data != null || extensionsToAdd != null))
            {
                if ((data != null && (data.Length < 2 ||
                    (data[0] != COMMAND_CLASS_SECURITY.ID || data[1] != COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.ID) &&
                    (data[0] != COMMAND_CLASS_SECURITY.ID || data[1] != COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET.ID) &&
                    (data[0] != COMMAND_CLASS_SECURITY_2.ID || data[1] != COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.ID))) ||
                    extensionsToAdd != null)
                {
                    bool isS0OnlySupported = SecurityManagerInfo.Network.HasCommandClass(new NodeTag(dstNodeId), COMMAND_CLASS_SECURITY.ID) &&
                        !SecurityManagerInfo.Network.HasCommandClass(new NodeTag(dstNodeId), COMMAND_CLASS_SECURITY_2.ID);

                    if ((SecurityManagerInfo.Network.GetCurrentSecurityScheme(dstNodeId) != SecuritySchemes.S0) &&
                        (!isS0OnlySupported || SecurityManagerInfo.IsInclusion) &&
                        (IsSendDataSchemeS2(dstNodeId, data, sendDataSchemeRequested, apiAction.SubstituteSettings) || extensionsToAdd != null))
                    {
                        var isSendDataOpValid = true;
                        InvariantPeerNodeId peerNodeId = new InvariantPeerNodeId(srcNodeId, dstNodeId);
                        if (sendDataSchemeRequested != null)
                        {
                            if (!SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId) || SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme != sendDataSchemeRequested)
                            {
                                SecurityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, (SecuritySchemes)sendDataSchemeRequested);
                            }
                        }
                        else if (!SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId))
                        {
                            var scheme = SecurityManagerInfo.Network.GetCurrentSecurityScheme(dstNodeId);
                            if (scheme == SecuritySchemes.NONE && apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseSecurity))
                            {
                                scheme = SecuritySchemes.S2_ACCESS;
                            }
                            SecurityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, scheme);
                        }

                        Extensions extensions = null;
                        if (apiAction.IsFollowup) // Is singlecast follow-up after multicast.
                        {
                            var lastGroupId = _securityS2CryptoProvider.LastSentMulticastGroupId;
                            var nodeGroupId = new NodeGroupId(srcNodeId, lastGroupId);
                            if (lastGroupId != 0 &&
                                SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).ReceiverGroupHandle.Contains(dstNodeId)
                                )
                            {
                                extensions = new Extensions();
                                extensions.AddMpanGrpExtension(lastGroupId);
                            }
                            else
                            {
                                isSendDataOpValid = false;
                            }
                        }
                        else
                        {
                            var mpanRecord = SecurityManagerInfo.MpanTable.GetLatestContainerByOwnerId(dstNodeId);
                            if (mpanRecord != null && (mpanRecord.IsMosState || mpanRecord.IsMosReportedState))
                            {
                                extensions = new Extensions();
                                extensions.AddMosExtension();
                                if (!mpanRecord.IsMosReportedState)
                                {
                                    mpanRecord.SetMosStateReported();
                                }
                            }
                        }

                        if (isSendDataOpValid)
                        {
                            if (extensionsToAdd != null)
                            {
                                // Note ignoring previous - only possible follow up - should not occure.
                                extensions = extensionsToAdd;
                            }

                            var sckey = SecurityManagerInfo.ScKeys[new InvariantPeerNodeId(srcNodeId, dstNodeId)];
                            var encryptedMsg = _securityS2CryptoProvider.EncryptSinglecastCommand(sckey, SecurityManagerInfo.SpanTable, srcNodeId, dstNodeId, SecurityManagerInfo.Network.HomeId, data, extensions, apiAction.SubstituteSettings);
                            if (encryptedMsg == null)
                            {
                                ret = new SendDataSecureS2Task(SecurityManagerInfo, _securityS2CryptoProvider, sckey, SecurityManagerInfo.SpanTable, SecurityManagerInfo.MpanTable, dstNodeId, data, apiAction.TxOptions)
                                {
                                    DataDelay = apiAction.DataDelay,
                                    SubstituteSettingsForRetransmission = apiAction.SubstituteSettings
                                };
                                if (isBroadCast)
                                {
                                    ((SendDataSecureS2Task)ret).TestNodeId = 0xFF;
                                }
                                ((SendDataSecureS2Task)ret).SubstituteCallback = apiAction.SubstituteCallback;
                                //If we send broadcast without being synchronized. Needed for SC followup messages
                                ((SendDataSecureS2Task)ret).ExtensionsToAdd = extensions;
                            }
                            else
                            {
                                SecurityManagerInfo.LastSendDataBuffer = encryptedMsg;
                                apiAction.Data = encryptedMsg;
                                apiAction.IsFollowup = false;
                                if (isBroadCast)
                                {
                                    apiAction.NodeId = 0xFF;
                                }
                                apiAction.SpecificResult.SubstituteStatus = SubstituteStatuses.Done;
                                if (apiAction.SubstituteCallback != null)
                                    apiAction.SubstituteCallback();
                                ret = apiAction;
                                #region MessageEncapsulation
                                apiAction.Data = SecurityManagerInfo.TestOverrideMessageEncapsulation(SecurityManagerInfo.ScKeys[peerNodeId], SecurityManagerInfo.SpanTable, _securityS2CryptoProvider, apiAction.SubstituteSettings, dstNodeId, data, peerNodeId, extensions, encryptedMsg, apiAction.Data);
                                #endregion
                            }
                        }
                        else
                        {
                            ret = null;
                        }
                    }
                    else if (IsSendDataSchemeS0(dstNodeId, data, sendDataSchemeRequested, apiAction.SubstituteSettings))
                    {
                        if (maxBytesPerFrameSize == 0 || data.Length <= maxBytesPerFrameSize)
                        {
                            ret = new SendDataSecureTask(SecurityManagerInfo, _securityS0CryptoProvider, dstNodeId, data, apiAction.TxOptions) { DataDelay = apiAction.DataDelay };
                            if (isBroadCast)
                            {
                                ((SendDataSecureTask)ret).TestNodeId = 0xFF;
                            }
                        }
                        else
                        {
                            ret = new SendDataFragmentedSecureTask(SecurityManagerInfo, _securityS0CryptoProvider, dstNodeId, data, apiAction.TxOptions) { DataDelay = apiAction.DataDelay };
                            ((SendDataFragmentedSecureTask)ret).SequenceCounter = _fragmentSequenceCounter++;
                            ((SendDataFragmentedSecureTask)ret).MaxBytesPerFrameSize = maxBytesPerFrameSize;
                        }
                    }
                    else if (sendDataSchemeRequested != null && sendDataSchemeRequested == SecuritySchemes.NONE)
                    {
                        ret = apiAction;
                        if (isBroadCast)
                        {
                            ((SendDataOperation)ret).NodeId = 0xFF;
                        }
                    }
                    else
                    {
                        ret = null;
                    }
                }
            }
            return ret;
        }

        private ActionBase SaSetLearnModeController(SetLearnModeControllerOperation apiAction)
        {
            ActionBase ret = null;
            if (apiAction.Mode != LearnModes.LearnModeDisable)
            {
                apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                apiAction.NewToken(true);
                ret = new SetLearnModeSecureOperation(SecurityManagerInfo, apiAction, SetDefault);
            }
            return ret;
        }

        private ActionBase SaSetSlaveLearnMode(SetSlaveLearnModeOperation apiAction)
        {
            ActionBase ret = null;
            if (apiAction.Mode == SlaveLearnModes.VirtualSlaveLearnModeEnable)
            {
                apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                apiAction.NewToken(true);
                ret = new SetLearnModeSecureOperation(SecurityManagerInfo, apiAction, SetDefault);
            }
            return ret;
        }

        private ActionBase SaSetLearnModeSlave(SetLearnModeSlaveOperation apiAction)
        {
            ActionBase ret = null;
            if (apiAction.Mode != LearnModes.LearnModeDisable)
            {
                apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                apiAction.NewToken(true);
                ret = new SetLearnModeSecureOperation(SecurityManagerInfo, apiAction, SetDefault);
            }
            return ret;
        }

        private ActionBase SaInclusionControllerInitiate(InclusionController.Initiate apiAction)
        {
            ActionBase ret;
            apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            apiAction.NewToken(true);
            ret = new InclusionControllerSecure.Initiate(SecurityManagerInfo)
            {
                RequestInclusionSupport = apiAction
            };
            return ret;
        }

        private ActionBase SaInclusionControllerComplete(InclusionController.Complete apiAction)
        {
            ActionBase ret;
            ret = new InclusionControllerSecure.Complete(SecurityManagerInfo)
            {
                SendInclusionComplete = apiAction
            };
            return ret;
        }

        private ActionBase SaSendDataMulti(ApiOperation apiAction)
        {
            ActionBase ret = null;
            byte[] multiActionNodeIds = null;
            TransmitOptions multiActionTxOptions = TransmitOptions.TransmitOptionNone;
            byte[] multiActionData = null;
            byte multiActionSourceNodeId = 0x00;
            if (apiAction is SendDataMultiExOperation)
            {
                apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenyFollowup);
                var sendDataMultiExAction = (SendDataMultiExOperation)apiAction;
                multiActionNodeIds = new byte[] { 0xFE };
                multiActionTxOptions = sendDataMultiExAction.TxOptions;
                multiActionData = sendDataMultiExAction.Data;
            }
            else if (apiAction is SendDataMultiOperation)
            {
                multiActionNodeIds = ((SendDataMultiOperation)apiAction).NodeIds;
                multiActionTxOptions = ((SendDataMultiOperation)apiAction).TxOptions;
                multiActionData = ((SendDataMultiOperation)apiAction).Data;
            }
            else if (apiAction is SendDataOperation)
            {
                var sdo = (SendDataOperation)apiAction;
                multiActionNodeIds = new byte[] { sdo.NodeId };
                multiActionTxOptions = sdo.TxOptions;
                multiActionData = sdo.Data;
            }
            else if (apiAction is SendDataBridgeOperation)
            {
                var sdbo = (SendDataBridgeOperation)apiAction;
                multiActionNodeIds = new byte[] { sdbo.DestNodeId };
                multiActionTxOptions = sdbo.TxOptions;
                multiActionData = sdbo.Data;
                multiActionSourceNodeId = sdbo.SrcNodeId;
            }
            else if (apiAction is SendDataExOperation)
            {
                var sdxo = (SendDataExOperation)apiAction;
                multiActionNodeIds = new byte[] { sdxo.NodeId };
                multiActionTxOptions = sdxo.TxOptions;
                multiActionData = sdxo.Data;
            }
            else if (apiAction is SendDataMultiBridgeOperation)
            {
                multiActionNodeIds = ((SendDataMultiBridgeOperation)apiAction).NodeIds;
                multiActionTxOptions = ((SendDataMultiBridgeOperation)apiAction).TxOptions;
                multiActionData = ((SendDataMultiBridgeOperation)apiAction).Data;
                multiActionSourceNodeId = ((SendDataMultiBridgeOperation)apiAction).SrcNodeId;
                // Possibility to send from specific Bridge's node is omitted.
            }
            if (multiActionSourceNodeId == 0x00 && SecurityManagerInfo.Network.IsBridgeController)
            {
                multiActionSourceNodeId = SecurityManagerInfo.Network.NodeId;
            }

            var txOptions = multiActionTxOptions & (~TransmitOptions.TransmitOptionAcknowledge);
            var groupId = SecurityManagerInfo.MpanTable.FindGroup(multiActionNodeIds);
            if (multiActionNodeIds.Count(x => x == 0xFE || SecurityManagerInfo.Network.HasSecurityScheme(x, SecuritySchemeSet.ALLS2)) > 0)
            {
                var ownerId = SecurityManagerInfo.Network.NodeId;
                var nodeGroupId = new NodeGroupId(ownerId, groupId);
                if (groupId == 0)
                {
                    groupId = ++_multGroupCounter;
                    nodeGroupId = new NodeGroupId(ownerId, groupId);
                    SecurityManagerInfo.MpanTable.AddOrReplace(nodeGroupId, 0x55, multiActionNodeIds, _securityS2CryptoProvider.GetRandomData());
                }

                if (!SecurityManagerInfo.McKeys.ContainsKey(new NodeGroupId(ownerId, groupId)))
                {
                    SecuritySchemes scheme = SecuritySchemes.NONE;
                    foreach (var multiNodeId in multiActionNodeIds)
                    {
                        scheme = SecurityManagerInfo.Network.GetCurrentSecurityScheme(multiNodeId);
                        if (scheme != SecuritySchemes.NONE && scheme != SecuritySchemes.S0)
                        {
                            break;
                        }
                    }
                    if (scheme == SecuritySchemes.NONE || scheme == SecuritySchemes.S0)
                    {
                        scheme = SecuritySchemes.S2_UNAUTHENTICATED; // Default.
                    }
                    SecurityManagerInfo.ActivateNetworkKeyS2Multi(groupId, ownerId, scheme);
                }

                var encryptedMulticastData = _securityS2CryptoProvider.EncryptMulticastCommand(SecurityManagerInfo.McKeys[nodeGroupId], SecurityManagerInfo.MpanTable, nodeGroupId, SecurityManagerInfo.Network.HomeId, multiActionData);

                if (!apiAction.SubstituteSettings.HasFlag(SubstituteFlags.DenyFollowup) && multiActionTxOptions.HasFlag(TransmitOptions.TransmitOptionAcknowledge))
                {
                    ActionSerialGroup ag = null;
                    if (SerialApiVersion >= 8)
                    {
                        ag = new ActionSerialGroup
                        (
                            //DK680
                            CreateSendDataMultiOperation(multiActionSourceNodeId, multiActionNodeIds, encryptedMulticastData, txOptions, apiAction.SubstituteSettings)
                        );
                    }
                    else
                    {
                        ag = new ActionSerialGroup
                        (
                        //DK680
                        new SendDataOperation(0xFF, encryptedMulticastData, txOptions) { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) }
                        );
                    }
                    foreach (var destNodeId in multiActionNodeIds)
                    {
                        if (SecurityManagerInfo.Network.HasSecurityScheme(destNodeId, SecuritySchemeSet.ALLS2))
                        {
                            var reqData = new RequestDataOperation(0,
                                destNodeId,
                                multiActionData.ToArray(),
                                multiActionTxOptions,
                                new COMMAND_CLASS_SUPERVISION.SUPERVISION_REPORT(),
                                2,
                                SC_FOLLOWUP_TIMEOUT);
                            reqData.IsFollowup = true;
                            ag.AddActions(reqData);
                        }
                    }
                    NextMPAN(nodeGroupId, new byte[16]); // Spec [CC:009F.01.00.11.028]
                    ret = ag;
                }
                else
                {
                    ret = CreateSendDataMultiOperation(multiActionSourceNodeId, multiActionNodeIds, encryptedMulticastData, txOptions, apiAction.SubstituteSettings);
                }
            }
            return ret;
        }

        private ActionBase SaSendDataBridge(SendDataBridgeOperation apiAction, SecuritySchemes? sendDataSchemeRequested)
        {
            ActionBase ret = null;
            Extensions extensionsToAdd = apiAction.Extensions as Extensions;
            int maxBytesPerFrameSize = SecurityManagerInfo.MaxFragmentSize;
            if (apiAction.SubstituteSettings.MaxBytesPerFrameSize > 0)
            {
                maxBytesPerFrameSize = apiAction.SubstituteSettings.MaxBytesPerFrameSize;
            }
            byte srcNodeId = apiAction.SrcNodeId;
            byte dstNodeId = apiAction.DestNodeId;
            byte[] data = apiAction.Data;
            bool isBroadCast = apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseBroadcast);
            if (dstNodeId > 0x00 && dstNodeId < 0xFF && (data != null || extensionsToAdd != null))
            {
                if ((data != null && (data.Length < 2 ||
                    (data[0] != COMMAND_CLASS_SECURITY.ID || data[1] != COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION.ID) &&
                    (data[0] != COMMAND_CLASS_SECURITY.ID || data[1] != COMMAND_CLASS_SECURITY.SECURITY_MESSAGE_ENCAPSULATION_NONCE_GET.ID) &&
                    (data[0] != COMMAND_CLASS_SECURITY_2.ID || data[1] != COMMAND_CLASS_SECURITY_2.SECURITY_2_MESSAGE_ENCAPSULATION.ID))) ||
                    extensionsToAdd != null)
                {
                    bool isS0OnlySupported = SecurityManagerInfo.Network.HasCommandClass(new NodeTag(dstNodeId), COMMAND_CLASS_SECURITY.ID) &&
                        !SecurityManagerInfo.Network.HasCommandClass(new NodeTag(dstNodeId), COMMAND_CLASS_SECURITY_2.ID);

                    if ((!isS0OnlySupported || SecurityManagerInfo.IsInclusion) &&
                       (IsSendDataSchemeS2(dstNodeId, data, sendDataSchemeRequested, apiAction.SubstituteSettings) || extensionsToAdd != null))
                    {
                        var isSendDataOpValid = true;
                        InvariantPeerNodeId peerNodeId = new InvariantPeerNodeId(srcNodeId, dstNodeId);
                        if (sendDataSchemeRequested != null)
                        {
                            if (!SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId) || SecurityManagerInfo.ScKeys[peerNodeId].SecurityScheme != sendDataSchemeRequested)
                            {
                                SecurityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, (SecuritySchemes)sendDataSchemeRequested);
                            }
                        }
                        else if (!SecurityManagerInfo.ScKeys.ContainsKey(peerNodeId))
                        {
                            var scheme = SecurityManagerInfo.Network.GetCurrentSecurityScheme(dstNodeId);
                            if (scheme == SecuritySchemes.NONE && apiAction.SubstituteSettings.HasFlag(SubstituteFlags.UseSecurity))
                            {
                                scheme = SecuritySchemes.S2_ACCESS;
                            }
                            SecurityManagerInfo.ActivateNetworkKeyS2ForNode(peerNodeId, scheme);
                        }

                        Extensions extensions = null;
                        if (apiAction.IsFollowup) // Is singlecast follow-up after multicast.
                        {
                            var lastGroupId = _securityS2CryptoProvider.LastSentMulticastGroupId;
                            var nodeGroupId = new NodeGroupId(srcNodeId, lastGroupId);
                            if (lastGroupId != 0 &&
                                SecurityManagerInfo.MpanTable.GetContainer(nodeGroupId).ReceiverGroupHandle.Contains(dstNodeId)
                                )
                            {
                                extensions = new Extensions();
                                extensions.AddMpanGrpExtension(lastGroupId);
                            }
                            else
                            {
                                isSendDataOpValid = false;
                            }
                        }
                        else
                        {
                            var mpanRecord = SecurityManagerInfo.MpanTable.GetLatestContainerByOwnerId(dstNodeId);
                            if (mpanRecord != null && (mpanRecord.IsMosState || mpanRecord.IsMosReportedState))
                            {
                                extensions = new Extensions();
                                extensions.AddMosExtension();
                                if (!mpanRecord.IsMosReportedState)
                                {
                                    mpanRecord.SetMosStateReported();
                                }
                            }
                        }

                        if (isSendDataOpValid)
                        {
                            if (extensionsToAdd != null)
                            {
                                // Note ignoring previous - only possible follow up - should not occure.
                                extensions = extensionsToAdd;
                            }
                            var sckey = SecurityManagerInfo.ScKeys[new InvariantPeerNodeId(srcNodeId, dstNodeId)];
                            var encryptedMsg = _securityS2CryptoProvider.EncryptSinglecastCommand(sckey, SecurityManagerInfo.SpanTable, srcNodeId, dstNodeId, SecurityManagerInfo.Network.HomeId, data, extensions, apiAction.SubstituteSettings);
                            if (encryptedMsg == null)
                            {
                                ret = new SendDataBridgeSecureS2Task(SecurityManagerInfo, _securityS2CryptoProvider, sckey, SecurityManagerInfo.SpanTable, SecurityManagerInfo.MpanTable, srcNodeId, dstNodeId, data, apiAction.TxOptions)
                                {
                                    DataDelay = apiAction.DataDelay,
                                    SubstituteSettingsForRetransmission = apiAction.SubstituteSettings
                                };
                                if (isBroadCast)
                                {
                                    ((SendDataBridgeSecureS2Task)ret).TestNodeId = 0xFF;
                                }
                                ((SendDataBridgeSecureS2Task)ret).SubstituteCallback = apiAction.SubstituteCallback;
                                //If we send broadcast without being synchronized. Needed for SC followup messages
                                ((SendDataBridgeSecureS2Task)ret).ExtensionsToAdd = extensions;
                            }
                            else
                            {
                                SecurityManagerInfo.LastSendDataBuffer = encryptedMsg;
                                apiAction.Data = encryptedMsg;
                                apiAction.IsFollowup = false;
                                if (isBroadCast)
                                {
                                    apiAction.DestNodeId = 0xFF;
                                }
                                apiAction.SpecificResult.SubstituteStatus = SubstituteStatuses.Done;
                                if (apiAction.SubstituteCallback != null)
                                    apiAction.SubstituteCallback();
                                ret = apiAction;
                                #region MessageEncapsulation
                                apiAction.Data = SecurityManagerInfo.TestOverrideMessageEncapsulation(SecurityManagerInfo.ScKeys[peerNodeId], SecurityManagerInfo.SpanTable, _securityS2CryptoProvider, apiAction.SubstituteSettings, dstNodeId, data, peerNodeId, extensions, encryptedMsg, apiAction.Data);
                                #endregion
                            }
                        }
                        else
                        {
                            ret = null;
                        }
                    }
                    else if (IsSendDataSchemeS0(dstNodeId, data, sendDataSchemeRequested, apiAction.SubstituteSettings))
                    {
                        if (maxBytesPerFrameSize == 0 || data.Length <= maxBytesPerFrameSize)
                        {
                            ret = new SendDataBridgeSecureTask(SecurityManagerInfo, _securityS0CryptoProvider, srcNodeId, dstNodeId, data, apiAction.TxOptions) { DataDelay = apiAction.DataDelay };
                            if (isBroadCast)
                            {
                                ((SendDataBridgeSecureTask)ret).TestNodeId = 0xFF;
                            }
                        }
                        else
                        {
                            ret = new SendDataFragmentedSecureTask(SecurityManagerInfo, _securityS0CryptoProvider, dstNodeId, data, apiAction.TxOptions) { DataDelay = apiAction.DataDelay };
                            ((SendDataFragmentedSecureTask)ret).SequenceCounter = _fragmentSequenceCounter++;
                            ((SendDataFragmentedSecureTask)ret).MaxBytesPerFrameSize = maxBytesPerFrameSize;
                        }
                    }
                    else if (sendDataSchemeRequested != null && sendDataSchemeRequested == SecuritySchemes.NONE)
                    {
                        ret = apiAction;
                        if (isBroadCast)
                        {
                            ((SendDataBridgeOperation)ret).DestNodeId = 0xFF;
                        }
                    }
                    else
                    {
                        ret = null;
                    }
                }
            }
            return ret;
        }

        private ActionBase SaRequestNodeInfo(RequestNodeInfoOperation apiAction)
        {
            ActionBase ret;
            ActionBase apiParent = apiAction.ParentAction;
            while (apiParent != null && apiParent.ParentAction != null && !(apiParent is InclusionTask))
            {
                apiParent = apiParent.ParentAction;
            }
            bool isInclusionTask = false;
            if (apiParent != null && apiParent is InclusionTask)
            {
                isInclusionTask = true;
                if ((apiParent as InclusionTask).SpecificResult.AddRemoveNode.AddRemoveNodeStatus == AddRemoveNodeStatuses.Replaced)
                {
                    SecurityManagerInfo.Network.ResetSecuritySchemes(apiAction.NodeId);
                    isInclusionTask = false;
                }
            }
            apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            apiAction.NewToken(true);
            ret = new RequestNodeInfoSecureTask(SecurityManagerInfo, apiAction, isInclusionTask);
            return ret;
        }

        private ActionBase SaAddNode(AddNodeOperation apiAction)
        {
            ActionBase ret = null;
            if (apiAction.InitMode != Modes.NodeStop)
            {
                apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                apiAction.NewToken(true);
                ret = new ActionSerialGroup(apiAction) { Name = "AddNodeSecure" };
                if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                    (ret as ActionSerialGroup).AddActions(new AddNodeS2Operation(SecurityManagerInfo));

                if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                    (ret as ActionSerialGroup).AddActions(new AddNodeS0Operation(SecurityManagerInfo));
            }

            return ret;
        }

        private ActionBase SaControllerChange(ControllerChangeOperation apiAction)
        {
            ActionBase ret = null;
            if (apiAction.InitMode != ControllerChangeModes.Stop)
            {
                apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                apiAction.NewToken(true);
                ret = new ActionSerialGroup(apiAction) { Name = "ControllerChangeSecure" };
                if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                    (ret as ActionSerialGroup).AddActions(new AddNodeS2Operation(SecurityManagerInfo));

                if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                    (ret as ActionSerialGroup).AddActions(new AddNodeS0Operation(SecurityManagerInfo));
            }

            return ret;
        }

        private ActionBase SaReplaceFailedNode(ReplaceFailedNodeOperation apiAction)
        {
            ActionBase ret;
            apiAction.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            apiAction.NewToken(true);
            ret = new ActionSerialGroup(apiAction,
                new RequestNodeInfoOperation(apiAction.ReplacedNodeId) { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) }
                )
            { Name = "ReplaceFailedNodeSecure" };
            if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                (ret as ActionSerialGroup).AddActions(new AddNodeS2Operation(SecurityManagerInfo));

            if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                (ret as ActionSerialGroup).AddActions(new AddNodeS0Operation(SecurityManagerInfo));
            return ret;
        }

        //private ActionBase CreateSendDataMultiOperation(byte bridgeNodeId, byte[] nodeIds, byte[] mcData)
        //{
        //    return CreateSendDataMultiOperation(bridgeNodeId, nodeIds, mcData, TransmitOptions.TransmitOptionLowPower);
        //}

        private ActionBase CreateSendDataMultiOperation(byte bridgeNodeId, byte[] nodeIds, byte[] mcData, TransmitOptions txOptions, SubstituteSettings substituteSettings)
        {
            ActionBase ret;

            if (!substituteSettings.HasFlag(SubstituteFlags.UseMulticast))
            {
                txOptions |= TransmitOptions.TransmitOptionLowPower;
            }

            if (bridgeNodeId > 0)
            {
                ret = new SendDataMultiBridgeOperation(bridgeNodeId, nodeIds, mcData, txOptions)
                {
                    SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
                };
            }
            else
            {
                ret = new SendDataMultiOperation(nodeIds, mcData, txOptions)
                {
                    SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
                };
            };
            return ret;
        }

        private bool IsEnabledS0()
        {
            return SecurityManagerInfo.Network.IsEnabledS0;
        }

        private bool IsEnabledS2()
        {
            return (SecurityManagerInfo.Network.IsEnabledS2_UNAUTHENTICATED ||
                SecurityManagerInfo.Network.IsEnabledS2_AUTHENTICATED ||
                SecurityManagerInfo.Network.IsEnabledS2_ACCESS);
        }

        public void NextMPAN(NodeGroupId nodeGroupId, byte[] outMpan)
        {
            SecurityS2CryptoProviderBase.IncrementMpan(SecurityManagerInfo.MpanTable, SecurityManagerInfo.McKeys[nodeGroupId], nodeGroupId, outMpan);
        }

        private void SetWasRetransmittedOnS2NonceGet(ApiOperation apiAction)
        {
            InvariantPeerNodeId peerNodeId;
            byte[] data = null;
            if (apiAction is SendDataExOperation)
            {
                var sendDataExAction = (SendDataExOperation)apiAction;
                peerNodeId = new InvariantPeerNodeId(0, sendDataExAction.NodeId);
                data = sendDataExAction.Data;
            }
            else if (apiAction is SendDataOperation)
            {
                var sendDataAction = (SendDataOperation)apiAction;
                peerNodeId = new InvariantPeerNodeId(0, sendDataAction.NodeId);
                data = sendDataAction.Data;
            }
            else
            {
                peerNodeId = new InvariantPeerNodeId(0, 0);
            }
            if (data != null && data.Length > 1 && data[0] == COMMAND_CLASS_SECURITY_2.ID &&
                data[1] == COMMAND_CLASS_SECURITY_2.SECURITY_2_NONCE_GET.ID && SecurityManagerInfo.RetransmissionTableS2.ContainsKey(peerNodeId))
            {
                SecurityManagerInfo.RetransmissionTableS2[peerNodeId].Counter = 1;
            }
        }

        private bool IsSendDataSchemeS0(byte nodeId, byte[] data, SecuritySchemes? securitySchemeRequested, SubstituteSettings substituteSettings)
        {
            bool ret = false;
            if (securitySchemeRequested == null)
            {
                if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemes.S0))
                {
                    if (substituteSettings.HasFlag(SubstituteFlags.DenySecurity))
                    {
                        ret = false;
                    }
                    else if (substituteSettings.HasFlag(SubstituteFlags.UseSecurity))
                    {
                        ret = true;
                    }
                    else
                    {
                        if (SecurityManagerInfo.Network.HasSecurityScheme(nodeId, SecuritySchemes.S0))
                        {

                            ret = (SecurityManagerInfo.IsInclusion ||
                                SecurityManagerInfo.SendDataSubstitutionCallback == null ||
                                SecurityManagerInfo.SendDataSubstitutionCallback(nodeId, data));
                        }
                        else if (data != null && data.Length > 0)
                        {

                            ret = data[0] == COMMAND_CLASS_SECURITY.ID;
                        }
                    }
                }
            }
            else
            {
                ret = securitySchemeRequested == SecuritySchemes.S0;
            }
            return ret;
        }

        private bool IsSendDataSchemeS2(byte nodeId, byte[] data, SecuritySchemes? securitySchemeRequested, SubstituteSettings substituteSettings)
        {
            bool ret = false;
            if (securitySchemeRequested == null)
            {
                if (data != null && data.Length > 1 && (data[0] != COMMAND_CLASS_SECURITY.ID ||
                    (data[1] != COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_REPORT.ID &&
                    data[1] != COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_GET.ID)))
                {
                    //Check that Controller supports S2 in first place
                    if (SecurityManagerInfo.Network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                    {
                        if (substituteSettings.HasFlag(SubstituteFlags.DenySecurity))
                        {
                            ret = false;
                        }
                        else if (substituteSettings.HasFlag(SubstituteFlags.UseSecurity))
                        {
                            //if (_securityS2CryptoProvider.HasActiveKeyForNode(nodeId))
                            //{
                            //    var scheme = _securityS2CryptoProvider.GetActiveSecuritySchemeForNode(nodeId);
                            //    ret = scheme == SecuritySchemes.S2_ACCESS ||
                            //        scheme == SecuritySchemes.S2_AUTHENTICATED ||
                            //        scheme == SecuritySchemes.S2_UNAUTHENTICATED;
                            //}
                            ret = true;
                        }
                        else
                        {
                            ret = SecurityManagerInfo.Network.HasSecurityScheme(nodeId, SecuritySchemeSet.ALLS2) && (SecurityManagerInfo.IsInclusion ||
                               SecurityManagerInfo.SendDataSubstitutionCallback == null ||
                               SecurityManagerInfo.SendDataSubstitutionCallback(nodeId, data));
                        }
                    }
                    else
                    {
                        ret = false;
                    }
                }
            }
            else
            {
                ret = (SecurityManagerInfo.IsInclusion && securitySchemeRequested == SecuritySchemes.S2_TEMP) ||
                    securitySchemeRequested == SecuritySchemes.S2_ACCESS ||
                    securitySchemeRequested == SecuritySchemes.S2_AUTHENTICATED ||
                    securitySchemeRequested == SecuritySchemes.S2_UNAUTHENTICATED;
            }
            return ret;
        }

        public override void SetDefault()
        {
            SecurityManagerInfo.SetNetworkKey(GenerateNetworkKey(SecuritySchemes.S0), SecuritySchemes.S0);
            SecurityManagerInfo.SetNetworkKey(GenerateNetworkKey(SecuritySchemes.S2_UNAUTHENTICATED), SecuritySchemes.S2_UNAUTHENTICATED);
            SecurityManagerInfo.SetNetworkKey(GenerateNetworkKey(SecuritySchemes.S2_AUTHENTICATED), SecuritySchemes.S2_AUTHENTICATED);
            SecurityManagerInfo.SetNetworkKey(GenerateNetworkKey(SecuritySchemes.S2_ACCESS), SecuritySchemes.S2_ACCESS);
            SecurityManagerInfo.SecretKeyS2 = _securityS2CryptoProvider.GenerateSecretKey();
            SecurityManagerInfo.ActivateNetworkKeyS0();
            SecurityManagerInfo.ActivateNetworkKeyS2ForNode(new InvariantPeerNodeId(0, 0), SecuritySchemes.S2_UNAUTHENTICATED);
            SecurityManagerInfo.Network.ResetAndEnableAndSelfRestore();
            SecurityManagerInfo.SpanTable.ClearNonceTable();
            SecurityManagerInfo.MpanTable.ClearMpanTable();
            SecurityManagerInfo.ScKeys.Clear();
            SecurityManagerInfo.McKeys.Clear();
            _isPrevDecryptFailed = false;
        }

        private List<ActionToken> mRunningActionTokens = new List<ActionToken>();
        private readonly object mLockObject = new object();
        public override List<ActionToken> GetRunningActionTokens()
        {
            List<ActionToken> ret = null;
            lock (mLockObject)
            {
                ret = new List<ActionToken>(mRunningActionTokens);
            }
            return ret;
        }

        public override void AddRunningActionToken(ActionToken token)
        {
            lock (mLockObject)
            {
                mRunningActionTokens.Add(token);
            }
        }

        public override void RemoveRunningActionToken(ActionToken token)
        {
            lock (mLockObject)
            {
                mRunningActionTokens.Remove(token);
            }
        }

        public override void Suspend()
        {
            base.Suspend();
            SecurityManagerInfo.IsActive = false;
            SecurityManagerInfo.RetransmissionTableS2.Clear();
        }

        public override void Resume()
        {
            base.Resume();
            SecurityManagerInfo.IsActive = true;
            SecurityManagerInfo.RetransmissionTableS2.Clear();
        }

        public void GenerateNewSecretKeyS2()
        {
            SecurityManagerInfo.SecretKeyS2 = _securityS2CryptoProvider.GenerateSecretKey();
        }
    }
}
