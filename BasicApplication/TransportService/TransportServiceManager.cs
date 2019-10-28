using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.BasicApplication.TransportService.Operations;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Layers.Frame;

namespace ZWave.BasicApplication.TransportService
{
    public class TransportServiceManager : SubstituteManagerBase
    {
        public const int FRAGMENT_RX_TIMEOUT = 800;

        protected override SubstituteIncomingFlags GetId()
        {
            return SubstituteIncomingFlags.TransportService;
        }

        private readonly object _lockObject = new object();
        private ActionSerialGroup _rxTimerAction;

        private byte _srcNodeId;
        internal byte SrcNodeId { get { return _srcNodeId; } }

        private byte _nodeIdWaitResponded;
        internal byte NodeIdWaitResponded { get { return _nodeIdWaitResponded; } }

        private ReassemblingData _reassemblingData;
        internal ReassemblingData ReassemblingData { get { return _reassemblingData; } }

        public TransportServiceManagerInfo TransportServiceManagerInfo { get; private set; }

        public TransportServiceManager(TransportServiceManagerInfo info)
        {
            IsActive = true;
            TransportServiceManagerInfo = info;
        }

        protected override CustomDataFrame SubstituteIncomingInternal(CustomDataFrame packet, byte destNodeId, byte srcNodeId, byte[] cmdData, int lenIndex, out ActionBase additionalAction, out ActionBase completeAction)
        {
            CustomDataFrame ret = packet;
            additionalAction = null;
            completeAction = null;
            if (IsActive)
            {
                if (cmdData.Length > 1 && cmdData[0] == COMMAND_CLASS_TRANSPORT_SERVICE_V2.ID && ValidateCRC16Checksum(cmdData))
                {
                    byte id = (byte)(cmdData[1] & COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_FIRST_SEGMENT.ID_MASK);

                    if (id == (COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_FIRST_SEGMENT.ID) &&
                        TransportServiceManagerInfo.TestNeedToIgnoreFirstSegment.CanBeUsed &&
                        TransportServiceManagerInfo.TestNeedToIgnoreFirstSegment.PullValue().Value
                        )
                    {
                        return ret;
                    }

                    if (id == (COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_SUBSEQUENT_SEGMENT.ID) &&
                        TransportServiceManagerInfo.TestNeedToIgnoreSubsequentSegment.CanBeUsed &&
                        TransportServiceManagerInfo.TestOffset.CanBeUsed
                        )
                    {
                        var datagramOffset = SegmentsContainer.GetSegmentOffset(cmdData);
                        var payloadLength = SegmentsContainer.GetSegmentPayloadLength(cmdData);
                        if (TransportServiceManagerInfo.TestOffset.PullValue(
                                val =>
                                {
                                    return val >= datagramOffset && val <= (datagramOffset + payloadLength);
                                }) != null &&
                            TransportServiceManagerInfo.TestNeedToIgnoreSubsequentSegment.PullValue().Value
                            )
                        {
                            return ret;
                        }
                    }

                    lock (_lockObject)
                    {
                        if (_reassemblingData == null || _reassemblingData.SegmentsContainer.IsCompleted)
                        {
                            if (id == (COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_FIRST_SEGMENT.ID))
                            {
                                RxReset();
                                _srcNodeId = srcNodeId;

                                var cmdHeader = new byte[lenIndex];
                                Array.Copy(packet.Data, 0, cmdHeader, 0, cmdHeader.Length);

                                var cmdFooter = new byte[packet.Data.Length - cmdHeader.Length - cmdData.Length - 1];
                                Array.Copy(packet.Data, lenIndex + packet.Data[lenIndex] + 1, cmdFooter, 0, cmdFooter.Length);

                                _reassemblingData = new ReassemblingData
                                {
                                    SegmentsContainer = new SegmentsContainer(cmdData),
                                    CompletedCmdHeader = cmdHeader,
                                    CompletedCmdFooter = cmdFooter
                                };

                                if (_reassemblingData.SegmentsContainer.IsCompleted)
                                {
                                    additionalAction = new SendDataOperation(srcNodeId, CreateSegmentCompletedCmd(), TransportServiceManagerInfo.TxOptions)
                                    {
                                        SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
                                    };
                                    ret = CreateDataFrameOnReassemblingCompleted(packet);
                                }
                                else
                                {
                                    // Start fragment rx timer.
                                    _rxTimerAction = CreateRxTimerAction();
                                    additionalAction = _rxTimerAction;
                                }
                            }
                            else if (id == (COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_SUBSEQUENT_SEGMENT.ID))
                            {
                                if (_reassemblingData != null && _reassemblingData.SegmentsContainer.CheckForLastSegment(cmdData))
                                {
                                    additionalAction = new SendDataOperation(srcNodeId, CreateSegmentCompletedCmd(), TransportServiceManagerInfo.TxOptions) { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) };
                                }
                                else if (srcNodeId != _nodeIdWaitResponded)
                                {
                                    _nodeIdWaitResponded = srcNodeId;
                                    additionalAction = new SendDataOperation(srcNodeId, CreateSegmentWaitCmd(1), TransportServiceManagerInfo.TxOptions) { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) };
                                }
                            }
                        }
                        else if (_srcNodeId == srcNodeId && id == (COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_SUBSEQUENT_SEGMENT.ID))
                        {
                            if (_rxTimerAction != null)
                            {
                                _rxTimerAction.Actions[0].Token.Reset(FRAGMENT_RX_TIMEOUT); // Reset fragment rx timer.
                            }
                            else
                            {
                                _rxTimerAction = CreateRxTimerAction();
                                additionalAction = _rxTimerAction;
                            }

                            _reassemblingData.SegmentsContainer.AddSegment(cmdData);
                            if (_reassemblingData.SegmentsContainer.IsLastSegmentReceived)
                            {
                                if (_reassemblingData.SegmentsContainer.IsCompleted)
                                {
                                    _rxTimerAction.Token.SetCompleted(); // Complete fragment rx timer.
                                    completeAction = _rxTimerAction;
                                    _rxTimerAction = null;

                                    additionalAction = new SendDataOperation(srcNodeId, CreateSegmentCompletedCmd(), TransportServiceManagerInfo.TxOptions)
                                    {
                                        SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0)
                                    };
                                    ret = CreateDataFrameOnReassemblingCompleted(packet);
                                }
                                else
                                {
                                    ushort missingOffset = _reassemblingData.SegmentsContainer.GetFirstMissingFragmentOffset();
                                    additionalAction = new SendDataOperation(srcNodeId,
                                        CreateSegmentRequestCmd(missingOffset),
                                        TransportServiceManagerInfo.TxOptions)
                                    { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) };
                                }
                            }
                        }
                        else
                        {
                            if (srcNodeId != _nodeIdWaitResponded)
                            {
                                _nodeIdWaitResponded = srcNodeId;
                                additionalAction = new SendDataOperation(srcNodeId,
                                    CreateSegmentWaitCmd(_reassemblingData.SegmentsContainer.PendingSegmentsCount),
                                    TransportServiceManagerInfo.TxOptions)
                                { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) };
                            }
                        }
                    }
                }
            }
            return ret;
        }

        private CustomDataFrame CreateDataFrameOnReassemblingCompleted(CustomDataFrame incomePacket)
        {
            var dataHeader = _reassemblingData.CompletedCmdHeader;
            var dataFooter = _reassemblingData.CompletedCmdFooter;
            var reasembledCmd = _reassemblingData.SegmentsContainer.GetDatagram();
            var packetData = new byte[_reassemblingData.CompletedCmdHeader.Length +
                _reassemblingData.CompletedCmdFooter.Length +
                reasembledCmd.Length + 1];
            int offset = 0;
            Array.Copy(dataHeader, packetData, dataHeader.Length);
            offset += dataHeader.Length;

            packetData[offset] = (byte)reasembledCmd.Length;
            offset++;

            Array.Copy(reasembledCmd, 0, packetData, offset, reasembledCmd.Length);
            offset += reasembledCmd.Length;
            Array.Copy(dataFooter, 0, packetData, offset, dataFooter.Length);

            return CreateNewFrame(incomePacket, packetData, reasembledCmd.Length);
        }

        private ActionSerialGroup CreateRxTimerAction()
        {
            var ret = new ActionSerialGroup(
                OnFragmentRxTimeout,
                new ExpectDataOperation(0, 0, new byte[] { COMMAND_CLASS_TRANSPORT_SERVICE_V2.ID, 0xFF }, 2,
                    FRAGMENT_RX_TIMEOUT)
                { Name = "First Rx Timeout" },
                new SendDataOperation(_srcNodeId,
                    null,
                    TransportServiceManagerInfo.TxOptions)
                { SubstituteSettings = new SubstituteSettings(SubstituteFlags.DenySecurity, 0) },
                new ExpectDataOperation(0, 0, new byte[] { COMMAND_CLASS_TRANSPORT_SERVICE_V2.ID, 0xFF }, 2,
                    FRAGMENT_RX_TIMEOUT)
                { Name = "Second Rx Timeout" });
            ret.CompletedCallback = OnRxTimerActionCompleted;
            return ret;
        }

        private void OnFragmentRxTimeout(ActionBase actionBase, ActionResult actionResult)
        {
            if (actionBase is SendDataOperation)
            {
                var sendSegmentRequestAction = (SendDataOperation)actionBase;
                ushort missingOffset = _reassemblingData.SegmentsContainer.GetFirstMissingFragmentOffset();
                sendSegmentRequestAction.Data = CreateSegmentRequestCmd(missingOffset);
            }
        }

        private void OnRxTimerActionCompleted(IActionItem action)
        {
            var actionRxTimerAction = (ActionSerialGroup)action;
            if (actionRxTimerAction.Actions[2].Token.State == ActionStates.Expired)
            {
                lock (_lockObject)
                {
                    RxReset();
                }
            }
        }

        private void RxReset()
        {
            _nodeIdWaitResponded = 0;
            _srcNodeId = 0;
            _reassemblingData = null;
            _rxTimerAction = null;
        }

        private byte[] CreateSegmentCompletedCmd()
        {
            var segmentCompleteActionCmd = new COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_SEGMENT_COMPLETE();
            if (TransportServiceManagerInfo.TestSegmentCompleteCmdSessionId.CanBeUsed)
            {
                segmentCompleteActionCmd.properties2.sessionId = TransportServiceManagerInfo.TestSegmentCompleteCmdSessionId.PullValue().Value;
            }
            else
            {
                segmentCompleteActionCmd.properties2.sessionId = _reassemblingData.SegmentsContainer.SessionId;
            }
            return segmentCompleteActionCmd;
        }

        private byte[] CreateSegmentWaitCmd(byte pendingSegments)
        {
            var segmentWaitCmd = new COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_SEGMENT_WAIT();
            segmentWaitCmd.pendingFragments = pendingSegments;
            return segmentWaitCmd;
        }

        private byte[] CreateSegmentRequestCmd(ushort missingOffset)
        {
            var segmentRequestAction = new COMMAND_CLASS_TRANSPORT_SERVICE_V2.COMMAND_SEGMENT_REQUEST();
            segmentRequestAction.properties2.sessionId = _reassemblingData.SegmentsContainer.SessionId;
            segmentRequestAction.properties2.datagramOffset1 = (byte)((missingOffset) >> 8);
            segmentRequestAction.datagramOffset2 = (byte)missingOffset;
            return segmentRequestAction;
        }

        public override ActionBase SubstituteActionInternal(ApiOperation action)
        {
            ActionBase ret = null;
            if (IsActive)
            {
                var sendDataOperation = action as SendDataOperation;
                if (TransportServiceManagerInfo.MaxCmdSize > 0 &&
                    sendDataOperation != null &&
                    sendDataOperation.Data != null &&
                    sendDataOperation.Data.Length > TransportServiceManagerInfo.MaxCmdSize)
                {
                    if (sendDataOperation.Data[0] != COMMAND_CLASS_TRANSPORT_SERVICE_V2.ID && sendDataOperation.Data[0] != COMMAND_CLASS_FIRMWARE_UPDATE_MD.ID
                        && !((ApiOperation)action).SubstituteSettings.HasFlag(SubstituteFlags.DenyTransportService))
                    {
                        byte nodeId = sendDataOperation.NodeId;
                        if (nodeId > 0x00 && nodeId < 0xFF && IsTransportServiceSupported(nodeId))
                        {
                            ret = new SendDataTransportTask(TransportServiceManagerInfo, nodeId, sendDataOperation.Data, sendDataOperation.TxOptions);
                        }
                    }
                }
            }
            return ret;
        }

        private bool IsTransportServiceSupported(byte nodeId)
        {
            bool ret = false;
            if (TransportServiceManagerInfo.SendDataSubstitutionCallback != null)
            {
                ret = TransportServiceManagerInfo.SendDataSubstitutionCallback(nodeId);
            }
            return ret;
        }

        public void SetNodeId(byte nodeId)
        {
            TransportServiceManagerInfo.NodeId = nodeId;
        }

        private readonly List<ActionToken> mRunningActionTokens = new List<ActionToken>();
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

        public override void SetDefault()
        {
            SetNodeId(0x01);
        }

        private bool ValidateCRC16Checksum(byte[] cmd)
        {
            var crc16Checksum = cmd.Skip(cmd.Length - 2).ToArray();
            ushort crc = Tools.ZW_CreateCrc16(null, 0, cmd, (byte)(cmd.Length - 2));
            return ((byte)(crc >> 8)) == crc16Checksum[0] && ((byte)crc) == crc16Checksum[1];
        }
    }

    public class ReassemblingData
    {
        public SegmentsContainer SegmentsContainer { get; set; }
        public ActionBase TxTimerAction { get; set; }
        public byte[] CompletedCmdHeader { get; set; }
        public byte[] CompletedCmdFooter { get; set; }
    }
}
