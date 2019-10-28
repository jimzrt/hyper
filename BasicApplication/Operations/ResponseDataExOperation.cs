using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class ResponseDataExOperation : ApiAchOperation
    {
        private readonly NetworkViewPoint _network;
        internal List<byte[]> Data { get; set; }
        internal TransmitOptions TxOptions { get; private set; }
        internal TransmitOptions2 TxOptions2 { get; private set; }
        internal TransmitSecurityOptions TxSecOptions { get; private set; }
        internal SecuritySchemes SecurityScheme { get; private set; }
        internal bool IsSecuritySchemeSpecified { get; private set; }
        public ResponseDataDelegate ReceiveCallback { get; private set; }
        public ResponseExDataDelegate ReceiveExCallback { get; private set; }

        public ResponseDataExOperation(NetworkViewPoint network, ResponseDataDelegate receiveCallback, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2, byte destNodeId, byte cmdClass, byte cmd)
            : base(destNodeId, 0, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            _network = network;
            ReceiveCallback = receiveCallback;
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
        }

        public ResponseDataExOperation(NetworkViewPoint network, ResponseDataDelegate receiveCallback, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte destNodeId, byte cmdClass, byte cmd)
            : base(destNodeId, 0, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            _network = network;
            ReceiveCallback = receiveCallback;
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            SecurityScheme = scheme;
            IsSecuritySchemeSpecified = true;
            TxOptions2 = txOptions2;
        }

        public ResponseDataExOperation(NetworkViewPoint network, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2, byte destNodeId, byte cmdClass, byte cmd)
            : base(destNodeId, 0, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            _network = network;
            Data = new List<byte[]>();
            Data.Add(data);
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
        }

        public ResponseDataExOperation(NetworkViewPoint network, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions,
            TransmitOptions2 txOptions2, byte destNodeId, int NumBytesToCompare
            , byte cmdClass, byte[] cmd)
            : base(destNodeId, 0, cmd, NumBytesToCompare)
        {
            _network = network;
            Data = new List<byte[]>();
            Data.Add(data);
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
        }

        public ResponseDataExOperation(NetworkViewPoint network, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte destNodeId, byte cmdClass, byte cmd)
            : base(destNodeId, 0, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            _network = network;
            Data = new List<byte[]>();
            Data.Add(data);
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
            SecurityScheme = scheme;
            IsSecuritySchemeSpecified = true;
        }

        public ResponseDataExOperation(NetworkViewPoint network, ResponseExDataDelegate receiveCallback, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2, byte destNodeId, byte cmdClass, byte cmd)
            : base(destNodeId, 0, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            _network = network;
            ReceiveExCallback = receiveCallback;
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
        }

        public ResponseDataExOperation(NetworkViewPoint network, List<byte[]> data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte destNodeId, byte cmdClass, byte cmd)
            : base(destNodeId, 0, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            _network = network;
            Data = data;
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            SecurityScheme = scheme;
            TxOptions2 = txOptions2;
            IsSecuritySchemeSpecified = true;
        }

        private byte handlingRequestFromNode = 0;
        private static readonly byte[] emptyArray = new byte[0];
        private byte[] handlingRequest = emptyArray;
        protected override void OnHandledInternal(DataReceivedUnit ou)
        {
            byte nodeId = ReceivedAchData.SrcNodeId;
            byte[] cmd = ReceivedAchData.Command;
            if (handlingRequestFromNode != nodeId || !handlingRequest.SequenceEqual(cmd))
            {
                handlingRequestFromNode = nodeId;
                handlingRequest = cmd;
                if (ReceiveCallback != null)
                {
                    byte[] data = ReceiveCallback(ReceivedAchData.Options, ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, ReceivedAchData.Command);
                    if (data != null && data.Length > 0)
                    {
                        Data = new List<byte[]>();
                        Data.Add(data);
                    }
                    else
                    {
                        Data = null;
                    }
                }
                else if (ReceiveExCallback != null)
                {
                    Data = ReceiveExCallback(ReceivedAchData.Options, ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, ReceivedAchData.Command);
                }
                ou.SetNextActionItems();
                List<ActionBase> nextOperations = new List<ActionBase>();
                if (Data != null)
                {
                    var scheme = IsSecuritySchemeSpecified ? SecurityScheme : (SecuritySchemes)ReceivedAchData.SecurityScheme;
                    foreach (var command in Data)
                    {
                        bool isSuportedScheme = IsSupportedScheme(_network, command, scheme);
                        if (command != null && command.Length > 1 && isSuportedScheme)
                        {
                            CallbackApiOperation operation = null;
                            operation = new SendDataExOperation(ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, command, TxOptions, TxSecOptions, scheme, TxOptions2);
                            operation.SubstituteSettings = new SubstituteSettings(SubstituteSettings.SubstituteFlags, SubstituteSettings.MaxBytesPerFrameSize);
                            if (ReceivedAchData.SubstituteIncomingFlags.HasFlag(SubstituteIncomingFlags.Crc16Encap))
                            {
                                operation.SubstituteSettings.SetFlag(SubstituteFlags.UseCrc16Encap);
                            }
                            nextOperations.Add(operation);
                        }
                    }
                }
                if (nextOperations.Count > 0)
                {
                    var next = new ActionSerialGroup(nextOperations.ToArray());
                    next.CompletedCallback = (x) =>
                    {
                        var action = x as ActionBase;
                        if (action != null)
                        {
                            handlingRequestFromNode = 0;
                            handlingRequest = emptyArray;
                            SpecificResult.TotalCount++;
                            if (action.Result.State != ActionStates.Completed)
                                SpecificResult.FailCount++;
                        }
                    };
                    ou.SetNextActionItems(next);
                }
                else
                {
                    handlingRequestFromNode = 0;
                    handlingRequest = emptyArray;
                }
            }
        }

        public ResponseDataResult SpecificResult
        {
            get { return (ResponseDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ResponseDataResult();
        }
    }
}
