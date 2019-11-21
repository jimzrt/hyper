using BasicApplication_netcore.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.BasicApplication.Tasks;
using ZWave.Devices;
using ZWave.Enums;
using ZWave.Exceptions;
using ZWave.Layers;
using ZWave.Layers.Application;
using ZWave.Security;

namespace ZWave.BasicApplication.Devices
{
    public abstract class Device : ApplicationClient, IDevice
    {
        #region Properties
        private NetworkViewPoint _network;
        public NetworkViewPoint Network
        {
            get { return _network; }
            set
            {
                _network = value;
                Notify("Network");
            }
        }

        private byte[] _dsk;
        public byte[] DSK
        {
            get { return _dsk; }
            set { _dsk = value; }
        }

        // hardcoded prk for devkit without NVR stored prk
        private byte[] _prk = new byte[]
        {
            0x77, 0x07, 0x6d, 0x0a, 0x73, 0x18, 0xa5, 0x7d, 0x3c, 0x16, 0xc1, 0x72, 0x51, 0xb2, 0x66, 0x45,
            0xdf, 0x4c, 0x2f, 0x87, 0xeb, 0xc0, 0x99, 0x2a, 0xb1, 0x77, 0xfb, 0xa5, 0x1d, 0xb9, 0x2c, 0x2a
        };
        public byte[] PRK
        {
            get { return _prk; }
            set { _prk = value; }
        }

        public byte Id
        {
            get { return _network.NodeId; }
            set { _network.NodeId = value; }
        }

        /// <summary>
        /// Returns 'HomeId' parameter of the MemoryGetId operation result
        /// </summary>
        public byte[] HomeId
        {
            get { return _network.HomeId; }
            set { _network.HomeId = value; }
        }

        /// <summary>
        /// Returns 'SUCNodeID' parameter of the GetSucNodeId operation result
        /// </summary>
        public byte SucNodeId
        {
            get { return _network.SucNodeId; }
            set { _network.SucNodeId = value; }
        }

        /// <summary>
        ///  Returns 'capabilities' parameter of the SerialApiGetInitData operation result
        /// </summary>
        public byte SerialApiCapability
        {
            get { return _network.SerialApiCapability; }
            set { _network.SerialApiCapability = value; }
        }

        #endregion Properties
        internal Device(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(ApiTypes.Basic, sessionId, sc, fc, tc)
        {
            _network = new NetworkViewPoint(Notify);
            SessionClient.PostSubstituteAction = action =>
            {
                ActionBase ret = null;
                var type = action.GetType();
                if (type == typeof(SendDataExOperation))
                {
                    if (SupportedSerialApiCommands != null && !SupportedSerialApiCommands.Contains((byte)CommandTypes.CmdZWaveSendDataEx))
                    {
                        var completedCallback = action.CompletedCallback;
                        var token = action.Token;
                        var result = action.Result;
                        var parent = action.ParentAction;
                        var actionId = action.Id;

                        var sendDataExAction = (SendDataExOperation)action;
                        var sendDataSchemeRequested = sendDataExAction.SecurityScheme;
                        var nodeId = sendDataExAction.NodeId;
                        var data = sendDataExAction.Data;
                        var dataDelay = sendDataExAction.DataDelay;
                        var newAction = new SendDataOperation(nodeId, data, sendDataExAction.TxOptions) { DataDelay = dataDelay };
                        newAction.SubstituteSettings = sendDataExAction.SubstituteSettings;

                        ret = newAction;
                        ret.Id = actionId;
                        ret.ParentAction = parent;
                        ret.Token = token;
                        if (ret.CompletedCallback == null)
                        {
                            ret.CompletedCallback = completedCallback;
                        }
                        else
                        {
                            var newCompletedCallback = ret.CompletedCallback;
                            ret.CompletedCallback = (x) =>
                            {
                                newCompletedCallback(x);
                                completedCallback(x);
                            };
                        }
                    }
                }
                return ret;
            };
        }

        public bool IsPrimaryController
        {
            get
            {
                return (SerialApiCapability & 0x04) == 0; //Bit 2
            }
        }

        public bool IsSlaveApi
        {
            get
            {
                return (SerialApiCapability & 0x01) > 0; //Bit 0
            }
        }

        public bool IsTimerFunctionsFupported
        {
            get
            {
                return (SerialApiCapability & 0x02) > 0; //Bit 1
            }
        }
        /// <summary>
        /// Returns 'ver' parameter of the SerialApiGetInitData operation result
        /// </summary>
        public byte SerialApiVersion { get; set; }
        /// <summary>
        /// Returns 'chip_type' parameter of the SerialApiGetInitData operation result
        /// </summary>
        public ChipTypes ChipType { get; set; }
        /// <summary>
        /// Returns 'chip_version' parameter of the SerialApiGetInitData operation result
        /// </summary>
        public byte ChipRevision { get; set; }
        /// <summary>
        /// Returns 'buffer' parameter of the Version operation result
        /// </summary>
        public string Version { get; set; }

        private Libraries _library;
        /// <summary>
        /// Returns 'library type' parameter of the Version operation result or TypeLibrary operation result
        /// </summary>
        public Libraries Library
        {
            get { return _library; }
            set
            {
                _library = value;
                Network.IsBridgeController = _library == Libraries.ControllerBridgeLib;
            }
        }

        /// <summary>
        /// Returns 'background RSSI' parameter of the GetBackgroundRSSI operation result
        /// </summary>
        public byte[] BackgroundRSSILevels { get; set; }
        /// <summary>
        /// Returns 'SERIAL_APPL_VERSION' parameter of the SerialApiGetCapabilities operation result
        /// </summary>
        public byte SerialApplicationVersion { get; set; }
        /// <summary>
        /// Returns 'SERIAL_APPL_REVISION' parameter of the SerialApiGetCapabilities operation result
        /// </summary>
        public byte SerialApplicationRevision { get; set; }
        /// <summary>
        /// Returns 'SERIALAPI_MANUFACTURER_ID' parameter of the SerialApiGetCapabilities operation result
        /// </summary>
        public ushort ManufacturerId { get; set; }
        /// <summary>
        /// Returns 'SERIALAPI_MANUFACTURER_PRODUCT_TYPE' parameter of the SerialApiGetCapabilities operation result
        /// </summary>
        public ushort ManufacturerProductType { get; set; }
        /// <summary>
        /// Returns 'SERIALAPI_MANUFACTURER_PRODUCT_ID' parameter of the SerialApiGetCapabilities operation result
        /// </summary>
        public ushort ManufacturerProductId { get; set; }
        /// <summary>
        /// Returns 'FUNCID_SUPPORTED_BITMASK' parameter of the SerialApiGetCapabilities operation result. Bitmask converted to list.
        /// </summary>
        public byte[] SupportedSerialApiCommands { get; set; }
        /// <summary>
        /// Return Hardware Version from NVR of device. Sets on device open.
        /// </summary>
        public byte HardwareVersion { get; set; }

        private readonly AutoResetEvent _controllerUpdateStatusSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent nsLearnReadySignal = new AutoResetEvent(false);
        private readonly AutoResetEvent nodeFoundSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent addingRemovingSlaveSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent addingRemovingControllerSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent protocolDoneSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent doneSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent failedSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent notPrimarySignal = new AutoResetEvent(false);
        private readonly AutoResetEvent assignCompleteSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent assignNodeIdDoneSignal = new AutoResetEvent(false);
        private readonly AutoResetEvent assignRangeInfoUpdateSignal = new AutoResetEvent(false);
        private ControllerUpdateStatuses _lastControllerUpdateStatusValue;
        public bool WaitControllerUpdateSignal(ControllerUpdateStatuses updateStatus, int timeout)
        {
            bool ret = false;
            ret = _controllerUpdateStatusSignal.WaitOne();
            while (_lastControllerUpdateStatusValue != updateStatus)
            {
                ret = _controllerUpdateStatusSignal.WaitOne();
            }
            return ret;
        }

        public void SetNodeStatusSignal(ControllerUpdateStatuses updateStatus)
        {
            _lastControllerUpdateStatusValue = updateStatus;
            _controllerUpdateStatusSignal.Set();
        }

        public bool WaitNodeStatusSignal(NodeStatuses nodeStatus, int timeout)
        {
            bool ret = false;
            switch (nodeStatus)
            {
                case NodeStatuses.Unknown:
                    break;
                case NodeStatuses.LearnReady:
                    ret = nsLearnReadySignal.WaitOne(timeout, true);
                    break;
                case NodeStatuses.NodeFound:
                    ret = nodeFoundSignal.WaitOne(timeout);
                    break;
                case NodeStatuses.AddingRemovingSlave:
                    ret = addingRemovingSlaveSignal.WaitOne(timeout, true);
                    break;
                case NodeStatuses.AddingRemovingController:
                    ret = addingRemovingControllerSignal.WaitOne(timeout, true);
                    break;
                case NodeStatuses.ProtocolDone:
                    ret = protocolDoneSignal.WaitOne(timeout, true);
                    break;
                case NodeStatuses.Done:
                    ret = doneSignal.WaitOne(timeout, true);
                    break;
                case NodeStatuses.Failed:
                    ret = failedSignal.WaitOne(timeout, true);
                    break;
                case NodeStatuses.NotPrimary:
                    ret = notPrimarySignal.WaitOne(timeout, true);
                    break;
                default:
                    break;
            }
            return ret;
        }

        public bool WaitAnyNodeStatusSignal(NodeStatuses[] nodeStatuses, int timeout)
        {
            bool ret = false;
            AutoResetEvent[] signals = new AutoResetEvent[nodeStatuses.Length];
            for (int i = 0; i < signals.Length; i++)
            {
                NodeStatuses nodeStatus = nodeStatuses[i];
                switch (nodeStatus)
                {
                    case NodeStatuses.Unknown:
                        break;
                    case NodeStatuses.LearnReady:
                        signals[i] = nsLearnReadySignal;
                        break;
                    case NodeStatuses.NodeFound:
                        signals[i] = nodeFoundSignal;
                        break;
                    case NodeStatuses.AddingRemovingSlave:
                        signals[i] = addingRemovingSlaveSignal;
                        break;
                    case NodeStatuses.AddingRemovingController:
                        signals[i] = addingRemovingControllerSignal;
                        break;
                    case NodeStatuses.ProtocolDone:
                        signals[i] = protocolDoneSignal;
                        break;
                    case NodeStatuses.Done:
                        signals[i] = doneSignal;
                        break;
                    case NodeStatuses.Failed:
                        signals[i] = failedSignal;
                        break;
                    case NodeStatuses.NotPrimary:
                        signals[i] = notPrimarySignal;
                        break;
                    default:
                        break;
                }
            }
            ret = WaitHandle.WaitAny(signals, timeout) != WaitHandle.WaitTimeout;
            return ret;
        }

        public bool WaitAssignStatusSignal(AssignStatuses assignStatus, int timeout)
        {
            bool ret = false;
            switch (assignStatus)
            {
                case AssignStatuses.AssignComplete:
                    ret = assignCompleteSignal.WaitOne(timeout, true);
                    break;
                case AssignStatuses.AssignNodeIdDone:
                    ret = assignNodeIdDoneSignal.WaitOne(timeout, true);
                    break;
                case AssignStatuses.AssignRangeInfoUpdate:
                    ret = assignRangeInfoUpdateSignal.WaitOne(timeout, true);
                    break;
                default:
                    break;
            }
            return ret;
        }

        public void SetNodeStatusSignal(NodeStatuses nodeStatus)
        {
            switch (nodeStatus)
            {
                case NodeStatuses.Unknown:
                    break;
                case NodeStatuses.LearnReady:
                    nsLearnReadySignal.Set();
                    break;
                case NodeStatuses.NodeFound:
                    nodeFoundSignal.Set();
                    break;
                case NodeStatuses.AddingRemovingSlave:
                    addingRemovingSlaveSignal.Set();
                    break;
                case NodeStatuses.AddingRemovingController:
                    addingRemovingControllerSignal.Set();
                    break;
                case NodeStatuses.ProtocolDone:
                    protocolDoneSignal.Set();
                    break;
                case NodeStatuses.Done:
                    doneSignal.Set();
                    break;
                case NodeStatuses.Failed:
                    failedSignal.Set();
                    break;
                case NodeStatuses.NotPrimary:
                    notPrimarySignal.Set();
                    break;
                default:
                    break;
            }
        }

        public void SetAssignStatusSignal(AssignStatuses assignStatus)
        {
            switch (assignStatus)
            {
                case AssignStatuses.AssignComplete:
                    assignCompleteSignal.Set();
                    break;
                case AssignStatuses.AssignNodeIdDone:
                    assignNodeIdDoneSignal.Set();
                    break;
                case AssignStatuses.AssignRangeInfoUpdate:
                    assignRangeInfoUpdateSignal.Set();
                    break;
                default:
                    break;
            }
        }

        protected void ResetNodeStatusSignals()
        {
            nsLearnReadySignal.Reset();
            nodeFoundSignal.Reset();
            addingRemovingSlaveSignal.Reset();
            addingRemovingControllerSignal.Reset();
            protocolDoneSignal.Reset();
            doneSignal.Reset();
            failedSignal.Reset();
            notPrimarySignal.Reset();
        }

        public void ResetAssignStatusSignals()
        {
            assignCompleteSignal.Reset();
            assignNodeIdDoneSignal.Reset();
            assignRangeInfoUpdateSignal.Reset();
        }

        protected void BeforeExecute(ActionBase operation)
        {
            ApiOperation op = (ApiOperation)operation;
            if (SupportedSerialApiCommands != null && op.SerialApiCommands != null)
            {
                foreach (var item in op.SerialApiCommands)
                {
                    if (!SupportedSerialApiCommands.Contains((byte)item))
                        OperationException.Throw(item + "=0x" + ((byte)item).ToString("X2") + " not supported");
                }
            }
        }

        public virtual ActionToken ExecuteAsync(IActionItem actionItem, Action<IActionItem> completedCallback)
        {
            var actionBase = actionItem as ActionBase;
            if (actionBase != null)
            {
                if (actionBase is SetDefaultOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            ActionResult res = action.Result;
                            if (res)
                            {
                                foreach (var sm in SessionClient.GetSubstituteManagers())
                                {
                                    sm.SetDefault();
                                }
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is RequestNodeInfoOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            RequestNodeInfoResult res = (RequestNodeInfoResult)action.Result;
                            if (res)
                            {
                                Network.SetCommandClasses(new NodeTag(res.NodeId), res.CommandClasses);
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is MemoryGetIdOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            MemoryGetIdResult res = (MemoryGetIdResult)action.Result;
                            if (res)
                            {
                                var prevId = Id;
                                var prevHomeId = new byte[4];
                                if (HomeId != null)
                                {
                                    prevHomeId = new byte[HomeId.Length];
                                    Array.Copy(HomeId, prevHomeId, HomeId.Length);
                                }
                                Id = res.NodeId;
                                HomeId = res.HomeId;

                                if (prevId != Id || !prevHomeId.SequenceEqual(HomeId))
                                {
                                    foreach (var sm in SessionClient.GetSubstituteManagers())
                                    {
                                        sm.SetDefault();
                                    }
                                }
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is SerialApiGetCapabilitiesOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            SerialApiGetCapabilitiesResult res = (SerialApiGetCapabilitiesResult)action.Result;
                            if (res)
                            {
                                SerialApplicationVersion = res.SerialApplicationVersion;
                                SerialApplicationRevision = res.SerialApplicationRevision;
                                ManufacturerId = res.ManufacturerId;
                                ManufacturerProductType = res.ManufacturerProductType;
                                ManufacturerProductId = res.ManufacturerProductId;
                                SupportedSerialApiCommands = res.SupportedSerialApiCommands;
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is SerialApiGetInitDataOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            SerialApiGetInitDataResult res = (SerialApiGetInitDataResult)action.Result;
                            if (res)
                            {
                                SerialApiVersion = res.SerialApiVersion;
                                SerialApiCapability = res.SerialApiCapability;
                                ChipType = res.ChipType;
                                ChipRevision = res.ChipRevision;
                                if (GetType() == typeof(Controller))
                                {
                                    ((Controller)this).IncludedNodes = res.IncludedNodes;
                                }
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is VersionOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            VersionResult res = (VersionResult)action.Result;
                            if (res)
                            {
                                Library = res.Library;
                                Version = res.Version;
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is TypeLibraryOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            TypeLibraryResult res = (TypeLibraryResult)action.Result;
                            if (res)
                            {
                                Library = res.Library;
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else if (actionBase is GetBackgroundRssiOperation)
                {
                    actionBase.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            GetBackgroundRssiResult res = (GetBackgroundRssiResult)action.Result;
                            if (res)
                            {
                                BackgroundRSSILevels = res.BackgroundRSSILevels;
                            }
                            if (completedCallback != null)
                                completedCallback(action);
                        }
                    };
                }
                else
                {
                    actionBase.CompletedCallback = completedCallback;
                }
                actionBase.Token.LogEntryPointCategory = "Basic";
                actionBase.Token.LogEntryPointSource = DataSource == null ? "" : DataSource.SourceName;
            }
            return SessionClient.ExecuteAsync(actionBase);
        }

        public virtual ActionResult Execute(IActionItem actionItem)
        {
            var action = actionItem as ActionBase;
            if (action != null)
            {
                action.Token.LogEntryPointCategory = "Basic";
                action.Token.LogEntryPointSource = DataSource == null ? "" : DataSource.SourceName;
            }
            ActionToken token = SessionClient.ExecuteAsync(action);
            token.WaitCompletedSignal();
            ActionResult ret = token.Result;

            if (actionItem is SetDefaultOperation)
            {
                ActionResult res = ret;
                if (res)
                {
                    foreach (var sm in SessionClient.GetSubstituteManagers())
                    {
                        sm.SetDefault();
                    }
                }
            }
            else if (actionItem is RequestNodeInfoOperation)
            {
                RequestNodeInfoResult res = (RequestNodeInfoResult)ret;
                if (res)
                {
                    Network.SetCommandClasses(new NodeTag(res.NodeId), res.CommandClasses);
                }
            }
            else if (actionItem is MemoryGetIdOperation)
            {
                MemoryGetIdResult res = (MemoryGetIdResult)ret;
                if (res)
                {
                    var prevId = Id;

                    Id = res.NodeId;
                    HomeId = res.HomeId;

                    if (prevId != Id)
                    {
                        foreach (var sm in SessionClient.GetSubstituteManagers())
                        {
                            sm.SetDefault();
                        }
                    }
                }
            }
            else if (actionItem is SerialApiGetCapabilitiesOperation)
            {
                SerialApiGetCapabilitiesResult res = (SerialApiGetCapabilitiesResult)ret;
                if (res)
                {
                    SerialApplicationVersion = res.SerialApplicationVersion;
                    SerialApplicationRevision = res.SerialApplicationRevision;
                    ManufacturerId = res.ManufacturerId;
                    ManufacturerProductType = res.ManufacturerProductType;
                    ManufacturerProductId = res.ManufacturerProductId;
                    SupportedSerialApiCommands = res.SupportedSerialApiCommands;
                }
            }
            else if (actionItem is SerialApiGetInitDataOperation)
            {
                SerialApiGetInitDataResult res = (SerialApiGetInitDataResult)ret;
                if (res)
                {
                    SerialApiVersion = res.SerialApiVersion;
                    SerialApiCapability = res.SerialApiCapability;
                    ChipType = res.ChipType;
                    ChipRevision = res.ChipRevision;
                    if (this is Controller)
                    {
                        ((Controller)this).IncludedNodes = res.IncludedNodes;
                    }
                }
            }
            else if (actionItem is VersionOperation)
            {
                VersionResult res = (VersionResult)ret;
                if (res)
                {
                    Library = res.Library;
                    Version = res.Version;
                }
            }
            else if (actionItem is TypeLibraryOperation)
            {
                TypeLibraryResult res = (TypeLibraryResult)ret;
                if (res)
                {
                    Library = res.Library;
                }
            }
            else if (actionItem is GetBackgroundRssiOperation)
            {
                GetBackgroundRssiResult res = (GetBackgroundRssiResult)ret;
                if (res)
                {
                    BackgroundRSSILevels = res.BackgroundRSSILevels;
                }
            }
            return ret;
        }

        public void Stop(Type type)
        {
            SessionClient.Cancel(type);
        }

        #region SetLearnMode
        public ApiOperation learnModeOperation = null;
        public abstract SetLearnModeResult SetLearnMode(LearnModes mode, bool isSubstituteDenied, int timeoutMs);
        public abstract ActionToken SetLearnMode(LearnModes mode, bool isSubstituteDenied, int timeoutMs, Action<IActionItem> completedCallback);
        public abstract SetLearnModeResult SetLearnMode(LearnModes mode, int timeoutMs);
        public abstract ActionToken SetLearnMode(LearnModes mode, int timeoutMs, Action<IActionItem> completedCallback);
        #endregion

        #region SerialApiGetCapabilities

        public SerialApiGetCapabilitiesResult SerialApiGetCapabilities()
        {
            return (SerialApiGetCapabilitiesResult)Execute(new SerialApiGetCapabilitiesOperation());
        }

        public ActionToken SerialApiGetCapabilities(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SerialApiGetCapabilitiesOperation(), completedCallback);
        }

        #endregion

        #region SetDefault
        public ActionResult SetDefault()
        {
            return Execute(new SetDefaultOperation());
        }

        public ActionToken SetDefault(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SetDefaultOperation(), completedCallback);
        }
        #endregion

        #region GetPRK

        public NVRGetValueResult GetPRK()
        {
            var ret = (NVRGetValueResult)Execute(new NVRGetValueOperation(0x43, 32));
            if (ret && ret.NVRValue.Length == 32)
            {
                bool isEmpty = true;
                foreach (var item in ret.NVRValue)
                {
                    isEmpty = item == 0xFF;
                    if (!isEmpty)
                    {
                        PRK = ret.NVRValue;
                        break;
                    }
                }
            }
            return ret;
        }

        #endregion

        #region MemoryGetId

        public MemoryGetIdResult MemoryGetId()
        {
            return (MemoryGetIdResult)Execute(new MemoryGetIdOperation());
        }

        public ActionToken MemoryGetId(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new MemoryGetIdOperation(), completedCallback);
        }

        #endregion

        #region SendDataEx

        public SendDataResult SendDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, SecuritySchemes scheme)
        {
            return (SendDataResult)Execute(new SendDataExOperation(nodeId, data, txOptions, TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, scheme, TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE));
        }

        public SendDataResult SendDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2)
        {
            return (SendDataResult)Execute(new SendDataExOperation(nodeId, data, txOptions, txSecOptions, scheme, txOptions2));
        }

        public ActionToken SendDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, SecuritySchemes scheme, Action<IActionItem> completedCallback)
        {
            SendDataExOperation operation = new SendDataExOperation(nodeId, data, txOptions, TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, scheme, TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionToken SendDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, Action<IActionItem> completedCallback)
        {
            SendDataExOperation operation = new SendDataExOperation(nodeId, data, txOptions, txSecOptions, scheme, txOptions2);
            return ExecuteAsync(operation, completedCallback);
        }

        public SendDataResult SendDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, out ActionToken token)
        {
            token = SendDataEx(nodeId, data, txOptions, txSecOptions, scheme, txOptions2, null);
            return (SendDataResult)token.WaitCompletedSignal();
        }

        #endregion

        #region SendData

        public SendDataResult SendData(byte nodeId, byte[] data, TransmitOptions txOptions)
        {
            return SendData(nodeId, data, txOptions, new SubstituteSettings());
        }

        public SendDataResult SendData(byte nodeId, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings, bool isSinglecastFollowup)
        {
            return (SendDataResult)Execute(new SendDataOperation(nodeId, data, txOptions)
            {
                SubstituteSettings = substituteSettings ?? new SubstituteSettings(),
                IsFollowup = isSinglecastFollowup
            });
        }

        public SendDataResult SendData(byte nodeId, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings)
        {
            return (SendDataResult)Execute(new SendDataOperation(nodeId, data, txOptions)
            {
                SubstituteSettings = substituteSettings ?? new SubstituteSettings()
            });
        }

        public SendDataResult SendData(byte nodeId, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings, out ActionToken token)
        {
            var operation = new SendDataOperation(nodeId, data, txOptions)
            {
                SubstituteSettings = substituteSettings ?? new SubstituteSettings()
            };
            token = ExecuteAsync(operation, null);
            return (SendDataResult)token.WaitCompletedSignal();
        }

        public ActionToken SendData(byte nodeId, byte[] data, TransmitOptions txOptions, Action<IActionItem> completedCallback)
        {
            return SendData(nodeId, data, txOptions, null, completedCallback);
        }

        public ActionToken SendData(byte nodeId, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings, Action<IActionItem> completedCallback)
        {
            SendDataOperation operation = new SendDataOperation(nodeId, data, txOptions)
            {
                SubstituteSettings = substituteSettings ?? new SubstituteSettings()
            };
            return ExecuteAsync(operation, completedCallback);
        }

        #endregion

        #region SendDataMulti

        public TransmitResult SendDataMulti(byte[] nodeIds, byte[] data, TransmitOptions txOptions)
        {
            return (TransmitResult)(_network.IsBridgeController ?
                Execute(new SendDataMultiBridgeOperation(Id, nodeIds, data, txOptions)) :
                Execute(new SendDataMultiOperation(nodeIds, data, txOptions)));
        }

        public TransmitResult SendDataMulti(byte[] nodeIds, byte[] data, TransmitOptions txOptions, out ActionToken token)
        {
            return SendDataMulti(nodeIds, data, txOptions, new SubstituteSettings(), out token);
        }

        public TransmitResult SendDataMulti(byte[] nodeIds, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings, out ActionToken token)
        {
            token = SendDataMulti(nodeIds, data, txOptions, substituteSettings, null);
            return (TransmitResult)token.WaitCompletedSignal();
        }

        public ActionToken SendDataMulti(byte[] nodeIds, byte[] data, TransmitOptions txOptions, Action<IActionItem> completedCallback)
        {
            return SendDataMulti(nodeIds, data, txOptions, new SubstituteSettings(), completedCallback);
        }

        public ActionToken SendDataMulti(byte[] nodeIds, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings, Action<IActionItem> completedCallback)
        {
            IActionItem sendDataMultiOperation = (_network.IsBridgeController ?
                new SendDataMultiBridgeOperation(Id, nodeIds, data, txOptions) { SubstituteSettings = substituteSettings } :
                (IActionItem)new SendDataMultiOperation(nodeIds, data, txOptions) { SubstituteSettings = substituteSettings });
            return ExecuteAsync(sendDataMultiOperation, completedCallback);
        }

        #endregion

        #region SendDataMultiEx

        public SendDataResult SendDataMultiEx(byte[] data, TransmitOptions txOptions, SecuritySchemes scheme, byte groupId)
        {
            return (SendDataResult)Execute(new SendDataMultiExOperation(data, txOptions, scheme, groupId));
        }

        public ActionToken SendDataMultiEx(byte[] data, TransmitOptions txOptions, SecuritySchemes scheme, byte groupId, Action<IActionItem> completedCallback)
        {
            SendDataMultiExOperation operation = new SendDataMultiExOperation(data, txOptions, scheme, groupId);
            return ExecuteAsync(operation, completedCallback);
        }

        public SendDataResult SendDataMultiEx(byte[] data, TransmitOptions txOptions, SecuritySchemes scheme, byte groupId, out ActionToken token)
        {
            token = SendDataMultiEx(data, txOptions, scheme, groupId, null);
            return (SendDataResult)token.WaitCompletedSignal();
        }

        #endregion

        #region SendNodeInformation

        public TransmitResult SendNodeInformation(byte destination, TransmitOptions txOptions)
        {
            return (TransmitResult)Execute(new SendNodeInformationOperation(destination, txOptions));
        }

        public ActionToken SendNodeInformation(byte destination, TransmitOptions txOptions, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SendNodeInformationOperation(destination, txOptions), completedCallback);
        }

        #endregion

        #region SetPromiscuousMode

        public ActionResult SetPromiscuousMode(bool state)
        {
            return Execute(new SetPromiscuousModeOperation(state));
        }

        public ActionToken SetPromiscuousMode(bool state, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SetPromiscuousModeOperation(state), completedCallback);
        }

        #endregion

        #region ExploreRequestInclusion

        public ActionResult ExploreRequestInclusion()
        {
            byte learnFuncId = 0;
            if (learnModeOperation != null)
                learnFuncId = learnModeOperation.SequenceNumber;
            return Execute(new ExploreRequestInclusionOperation(learnFuncId));
        }
        public ActionResult ExploreRequestExclusion()
        {
            byte learnFuncId = 0;
            if (learnModeOperation != null)
                learnFuncId = learnModeOperation.SequenceNumber;
            return Execute(new ExploreRequestExclusionOperation(learnFuncId));
        }

        #endregion

        public ActionResult SerialApiSoftReset()
        {
            return Execute(new SerialApiSoftResetOperation());
        }

        public ActionResult SerialApiSetTimeouts(byte rxAckTimeout, byte rxByteTimeout)
        {
            return Execute(new SerialApiSetTimeoutsOperation(rxAckTimeout, rxByteTimeout));
        }

        public GetProtocolStatusResult GetProtocolStatus()
        {
            return (GetProtocolStatusResult)Execute(new GetProtocolStatusOperation());
        }

        public GetRandomWordResult GetRandomWord(byte count)
        {
            return (GetRandomWordResult)Execute(new GetRandomWordOperation(count));
        }

        public MemoryGetBufferResult MemoryGetBuffer(ushort offset, byte length)
        {
            return (MemoryGetBufferResult)Execute(new MemoryGetBufferOperation(offset, length));
        }

        public ReadNVRamResult ReadNVRam(ushort offset, byte length)
        {
            return (ReadNVRamResult)Execute(new ReadNVRamOperation(offset, length));
        }

        public WriteNVRamResult WriteNVRam(ushort offset, byte length, byte[] data)
        {
            return (WriteNVRamResult)Execute(new WriteNVRamOperation(offset, length, data));
        }

        public MemoryGetByteResult MemoryGetByte(ushort offset)
        {
            return (MemoryGetByteResult)Execute(new MemoryGetByteOperation(offset));
        }

        public ActionResult MemoryPutBuffer(ushort offset, ushort length, byte[] data)
        {
            return Execute(new MemoryPutBufferOperation(offset, length, data));
        }

        public ActionResult MemoryPutByte(ushort offset, byte data)
        {
            return Execute(new MemoryPutByteOperation(offset, data));
        }

        public RandomResult RandomByte()
        {
            return (RandomResult)Execute(new RandomOperation());
        }

        public ActionToken RequestNetworkUpdate(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new RequestNetworkUpdateOperation(), completedCallback);
        }

        public RequestNetworkUpdateResult RequestNetworkUpdate()
        {
            return (RequestNetworkUpdateResult)Execute(new RequestNetworkUpdateOperation());
        }

        public RequestNodeInfoResult RequestNodeInfo(byte nodeId)
        {
            return (RequestNodeInfoResult)Execute(new RequestNodeInfoOperation(nodeId));
        }

        public ActionToken RequestNodeInfo(byte nodeId, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new RequestNodeInfoOperation(nodeId), completedCallback);
        }

        public RequestNodeInfoResult RequestNodeInfo(byte nodeId, out ActionToken token)
        {
            token = RequestNodeInfo(nodeId, null);
            return (RequestNodeInfoResult)token.WaitCompletedSignal();
        }

        public NodeInfoResult NodeInfo(byte nodeId)
        {
            return (NodeInfoResult)Execute(new NodeInfoTask(Network, nodeId));
        }

        public ActionToken NodeInfo(byte nodeId, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new NodeInfoTask(Network, nodeId), completedCallback);
        }

        public NodeInfoResult NodeInfo(byte nodeId, out ActionToken token)
        {
            token = NodeInfo(nodeId, null);
            return (NodeInfoResult)token.WaitCompletedSignal();
        }

        public RFPowerLevelGetResult RFPowerLevelGet()
        {
            return (RFPowerLevelGetResult)Execute(new RFPowerLevelGetOperation());
        }

        public ActionResult RFPowerlevelRediscoverySet(byte powerLevel)
        {
            return Execute(new RFPowerlevelRediscoverySetOperation(powerLevel));
        }

        public ActionResult RFPowerLevelSet(byte powerLevel)
        {
            return Execute(new RFPowerLevelSetOperation(powerLevel));
        }

        public ActionResult SendDataAbort()
        {
            return Execute(new SendDataAbortOperation());
        }

        public TransmitResult SendTestFrame(byte nodeId, byte powerLevel)
        {
            return (TransmitResult)Execute(new SendTestFrameOperation(nodeId, powerLevel));
        }

        public ActionToken SendTestFrame(byte nodeId, byte powerLevel, Action<IActionItem> completedCallback)
        {
            SendTestFrameOperation operation = new SendTestFrameOperation(nodeId, powerLevel);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionResult SendTestFrame(byte nodeId, byte powerLevel, out ActionToken token)
        {
            token = SendTestFrame(nodeId, powerLevel, null);
            return (TransmitResult)token.WaitCompletedSignal();
        }

        public ActionResult ApplicationNodeInformationCmdClasses(byte[] nodeParameters, byte[] unsecureNodeParameters, byte[] secureNodeParameters)
        {
            //return Execute(new ApplicationNodeInformationCmdClassesOperation(nodeParameters, unsecureNodeParameters, secureNodeParameters));
            return Execute(new ActionSerialGroup(new ApplicationNodeInformationCmdClassesOperation(nodeParameters, unsecureNodeParameters, secureNodeParameters),
                new DelayOperation(200))
            { Name = "ApplicationNodeInformationCmdClassesOperation" });
        }

        public ActionResult ApplicationNodeInformation(bool isListening, byte generic, byte specific, byte[] nodeParameters)
        {
            //return Execute(new ApplicationNodeInformationOperation(isListening, generic, specific, nodeParameters));
            return Execute(new ActionSerialGroup(new ApplicationNodeInformationOperation(isListening, generic, specific, nodeParameters),
                new DelayOperation(200))
            { Name = "ApplicationNodeInformation" });
        }

        public ActionResult ApplicationNodeInformation(DeviceOptions deviceOptions, byte generic, byte specific, byte[] nodeParameters)
        {
            //return Execute(new ApplicationNodeInformationOperation(deviceOptions, generic, specific, nodeParameters));
            return Execute(new ActionSerialGroup(new ApplicationNodeInformationOperation(deviceOptions, generic, specific, nodeParameters),
                new DelayOperation(200))
            { Name = "ApplicationNodeInformation" });
        }

        public ActionResult ApplicationUserInput(byte[] UserInputId, byte[] UserInputLocal)
        {
            return Execute(new ApplicationUserInputOperation(UserInputId, UserInputLocal));
        }

        public ActionResult ApplicationGetUserInput()
        {
            return Execute(new ApplicationGetUserInputOperation());
        }

        public SerialApiGetInitDataResult SerialApiGetInitData()
        {
            return (SerialApiGetInitDataResult)Execute(new SerialApiGetInitDataOperation());
        }

        public ActionToken SerialApiGetInitData(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SerialApiGetInitDataOperation(), completedCallback);
        }

        public ActionResult SetExtIntLevel(byte intSrc, bool triggerLevel)
        {
            return Execute(new SetExtIntLevelOperation(intSrc, triggerLevel));
        }

        public ActionResult SetMaxInclusionRequestIntervals(byte requestintervalSec)
        {
            return Execute(new SetMaxInclusionRequestIntervalsOperation(requestintervalSec));
        }

        #region SetRFReceiveMode
        public ActionResult SetRFReceiveMode(byte mode)
        {
            return Execute(new SetRFReceiveModeOperation(mode));
        }

        public ActionResult SetRFReceiveMode(byte mode, bool isNoDelay)
        {
            return Execute(new SetRFReceiveModeOperation(mode, isNoDelay));
        }

        public ActionToken SetRFReceiveMode(byte mode, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SetRFReceiveModeOperation(mode), completedCallback);
        }
        #endregion

        public ActionResult SetSleepMode(SleepModes sleepModes, byte intEnable)
        {
            return Execute(new SetSleepModeOperation(sleepModes, intEnable));
        }

        public ActionResult SetWutTimeout(byte timeoutSec)
        {
            return Execute(new SetWutTimeoutOperation(timeoutSec));
        }

        public ActionResult PowerMgmtStayAwake(byte powerLockType, int powerTimeout)
        {
            return Execute(new PowerMgmtStayAwakeOperation(powerLockType, powerTimeout));
        }

        public ActionResult PowerMgmtCancel(byte powerLockType)
        {
            return Execute(new PowerMgmtCancelOperation(powerLockType));
        }

        public VersionResult GetVersion()
        {
            return (VersionResult)Execute(new VersionOperation(false));
        }

        public VersionResult GetVersion(bool isNoAck)
        {
            return (VersionResult)Execute(new VersionOperation(isNoAck));
        }

        public GetSecurityKeysResult GetSecurityKeys()
        {
            return (GetSecurityKeysResult)Execute(new GetSecurityKeysOperation());
        }

        public ActionToken GetVersion(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new VersionOperation(), completedCallback);
        }

        public TypeLibraryResult GetTypeLibrary()
        {
            return (TypeLibraryResult)Execute(new TypeLibraryOperation());
        }

        public ActionResult WatchDogDisable()
        {
            return Execute(new WatchDogDisableOperation());
        }

        public ActionResult WatchDogEnable()
        {
            return Execute(new WatchDogEnableOperation());
        }

        public ActionResult WatchDogKick()
        {
            return Execute(new WatchDogKickOperation());
        }

        public ActionResult WatchDogStart()
        {
            return Execute(new WatchDogStartOperation());
        }

        public ActionResult WatchDogStop()
        {
            return Execute(new WatchDogStopOperation());
        }

        public ActionToken SerialApiTest(Action<SerialApiTestResult> receiveCallback, byte testCmd, ushort testDelay, byte testPayloadLength, ushort testCount, TransmitOptions txOptions, byte[] nodeIds, bool isStopOnErrors, Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SerialApiTestOperation(receiveCallback, testCmd, testDelay, testPayloadLength, testCount, txOptions, nodeIds, isStopOnErrors), completedCallback);
        }

        public SerialApiTestResult SerialApiTest(Action<SerialApiTestResult> receiveCallback, byte testCmd, ushort testDelay, byte testPayloadLength, ushort testCount, TransmitOptions txOptions, byte[] nodeIds, bool isStopOnErrors)
        {
            return (SerialApiTestResult)Execute(new SerialApiTestOperation(receiveCallback, testCmd, testDelay, testPayloadLength, testCount, txOptions, nodeIds, isStopOnErrors));
        }

        public SerialApiSetupResult SerialApiSetup(params byte[] args)
        {
            return (SerialApiSetupResult)Execute(new SerialApiSetupOperation(args));
        }

        public GetMaxPayloadSizeResult GetMaxPayloadSize()
        {
            return (GetMaxPayloadSizeResult)Execute(new GetMaxPayloadSizeOperation());
        }

        public ActionResult GetBackgroundRSSI()
        {
            return (GetBackgroundRssiResult)Execute(new GetBackgroundRssiOperation());
        }

        public ActionToken ExpectControllerUpdate(ControllerUpdateStatuses updateStatus, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ExpectControllerUpdateOperation operation = new ExpectControllerUpdateOperation(updateStatus, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ExpectDataResult ExpectData(byte[] data, int timeoutMs)
        {
            ExpectDataOperation operation = new ExpectDataOperation(0, 0, data, 2, timeoutMs);
            return (ExpectDataResult)Execute(operation);
        }

        public ActionToken ExpectData(byte[] data, int numberOfBytesToCompare, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ExpectDataOperation operation = new ExpectDataOperation(0, 0, data, numberOfBytesToCompare, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionToken ExpectData(byte[] data, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ExpectDataOperation operation = new ExpectDataOperation(0, 0, data, 2, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionToken ExpectData(byte nodeId, byte[] data, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ExpectDataOperation operation = new ExpectDataOperation(0, nodeId, data, 2, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionToken ExpectData(byte nodeId, byte[] data, ExtensionTypes[] extensionTypes, int timeoutMs, Action<IActionItem> completedCallback)
        {
            var dataLen = (data ?? new byte[0]).Length;
            if (dataLen > 2)
            {
                dataLen = 2;
            }
            ExpectDataOperation operation = new ExpectDataOperation(0, nodeId, data, dataLen, extensionTypes, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public RequestDataResult RequestData(byte nodeId, byte[] data, TransmitOptions txOptions, byte[] expectData, int timeoutMs)
        {
            return RequestData(0, nodeId, data, txOptions, expectData, timeoutMs);
        }

        public RequestDataResult RequestData(byte srcNodeId, byte nodeId, byte[] data, TransmitOptions txOptions, byte[] expectData, int timeoutMs)
        {
            RequestDataOperation operation = new RequestDataOperation(srcNodeId, nodeId, data, txOptions, expectData, 2, timeoutMs);
            return (RequestDataResult)Execute(operation);
        }

        public ActionToken RequestData(byte nodeId, byte[] data, TransmitOptions txOptions, byte[] expectData, int timeoutMs, Action<IActionItem> completedCallback)
        {
            RequestDataOperation operation = new RequestDataOperation(0, nodeId, data, txOptions, expectData, 2, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public RequestDataResult RequestData(byte nodeId, byte[] data, TransmitOptions txOptions, byte[] expectData, int timeoutMs, out ActionToken token)
        {
            token = RequestData(nodeId, data, txOptions, expectData, timeoutMs, null);
            return (RequestDataResult)token.WaitCompletedSignal();
        }

        public RequestDataResult RequestDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte[] expectData, int timeoutMs)
        {
            RequestDataExOperation operation = new RequestDataExOperation(0, nodeId, data, txOptions, txSecOptions, scheme, txOptions2, expectData[0], expectData[1], timeoutMs);
            return (RequestDataResult)Execute(operation);
        }

        public ActionToken RequestDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte[] expectData, int timeoutMs, Action<IActionItem> completedCallback)
        {
            RequestDataExOperation operation = new RequestDataExOperation(0, nodeId, data, txOptions, txSecOptions, scheme, txOptions2, expectData[0], expectData[1], timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public RequestDataResult RequestDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte[] expectData, int timeoutMs, out ActionToken token)
        {
            token = RequestDataEx(nodeId, data, txOptions, txSecOptions, scheme, txOptions2, expectData, timeoutMs, null);
            return (RequestDataResult)token.WaitCompletedSignal();
        }

        public ActionToken ListenData(ListenDataDelegate listenCallback)
        {
            ListenDataOperation operation = new ListenDataOperation(listenCallback);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ListenDebugData(ListenDebugDataDelegate listenCallback)
        {
            var operation = new ListenDebugDataOperation(listenCallback);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseData(byte[] data, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataOperation operation = new ResponseDataOperation(data, txOptions, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseData(ResponseDataDelegate receiveCallback, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataOperation operation = new ResponseDataOperation(receiveCallback, txOptions, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseDataEx(byte[] data, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, data, txOptions, TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseDataEx(byte[] data, TransmitOptions txOptions, byte[] expectData, int NumBytesToCompare)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, data, txOptions, TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE, 0, NumBytesToCompare, expectData[0], expectData);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseDataEx(byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2, byte[] expectData)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, data, txOptions, txSecOptions, txOptions2, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseDataEx(byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte[] expectData)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, data, txOptions, txSecOptions, scheme, txOptions2, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseDataEx(ResponseDataDelegate receiveCallback, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, receiveCallback, txOptions, TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY, TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseDataEx(ResponseDataDelegate receiveCallback, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte[] expectData)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, receiveCallback, txOptions, txSecOptions, scheme, txOptions2, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseMultiDataEx(ResponseExDataDelegate receiveCallback, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataExOperation operation = new ResponseDataExOperation(Network, receiveCallback, txOptions, TransmitSecurityOptions.S2_TXOPTION_VERIFY_DELIVERY,
                TransmitOptions2.TRANSMIT_OPTION_2_TRANSPORT_SERVICE, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseMultiData(ResponseExDataDelegate receiveCallback, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataOperation operation = new ResponseDataOperation(receiveCallback, txOptions, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseData(ResponseAchDataDelegate receiveCallback, TransmitOptions txOptions, byte[] expectData)
        {
            ResponseDataOperation operation = new ResponseDataOperation(receiveCallback, txOptions, 0, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken NoiseData(byte nodeId, byte[] data, TransmitOptions txOptions, byte[] expectData, int intervalMs, int timeoutMs)
        {
            NoiseDataOperation operation = null;
            if (expectData != null)
                operation = new NoiseDataOperation(nodeId, data, txOptions, expectData[0], expectData[1], intervalMs, timeoutMs);
            else
                operation = new NoiseDataOperation(nodeId, data, txOptions, intervalMs);
            return ExecuteAsync(operation, null);
        }

        public ActionToken NoiseDataEx(byte nodeId, byte[] data, TransmitOptions txOptions, byte[] expectData, int intervalMs, int timeoutMs, SecuritySchemes securityScheme, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2)
        {
            NoiseDataExOperation operation = null;
            if (expectData != null)
                operation = new NoiseDataExOperation(nodeId, data, txOptions, expectData[0], expectData[1], intervalMs, timeoutMs, securityScheme, txSecOptions, txOptions2);
            else
                operation = new NoiseDataExOperation(nodeId, data, txOptions, intervalMs, securityScheme, txSecOptions, txOptions2);
            return ExecuteAsync(operation, null);
        }

        #region FirmwareUpdateLocal / OTW

        public FirmwareUpdateNvmInitResult FirmwareUpdateNvmInit()
        {
            return (FirmwareUpdateNvmInitResult)Execute(new FirmwareUpdateNvmInitOperation());
        }

        public FirmwareUpdateNvmSetNewImageResult FirmwareUpdateNvmSetNewImage(bool isNewImage)
        {
            return (FirmwareUpdateNvmSetNewImageResult)Execute(new FirmwareUpdateNvmSetNewImageOperation(isNewImage));
        }

        public FirmwareUpdateNvmGetNewImageResult FirmwareUpdateNvmGetNewImage()
        {
            return (FirmwareUpdateNvmGetNewImageResult)Execute(new FirmwareUpdateNvmGetNewImageOperation());
        }

        public FirmwareUpdateNvmUpdateCrc16Result FirmwareUpdateNvmUpdateCrc16(int offset, ushort length, ushort seed)
        {
            return (FirmwareUpdateNvmUpdateCrc16Result)Execute(new FirmwareUpdateNvmUpdateCrc16Operation(offset, length, seed));
        }

        public FirmwareUpdateNvmIsValidCrc16Result FirmwareUpdateNvmIsValidCrc16()
        {
            return (FirmwareUpdateNvmIsValidCrc16Result)Execute(new FirmwareUpdateNvmIsValidCrc16Operation());
        }

        public FirmwareUpdateNvmWriteResult FirmwareUpdateNvmWrite(int offset, ushort length, byte[] buffer)
        {
            return (FirmwareUpdateNvmWriteResult)Execute(new FirmwareUpdateNvmWriteOperation(offset, length, buffer));
        }

        #endregion

        #region NVM Backup/Restore

        public NvmBackupRestoreOpenResult NvmBackupRestoreOpen()
        {
            return (NvmBackupRestoreOpenResult)Execute(new NvmBackupRestoreOpenOperation());
        }

        public NvmBackupRestoreReadResult NvmBackupRestoreRead(byte length, int offset)
        {
            return (NvmBackupRestoreReadResult)Execute(new NvmBackupRestoreReadOperation(length, offset));
        }

        public NvmBackupRestoreWriteResult NvmBackupRestoreWrite(byte length, int offset, byte[] data)
        {
            return (NvmBackupRestoreWriteResult)Execute(new NvmBackupRestoreWriteOperation(length, offset, data));
        }

        public ActionResult NvmBackupRestoreClose()
        {
            return Execute(new NvmBackupRestoreCloseOperation());
        }

        #endregion

        #region SoftReset
        public ActionResult SoftReset()
        {
            return Execute(new SoftResetOperation());
        }

        public ActionToken SoftReset(Action<IActionItem> completedCallback)
        {
            return ExecuteAsync(new SoftResetOperation(), completedCallback);
        }
        #endregion

        public ActionResult SetRoutingMAX(byte maxRouteTries)
        {
            return Execute(new SetRoutingMAXOperation(maxRouteTries));
        }

        public NVRGetValueResult NVRGetValue(byte offset, byte lenght)
        {
            return (NVRGetValueResult)Execute(new NVRGetValueOperation(offset, lenght));
        }

        public ReturnValueResult SetListenBeforeTalkThreshold(byte channel, byte threshhold)
        {
            return (ReturnValueResult)Execute(new SetListenBeforeTalkThresholdOperation(channel, threshhold));
        }

        public ClearNetworkStatsResult ClearNetworkStats()
        {
            return (ClearNetworkStatsResult)Execute(new ClearNetworkStatsOperation());
        }

        public GetNetworkStatsResult GetNetworkStats()
        {
            return (GetNetworkStatsResult)Execute(new GetNetworksStatsOperation());
        }

        public GetTxTimerResult GetTxTimer()
        {
            return (GetTxTimerResult)Execute(new GetTxTimerOperation());
        }

        public ActionResult ClearTxTimers()
        {
            return (ActionResult)Execute(new ClearTxTimersOperation());
        }


        public override string ToString()
        {
            return $"{GetType().Name}^{SessionId:00} ({Library})";
        }

        public ReturnValueResult TestInterfaceSendData(byte[] TestInterfaceCmd, int timeoutMs)
        {
            TestInterfaceSendDataOperation operation = new TestInterfaceSendDataOperation(TestInterfaceCmd, timeoutMs);
            return (ReturnValueResult)Execute(operation);
        }

        public ActionToken TestInterfaceSendData(byte[] TestInterfaceCmd, int timeoutMs, Action<IActionItem> completedCallback)
        {
            TestInterfaceSendDataOperation operation = new TestInterfaceSendDataOperation(TestInterfaceCmd, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionToken ExpectZW(IEnumerable<ByteIndex[]> filter, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ExpectZWOperation operation = new ExpectZWOperation(filter, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionResult WriteZW(byte[] data)
        {
            WriteZWOperation operation = new WriteZWOperation(data);
            return Execute(operation);
        }

        public ExpectZWResult RequestZW(byte[] data, ByteIndex[] mask, int timeoutMs)
        {
            return (ExpectZWResult)Execute(new RequestZWOperation(data, mask, timeoutMs));
        }

        public ReturnValueResult SetSecurityInclusionRequestedKeys(SecuritySchemes[] securitySchemes)
        {
            SetSecurityInclusionRequestedKeysOperation operation = new SetSecurityInclusionRequestedKeysOperation(securitySchemes);
            return (ReturnValueResult)Execute(operation);
        }

        public DefaultTxPowerLevelGetResult GetDefaultTxPowerLevel()
        {
            return (DefaultTxPowerLevelGetResult)Execute(new GetDefaultTxPowerLevelOperation());
        }

        public bool SetDefaultTxPowerLevel(byte normalTxPower, byte measured0dBmPower)
        {
            return Execute(new SetDefaultTxPowerLevelOperation(normalTxPower, measured0dBmPower));
        }

        public RfRegionGetResult GetRfRegion()
        {
            return (RfRegionGetResult)Execute(new GetRfRegionOperation());
        }

        public bool SetRfRegion(RfRegions rfRegion)
        {
            return Execute(new SetRfRegionOperation(rfRegion));
        }
    }
}
