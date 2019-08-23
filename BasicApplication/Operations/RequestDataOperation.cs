using System;
using Utils;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class RequestDataOperation : ApiOperation
    {
        private int _timeoutMs;
        internal byte SrcNodeId { get; set; }
        internal byte DestNodeId { get; set; }
        internal byte[] Data { get; set; }
        internal int DataDelay { get; set; }
        internal ReceiveStatuses RxStatuses { get; set; }
        internal ReceiveStatuses IgnoreRxStatuses { get; set; }
        internal bool IsFollowup { get; set; }
        internal TransmitOptions TxOptions { get; set; }
        private ByteIndex[] _dataToCompare;
        internal ByteIndex[] DataToCompare
        {
            get { return _dataToCompare; }
            set
            {
                _dataToCompare = value;
                if (ExpectData != null)
                {
                    ExpectData.SetDataToCompare(_dataToCompare);
                }
            }
        }

        public Action SendDataSubstituteCallback { get; set; }

        public RequestDataOperation(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions, byte[] dataToCompare, int bytesToCompare, int timeoutMs)
            : this(srcNodeId, destNodeId, data, txOptions, timeoutMs)
        {
            _dataToCompare = new ByteIndex[bytesToCompare];
            for (int i = 0; i < bytesToCompare; i++)
            {
                _dataToCompare[i] = new ByteIndex(dataToCompare[i]);
            }
        }

        public RequestDataOperation(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions, ByteIndex[] dataToCompare, int timeoutMs)
            : this(srcNodeId, destNodeId, data, txOptions, timeoutMs)
        {
            _dataToCompare = dataToCompare;
        }

        public RequestDataOperation(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions, int timeoutMs)
            : base(false, null, false)
        {
            _timeoutMs = timeoutMs;
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
            Data = data;
            TxOptions = txOptions;
        }

        private SendDataOperation _sendData;
        private SendDataBridgeOperation _sendDataBridge;
        public ExpectDataOperation ExpectData { get; private set; }

        protected override void CreateWorkflow()
        {
            if (SrcNodeId == 0)
            {
                ActionUnits.Add(new StartActionUnit(OnStart, _timeoutMs, ExpectData, _sendData));
                ActionUnits.Add(new ActionCompletedUnit(_sendData, OnSendDataComleted));
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(OnStart, _timeoutMs, ExpectData, _sendDataBridge));
                ActionUnits.Add(new ActionCompletedUnit(_sendDataBridge, OnSendDataComleted));
            }
            ActionUnits.Add(new ActionCompletedUnit(ExpectData, OnExpectDataComleted));
        }

        public void SetNewExpectTimeout(int timeoutMs)
        {
            _timeoutMs = timeoutMs;
        }

        private void OnStart(StartActionUnit ou)
        {
            ExpectData.DestNodeId = SrcNodeId;
            ExpectData.RxStatuses = RxStatuses;
            ExpectData.IgnoreRxStatuses = IgnoreRxStatuses;
            _sendData.NodeId = DestNodeId;
            _sendData.Data = Data;
            _sendData.DataDelay = DataDelay;
            _sendDataBridge.DestNodeId = DestNodeId;
            _sendDataBridge.SrcNodeId = SrcNodeId;
            _sendDataBridge.Data = Data;
        }

        private void OnSendDataComleted(ActionCompletedUnit ou)
        {
            AddTraceLogItems(ou.Action.Result.TraceLog);
            SpecificResult.TransmitStatus = ((SendDataResult)ou.Action.Result).TransmitStatus;
            SpecificResult.TxSubstituteStatus = ((SendDataResult)ou.Action.Result).SubstituteStatus;
            if (ou.Action.Result.State == ActionStates.Completed)
            {
                if (ExpectData.Result)
                {
                    SpecificResult.NodeId = ExpectData.SpecificResult.SrcNodeId;
                    SpecificResult.Command = ExpectData.SpecificResult.Command;
                    SpecificResult.RxRssi = ExpectData.SpecificResult.Rssi;
                    SpecificResult.RxSecurityScheme = ExpectData.SpecificResult.SecurityScheme;
                    SpecificResult.RxSubstituteStatus = ExpectData.SpecificResult.SubstituteStatus;
                    SetStateCompleted(ou);
                }
            }
            else
            {
                ExpectData.Token.SetCancelled();
                SetStateFailed(ou);
            }
        }

        private void OnExpectDataComleted(ActionUnit ou)
        {
            AddTraceLogItems(ExpectData.Result.TraceLog);
            if (ExpectData.Result)
            {
                SpecificResult.NodeId = ExpectData.SpecificResult.SrcNodeId;
                SpecificResult.Command = ExpectData.SpecificResult.Command;
                SpecificResult.RxRssi = ExpectData.SpecificResult.Rssi;
                SpecificResult.RxSecurityScheme = ExpectData.SpecificResult.SecurityScheme;
                SpecificResult.RxSubstituteStatus = ExpectData.SpecificResult.SubstituteStatus;
                SetStateCompleted(ou);
            }
            else
            {
                SetStateExpired(ou);
            }
        }

        protected override void CreateInstance()
        {
            _sendData = new SendDataOperation(DestNodeId, Data, TxOptions) { IsFollowup = IsFollowup };
            _sendData.DataDelay = DataDelay;
            _sendData.SubstituteCallback = SendDataSubstituteCallback;
            _sendData.SubstituteSettings = SubstituteSettings;

            _sendDataBridge = new SendDataBridgeOperation(SrcNodeId, DestNodeId, Data, TxOptions);
            _sendDataBridge.DataDelay = DataDelay;
            _sendDataBridge.SubstituteCallback = SendDataSubstituteCallback;
            _sendDataBridge.SubstituteSettings = SubstituteSettings;
            ExpectData = new ExpectDataOperation(SrcNodeId, DestNodeId, _dataToCompare, _timeoutMs);
        }

        public RequestDataResult SpecificResult
        {
            get { return (RequestDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new RequestDataResult();
        }
    }

    public class RequestDataResult : TransmitResult
    {
        public byte NodeId { get; set; }
        public bool IsBroadcast { get; set; }
        public byte[] Command { get; set; }
        public byte RxRssi { get; set; }
        public SecuritySchemes RxSecurityScheme { get; set; }
        public SubstituteStatuses TxSubstituteStatus { get; set; }
        public SubstituteStatuses RxSubstituteStatus { get; set; }
    }
}
