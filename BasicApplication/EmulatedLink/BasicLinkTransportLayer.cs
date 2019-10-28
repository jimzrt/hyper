using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;
using ZWave.Layers;

namespace ZWave.BasicApplication.EmulatedLink
{
    public class BasicLinkTransportLayer : ZWave.Layers.Transport.TransportLayer
    {
        private const byte rssiVal = 0x7e; // RSSI_MAX_POWER_SATURATED
        private readonly Dictionary<byte, BlockingCollection<byte[]>> _pool = new Dictionary<byte, BlockingCollection<byte[]>>();
        private readonly Dictionary<byte, BasicLinkModuleMemory> _modules = new Dictionary<byte, BasicLinkModuleMemory>();

        public override ITransportListener Listener { get; set; }
        private const int MAX_FRAME_SIZE = 54;
        public int? MaxFrameSize { get; set; }
        private int ActualMaxFrameSize
        {
            get { return MaxFrameSize ?? MAX_FRAME_SIZE; }
        }

        public override ITransportClient CreateClient(byte sessionId)
        {
            BasicLinkTransportClient ret = new BasicLinkTransportClient(TransmitCallback, WriteData, ReadData, UnregisterModule)
            {
                SuppressDebugOutput = SuppressDebugOutput,
                SessionId = sessionId
            };
            RegisterModule(sessionId, new BasicLinkModuleMemory(0x01));
            return ret;
        }

        private void UnregisterModule(byte sessionId)
        {
            _modules.Remove(sessionId);
            if (_pool.ContainsKey(sessionId))
            {
                _pool[sessionId].Dispose();
                _pool.Remove(sessionId);
            }
        }

        private void RegisterModule(byte sessionId, BasicLinkModuleMemory module)
        {
            _modules.Add(sessionId, module);
            _pool.Add(sessionId, new BlockingCollection<byte[]>());
        }

        public void SetUpModulesNetwork(byte sessionId, params byte[] joinSessionIds)
        {
            foreach (var joinSessionId in joinSessionIds)
            {
                _modules[joinSessionId].NodeId = _modules[sessionId].SeedNextNodeId();
                _modules[joinSessionId].HomeId = _modules[sessionId].HomeId;
            }
        }

        #region Helpers

        private byte[] ComposeAddNodeClientResponse(byte status, byte funcId, byte nodeToIncludeId, byte[] nodeSupportedCC)
        {
            var supportedCC = nodeSupportedCC ?? new byte[0];
            byte bLen = (byte)(3 /*basic | generic | specific length*/ + supportedCC.Length);
            return CreateFrame(new byte[]
            {
                (byte)FrameTypes.Request,
                (byte)CommandTypes.CmdZWaveAddNodeToNetwork,
                funcId,
                status,
                nodeToIncludeId,
                bLen,
                1, 2, 1 // basic | generic | specific
            }.Concat(supportedCC));
        }

        private byte[] CreateFrame(IEnumerable<byte> data)
        {
            byte[] ret = new byte[data.Count() + 3];
            var index = 0;
            ret[index++] = (byte)HeaderTypes.StartOfFrame;
            ret[index++] = (byte)(data.Count() + 1);
            foreach (var b in data)
            {
                ret[index++] = b;
            }
            ret[index] = CalculateChecksum(data);
            return ret;
        }

        private byte CalculateChecksum(IEnumerable<byte> data)
        {
            byte calcChksum = 0xFF;
            calcChksum ^= (byte)(data.Count() + 1); // Length
            foreach (var b in data)
            {
                calcChksum ^= b; // Data
            }
            return calcChksum;
        }

        private byte[] GetFuncIdSupportedBitmask(byte sessionId)
        {
            byte[] ret = new byte[32];
            var sapList = new List<CommandTypes>(_supported);
            sapList.Sort();
            foreach (var item in sapList)
            {
                var val = (byte)item - 1; // started from 1
                var index = val / 8;
                var bit = val % 8;
                ret[index] = (byte)(ret[index] ^ (1 << bit));
            }
            return ret;
        }

        #endregion

        #region Direct Link

        private int ReadData(byte sessionId, byte[] buffer, CancellationToken ctoken)
        {
            var data = _pool[sessionId].Take(ctoken);
            Array.Copy(data, buffer, data.Length);
            return data.Length;
        }

        private readonly CommandTypes[] _supported = new[]
       {
            CommandTypes.CmdSerialApiGetCapabilities,
            CommandTypes.CmdZWaveAddNodeToNetwork,
            CommandTypes.CmdZWaveSetLearnMode,
            CommandTypes.CmdZWaveSendData,
            CommandTypes.CmdZWaveSendDataAbort,
            CommandTypes.CmdMemoryGetId,
            CommandTypes.CmdSerialApiGetInitData,
            CommandTypes.CmdSerialApiApplNodeInformation,
            CommandTypes.CmdZWaveRequestNodeInfo,
            CommandTypes.CmdZWaveGetSucNodeId,
            CommandTypes.CmdZWaveSendDataMulti,
            CommandTypes.CmdZWaveSetDefault,
            CommandTypes.CmdZWaveSetRFReceiveMode,
            CommandTypes.CmdZWaveGetVersion,
            CommandTypes.CmdZWaveNVRGetValue,
            CommandTypes.CmdSerialApiSoftReset,
            CommandTypes.CmdZWaveRemoveNodeFromNetwork,
            CommandTypes.CmdSerialApiSetup,
            CommandTypes.CmdZWaveSetSucNodeId,
            CommandTypes.CmdZWaveIsFailedNode,
            CommandTypes.CmdZWaveRequestNodeNeighborUpdate
        };

        private int WriteData(byte sessionId, byte[] data)
        {
            if (data != null && data.Length > 1)
            {
                _pool[sessionId].Add(new byte[] { 0x06 });
                if (data[2] == (byte)FrameTypes.Request)
                {
                    switch ((CommandTypes)data[3])
                    {
                        case CommandTypes.CmdSerialApiGetCapabilities:
                            CmdSerialApiGetCapabilities(sessionId);
                            break;
                        case CommandTypes.CmdZWaveAddNodeToNetwork:
                            CmdZWaveAddNodeToNetwork(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveSetLearnMode:
                            CmdZWaveSetLearnMode(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveSendData:
                            CmdZWaveSendData(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveSendDataAbort:
                            CmdZWaveSendDataAbort(sessionId);
                            break;
                        case CommandTypes.CmdMemoryGetId:
                            CmdMemoryGetId(sessionId);
                            break;
                        case CommandTypes.CmdSerialApiGetInitData:
                            CmdSerialApiGetInitData(sessionId);
                            break;
                        case CommandTypes.CmdSerialApiApplNodeInformation:
                            CmdSerialApiApplNodeInformation(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveRequestNodeInfo:
                            CmdZWaveRequestNodeInfo(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveGetSucNodeId:
                            CmdZWaveGetSucNodeId(sessionId);
                            break;
                        case CommandTypes.CmdZWaveSendDataMulti:
                            CmdZWaveSendDataMulti(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveSetDefault:
                            CmdZWaveSetDefault(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveSetRFReceiveMode:
                            CmdZWaveSetRFReceiveMode(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveGetVersion:
                            CmdZWaveGetVersion(sessionId);
                            break;
                        case CommandTypes.CmdZWaveNVRGetValue:
                            CmdZWaveNVRGetValue(sessionId, data);
                            break;
                        case CommandTypes.CmdSerialApiSoftReset:
                            CmdSerialApiSoftReset(sessionId);
                            break;
                        case CommandTypes.CmdZWaveRemoveNodeFromNetwork:
                            CmdZWaveRemoveNodeFromNetwork(sessionId, data);
                            break;
                        case CommandTypes.CmdSerialApiSetup:
                            CmdSerialApiSetup(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveSetSucNodeId:
                            CmdZWaveSetSucNodeId(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveIsFailedNode:
                            CmdZWaveIsFailedNode(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveRequestNodeNeighborUpdate:
                            CmdZWaveRequestNodeNeighborUpdate(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveRemoveFailedNodeId:
                            CmdZWaveRemoveFailedNodeId(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveReplaceFailedNode:
                            CmdZWaveReplaceFailedNode(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveExploreRequestInclusion:
                            //CmdZWaveExploreRequestInclusion(sessionId, data);
                            break;
                        case CommandTypes.CmdZWaveControllerChange:
                            //CmdZWaveControllerChange(sessionId, data);
                            break;

                        default:
                            throw new NotImplementedException("CommandType: " + (CommandTypes)data[3]);
                    }
                }
                return data.Length;
            }
            else
            {
                return 0;
            }
        }

        private void CmdZWaveRemoveFailedNodeId(byte sessionId, byte[] data)
        {
            // HOST->ZW: REQ | 0x61 | nodeID | funcID
            // ZW->HOST: RES | 0x61 | retVal
            // ZW->HOST: REQ | 0x61 | funcID | txStatus
            _pool[sessionId].Add(CreateFrame(new byte[]
              {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdZWaveRemoveFailedNodeId,
                    (byte)FailedNodeRetValues.ZW_FAILED_NODE_REMOVE_STARTED
              }));

            _pool[sessionId].Add(CreateFrame(new byte[]
             {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveRemoveFailedNodeId,
                    data[5],
                    (byte)FailedNodeStatuses.ZW_FAILED_NODE_REMOVED
             }));
        }

        private void CmdSerialApiSoftReset(byte sessionId)
        {

        }

        private void CmdSerialApiSetup(byte sessionId, byte[] data)
        {
            // HOST->ZW: REQ | 0x0B | 0x02 | bEnable
            // ZW->HOST: RES | 0x0B | 0x02 | RetVal
            _pool[sessionId].Add(CreateFrame(new byte[]
               {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdSerialApiSetup,
                    0x01
               }));
        }

        private void CmdZWaveIsFailedNode(byte sessionId, byte[] data)
        {
            // HOST->ZW: REQ | 0x62 | nodeID
            // ZW->HOST: RES | 0x62 | retVal
            var target = _modules.Where(x => x.Value.IsRFReceiveMode && x.Value.NodeId == data[4] && x.Value.HomeId.SequenceEqual(_modules[sessionId].HomeId));
            _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdZWaveIsFailedNode,
                    (byte)(target.Any()?0:1)
                }));
        }

        private void CmdZWaveRequestNodeNeighborUpdate(byte sessionId, byte[] data)
        {
            // HOST->ZW: REQ | 0x48 | nodeID | funcID
            // ZW->HOST: REQ | 0x48 | funcID | bStatus
            _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveRequestNodeNeighborUpdate,
                    data[5],
                    (byte)RequestNeighborUpdateStatuses.Started
                }));

            _pool[sessionId].Add(CreateFrame(new byte[]
               {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveRequestNodeNeighborUpdate,
                    data[5],
                    (byte)RequestNeighborUpdateStatuses.Done
               }));
        }

        private void CmdZWaveSetSucNodeId(byte sessionId, byte[] data)
        {
            // HOST->ZW: REQ | 0x54 | nodeID | SUCState | bTxOption | capabilities | funcID
            // ZW->HOST: RES | 0x54 | RetVal
            // ZW->HOST: REQ | 0x54 | funcID | txStatus
            _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdZWaveSetSucNodeId,
                    0x01
                }));

            _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveSetSucNodeId,
                    data[8],
                    (byte)SetSucReturnValues.SucSetSucceeded
                }));

            var network = _modules.Where(x => x.Value.HomeId.SequenceEqual(_modules[sessionId].HomeId));
            foreach (var item in network)
            {
                if (data[5] > 0)
                {
                    item.Value.SucNodeId = data[4];
                }
                else
                {
                    item.Value.SucNodeId = 0;
                }
            }
        }

        private void CmdZWaveNVRGetValue(byte sessionId, byte[] data)
        {
            // HOST->ZW: REQ | 0x28 | offset | length
            // ZW->HOST: RES | 0x28 | NVRdata[]

            byte[] prk = "8DDD34AC7B136AFB666711FD05F4FEAD8A6D01685F4E49160C704A9D38BB9641".GetBytes();
            byte[] pub = "9DE45ED3AE44DC54FB54ED8DB93FB399D48FF461BC901C6AEEB94DFCE4EF7C59".GetBytes();

            byte[] nvr = new byte[256];
            Array.Copy(pub, 0, nvr, 35, 32);
            Array.Copy(pub, 0, nvr, 35 + 32, 32);

            var offset = data[4];
            var length = data[5];

            var nvrData = nvr.Skip(offset).Take(length).ToArray();

            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdZWaveNVRGetValue
            }.
            Concat(nvrData)));
        }

        private void CmdZWaveGetVersion(byte sessionId)
        {
            // HOST->ZW: REQ | 0x15
            // ZW->HOST: RES | 0x15 | buffer(12 bytes) | library type
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdZWaveGetVersion
            }.
            Concat(new byte[12]).
            Concat(new byte[] { (byte)Libraries.ControllerStaticLib })));
        }

        private void CmdSerialApiGetCapabilities(byte sessionId)
        {
            // HOST->ZW: REQ | 0x07
            // ZW->HOST: RES | 0x07 | version | revision | mfrId1 | mfrId2 | mfrProdType1 | mfrProdType2 | mfrProdId1 | mfrProdId2 | funcidSupportedBitmask[]
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdSerialApiGetCapabilities,
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08
            }.Concat(GetFuncIdSupportedBitmask(sessionId))));
        }

        private void CmdZWaveSetRFReceiveMode(byte sessionId, byte[] data)
        {
            byte mode = data[4];
            _modules[sessionId].IsRFReceiveMode = mode > 0;
            // HOST->ZW: REQ | 0x10 | mode
            // ZW->HOST: RES | 0x10 | retVal            
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdZWaveSetRFReceiveMode,
                mode
            }));
        }

        private void CmdZWaveSendDataMulti(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;

            // HOST->ZW: REQ | 0x14 | numberNodes | pNodeIDList[ ] | dataLength | pData[ ] | txOptions | funcID
            if (data.Length > ActualMaxFrameSize)
            {
                // Response fail to client.
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdZWaveSendDataMulti,
                    0x00
                }));
            }
            else
            {
                // Response to client.
                var srcNodeId = _modules[sessionId].NodeId;
                int index = 4; // numberNodes
                byte numberNodes = data[index];
                var nodes = new byte[numberNodes];
                Array.Copy(data, index + 1, nodes, 0, numberNodes);
                index += numberNodes + 1; // dataLength
                byte dataLength = data[index];
                var cmdData = new byte[dataLength];
                Array.Copy(data, index + 1, cmdData, 0, dataLength);
                index += dataLength + 1 + 1/*txOptions*/;
                byte funcId = data[index];

                // ZW->HOST: RES | 0x14 | RetVal
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                                    (byte)FrameTypes.Response,
                                    (byte)CommandTypes.CmdZWaveSendDataMulti,
                                    0x01
                }));

                // ZW->HOST: REQ | 0x14 | funcID | txStatus
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                                    (byte)FrameTypes.Request,
                                    (byte)CommandTypes.CmdZWaveSendDataMulti,
                                    funcId,
                                    (byte)TransmitStatuses.CompleteNoAcknowledge
                }));

                var destinations = _modules.Where(x => x.Value.IsRFReceiveMode && nodes.Contains(x.Value.NodeId) && x.Value.HomeId.SequenceEqual(_modules[sessionId].HomeId));
                foreach (var destination in destinations)
                {
                    // Create application command handler.
                    // ZW->PC: REQ | 0x04 | rxStatus | sourceNode | cmdLength | pCmd[] | rssiVal 
                    const byte rxStatus = 0x08; // RECEIVE_STATUS_TYPE_MULTI
                    _pool[destination.Key].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request, (byte)CommandTypes.CmdApplicationCommandHandler, rxStatus, srcNodeId, dataLength
                    }.
                    Concat(cmdData).
                    Concat(new[] { rssiVal })));
                }
            }
        }

        private void CmdZWaveGetSucNodeId(byte sessionId)
        {
            _modules[sessionId].IsRFReceiveMode = true;
            // HOST->ZW: REQ | 0x56
            // ZW->HOST: RES | 0x56 | SUCNodeID
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdZWaveGetSucNodeId,
                _modules[sessionId].SucNodeId
            }));
        }

        private void CmdZWaveRequestNodeInfo(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;
            // HOST->ZW: REQ | 0x60 | NodeID
            // ZW->HOST: RES | 0x60 | retVal
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdZWaveRequestNodeInfo,
                0x01 /*TRUE*/
            }));

            var nodeId = data[4];
            var requestedModules = _modules.Where(x => x.Value.IsRFReceiveMode && x.Value.NodeId == nodeId);
            if (requestedModules.Any())
            {
                var cmdClasses = requestedModules.First().Value.CmdClasses ?? new byte[0];
                //ZW->HOST: REQ | 0x49 | bStatus | bNodeID | bLen | basic | generic | specific | commandclasses[ ]
                byte bLen = (byte)(3 /*basic | generic | specific length*/ + cmdClasses.Length);
                var reqCmd = new byte[bLen + 5];
                reqCmd[0] = (byte)FrameTypes.Request;
                reqCmd[1] = (byte)CommandTypes.CmdApplicationControllerUpdate;
                reqCmd[2] = 0x84;
                reqCmd[3] = data[4];
                reqCmd[4] = bLen;
                reqCmd[5] = 0x01;
                reqCmd[6] = 0x02;
                reqCmd[7] = 0x01;
                Array.Copy(cmdClasses, 0, reqCmd, 8, cmdClasses.Length);
                _pool[sessionId].Add(CreateFrame(reqCmd));
            }
        }

        private void CmdZWaveRemoveNodeFromNetwork(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;
            // HOST->ZW: REQ | 0x4B | bMode | funcID 
            // ZW->HOST: REQ | 0x4B | funcID | bStatus | bSource | bLen | basic | generic | specific | cmdclasses[ ]
            var funcId = data[5];
            if (data[4] == (byte)Modes.NodeAny || data[4] == (byte)Modes.NodeOptionNetworkWide)
            {
                _modules[sessionId].IsRemovingNode = true;
                _modules[sessionId].FuncId = funcId;
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveRemoveNodeFromNetwork,
                    funcId,
                    (byte)NodeStatuses.LearnReady,
                    0,
                    0
                }));
            }
            else if (data[4] == (byte)Modes.NodeStop)
            {
                _modules[sessionId].IsAddingNode = false;
                if (funcId != 0)
                {
                    _pool[sessionId].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveRemoveNodeFromNetwork,
                        funcId,
                        (byte)NodeStatuses.Done,
                        0,
                        0,
                        0
                    }));
                }
                else
                {
                    // Assume node add completed and no response for that by design.
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void CmdZWaveReplaceFailedNode(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;
            // HOST->ZW: REQ | 0x63 | nodeID | funcID
            // ZW->HOST: RES | 0x63 | retVal
            // ZW->HOST: REQ | 0x63 | funcID | txStatus
            var funcId = data[5];
            _modules[sessionId].IsReplacingNode = true;
            _modules[sessionId].FuncId = funcId;
            _modules[sessionId].AddOrReplaceNodeId = data[4];
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdZWaveReplaceFailedNode,
                (byte)FailedNodeRetValues.ZW_FAILED_NODE_REMOVE_STARTED
            }));
        }

        private void CmdZWaveAddNodeToNetwork(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;

            // HOST->ZW: REQ | 0x4A | mode | funcID
            // ZW->HOST: REQ | 0x4A | funcID | bStatus | bSource | bLen | basic | generic | specific | cmdclasses[]
            var funcId = data[5];
            if (data[4] == (byte)Modes.NodeAny || data[4] == (byte)Modes.NodeOptionNetworkWide)
            {
                _modules[sessionId].IsAddingNode = true;
                _modules[sessionId].FuncId = funcId;
                _modules[sessionId].AddOrReplaceNodeId = _modules[sessionId].SeedNextNodeId();
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveAddNodeToNetwork,
                    funcId,
                    (byte)NodeStatuses.LearnReady,
                    0,
                    0
                }));
            }
            else if (data[4] == (byte)Modes.NodeStop)
            {
                _modules[sessionId].IsAddingNode = false;
                if (funcId != 0)
                {
                    _pool[sessionId].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveAddNodeToNetwork,
                        funcId,
                        (byte)NodeStatuses.Done,
                        0,
                        0,
                        0
                    }));
                }
                else
                {
                    // Assume node add completed and no response for that by design.
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void CmdZWaveSetLearnMode(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;

            // HOST->ZW: REQ | 0x50 | mode | funcID
            // ZW->HOST: REQ | 0x50 | funcID | bStatus | bSource | bLen | pCmd[]
            var learnModeFuncId = data[5];
            if (data[4] == (byte)LearnModes.LearnModeClassic ||
                data[4] == (byte)LearnModes.LearnModeNWE ||
                data[4] == (byte)LearnModes.LearnModeNWI)
            {
                for (int i = 0; i < 3; i++)
                {
                    var ctrModules = _modules.Where(x => x.Value.IsAddingNode || x.Value.IsRemovingNode || x.Value.IsReplacingNode);
                    if (ctrModules.Any())
                    {
                        break;
                    }
                    Thread.Sleep(100);
                }
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveSetLearnMode,
                    learnModeFuncId,
                    (byte)NodeStatuses.LearnReady,
                    0,
                    0,
                    0
                }));

                var inclusionModules = _modules.Where(x => x.Value.IsAddingNode);
                var exclusionModules = _modules.Where(x => x.Value.IsRemovingNode);
                var replacingModules = _modules.Where(x => x.Value.IsReplacingNode);
                if (inclusionModules.Any())
                {
                    var inclusionModule = inclusionModules.First();

                    _modules[sessionId].NodeId = inclusionModule.Value.AddOrReplaceNodeId;
                    _modules[sessionId].HomeId = inclusionModule.Value.HomeId.Take(4).ToArray();
                    _modules[sessionId].SucNodeId = inclusionModule.Value.SucNodeId;

                    _pool[inclusionModule.Key].Add(ComposeAddNodeClientResponse(
                        (byte)NodeStatuses.NodeFound,
                        _modules[inclusionModule.Key].FuncId,
                        _modules[sessionId].NodeId,
                        _modules[sessionId].CmdClasses));

                    _pool[inclusionModule.Key].Add(ComposeAddNodeClientResponse(
                        (byte)NodeStatuses.AddingRemovingController,
                        _modules[inclusionModule.Key].FuncId,
                        _modules[sessionId].NodeId,
                        _modules[sessionId].CmdClasses));

                    _pool[inclusionModule.Key].Add(ComposeAddNodeClientResponse(
                        (byte)NodeStatuses.ProtocolDone,
                        _modules[inclusionModule.Key].FuncId,
                        _modules[sessionId].NodeId,
                        _modules[sessionId].CmdClasses));

                    _modules[inclusionModule.Key].IsAddingNode = false;
                }
                if (replacingModules.Any())
                {
                    var replacingModule = replacingModules.First();

                    _modules[sessionId].NodeId = replacingModule.Value.AddOrReplaceNodeId;
                    _modules[sessionId].HomeId = replacingModule.Value.HomeId.Take(4).ToArray();
                    _modules[sessionId].SucNodeId = replacingModule.Value.SucNodeId;

                    _pool[replacingModule.Key].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveReplaceFailedNode,
                        _modules[replacingModule.Key].FuncId,
                        (byte)FailedNodeStatuses.ZW_FAILED_NODE_REPLACE
                    }));

                    _pool[replacingModule.Key].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveReplaceFailedNode,
                        _modules[replacingModule.Key].FuncId,
                        (byte)FailedNodeStatuses.ZW_FAILED_NODE_REPLACE_DONE
                    }));

                    _modules[replacingModule.Key].IsReplacingNode = false;
                }
                else if (exclusionModules.Any())
                {
                    var exclusionModule = exclusionModules.First();

                    _pool[exclusionModule.Key].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveRemoveNodeFromNetwork,
                        _modules[exclusionModule.Key].FuncId,
                        (byte)NodeStatuses.NodeFound,
                        0,
                        0
                    }));

                    _pool[exclusionModule.Key].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveRemoveNodeFromNetwork,
                        _modules[exclusionModule.Key].FuncId,
                        (byte)NodeStatuses.AddingRemovingController,
                        _modules[sessionId].NodeId,
                        (byte)(3 + (_modules[sessionId].CmdClasses??new byte[0]).Length),
                        _modules[sessionId].Basic,
                        _modules[sessionId].Generic,
                        _modules[sessionId].Specific
                    }.
                    Concat(_modules[sessionId].CmdClasses ?? new byte[0])));

                    _modules[sessionId].Reset();

                    _pool[exclusionModule.Key].Add(CreateFrame(new byte[]
                    {
                        (byte)FrameTypes.Request,
                        (byte)CommandTypes.CmdZWaveRemoveNodeFromNetwork,
                        _modules[exclusionModule.Key].FuncId,
                        (byte)NodeStatuses.Done,
                        0,
                        0
                    }));

                    _modules[exclusionModule.Key].IsRemovingNode = false;
                }
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveSetLearnMode,
                    learnModeFuncId,
                    (byte)NodeStatuses.Done,
                    _modules[sessionId].NodeId,
                    0,
                    0
                }));
            }
            else if (data[4] == (byte)LearnModes.LearnModeDisable)
            {
                // Assume we disable learn mode and no response for that by design.
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void CmdZWaveSendData(byte sessionId, byte[] data)
        {
            _modules[sessionId].IsRFReceiveMode = true;

            // HOST->ZW: REQ | 0x13 | nodeID | dataLength | pData[ ] | txOptions | funcID
            if (data.Length > ActualMaxFrameSize)
            {
                // Response fail to client.
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdZWaveSendData,
                    0x00
                }));
            }
            else
            {
                // Response to client.
                var nodeId = data[4];
                var dataLength = data[5];
                var funcId = data[5 + 1/*cmd*/ + dataLength + 1/*txOptions*/];
                var srcNodeId = _modules[sessionId].NodeId;
                var cmdData = new byte[dataLength];
                Array.Copy(data, 6, cmdData, 0, dataLength);

                // ZW->HOST: RES | 0x13 | RetVal
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Response,
                    (byte)CommandTypes.CmdZWaveSendData,
                    0x01
                }));
                // ZW->HOST: REQ | 0x13 | funcID | txStatus
                _pool[sessionId].Add(CreateFrame(new byte[]
                {
                    (byte)FrameTypes.Request,
                    (byte)CommandTypes.CmdZWaveSendData,
                    funcId,
                    (byte)TransmitStatuses.CompleteOk
                }));

                // Send data to linked device.
                if (nodeId == 0xFF)
                {
                    var destinations = _modules.Where(x => x.Value.IsRFReceiveMode && x.Value.NodeId != srcNodeId && x.Value.HomeId.SequenceEqual(_modules[sessionId].HomeId));
                    foreach (var destination in destinations)
                    {
                        // Create application command handler.
                        // ZW->PC: REQ | 0x04 | rxStatus | sourceNode | cmdLength | pCmd[] | rssiVal 
                        byte rxStatus = 0x04;
                        var toClientDataList = new List<byte>();
                        toClientDataList.AddRange(new byte[]
                        {
                            (byte)FrameTypes.Request,
                            (byte)CommandTypes.CmdApplicationCommandHandler,
                            rxStatus,
                            srcNodeId,
                            dataLength
                        });
                        toClientDataList.AddRange(cmdData);
                        toClientDataList.Add(rssiVal);
                        _pool[destination.Key].Add(CreateFrame(toClientDataList.ToArray()));
                    }
                }
                else
                {
                    var destination = _modules.Where(x => x.Value.IsRFReceiveMode && x.Value.NodeId != srcNodeId && x.Value.NodeId == nodeId && x.Value.HomeId.SequenceEqual(_modules[sessionId].HomeId));
                    if (destination.Any())
                    {
                        // Create application command handler.
                        // ZW->PC: REQ | 0x04 | rxStatus | sourceNode | cmdLength | pCmd[] | rssiVal 
                        byte rxStatus = 0x00;
                        var toClientDataList = new List<byte>();
                        toClientDataList.AddRange(new byte[]
                        {
                            (byte)FrameTypes.Request,
                            (byte)CommandTypes.CmdApplicationCommandHandler,
                            rxStatus,
                            srcNodeId,
                            dataLength
                        });
                        toClientDataList.AddRange(cmdData);
                        toClientDataList.Add(rssiVal);
                        _pool[destination.First().Key].Add(CreateFrame(toClientDataList.ToArray()));
                    }
                }
            }
        }

        private static void CmdZWaveSendDataAbort(byte sessionId)
        {
            // HOST->ZW: REQ | 0x16
            // Ignore.
        }

        private void CmdSerialApiGetInitData(byte sessionId)
        {
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdSerialApiGetInitData,
                0,
                0,
                0,
                0,
                0
            }));
        }

        private void CmdSerialApiApplNodeInformation(byte sessionId, byte[] data)
        {
            int ccLen = data[7];
            var supportedCmdClasses = new byte[ccLen];
            Array.Copy(data, 8, supportedCmdClasses, 0, ccLen);
            _modules[sessionId].CmdClasses = supportedCmdClasses;
        }

        private void CmdMemoryGetId(byte sessionId)
        {
            // HOST->ZW: REQ | 0x20
            // ZW->HOST: RES | 0x20 | HomeId(4 bytes) | NodeId
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Response,
                (byte)CommandTypes.CmdMemoryGetId,
                _modules[sessionId].HomeId[0],
                _modules[sessionId].HomeId[1],
                _modules[sessionId].HomeId[2],
                _modules[sessionId].HomeId[3],
                _modules[sessionId].NodeId
            }));
        }

        private void CmdZWaveSetDefault(byte sessionId, byte[] data)
        {
            _modules[sessionId].Reset();

            var funcId = data[4];
            // HOST->ZW: REQ | 0x42 | funcID
            // ZW->HOST: REQ | 0x42 | funcID
            _pool[sessionId].Add(CreateFrame(new byte[]
            {
                (byte)FrameTypes.Request,
                (byte)CommandTypes.CmdZWaveSetDefault,
                funcId
            }));
        }

        #endregion
    }
}
