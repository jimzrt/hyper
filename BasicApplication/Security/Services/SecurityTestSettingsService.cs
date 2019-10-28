using System.Threading;
using ZWave.BasicApplication.Operations;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Security
{
    public class SecurityTestSettingsService : ISecurityTestSettingsService
    {
        private readonly SecurityManagerInfo _securityManagerInfo;
        private readonly bool _isInclusionProcesses;

        public SecurityTestSettingsService(SecurityManagerInfo securityManagerInfo, bool isInclusionProcesses)
        {
            _securityManagerInfo = securityManagerInfo;
            _isInclusionProcesses = isInclusionProcesses;
        }

        public void ActivateTestPropertiesForFrame(SecurityS2TestFrames testFrameType, ApiOperation apiOperation)
        {
            if (_securityManagerInfo.TestFramesS2.ContainsKey(testFrameType))
            {
                var testFrame = _securityManagerInfo.TestFramesS2[testFrameType];
                byte nodeId = 0x00;
                InvariantPeerNodeId peerNodeId;

                if (apiOperation is RequestDataOperation)
                {
                    var operation = (apiOperation as RequestDataOperation);
                    nodeId = operation.DestNodeId;
                    operation.Data = testFrame.Command ?? operation.Data;
                }
                else if (apiOperation is SendDataOperation)
                {
                    var operation = (apiOperation as SendDataOperation);
                    nodeId = operation.NodeId;
                    operation.Data = testFrame.Command ?? operation.Data;
                }
                else if (apiOperation is SendDataExOperation)
                {
                    var operation = (apiOperation as SendDataExOperation);
                    nodeId = operation.NodeId;
                    operation.Data = testFrame.Command ?? operation.Data;
                }
                else if (apiOperation is SendDataBridgeOperation)
                {
                    var operation = (apiOperation as SendDataBridgeOperation);
                    nodeId = operation.DestNodeId;
                    operation.Data = testFrame.Command ?? operation.Data;
                }
                peerNodeId = new InvariantPeerNodeId(_securityManagerInfo.Network.NodeId, nodeId);

                if (testFrame.IsEncryptedSpecified)
                {
                    if (testFrame.IsEncrypted)
                    {
                        apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.DenySecurity);
                        if (testFrame.IsTemp)
                        {
                            if (testFrame.NetworkKey != null)
                            {
                                _securityManagerInfo.ActivateNetworkKeyS2CustomForNode(peerNodeId, testFrame.IsTemp, testFrame.NetworkKey);
                            }
                            else
                            {
                                _securityManagerInfo.ActivateNetworkKeyS2CustomForNode(peerNodeId, testFrame.IsTemp, _securityManagerInfo.GetActualNetworkKeyS2Temp());
                            }
                        }
                        else
                        {
                            if (testFrame.NetworkKey != null)
                            {
                                _securityManagerInfo.ActivateNetworkKeyS2CustomForNode(peerNodeId, testFrame.IsTemp, testFrame.NetworkKey);
                            }
                        }
                    }
                    else
                    {
                        apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
                    }
                }
                if (testFrame.IsMulticastSpecified)
                {
                    if (testFrame.IsMulticast)
                    {
                        apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.DenyMulticast);
                        apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.UseMulticast);
                        if (_isInclusionProcesses)
                        {
                            apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.UseFollowup);
                            apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.DenyFollowup);
                        }
                    }
                    else
                    {
                        apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.UseMulticast);
                        apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.DenyMulticast);
                    }
                }
                if (testFrame.IsBroadcastSpecified)
                {
                    if (testFrame.IsBroadcast)
                    {
                        apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.DenyBroadcast);
                        apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.UseBroadcast);
                        if (_isInclusionProcesses)
                        {
                            apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.UseFollowup);
                            apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.DenyFollowup);
                        }
                    }
                    else
                    {
                        apiOperation.SubstituteSettings.ClearFlag(SubstituteFlags.UseBroadcast);
                        apiOperation.SubstituteSettings.SetFlag(SubstituteFlags.DenyBroadcast);
                    }
                }
                if (testFrame.DelaySpecified)
                {
                    Thread.Sleep(testFrame.Delay);
                };
            }
        }
    }
}
