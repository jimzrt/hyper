using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.CommandClasses;
using ZWave.Enums;

namespace ZWave.Devices
{
    public class NetworkViewPoint
    {
        public const int MAX_NODES = 256;
        public const int MAX_ENDPOINTS = 128;
        private byte[] _secureCommandClasses = new byte[]
        {
            COMMAND_CLASS_VERSION_V3.ID,
            COMMAND_CLASS_SWITCH_BINARY_V2.ID,
            COMMAND_CLASS_NOTIFICATION_V8.ID,
            COMMAND_CLASS_MANUFACTURER_SPECIFIC_V2.ID,
            COMMAND_CLASS_POWERLEVEL.ID,
            COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ID,
            COMMAND_CLASS_ASSOCIATION_V2.ID,
            COMMAND_CLASS_FIRMWARE_UPDATE_MD_V5.ID,
            COMMAND_CLASS_MULTI_CHANNEL_V4.ID,
            COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID,
            COMMAND_CLASS_WAKE_UP_V2.ID
        };

        public static byte[] HighSecureCommandClasses = new byte[]
        {
            COMMAND_CLASS_SECURITY.ID,
            COMMAND_CLASS_SECURITY_2.ID ,
            COMMAND_CLASS_BASIC.ID ,
            COMMAND_CLASS_SUPERVISION.ID ,
            COMMAND_CLASS_INCLUSION_CONTROLLER.ID ,
            COMMAND_CLASS_ZWAVEPLUS_INFO_V2.ID ,
            COMMAND_CLASS_CENTRAL_SCENE_V3.ID
        };
        public bool CheckIfSupportSecurityCC { get; set; }
        private byte _nodeId;
        public byte NodeId
        {
            get { return _nodeId; }
            set
            {
                if (_nodeId != value)
                {
                    ResetAndSelfRestore(_nodeId, value);
                    _nodeId = value;
                    SetEnabledSecuritySchemes();
                    Notify("Id");
                }
            }
        }

        public NodeTag NodeTag
        {
            get { return new NodeTag(_nodeId, 0); }
        }

        private byte[] _homeId;
        public byte[] HomeId
        {
            get { return _homeId; }
            set
            {
                _homeId = value;
                Notify("HomeId");
            }
        }

        public void SetFailed(NodeTag node, bool value)
        {
            Nodes[node].IsFailed = value;
        }

        public bool IsFailed(NodeTag node)
        {
            return Nodes[node].IsFailed;
        }

        public void SetVirtual(NodeTag node, bool value)
        {
            Nodes[node].IsVirtual = value;
        }

        public bool IsVirtual(NodeTag node)
        {
            return Nodes[node].IsVirtual;
        }

        public bool IsSlaveApi(NodeTag node)
        {
            return Nodes[node].IsSlaveApi;
        }

        public bool IsDeviceListening(NodeTag node)
        {
            return Nodes[node].IsForcedListening || Nodes[node].IsListening || Nodes[node].IsFlirs250ms || Nodes[node].IsFlirs1000ms;
        }

        public bool IsListening(NodeTag node)
        {
            return Nodes[node].IsListening;
        }

        public bool IsForcedListening(NodeTag node)
        {
            return Nodes[node].IsForcedListening;
        }

        public void SetForcedListening(NodeTag node, bool value)
        {
            Nodes[node].IsForcedListening = value;
        }

        public bool IsFlirs(NodeTag node)
        {
            return Nodes[node].IsFlirs;
        }

        public bool IsFlirs(byte nodeId)
        {
            return Nodes[nodeId].IsFlirs;
        }

        private byte _sucNodeId;
        public byte SucNodeId
        {
            get { return _sucNodeId; }
            set
            {
                _sucNodeId = value;
                Notify("SucNodeId");
            }
        }

        private byte _serialApiCapability;
        public byte SerialApiCapability
        {
            get { return _serialApiCapability; }
            set
            {
                _serialApiCapability = value;
                Notify("SerialApiCapability");
            }
        }

        private NodeViewPointCollection Nodes { get; set; }
        private Action<string> _propertyChanged;
        public NetworkViewPoint(Action<string> propertyChanged)
        {
            _propertyChanged = propertyChanged;
            Nodes = new NodeViewPointCollection(MAX_NODES, MAX_ENDPOINTS);
        }

        public void Notify(string name)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(name);
            }
        }

        public void SetWakeupInterval(byte node, bool value)
        {
            Nodes[node].WakeupInterval = value;
        }

        public bool GetWakeupInterval(byte node)
        {
            return Nodes[node].WakeupInterval;
        }

        public void SetRoleType(NodeTag node, RoleTypes roleType)
        {
            Nodes[node].RoleType = roleType;
        }

        public RoleTypes GetRoleType(NodeTag node)
        {
            return Nodes[node].RoleType;
        }

        private void Reset()
        {
            Nodes.Reset();
        }

        private void ResetAndSelfRestore(byte fromNode, byte toNode)
        {
            var fromNodeTag = new NodeTag(fromNode);
            var toNodeTag = new NodeTag(toNode);
            var nInfo = GetNodeInfo(fromNodeTag);
            var cmdClasses = GetCommandClasses(fromNodeTag);
            Reset();
            SetNodeInfo(toNodeTag, nInfo);
            SetCommandClasses(toNodeTag, cmdClasses);
        }

        public void ResetAndEnableAndSelfRestore()
        {
            ResetAndSelfRestore(NodeId, NodeId);
            SetEnabledSecuritySchemes();
        }

        private void SetEnabledSecuritySchemes()
        {
            List<SecuritySchemes> tmp = new List<SecuritySchemes>();
            if (IsEnabledS0)
            {
                tmp.Add(SecuritySchemes.S0);
            }
            if (IsEnabledS2_UNAUTHENTICATED)
            {
                tmp.Add(SecuritySchemes.S2_UNAUTHENTICATED);
            }
            if (IsEnabledS2_AUTHENTICATED)
            {
                tmp.Add(SecuritySchemes.S2_AUTHENTICATED);
            }
            if (IsEnabledS2_ACCESS)
            {
                tmp.Add(SecuritySchemes.S2_ACCESS);
            }
            SetSecuritySchemes(tmp.ToArray());
        }

        public void SetCurrentSecurityScheme(byte nodeId, SecuritySchemes scheme)
        {
            Nodes[nodeId].SetCurrentSecurityScheme(scheme);
        }

        public void ResetCurrentSecurityScheme(byte nodeId)
        {
            Nodes[nodeId].ResetCurrentSecurityScheme();
        }

        public void ResetCurrentSecurityScheme()
        {
            Nodes.ResetCurrentSecurityScheme();
        }

        public bool IsSecuritySchemesSpecified(byte nodeId)
        {
            return Nodes[nodeId].SecuritySchemesSpecified;
        }

        public bool HasSecurityScheme(byte nodeId, SecuritySchemes[] schemes)
        {
            bool ret = false;
            if (schemes != null)
            {
                foreach (var scheme in schemes)
                {
                    if (HasSecurityScheme(nodeId, scheme))
                    {
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }

        public bool HasSecurityScheme(SecuritySchemes[] schemes)
        {
            return HasSecurityScheme(NodeId, schemes);
        }

        public bool HasSecurityScheme(SecuritySchemes scheme)
        {
            return HasSecurityScheme(NodeId, scheme);
        }

        public bool HasSecurityScheme(byte nodeId, SecuritySchemes scheme)
        {
            bool ret =
                IsSecuritySchemeEnabled(scheme) &&
                Nodes[NodeTag].HasSecurityScheme(scheme) &&
                Nodes[nodeId].HasSecurityScheme(scheme);
            return ret;
        }

        public void ResetSecuritySchemes()
        {
            Nodes.ResetSecuritySchemes();
        }

        public void ResetSecuritySchemes(byte nodeId)
        {
            Nodes[nodeId].ResetSecuritySchemes();
        }

        public void SetCommandClasses(byte[] cmdClasses)
        {
            SetCommandClasses(NodeTag, cmdClasses);
        }

        public void SetCommandClasses(NodeTag node, byte[] cmdClasses)
        {
            Nodes[node].SetCommandClasses(cmdClasses);
        }

        public void SetSecuritySchemesSpecified(byte nodeId)
        {
            Nodes[nodeId].SecuritySchemesSpecified = true;
        }

        public void SetNodeInfo(NodeInfo nodeInfo)
        {
            SetNodeInfo(NodeTag, nodeInfo);
        }

        public void SetNodeInfo(NodeTag node, NodeInfo nodeInfo)
        {
            Nodes[node].NodeInfo = nodeInfo;
        }

        public void SetNodeInfo(NodeTag node, byte generic, byte specific)
        {
            var parentNodeInfo = Nodes[node.Parent].NodeInfo;
            Nodes[node].NodeInfo = NodeInfo.
                UpdateTo(Nodes[node].NodeInfo, parentNodeInfo.Basic, generic, specific);
        }

        public void SetNodeInfo(NodeTag node, byte basic, byte generic, byte specific)
        {
            Nodes[node].NodeInfo = NodeInfo.UpdateTo(Nodes[node].NodeInfo, basic, generic, specific);
        }

        public void SetSecuritySchemes(SecuritySchemes[] schemes)
        {
            SetSecuritySchemes(NodeId, schemes);
        }

        public void SetSecuritySchemes(byte nodeId, SecuritySchemes[] schemes)
        {
            Nodes[nodeId].SetSecuritySchemes(schemes);
        }

        public byte[] GetSecureCommandClasses()
        {
            return GetSecureCommandClasses(NodeTag);
        }

        public byte[] GetSecureFilteredCommandClasses(byte[] commandClasses, bool securePart)
        {
            if (commandClasses != null)
            {
                if (securePart)
                {
                    return _secureCommandClasses.Where(x => commandClasses.Contains(x)).ToArray();
                }
                else
                {
                    return commandClasses.Where(x => !_secureCommandClasses.Contains(x)).ToArray();
                }
            }
            else
            {
                return null;
            }
        }

        public byte[] GetSecureCommandClasses(NodeTag node)
        {
            if (node == NodeTag)
            {
                var commandClasses = GetCommandClasses(node);
                if (commandClasses != null)
                {
                    //Shouldn't we return the Command Classes that are secured for a specific device? Not only the ones defined on _secureCommandClasses ....
                    return _secureCommandClasses.Where(x => commandClasses.Contains(x)).ToArray();
                    //return commandClasses;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return Nodes[node].GetSecureCommandClasses();
            }
        }

        public NodeInfo GetNodeInfo()
        {
            return GetNodeInfo(NodeTag);
        }

        public NodeInfo GetNodeInfo(NodeTag node)
        {
            return Nodes[node].NodeInfo;
        }

        public SecuritySchemes[] GetSecuritySchemes()
        {
            return GetSecuritySchemes(NodeId);
        }

        public SecuritySchemes[] GetSecuritySchemes(byte nodeId)
        {
            if (_isEnabledS0 || _isEnabledS2_UNAUTHENTICATED || _isEnabledS2_AUTHENTICATED || _isEnabledS2_ACCESS)
            {
                var enabledSchemes = new List<SecuritySchemes>();
                var schemes = Nodes[nodeId].GetSecuritySchemes();
                if (schemes != null)
                {
                    foreach (var scheme in schemes)
                    {
                        if (IsSecuritySchemeEnabled(scheme))
                        {
                            enabledSchemes.Add(scheme);
                        }
                    }
                    return enabledSchemes.ToArray();
                }
                else
                {
                    return null;

                }
            }
            else
            {
                return null;
            }
        }

        public SecuritySchemes GetCurrentSecurityScheme(byte nodeId)
        {
            return Nodes[nodeId].GetCurrentSecurityScheme();
        }

        public bool HasSecureCommandClass(byte cmdClass)
        {
            return HasSecureCommandClass(NodeTag, cmdClass);
        }

        public bool HasSecureCommandClass(NodeTag node, byte cmdClass)
        {
            if (Nodes[(byte)node].GetSecuritySchemes() == null)
            {
                return false;
            }
            else
            {
                if (node == NodeTag)
                {
                    var commandClasses = GetCommandClasses(node);
                    if (commandClasses != null)
                    {
                        return _secureCommandClasses.Contains(cmdClass) && commandClasses.Contains(cmdClass);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return Nodes[node].HasSecureCommandClass(cmdClass);
                }
            }
        }

        public bool HasNetworkAwareCommandClass(byte cmdClass)
        {
            return GetNetworkAwareCommandClasses().Contains(cmdClass);
        }

        public bool HasCommandClass(byte cmdClass)
        {
            return HasCommandClass(NodeTag, cmdClass);
        }

        public bool HasCommandClass(NodeTag node, byte cmdClass)
        {
            return Nodes[node].HasCommandClass(cmdClass) || HasSecureCommandClass(node, cmdClass);
        }

        public byte[] GetNetworkAwareCommandClasses()
        {
            var commandClasses = GetCommandClasses(NodeTag);
            if (NodeId > 1 && commandClasses != null && (commandClasses.Contains(COMMAND_CLASS_SECURITY.ID) || commandClasses.Contains(COMMAND_CLASS_SECURITY_2.ID)))
            {
                return commandClasses.Where(x => !_secureCommandClasses.Contains(x)).ToArray();
            }
            else
            {
                return commandClasses;
            }
        }

        public byte[] GetCommandClasses(NodeTag node)
        {
            var cc = Nodes[node].GetCommandClasses();
            //"@@@@@= {0}:{1}"._DLOG(node, cc.GetHex());
            return cc;
        }

        private byte[] _commandClassesSecureVirtual;
        public void SetCommandClassesSecureVirtual(byte[] cmdClasses)
        {
            _commandClassesSecureVirtual = cmdClasses;
        }

        public byte[] GetVirtualSecureCommandClasses()
        {
            return _commandClassesSecureVirtual;
        }

        public bool IsSecuritySchemeEnabled(SecuritySchemes scheme)
        {
            bool ret = false;
            switch (scheme)
            {
                case SecuritySchemes.NONE:
                    ret = true;
                    break;
                case SecuritySchemes.S2_UNAUTHENTICATED:
                    ret = IsEnabledS2_UNAUTHENTICATED;
                    break;
                case SecuritySchemes.S2_AUTHENTICATED:
                    ret = IsEnabledS2_AUTHENTICATED;
                    break;
                case SecuritySchemes.S2_ACCESS:
                    ret = IsEnabledS2_ACCESS;
                    break;
                case SecuritySchemes.S0:
                    ret = IsEnabledS0;
                    break;
                case SecuritySchemes.S2_TEMP:
                    ret = true;
                    break;
                default:
                    break;
            }
            return ret;
        }

        private void AddSecurityScheme(SecuritySchemes addScheme)
        {
            var schemes = GetSecuritySchemes();
            if (schemes != null)
            {
                SetSecuritySchemes(schemes.Union(new[] { addScheme }).ToArray());
            }
            else
            {
                SetSecuritySchemes(new[] { addScheme });
            }
        }

        private bool _isCsaEnabled;
        public bool IsCsaEnabled
        {
            get { return _isCsaEnabled; }
            set { _isCsaEnabled = value; }
        }

        public byte[] HasCommandClass(object iD)
        {
            throw new NotImplementedException();
        }

        private bool _isEnabledS0;
        public bool IsEnabledS0
        {
            get { return _isEnabledS0; }
            set
            {
                if (value && !Nodes[NodeTag].HasSecurityScheme(SecuritySchemes.S0))
                {
                    AddSecurityScheme(SecuritySchemes.S0);
                }
                if (_isEnabledS0 != value && EnableSecuritySchemeSettingsChanged != null)
                {
                    EnableSecuritySchemeSettingsChanged(SecuritySchemes.S0, value);
                }
                _isEnabledS0 = value;
            }
        }

        public event Action<SecuritySchemes, bool> EnableSecuritySchemeSettingsChanged;

        private bool _isEnabledS2_UNAUTHENTICATED;
        public bool IsEnabledS2_UNAUTHENTICATED
        {
            get { return _isEnabledS2_UNAUTHENTICATED; }
            set
            {
                if (value && !Nodes[NodeTag].HasSecurityScheme(SecuritySchemes.S2_UNAUTHENTICATED))
                {
                    AddSecurityScheme(SecuritySchemes.S2_UNAUTHENTICATED);
                }
                if (_isEnabledS2_UNAUTHENTICATED != value && EnableSecuritySchemeSettingsChanged != null)
                {
                    EnableSecuritySchemeSettingsChanged(SecuritySchemes.S2_UNAUTHENTICATED, value);
                }
                _isEnabledS2_UNAUTHENTICATED = value;
            }
        }

        private bool _isEnabledS2_AUTHENTICATED;
        public bool IsEnabledS2_AUTHENTICATED
        {
            get { return _isEnabledS2_AUTHENTICATED; }
            set
            {
                if (value && !Nodes[NodeTag].HasSecurityScheme(SecuritySchemes.S2_AUTHENTICATED))
                {
                    AddSecurityScheme(SecuritySchemes.S2_AUTHENTICATED);
                }
                if (_isEnabledS2_AUTHENTICATED != value && EnableSecuritySchemeSettingsChanged != null)
                {
                    EnableSecuritySchemeSettingsChanged(SecuritySchemes.S2_AUTHENTICATED, value);
                }
                _isEnabledS2_AUTHENTICATED = value;
            }
        }

        private bool _isEnabledS2_ACCESS;
        public bool IsEnabledS2_ACCESS
        {
            get { return _isEnabledS2_ACCESS; }
            set
            {
                if (value && !Nodes[NodeTag].HasSecurityScheme(SecuritySchemes.S2_ACCESS))
                {
                    AddSecurityScheme(SecuritySchemes.S2_ACCESS);
                }
                if (_isEnabledS2_ACCESS != value && EnableSecuritySchemeSettingsChanged != null)
                {
                    EnableSecuritySchemeSettingsChanged(SecuritySchemes.S2_ACCESS, value);
                }
                _isEnabledS2_ACCESS = value;
            }
        }

        public void SetSecureCommandClasses(NodeTag node, byte[] cmdClasses)
        {
            Nodes[node].SetSecureCommandClasses(cmdClasses);
        }

        public int GetTimeoutValue(NodeTag[] nodeIds, bool IsInclusion)
        {
            //Timeout Constants ms
            const int BASIC = 65000;
            const int LISTENING = 217;
            const int FLIRS = 3517;
            const int NETWRORK = 732;

            int ret = BASIC;
            if (nodeIds != null && nodeIds.Length > 0)
            {
                int listenningNodesCount = 0;
                int flirsNodesCount = 0;
                int networkNodesCount = 0;
                foreach (var nodeId in nodeIds)
                {
                    if (!nodeId.Equals(NodeId))
                    {
                        if (Nodes[nodeId].IsListening)
                        {
                            listenningNodesCount++;
                        }
                        if (Nodes[nodeId].IsFlirs)
                        {
                            flirsNodesCount++;
                        }
                        networkNodesCount++;
                    }
                }
                ret = BASIC +
                    listenningNodesCount * LISTENING +
                    flirsNodesCount * FLIRS +
                    networkNodesCount * NETWRORK;
            }
            return ret;
        }

        private bool _isBridgeController;
        public bool IsBridgeController
        {
            get { return _isBridgeController; }
            set
            {
                _isBridgeController = value;
            }
        }

    }
}
