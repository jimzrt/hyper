using System.Collections.Generic;
using System.Text;
using Utils;
using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class MultiChannelSupport : ApiAchOperation
    {
        private const byte END_POINTS_COUNT = 3;
        private const byte GENERIC_DEVICE_CLASS = 0x18; //0x10 TODO:check
        private const byte SPECIFIC_DEVICE_CLASS = 0x01;
        private byte[] ENDPOINTS_COMMAND_CLASSES = new byte[]
        {
            COMMAND_CLASS_SWITCH_BINARY.ID,
            COMMAND_CLASS_ASSOCIATION.ID,
            COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ID,
            COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID,
            COMMAND_CLASS_NOTIFICATION_V8.ID,
            COMMAND_CLASS_SUPERVISION.ID,
        };

        private byte _groupId { get; set; }
        private string _groupName { get; set; }
        private List<byte>[] _associatedNodeIds { get; set; }
        private byte _maxNodesSupported { get; set; }


        private NetworkViewPoint _network;
        public TransmitOptions TxOptions { get; set; }

        public MultiChannelSupport(NetworkViewPoint network, TransmitOptions txOptions)
            : base(0, 0, new ByteIndex(COMMAND_CLASS_MULTI_CHANNEL_V4.ID))
        {
            _groupId = 0x01;
            _groupName = "Lifeline";
            _associatedNodeIds = new List<byte>[END_POINTS_COUNT];
            for (int i = 0; i < END_POINTS_COUNT; i++)
            {
                _associatedNodeIds[i] = new List<byte>();
            }
            _maxNodesSupported = 0xE8;

            _network = network;
            TxOptions = txOptions;
        }

        //TODO:
        //MULTI_CHANNEL_END_POINT_FIND
        //MULTI_INSTANCE_CMD_ENCAP
        //MULTI_INSTANCE_GET
        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            ou.SetNextActionItems();
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] command = ReceivedAchData.Command;
            var scheme = (SecuritySchemes)ReceivedAchData.SecurityScheme;
            bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
            if (command != null && command.Length > 1 && isSuportedScheme)
            {
                if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_END_POINT_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_END_POINT_GET)command;
                    var rpt = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_END_POINT_REPORT()
                    {
                        properties1 = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_END_POINT_REPORT.Tproperties1()
                        {
                            res1 = 0x00,
                            identical = 0x01,
                            dynamic = 0x00,
                        },
                        properties2 = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_END_POINT_REPORT.Tproperties2()
                        {
                            individualEndPoints = END_POINTS_COUNT,
                            res2 = 0x00
                        },
                        properties3 = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_END_POINT_REPORT.Tproperties3()
                        {
                        }
                    };
                    var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                    ou.SetNextActionItems(sendData);
                }
                else if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CAPABILITY_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CAPABILITY_GET)command;
                    if (cmd.properties1.endPoint > 0 &&
                        cmd.properties1.endPoint <= END_POINTS_COUNT)
                    {

                        var rpt = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CAPABILITY_REPORT()
                        {
                            genericDeviceClass = GENERIC_DEVICE_CLASS,
                            specificDeviceClass = SPECIFIC_DEVICE_CLASS,
                            properties1 =
                            {
                                dynamic = 0,
                                endPoint = cmd.properties1.endPoint
                            },
                            commandClass = GetCapability(cmd.properties1.endPoint)
                        };
                        var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                        ou.SetNextActionItems(sendData);
                    }
                }
                else if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_AGGREGATED_MEMBERS_GET.ID)
                {
                    var cmd = (COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_AGGREGATED_MEMBERS_GET)command;
                    if (cmd.properties1.aggregatedEndPoint > 0 &&
                        cmd.properties1.aggregatedEndPoint <= END_POINTS_COUNT)
                    {
                        var rpt = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_AGGREGATED_MEMBERS_REPORT()
                        {
                            properties1 =
                            {
                                aggregatedEndPoint = cmd.properties1.aggregatedEndPoint,
                                res = 0
                            },
                            numberOfBitMasks = 0x01
                        };
                        rpt.aggregatedMembersBitMask.Add(0x03);
                        var sendData = new SendDataExOperation(nodeId, rpt, TxOptions, scheme);
                        ou.SetNextActionItems(sendData);
                    }
                }
                else if (command[1] == COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP.ID)
                {
                    var data = UnboxAndGetResponce(command);
                    if (data != null && data.Length > 0)
                    {
                        var sendData = new SendDataExOperation(nodeId, data, TxOptions, scheme);
                        ou.SetNextActionItems(sendData);
                    }
                }
            }
        }

        private byte[] UnboxAndGetResponce(byte[] encapedCommand)
        {
            byte[] data = null;
            var encapCmd = (COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CMD_ENCAP)encapedCommand;
            var destEPId = encapCmd.properties2.destinationEndPoint;
            if (destEPId > 0 && destEPId <= END_POINTS_COUNT)
            {
                var command = new List<byte>();
                command.Add(encapCmd.commandClass);
                command.Add(encapCmd.command);
                command.AddRange(encapCmd.parameter);

                #region Security and Capability
                if (encapCmd.commandClass == COMMAND_CLASS_SECURITY_2.ID &&
                    encapCmd.command == COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_GET.ID)
                {
                    var rpt = new COMMAND_CLASS_SECURITY_2.SECURITY_2_COMMANDS_SUPPORTED_REPORT()
                    {
                        commandClass = new List<byte>(_network.GetSecureFilteredCommandClasses(ENDPOINTS_COMMAND_CLASSES, true))
                    };
                    data = EncapData(rpt, destEPId);
                }
                else if (encapCmd.commandClass == COMMAND_CLASS_SECURITY.ID &&
                    encapCmd.command == COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_GET.ID)
                {
                    var rpt = new COMMAND_CLASS_SECURITY.SECURITY_COMMANDS_SUPPORTED_REPORT()
                    {
                        commandClassSupport = new List<byte>(_network.GetSecureFilteredCommandClasses(ENDPOINTS_COMMAND_CLASSES, true))
                    };
                    data = EncapData(rpt, destEPId);
                }
                else if (encapCmd.commandClass == COMMAND_CLASS_MULTI_CHANNEL_V4.ID &&
                    encapCmd.command == COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CAPABILITY_GET.ID)
                {
                    var rpt = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CAPABILITY_REPORT()
                    {
                        genericDeviceClass = GENERIC_DEVICE_CLASS,
                        specificDeviceClass = SPECIFIC_DEVICE_CLASS,
                        properties1 = new COMMAND_CLASS_MULTI_CHANNEL_V4.MULTI_CHANNEL_CAPABILITY_REPORT.Tproperties1()
                        {
                            endPoint = destEPId
                        },
                        commandClass = GetCapability(destEPId)
                    };
                    data = EncapData(rpt, destEPId);
                }
                #endregion

                #region MULTI CHANNEL ASSOCIATIONS
                else if (encapCmd.commandClass == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID)
                {
                    if (encapCmd.command == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GET.ID)
                    {
                        var unboxedCmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GET)command.ToArray();
                        if (unboxedCmd.groupingIdentifier == _groupId)
                        {
                            var rpt = new COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_REPORT()
                            {
                                groupingIdentifier = _groupId,
                                nodeId = _associatedNodeIds[destEPId - 1],
                                maxNodesSupported = _maxNodesSupported
                            };
                            data = EncapData(rpt, destEPId);
                        }
                    }
                    else if (encapCmd.command == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET.ID)
                    {
                        var unboxedCmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GROUPINGS_GET)command.ToArray();
                        {
                            var rpt = new COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_GROUPINGS_REPORT()
                            {
                                supportedGroupings = 0x01
                            };
                        }
                    }
                    else if (encapCmd.command == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_REMOVE.ID)
                    {
                        var unboxedCmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_REMOVE)command.ToArray();
                        if (unboxedCmd.groupingIdentifier == _groupId)
                        {
                            foreach (var associateNodeId in unboxedCmd.nodeId)
                            {
                                if (!_associatedNodeIds[destEPId - 1].Contains(associateNodeId))
                                {
                                    _associatedNodeIds[destEPId - 1].Remove(associateNodeId);
                                }
                            }
                        }
                    }
                    else if (encapCmd.command == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_SET.ID)
                    {
                        var unboxedCmd = (COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.MULTI_CHANNEL_ASSOCIATION_SET)command.ToArray();
                        if (unboxedCmd.groupingIdentifier == _groupId)
                        {
                            foreach (var associateNodeId in unboxedCmd.nodeId)
                            {
                                if (!_associatedNodeIds[destEPId - 1].Contains(associateNodeId))
                                {
                                    _associatedNodeIds[destEPId - 1].Add(associateNodeId);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Association GRP
                else if (encapCmd.commandClass == COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ID)
                {
                    if (encapCmd.command == COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_NAME_GET.ID)
                    {
                        var associationNameGetCmd = (COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_NAME_GET)command.ToArray();
                        var requestedGroupId = associationNameGetCmd.groupingIdentifier;
                        if (requestedGroupId == _groupId)
                        {
                            var rpt = new COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_NAME_REPORT()
                            {
                                groupingIdentifier = _groupId,
                                lengthOfName = (byte)Encoding.UTF8.GetByteCount(_groupName),
                                name = Encoding.UTF8.GetBytes(_groupName)
                            };
                            data = EncapData(rpt, destEPId);
                        }
                    }
                    else if (encapCmd.command == COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_INFO_GET.ID)
                    {
                        var associationInfoGetCmd = (COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_INFO_GET)command.ToArray();
                        var requestedGroupId = associationInfoGetCmd.groupingIdentifier;
                        var rpt = new COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_INFO_REPORT()
                        {
                            properties1 = new COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_INFO_REPORT.Tproperties1()
                            {
                                listMode = associationInfoGetCmd.properties1.listMode,
                                groupCount = 0x01
                            },
                            vg1 = new List<COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_INFO_REPORT.TVG1>()
                        {
                            new COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_INFO_REPORT.TVG1()
                            {
                                groupingIdentifier = _groupId,
                                mode = 0,
                                profile1 = 0,
                                profile2 = 1
                            }
                        }
                        };
                        data = EncapData(rpt, destEPId);
                    }
                    else if (encapCmd.command == COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_COMMAND_LIST_GET.ID)
                    {
                        var associationCommandListGetCmd = (COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_COMMAND_LIST_GET)command.ToArray();
                        var requestedGroupId = associationCommandListGetCmd.groupingIdentifier;
                        var rpt = new COMMAND_CLASS_ASSOCIATION_GRP_INFO_V3.ASSOCIATION_GROUP_COMMAND_LIST_REPORT()
                        {
                            groupingIdentifier = _groupId,
                            listLength = 0x02,
                            command = new List<byte>()
                        {
                            COMMAND_CLASS_BASIC.ID,
                            COMMAND_CLASS_BASIC.BASIC_GET.ID,
                        }
                        };
                        data = EncapData(rpt, destEPId);
                    }
                }
                #endregion

                #region Multichannel Association
                else //if (encapCmd.commandClass == COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION_V3.ID)
                {

                }
                #endregion
            }
            return data;
        }

        private IList<byte> GetCapability(int endpointId)
        {
            var ret = new List<byte>();
            ret.Add(COMMAND_CLASS_ZWAVEPLUS_INFO_V2.ID);
            if (endpointId != 2) // exclude secure classes for testing
            {
                if (_network.HasSecurityScheme(SecuritySchemes.S0))
                {
                    ret.Add(COMMAND_CLASS_SECURITY.ID);
                }
                if (_network.HasSecurityScheme(SecuritySchemeSet.ALLS2))
                {
                    ret.Add(COMMAND_CLASS_SECURITY_2.ID);
                }
            }
            if (!_network.HasSecurityScheme(SecuritySchemeSet.ALL))
            {
                ret.AddRange(ENDPOINTS_COMMAND_CLASSES);
            }
            else
            {
                ret.AddRange(_network.GetSecureFilteredCommandClasses(ENDPOINTS_COMMAND_CLASSES, false));
            }
            return ret;
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

    }
}
