using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class RequestDataExOperation : ApiOperation
    {
        internal byte SrcNodeId { get; set; }
        internal byte DestNodeId { get; set; }
        internal byte[] Data { get; set; }
        internal TransmitOptions TxOptions { get; private set; }
        internal TransmitSecurityOptions TxSecOptions { get; private set; }
        internal SecuritySchemes SecurityScheme { get; private set; }
        internal TransmitOptions2 TxOptions2 { get; private set; }
        private byte CmdClass { get; set; }
        private byte Cmd { get; set; }
        private int TimeoutMs { get; set; }

        public RequestDataExOperation(byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions, TransmitSecurityOptions txSecOptions, SecuritySchemes scheme, TransmitOptions2 txOptions2, byte cmdClass, byte cmd, int timeoutMs)
            : base(false, null, false)
        {
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
            Data = data;
            TxOptions = txOptions;
            TxSecOptions = txSecOptions;
            SecurityScheme = scheme;
            TxOptions2 = txOptions2;
            CmdClass = cmdClass;
            Cmd = cmd;
            TimeoutMs = timeoutMs;
        }

        SendDataExOperation sendData;
        ExpectDataOperation expectData;
        public bool IsSendDataCompleted()
        {
            bool res = false;
            res = sendData != null && sendData.IsStateCompleted;
            return res;
        }

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnStart, 0, expectData, sendData));
            ActionUnits.Add(new ActionCompletedUnit(sendData, OnSendDataComleted));
            ActionUnits.Add(new ActionCompletedUnit(expectData, OnExpectDataComleted));
        }

        private void OnStart(StartActionUnit ou)
        {
            expectData.SrcNodeId = SrcNodeId;
            sendData.NodeId = DestNodeId;
            sendData.Data = Data;
        }

        private void OnSendDataComleted(ActionCompletedUnit ou)
        {
            AddTraceLogItems(ou.Action.Result.TraceLog);
            SpecificResult.TransmitStatus = ((SendDataResult)ou.Action.Result).TransmitStatus;
            SpecificResult.TxSubstituteStatus = ((SendDataResult)ou.Action.Result).SubstituteStatus;
            if (ou.Action.Result.State == ActionStates.Completed)
            {
                if (expectData.Result)
                {
                    SpecificResult.NodeId = expectData.SpecificResult.SrcNodeId;
                    SpecificResult.Command = expectData.SpecificResult.Command;
                    SpecificResult.RxRssi = expectData.SpecificResult.Rssi;
                    SpecificResult.RxSecurityScheme = expectData.SpecificResult.SecurityScheme;
                    SpecificResult.RxSubstituteStatus = expectData.SpecificResult.SubstituteStatus;
                    SetStateCompleted(ou);
                }
            }
            else
            {
                expectData.Token.SetCancelled();
                SetStateFailed(ou);
            }
        }

        private void OnExpectDataComleted(ActionUnit ou)
        {
            AddTraceLogItems(expectData.Result.TraceLog);
            if (expectData.Result)
            {
                SpecificResult.NodeId = expectData.SpecificResult.SrcNodeId;
                SpecificResult.Command = expectData.SpecificResult.Command;
                SpecificResult.RxRssi = expectData.SpecificResult.Rssi;
                SpecificResult.RxSecurityScheme = expectData.SpecificResult.SecurityScheme;
                SpecificResult.RxSubstituteStatus = expectData.SpecificResult.SubstituteStatus;
                SetStateCompleted(ou);
            }
            else
            {
                SetStateExpired(ou);
            }
        }

        protected override void CreateInstance()
        {
            sendData = new SendDataExOperation(DestNodeId, Data, TxOptions, TxSecOptions, SecurityScheme, TxOptions2);
            sendData.SubstituteSettings = SubstituteSettings;
            expectData = new ExpectDataOperation(0, DestNodeId, new byte[] { CmdClass, Cmd }, 2, TimeoutMs);
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
}
