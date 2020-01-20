using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Utils;
using Utils.UI;
using Utils.UI.MVVM;
using Utils.UI.Wrappers;
using ZWave.BasicApplication.Security;
using ZWave.Configuration;
using ZWave.Devices;
using ZWave.Enums;

namespace hyper
{
    internal class ConfigurationItem
    {
        private Collection<Node> nodeField;

        public Collection<Node> Node
        {
            get
            {
                return this.nodeField;
            }
            set
            {
                this.nodeField = value;
            }
        }

        public NetworkViewPoint Network { get; set; }
        private ObservableCollection<SelectableItem<NodeTag>> _nodes = new ObservableCollection<SelectableItem<NodeTag>>();

        [XmlIgnore]
        public ObservableCollection<SelectableItem<NodeTag>> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public void RefreshNodes()
        {
            foreach (var item in Nodes)
            {
                item.RefreshBinding();
            }
        }

        public ConfigurationItem()
        {
            //SecuritySettings = new SecuritySettings();
            //PreKitting = new PreKitting();
            Node = new Collection<Node>();
            //ViewSettings = new ViewSettings();
        }

        public ConfigurationItem(NetworkViewPoint network)
            : this()
        {
            Network = network;
        }

        public void RemoveNode(byte nodeId)
        {
            if (Nodes.Count > 0)
            {
                for (int i = Nodes.Count - 1; i >= 0; i--)
                {
                    if (Nodes[i].Item.Id == nodeId)
                    {
                        var node = Nodes[i];
                        Nodes.RemoveAt(i);
                    }
                }
            }

            if (Node.Count > 0)
            {
                for (int i = Node.Count - 1; i >= 0; i--)
                {
                    if (Node[i].NodeTag.Id == nodeId)
                    {
                        var node = Node[i];
                        Node.RemoveAt(i);
                    }
                }
            }

            //ProvisioningItem[] removePItems = this.PreKitting.ProvisioningList.Where(p => p.NodeId == nodeId).ToArray();
            //foreach (var item in removePItems)
            //{
            //    item.NodeId = 0;
            //    item.NodeIdSpecified = false;
            //    item.State = PreKittingState.Pending;
            //    PreKitting.PendingProvisioningCount++;
            //}
            //Save();
        }

        public void AddOrUpdateNode(NodeTag node)
        {
            var cnode = Node.FirstOrDefault(x => x.NodeTag == node);
            if (cnode == null)
            {
                cnode = new Node(node);
                Node.Add(cnode);
            }

            var snode = Nodes.FirstOrDefault(x => x.Item == node);
            if (snode == null)
            {
                snode = new SelectableItem<NodeTag>(node);
                Nodes.Add(snode);
            }
            else
            {
                Nodes.Remove(snode);
                Nodes.Add(snode);
            }

            cnode.NodeInfo = Network.GetNodeInfo(node);
            cnode.CommandClasses = Network.GetCommandClasses(node);
            cnode.RoleType = (byte)Network.GetRoleType(node);
            cnode.RoleTypeSpecified = Network.GetRoleType(node) != RoleTypes.None;
            cnode.IsVirtual = Network.IsVirtual(node);
            cnode.IsVirtualSpecified = Network.IsVirtual(node);
            cnode.IsWakeupIntervalSet = Network.GetWakeupInterval(node);
            cnode.IsWakeupIntervalSetSpecified = Network.GetWakeupInterval(node);
            var secureCommandClasses = Network.GetSecureCommandClasses(node);
            var schemes = Network.GetSecuritySchemes(node);
            cnode.SecurityExtension.Clear();
            if (schemes != null && schemes.Length > 0)
            {
                NetworkKeyS2Flags keysMask = NetworkKeyS2Flags.None;
                foreach (var scheme in schemes)
                {
                    switch (scheme)
                    {
                        case SecuritySchemes.NONE:
                            break;

                        case SecuritySchemes.S2_UNAUTHENTICATED:
                            keysMask |= NetworkKeyS2Flags.S2Class0;
                            break;

                        case SecuritySchemes.S2_AUTHENTICATED:
                            keysMask |= NetworkKeyS2Flags.S2Class1;
                            break;

                        case SecuritySchemes.S2_ACCESS:
                            keysMask |= NetworkKeyS2Flags.S2Class2;
                            break;

                        case SecuritySchemes.S0:
                            keysMask |= NetworkKeyS2Flags.S0;
                            break;

                        case SecuritySchemes.S2_TEMP:
                            break;

                        default:
                            break;
                    }
                }
                cnode.SecurityExtension.Add(new SecurityExtension(keysMask, secureCommandClasses));
            }

            snode.RefreshBinding();
            //Save();
        }

        public void FillNodes(byte[] nodeIds)
        {
            Nodes.Clear();
            if (nodeIds != null)
            {
                foreach (var nodeId in nodeIds)
                {
                    var nodes = FillNode(nodeId);
                    foreach (var node in nodes)
                    {
                        AddOrUpdateNode(node);
                    }
                }
            }
            for (int i = Node.Count - 1; i >= 0; i--)
            {
                if (!Nodes.Where(x => x.Item == Node[i].NodeTag).Any())
                {
                    Node.RemoveAt(i);
                }
            }
        }

        private NodeTag[] FillNode(byte nodeId)
        {
            try
            {
                var cnodes = Node.Where(x => x.Id == nodeId);
                if (cnodes.Any())
                {
                    var nodes = new List<NodeTag>();
                    foreach (var cnode in cnodes)
                    {
                        var node = cnode.NodeTag;
                        Network.SetNodeInfo(node, cnode.NodeInfo);
                        Network.SetCommandClasses(node, cnode.CommandClasses);
                        if (cnode.RoleTypeSpecified)
                        {
                            Network.SetRoleType(node, (RoleTypes)cnode.RoleType);
                        }
                        Network.SetVirtual(node, cnode.IsVirtual);
                        if (cnode.IsWakeupIntervalSetSpecified)
                        {
                            Network.SetWakeupInterval(node, cnode.IsWakeupIntervalSet);
                        }
                        if (cnode.SecurityExtension != null)
                        {
                            List<SecuritySchemes> tmpList = new List<SecuritySchemes>();
                            foreach (var scheme in cnode.SecurityExtension)
                            {
                                if (scheme.KeysValue.HasFlag(NetworkKeyS2Flags.S0))
                                {
                                    tmpList.Add(SecuritySchemes.S0);
                                }
                                if (scheme.KeysValue.HasFlag(NetworkKeyS2Flags.S2Class0))
                                {
                                    tmpList.Add(SecuritySchemes.S2_UNAUTHENTICATED);
                                }
                                if (scheme.KeysValue.HasFlag(NetworkKeyS2Flags.S2Class1))
                                {
                                    tmpList.Add(SecuritySchemes.S2_AUTHENTICATED);
                                }
                                if (scheme.KeysValue.HasFlag(NetworkKeyS2Flags.S2Class2))
                                {
                                    tmpList.Add(SecuritySchemes.S2_ACCESS);
                                }
                            }
                            var schemes = tmpList.Distinct().ToArray();
                            if (schemes != null && schemes.Length > 0)
                            {
                                Network.SetSecuritySchemes(node, schemes);
                            }
                            else
                            {
                                Network.SetSecuritySchemes(node, null);
                            }
                            var secureCommandClasses = cnode.SecurityExtension.Select(x => x.CommandClasses);
                            if (secureCommandClasses.Any())
                            {
                                Network.SetSecureCommandClasses(node, secureCommandClasses.FirstOrDefault(x => x != null));
                            }
                        }
                        nodes.Add(node);
                    }
                    return nodes.ToArray();
                }
                else
                {
                    return new[] { new NodeTag(nodeId) };
                }
            }
            catch (Exception ex)
            {
                "Failed config {0}"._DLOG(ex.Message);
                return new[] { new NodeTag(nodeId) };
            }
        }
    }
}