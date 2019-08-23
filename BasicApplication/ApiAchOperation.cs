using System;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;
using ZWave.Security;

namespace ZWave.BasicApplication.Operations
{
    public class ApiAchOperation : ApiOperation
    {
        internal byte SrcNodeId { get; set; }
        internal byte DestNodeId { get; set; }
        internal ByteIndex[] DataToCompare { get; set; }
        internal bool IsFillReceived { get; set; }
        protected AchData ReceivedAchData { get; set; }
        internal int TimeoutMs { get; set; }
        internal ReceiveStatuses RxStatuses { get; set; }
        internal ReceiveStatuses IgnoreRxStatuses { get; set; }
        private byte[] _extensionS2Types;
        public bool ExtensionS2TypeSpecified { get; set; }
        public ApiAchOperation(byte destNodeId, byte srcNodeId, params ByteIndex[] compareData)
            : base(false, null, false)
        {
            IsFillReceived = true;
            ReceivedAchData = new AchData();
            DestNodeId = destNodeId;
            SrcNodeId = srcNodeId;
            SetDataToCompare(compareData);
        }

        public ApiAchOperation(byte destNodeId, byte srcNodeId, byte[] data, int bytesToCompare)
            : base(false, null, false)
        {
            IsFillReceived = true;
            ReceivedAchData = new AchData();
            DestNodeId = destNodeId;
            SrcNodeId = srcNodeId;

            if (data != null)
            {
                bytesToCompare = data.Length < bytesToCompare ? data.Length : bytesToCompare;
                var compareData = new ByteIndex[bytesToCompare];
                for (int i = 0; i < bytesToCompare && i < data.Length; i++)
                {
                    compareData[i] = new ByteIndex(data[i]);
                }
                SetDataToCompare(compareData);
            }
        }

        public ApiAchOperation(byte destNodeId, byte srcNodeId, byte[] data, int bytesToCompare, ExtensionTypes[] extensionTypes)
            : base(false, null, false)
        {
            _extensionS2Types = extensionTypes.Select(val => (byte)((byte)val & 0x3F)).ToArray();
            ExtensionS2TypeSpecified = true;
            IsFillReceived = true;
            ReceivedAchData = new AchData();
            DestNodeId = destNodeId;
            SrcNodeId = srcNodeId;
            var compareData = new ByteIndex[bytesToCompare];
            if (data != null)
            {
                for (int i = 0; i < bytesToCompare && i < data.Length; i++)
                {
                    compareData[i] = new ByteIndex(data[i]);
                }
                SetDataToCompare(compareData);
            }
        }

        public void SetDataToCompare(ByteIndex[] data)
        {
            if (data != null)
            {
                DataToCompare = new ByteIndex[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    if (i == 1 && data[0].Value == COMMAND_CLASS_TRANSPORT_SERVICE_V2.ID)
                    {
                        DataToCompare[i] = new ByteIndex(data[i].Value, 0xF8);
                    }
                    else
                    {
                        DataToCompare[i] = data[i];
                    }
                }
            }
        }

        private ApiHandler _expectReceived;
        private ApiHandler _expectPModeReceived;
        private ApiHandler _expectBridgeReceived;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, TimeoutMs));
            ActionUnits.Add(new DataReceivedUnit(_expectReceived, OnHandled));
            ActionUnits.Add(new DataReceivedUnit(_expectPModeReceived, OnPModeHandled));
            ActionUnits.Add(new DataReceivedUnit(_expectBridgeReceived, OnBridgeHandled));
        }

        protected override void CreateInstance()
        {
            var rxMask = ~(~RxStatuses & ~IgnoreRxStatuses);
            _expectReceived = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationCommandHandler);
            _expectReceived.AddConditions(
               new ByteIndex((byte)(RxStatuses & ~IgnoreRxStatuses), (byte)rxMask),
               SrcNodeId > 0 && SrcNodeId < 255 ? new ByteIndex(SrcNodeId) : ByteIndex.AnyValue,
               ByteIndex.AnyValue);
            _expectPModeReceived = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationCommandHandlerPMode);
            _expectPModeReceived.AddConditions(
               new ByteIndex((byte)(RxStatuses & ~IgnoreRxStatuses), (byte)rxMask),
               SrcNodeId > 0 && SrcNodeId < 255 ? new ByteIndex(SrcNodeId) : ByteIndex.AnyValue,
               ByteIndex.AnyValue);
            _expectBridgeReceived = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationCommandHandler_Bridge);
            _expectBridgeReceived.AddConditions(
                new ByteIndex((byte)(RxStatuses & ~IgnoreRxStatuses), (byte)rxMask),
                DestNodeId > 0 && DestNodeId < 255 ? new ByteIndex(DestNodeId) : ByteIndex.AnyValue,
                SrcNodeId > 0 && SrcNodeId < 255 ? new ByteIndex(SrcNodeId) : ByteIndex.AnyValue,
                ByteIndex.AnyValue);

            if (DataToCompare != null)
            {
                _expectReceived.AddConditions(DataToCompare);
                _expectBridgeReceived.AddConditions(DataToCompare);
            }
        }

        private void OnHandled(DataReceivedUnit ou)
        {
            ReceivedAchData.TimeStamp = DateTime.Now;
            ReceivedAchData.CommandType = CommandTypes.CmdApplicationCommandHandler;
            ReceivedAchData.SubstituteIncomingFlags = ou.DataFrame.SubstituteIncomingFlags;
            int cmdLength = ou.DataFrame.CmdLength;
            if (IsFillReceived)
            {
                FillReceived(ou.DataFrame.Payload, 0, 1, 2, cmdLength);
                ReceivedAchData.Extensions = ou.DataFrame.Extensions;
            }
            OnHandledAA(ou);
        }

        private void OnHandledAA(DataReceivedUnit ou)
        {
            if (ExtensionS2TypeSpecified)
            {
                try
                {
                    if (ou.DataFrame.Extensions != null)
                    {
                        if (_extensionS2Types != null && _extensionS2Types.Length > 0)
                        {
                            if (ou.DataFrame.Extensions.ExtensionsList.Any() && ou.DataFrame.Extensions.EncryptedExtensionsList.Any())
                            {
                                var ext = ou.DataFrame.Extensions.ExtensionsList.Union(ou.DataFrame.Extensions.EncryptedExtensionsList);
                                if (ext.Where(x => _extensionS2Types.Contains(x.properties1.type)).Count() == _extensionS2Types.Length)
                                {
                                    OnHandledInternal(ou);
                                }
                            }
                            else if (ou.DataFrame.Extensions.ExtensionsList.Any())
                            {
                                var ext = ou.DataFrame.Extensions.ExtensionsList;
                                if (ext.Where(x => _extensionS2Types.Contains(x.properties1.type)).Count() == _extensionS2Types.Length)
                                {
                                    OnHandledInternal(ou);
                                }
                            }
                            else if (ou.DataFrame.Extensions.EncryptedExtensionsList.Any())
                            {
                                var ext = ou.DataFrame.Extensions.EncryptedExtensionsList;
                                if (ext.Where(x => _extensionS2Types.Contains(x.properties1.type)).Count() == _extensionS2Types.Length)
                                {
                                    OnHandledInternal(ou);
                                }
                            }
                        }
                        else
                        {
                            if (!ou.DataFrame.Extensions.ExtensionsList.Any() && !ou.DataFrame.Extensions.EncryptedExtensionsList.Any())
                            {
                                OnHandledInternal(ou);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    "{0}"._EXLOG(ex.Message);
                }
            }
            else
            {
                OnHandledInternal(ou);
            }
        }

        private void OnPModeHandled(DataReceivedUnit ou)
        {
            ReceivedAchData.TimeStamp = DateTime.Now;
            ReceivedAchData.CommandType = CommandTypes.CmdApplicationCommandHandlerPMode;
            ReceivedAchData.SubstituteIncomingFlags = ou.DataFrame.SubstituteIncomingFlags;
            int cmdLength = ou.DataFrame.CmdLength;
            if (IsFillReceived)
            {
                FillReceived(ou.DataFrame.Payload, 0, 1, 2, cmdLength);
                ReceivedAchData.Extensions = ou.DataFrame.Extensions;
            }
            if (DestNodeId == 0 || DestNodeId == ReceivedAchData.DestNodeId)
            {
                OnHandledAA(ou);
            }
        }


        private void OnBridgeHandled(DataReceivedUnit ou)
        {
            ReceivedAchData.TimeStamp = DateTime.Now;
            ReceivedAchData.CommandType = CommandTypes.CmdApplicationCommandHandler_Bridge;
            ReceivedAchData.SubstituteIncomingFlags = ou.DataFrame.SubstituteIncomingFlags;
            int cmdLength = ou.DataFrame.CmdLength;
            if (IsFillReceived)
            {
                FillReceived(ou.DataFrame.Payload, 1, 2, 3, cmdLength);
                ReceivedAchData.Extensions = ou.DataFrame.Extensions;
            }
            OnHandledAA(ou);
        }

        private void FillReceived(byte[] payload, int dstIndex, int srcIndex, int lenIndex, int cmdLen)
        {
            ReceivedAchData.DestNodeId = 0;
            ReceivedAchData.SrcNodeId = 0;
            ReceivedAchData.Command = null;
            ReceivedAchData.Rssi = 0;
            ReceivedAchData.SecurityScheme = 0;
            if (payload.Length > 0)
            {
                ReceivedAchData.Options = payload[0];
            }
            if (dstIndex > 0 && payload.Length > dstIndex)
            {
                ReceivedAchData.DestNodeId = payload[dstIndex];
            }
            if (srcIndex > 0 && payload.Length > srcIndex)
            {
                ReceivedAchData.SrcNodeId = payload[srcIndex];
            }
            if (lenIndex + 1 > 0 && lenIndex < payload.Length)
            {
                var cmdLength = cmdLen > 0 ? cmdLen : payload[lenIndex];
                if (payload.Length > lenIndex + cmdLength)
                {
                    ReceivedAchData.Command = new byte[cmdLength];
                    Array.Copy(payload, lenIndex + 1, ReceivedAchData.Command, 0, cmdLength);
                }
                int rssiOffsetAfterCommand = 0;
                if (ReceivedAchData.CommandType == CommandTypes.CmdApplicationCommandHandler_Bridge &&
                    payload.Length > lenIndex + cmdLength + 1)
                {
                    rssiOffsetAfterCommand = 1 + payload[lenIndex + cmdLength + 1];
                }
                if (ReceivedAchData.CommandType == CommandTypes.CmdApplicationCommandHandlerPMode &&
                  payload.Length > lenIndex + cmdLength + 1)
                {
                    rssiOffsetAfterCommand = 1;
                    ReceivedAchData.DestNodeId = payload[lenIndex + cmdLength + 1];
                }
                if (payload.Length > lenIndex + cmdLength + 1 + rssiOffsetAfterCommand)
                {
                    ReceivedAchData.Rssi = payload[lenIndex + cmdLength + 1 + rssiOffsetAfterCommand];
                }
                if (payload.Length > lenIndex + cmdLength + 2 + rssiOffsetAfterCommand)
                {
                    ReceivedAchData.SecurityScheme = payload[lenIndex + cmdLength + 2 + rssiOffsetAfterCommand];
                    "get scheme = {0}"._DLOG(ReceivedAchData.SecurityScheme);
                }
            }
        }

        protected virtual void OnHandledInternal(DataReceivedUnit ou)
        {

        }

        protected bool IsSupportedScheme(NetworkViewPoint network, byte[] command, SecuritySchemes scheme)
        {
            bool ret = false;
            if (command != null && command.Length > 0)
            {
                if (scheme == SecuritySchemes.NONE && !network.HasNetworkAwareCommandClass(command[0]) && network.HasSecurityScheme(SecuritySchemeSet.ALL))
                {
                    ret = !network.HasSecureCommandClass(command[0]);
                }
                else
                {
                    ret = true;
                }
            }
            return ret;
        }
    }

    public class AchData
    {
        public CommandTypes CommandType { get; set; }
        public byte Options { get; set; }
        public byte SrcNodeId { get; set; }
        public byte DestNodeId { get; set; }
        public byte[] Command { get; set; }
        public byte Rssi { get; set; }
        public byte SecurityScheme { get; set; }
        public SubstituteIncomingFlags SubstituteIncomingFlags { get; set; }
        public DateTime TimeStamp { get; set; }
        public Extensions Extensions { get; set; }
    }
}
