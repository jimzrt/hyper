using System.Collections.Generic;
using System.Linq;
using Utils;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public delegate byte[] ResponseDataDelegate(byte options, byte destNodeId, byte srcNodeId, byte[] data);
    public delegate List<byte[]> ResponseAchDataDelegate(AchData achData);
    public delegate List<byte[]> ResponseExDataDelegate(byte options, byte destNodeId, byte srcNodeId, byte[] data);
    public class ResponseDataOperation : ApiAchOperation
    {
        internal List<byte[]> Data { get; set; }
        internal TransmitOptions TxOptions { get; private set; }
        public ResponseDataDelegate ReceiveCallback { get; private set; }
        public ResponseAchDataDelegate ReceiveAchDataCallback { get; private set; }
        public ResponseExDataDelegate ReceiveExCallback { get; private set; }

        public ResponseDataOperation(ResponseAchDataDelegate receiveAchDataCallback, TransmitOptions txOptions, byte srcNodeId, byte cmdClass, byte cmd)
            : base(0, srcNodeId, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            ReceiveAchDataCallback = receiveAchDataCallback;
            TxOptions = txOptions;
        }

        public ResponseDataOperation(ResponseDataDelegate receiveCallback, TransmitOptions txOptions, byte srcNodeId, byte cmdClass, byte cmd)
            : base(0, srcNodeId, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            ReceiveCallback = receiveCallback;
            TxOptions = txOptions;
        }

        public ResponseDataOperation(byte[] data, TransmitOptions txOptions, byte srcNodeId, byte cmdClass, byte cmd)
            : base(0, srcNodeId, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            Data = new List<byte[]>();
            Data.Add(data);
            TxOptions = txOptions;
        }

        public ResponseDataOperation(ResponseExDataDelegate receiveCallback, TransmitOptions txOptions, byte srcNodeId, byte cmdClass, byte cmd)
            : base(0, srcNodeId, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            ReceiveExCallback = receiveCallback;
            TxOptions = txOptions;
        }

        public ResponseDataOperation(List<byte[]> data, TransmitOptions txOptions, byte srcNodeId, byte cmdClass, byte cmd)
            : base(0, srcNodeId, new ByteIndex(cmdClass), new ByteIndex(cmd))
        {
            Data = data;
            TxOptions = txOptions;
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
                }
                else if (ReceiveExCallback != null)
                {
                    Data = ReceiveExCallback(ReceivedAchData.Options, ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, ReceivedAchData.Command);
                }
                else if (ReceiveAchDataCallback != null)
                {
                    Data = ReceiveAchDataCallback(ReceivedAchData);
                }
                if (Data != null)
                {
                    if (ReceivedAchData.DestNodeId == 0)
                    {
                        ApiOperation[] operations = new ApiOperation[Data.Count];
                        for (int i = 0; i < Data.Count; i++)
                        {
                            operations[i] = new SendDataOperation(ReceivedAchData.SrcNodeId, Data[i], TxOptions);
                            operations[i].SubstituteSettings = SubstituteSettings;
                            operations[i].CompletedCallback = (x) =>
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
                        }
                        ou.SetNextActionItems(operations);
                    }
                    else
                    {
                        ApiOperation[] operations = new ApiOperation[Data.Count];
                        for (int i = 0; i < Data.Count; i++)
                        {
                            operations[i] = new SendDataBridgeOperation(ReceivedAchData.DestNodeId, ReceivedAchData.SrcNodeId, Data[i], TxOptions);
                            operations[i].SubstituteSettings = SubstituteSettings;
                            operations[i].CompletedCallback = (x) =>
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
                        }
                        ou.SetNextActionItems(operations);
                    }
                }
                else
                {
                    handlingRequestFromNode = 0;
                    handlingRequest = emptyArray;
                    ou.SetNextActionItems();
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

    public class ResponseDataResult : ActionResult
    {
        public int TotalCount { get; set; }
        public int FailCount { get; set; }
    }
}
