using System;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Operations;
using ZWave.Enums;
using ZWave.Layers;

namespace ZWave.BasicApplication.Devices
{
    public class BridgeController : Controller
    {
        internal BridgeController(byte sessionId, ISessionClient sc, IFrameClient fc, ITransportClient tc)
            : base(sessionId, sc, fc, tc)
        { }

        public GetVirtualNodesResult GetVirtualNodes()
        {
            return (GetVirtualNodesResult)Execute(new GetVirtualNodesOperation());
        }

        public IsVirtualNodeResult IsVirtualNode(byte nodeId)
        {
            return (IsVirtualNodeResult)Execute(new IsVirtualNodeOperation(nodeId));
        }

        public SendDataResult SendDataBridge(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions transmitOptions)
        {
            return (SendDataResult)Execute(new SendDataBridgeOperation(srcNodeId, destNodeId, data, transmitOptions));
        }

        public ActionToken SendDataBridge(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions transmitOptions, Action<IActionItem> completedCallback)
        {
            SendDataBridgeOperation operation = new SendDataBridgeOperation(srcNodeId, destNodeId, data, transmitOptions);
            return ExecuteAsync(operation, completedCallback);
        }

        public TransmitResult SendDataMetaBridge(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions transmitOptions)
        {
            return (TransmitResult)Execute(new SendDataMetaBridgeOperation(srcNodeId, destNodeId, data, transmitOptions));
        }

        public TransmitResult SendDataMultiBridge(byte srcNodeId, byte[] receiverNodeIds, byte[] data, TransmitOptions transmitOptions)
        {
            return (TransmitResult)Execute(new SendDataMultiBridgeOperation(srcNodeId, receiverNodeIds, data, transmitOptions));
        }

        public TransmitResult SendDataMultiBridge(byte srcNodeId, byte[] receiverNodeIds, byte[] data, TransmitOptions txOptions, out ActionToken token)
        {
            return SendDataMultiBridge(srcNodeId, receiverNodeIds, data, txOptions, new SubstituteSettings(), out token);
        }

        public TransmitResult SendDataMultiBridge(byte srcNodeId, byte[] receiverNodeIds, byte[] data, TransmitOptions txOptions, SubstituteSettings substituteSettings, out ActionToken token)
        {
            token = SendDataMultiBridge(srcNodeId, receiverNodeIds, data, txOptions, substituteSettings, null);
            return (TransmitResult)token.WaitCompletedSignal();
        }

        public ActionToken SendDataMultiBridge(byte srcNodeId, byte[] receiverNodeIds, byte[] data, TransmitOptions transmitOptions, Action<IActionItem> completedCallback)
        {
            return SendDataMultiBridge(srcNodeId, receiverNodeIds, data, transmitOptions, new SubstituteSettings(), completedCallback);
        }

        public ActionToken SendDataMultiBridge(byte srcNodeId, byte[] receiverNodeIds, byte[] data, TransmitOptions transmitOptions, SubstituteSettings substituteSettings, Action<IActionItem> completedCallback)
        {
            SendDataMultiBridgeOperation operation = new SendDataMultiBridgeOperation(srcNodeId, receiverNodeIds, data, transmitOptions)
            {
                SubstituteSettings = substituteSettings
            };
            return ExecuteAsync(operation, completedCallback);
        }

        public TransmitResult SendSlaveNodeInformation(byte srcNodeId, byte destNodeId, TransmitOptions transmitOptions)
        {
            return (TransmitResult)Execute(new SendSlaveNodeInformationOperation(srcNodeId, destNodeId, transmitOptions));
        }

        public SetLearnModeResult SetSlaveLearnMode(byte nodeId, SlaveLearnModes mode, int timeoutMs)
        {
            return (SetLearnModeResult)Execute(new SetSlaveLearnModeOperation(nodeId, mode, SetAssignStatusSignal, timeoutMs));
        }

        public ActionToken SetSlaveLearnMode(byte nodeId, SlaveLearnModes mode, int timeoutMs, Action<IActionItem> completedCallback)
        {
            SetSlaveLearnModeOperation oper = new SetSlaveLearnModeOperation(nodeId, mode, SetAssignStatusSignal, timeoutMs);
            return ExecuteAsync(oper, completedCallback);
        }

        public ExpectDataResult ExpectData(byte destNodeId, byte[] data, int timeoutMs)
        {
            ExpectDataOperation operation = new ExpectDataOperation(destNodeId, 0, data, 2, timeoutMs);
            return (ExpectDataResult)Execute(operation);
        }

        public new ActionToken ExpectData(byte destNodeId, byte[] data, int timeoutMs, Action<IActionItem> completedCallback)
        {
            ExpectDataOperation operation = new ExpectDataOperation(destNodeId, 0, data, 2, timeoutMs);
            return ExecuteAsync(operation, completedCallback);
        }

        public ActionToken ResponseData(byte[] data, TransmitOptions txOptions, byte destNodeId, byte[] expectData)
        {
            ResponseDataOperation operation = new ResponseDataOperation(data, txOptions, destNodeId, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionToken ResponseData(ResponseDataDelegate receiveCallback, TransmitOptions txOptions, byte destNodeId, byte[] expectData)
        {
            ResponseDataOperation operation = new ResponseDataOperation(receiveCallback, txOptions, destNodeId, expectData[0], expectData[1]);
            return ExecuteAsync(operation, null);
        }

        public ActionResult ApplicationSlaveNodeInformation(bool isListening, byte generic, byte specific, byte[] cmdClasses)
        {
            return ApplicationSlaveNodeInformation(0, isListening, generic, specific, cmdClasses);
        }

        public ActionResult ApplicationSlaveNodeInformation(byte slaveNodeId, bool isListening, byte generic, byte specific, byte[] cmdClasses)
        {
            return Execute(new ApplicationSlaveNodeInformationOperation(slaveNodeId, isListening, generic, specific, cmdClasses));
        }
    }
}
