using System;
using System.Collections.Generic;
using System.Linq;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Enums;
using ZWave.Layers.Frame;

namespace ZWave.BasicApplication
{
    public class SupervisionManager : SubstituteManagerBase
    {
        protected override SubstituteIncomingFlags GetId()
        {
            return SubstituteIncomingFlags.Supervision;
        }

        public TransmitOptions TxOptions = TransmitOptions.TransmitOptionAcknowledge | TransmitOptions.TransmitOptionAutoRoute | TransmitOptions.TransmitOptionExplore;
        private static byte _sessionId = 0x01;

        private Func<byte, bool> mSendDataSubstitutionCallback;
        public Func<byte, bool> SendDataSubstitutionCallback
        {
            get
            {
                return mSendDataSubstitutionCallback;
            }
            internal set
            {
                mSendDataSubstitutionCallback = value;
            }
        }

        public SupervisionManager(Func<byte, bool> sendDataSubstitutionCallback)
        {
            SendDataSubstitutionCallback = sendDataSubstitutionCallback;
        }

        public override void OnIncomingSubstituted(CustomDataFrame dataFrameOri, CustomDataFrame dataFrameSub, List<ActionHandlerResult> ahResults)
        {
            if (IsActive)
            {
                bool hasSendData = ahResults.FirstOrDefault(
                    x => x.NextActions != null && x.NextActions.FirstOrDefault(
                        y => y is SendDataOperation || y is SendDataExOperation) != null) != null;
                if (!hasSendData)
                {
                    byte[] cmdData;
                    byte srcNodeId;
                    byte destNodeId;
                    int lenIndex;
                    if (TryParseCommand(dataFrameOri, out destNodeId, out srcNodeId, out lenIndex, out cmdData))
                    {
                        if (cmdData[0] == COMMAND_CLASS_SUPERVISION.ID && cmdData[1] == COMMAND_CLASS_SUPERVISION.SUPERVISION_GET.ID)
                        {
                            COMMAND_CLASS_SUPERVISION.SUPERVISION_GET cmd = cmdData;
                            var data = new COMMAND_CLASS_SUPERVISION.SUPERVISION_REPORT()
                            {
                                duration = 0,
                                properties1 = new COMMAND_CLASS_SUPERVISION.SUPERVISION_REPORT.Tproperties1()
                                {
                                    sessionId = cmd.properties1.sessionId
                                },
                                status = 0xFF
                            };

                            var tempAction = new SendDataOperation(srcNodeId, data, TxOptions);
                            if ((((SubstituteIncomingFlags)dataFrameOri.SubstituteIncomingFlags) & SubstituteIncomingFlags.Security) > 0)
                            {
                                tempAction.SubstituteSettings.SetFlag(SubstituteFlags.UseSecurity);
                            }
                            var ahRes = new ActionHandlerResult(null);
                            ahRes.NextActions.Add(tempAction);
                            ahResults.Add(ahRes);
                        }
                    }
                }
            }
        }

        protected override CustomDataFrame SubstituteIncomingInternal(CustomDataFrame packet, byte destNodeId, byte srcNodeId, byte[] cmdData, int lenIndex, out ActionBase additionalAction, out ActionBase completeAction)
        {
            CustomDataFrame ret = packet;
            additionalAction = null;
            completeAction = null;
            if (IsActive)
            {
                if (cmdData.Length > 4 && cmdData[0] == COMMAND_CLASS_SUPERVISION.ID && cmdData[1] == COMMAND_CLASS_SUPERVISION.SUPERVISION_GET.ID)
                {
                    COMMAND_CLASS_SUPERVISION.SUPERVISION_GET cmd = cmdData;
                    if (cmd.encapsulatedCommand != null && cmd.encapsulatedCommand.Count > 0)
                    {
                        byte[] newFrameData = new byte[packet.Data.Length - 4];
                        Array.Copy(packet.Data, 0, newFrameData, 0, lenIndex);
                        newFrameData[lenIndex] = (byte)(cmdData.Length - 4);
                        Array.Copy(cmdData, 4, newFrameData, lenIndex + 1, cmdData.Length - 4);
                        Array.Copy(packet.Data, lenIndex + 1 + cmdData.Length, newFrameData, lenIndex + 1 + cmdData.Length - 4,
                            packet.Data.Length - lenIndex - 1 - cmdData.Length);
                        ret = CreateNewFrame(packet, newFrameData);
                    }
                }
            }
            return ret;
        }

        public override ActionBase SubstituteActionInternal(ApiOperation action)
        {
            ActionBase ret = null;
            if (IsActive)
            {
                var sendDataAction = action as SendDataOperation;
                if (sendDataAction != null && sendDataAction.IsFollowup)
                {
                    if (IsSupervisionSupported(sendDataAction.NodeId))
                    {
                        if (sendDataAction.Data.Length >= 2 && (sendDataAction.Data[0] != COMMAND_CLASS_SUPERVISION.ID || sendDataAction.Data[1] != COMMAND_CLASS_SUPERVISION.SUPERVISION_GET.ID))
                        {
                            if (sendDataAction.Data[0] == COMMAND_CLASS_MULTI_CHANNEL_V4.ID && sendDataAction.Data[1] == COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP.ID)
                            {
                                COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP multiChannelCommand = sendDataAction.Data;
                                List<byte> innerdata = new List<byte>();
                                innerdata.Add(multiChannelCommand.commandClass);
                                innerdata.Add(multiChannelCommand.command);
                                if (multiChannelCommand.parameter != null)
                                {
                                    innerdata.AddRange(multiChannelCommand.parameter);
                                }
                                var supervisionGetCmd = new COMMAND_CLASS_SUPERVISION.SUPERVISION_GET()
                                {
                                    properties1 = new COMMAND_CLASS_SUPERVISION.SUPERVISION_GET.Tproperties1()
                                    {
                                        sessionId = _sessionId++
                                    },
                                    encapsulatedCommandLength = (byte)innerdata.Count,
                                    encapsulatedCommand = innerdata.ToArray()
                                };
                                byte[] supervisionGetCmdData = supervisionGetCmd;
                                multiChannelCommand.commandClass = supervisionGetCmdData[0];
                                multiChannelCommand.command = supervisionGetCmdData[1];
                                if (supervisionGetCmdData.Length > 2)
                                {
                                    var tmp = new byte[supervisionGetCmdData.Length - 2];
                                    Array.Copy(supervisionGetCmdData, 2, tmp, 0, tmp.Length);
                                    multiChannelCommand.parameter = tmp;
                                }
                                sendDataAction.Data = multiChannelCommand;
                                ret = sendDataAction;
                            }
                            else
                            {
                                var substitutedData = new COMMAND_CLASS_SUPERVISION.SUPERVISION_GET()
                                {
                                    properties1 = new COMMAND_CLASS_SUPERVISION.SUPERVISION_GET.Tproperties1()
                                    {
                                        sessionId = _sessionId++
                                    },
                                    encapsulatedCommandLength = (byte)sendDataAction.Data.Length,
                                    encapsulatedCommand = sendDataAction.Data
                                };
                                sendDataAction.Data = substitutedData;
                                ret = sendDataAction;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        private bool IsSupervisionSupported(byte nodeId)
        {
            bool ret = false;
            if (SendDataSubstitutionCallback != null)
            {
                ret = SendDataSubstitutionCallback(nodeId);
            }
            return ret;
        }
    }
}
