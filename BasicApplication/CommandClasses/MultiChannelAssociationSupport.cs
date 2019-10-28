using System.Collections.Generic;
using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class MultiChannelAssociationSupport : ApiAchOperation
    {
        private byte _groupId { get; set; }
        private string _groupName { get; set; }
        private List<byte> _associatedNodeIds { get; set; }
        private byte _maxNodesSupported { get; set; }

        private readonly NetworkViewPoint _network;
        public TransmitOptions TxOptions { get; set; }

        public MultiChannelAssociationSupport(NetworkViewPoint network, TransmitOptions txOptions)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID))
        {
            _groupId = 0x01;
            _groupName = "Lifeline";
            _associatedNodeIds = new List<byte>();
            _maxNodesSupported = 0xE8;

            _network = network;
            TxOptions = txOptions;
        }

        private byte[] EncapData(byte[] data, byte destinationEndPoint)
        {
            if (destinationEndPoint > 0)
            {
                COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP multiChannelCmd = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP();
                multiChannelCmd.commandClass = data[0];
                multiChannelCmd.command = data[1];
                multiChannelCmd.parameter = new List<byte>();
                for (int i = 2; i < data.Length; i++)
                {
                    multiChannelCmd.parameter.Add(data[i]);
                }
                multiChannelCmd.properties1.res = 0;
                multiChannelCmd.properties1.sourceEndPoint = 0;
                multiChannelCmd.properties2.bitAddress = 0;
                multiChannelCmd.properties2.destinationEndPoint = destinationEndPoint;
                data = multiChannelCmd;
            }
            return data;
        }

        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] command = ReceivedAchData.Command;
            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
            if (command != null && command.Length > 1 && isSuportedScheme)
            {
                if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GET)command;
                    if (cmd.groupingIdentifier == _groupId)
                    {
                        var rpt = new COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_REPORT()
                        {
                            groupingIdentifier = _groupId,
                            nodeId = _associatedNodeIds,
                            maxNodesSupported = _maxNodesSupported
                        };
                        var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                        ou.SetNextActionItems(sendData);
                    }
                }
                else if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET)command;
                    {
                        var rpt = new COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT()
                        {
                            supportedGroupings = 0x01
                        };
                        var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                        ou.SetNextActionItems(sendData);
                    }
                }
                else if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_REMOVE.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_REMOVE)command;
                    if (cmd.groupingIdentifier == _groupId)
                    {
                        foreach (var associateNodeId in cmd.nodeId)
                        {
                            if (!_associatedNodeIds.Contains(associateNodeId))
                            {
                                _associatedNodeIds.Remove(associateNodeId);
                            }
                        }
                    }
                }
                else if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_SET.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_SET)command;
                    if (cmd.groupingIdentifier == _groupId)
                    {
                        foreach (var associateNodeId in cmd.nodeId)
                        {
                            if (!_associatedNodeIds.Contains(associateNodeId))
                            {
                                _associatedNodeIds.Add(associateNodeId);
                            }
                        }
                    }
                }

            }
        }

    }
}
