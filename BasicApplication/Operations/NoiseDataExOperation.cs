using System.Threading;
using ZWave.Enums;
namespace ZWave.BasicApplication.Operations
{
    public class NoiseDataExOperation : ApiOperation
    {
        protected TransmitOptions TxOptions { get; set; }
        protected TransmitOptions2 TxOptions2 { get; set; }
        internal byte NodeId { get; set; }
        internal byte[] Data { get; set; }
        internal byte CmdClass { get; private set; }
        internal byte Cmd { get; private set; }
        internal int IntervalMs { get; private set; }
        internal int TimeoutMs { get; private set; }
        internal TransmitSecurityOptions TxSecOptions { get; private set; }
        internal SecuritySchemes SecurityScheme { get; private set; }

        public NoiseDataExOperation(byte nodeId, byte[] data, TransmitOptions txOptions, byte cmdClass, byte cmd, int intervalMs, int timeoutMs, SecuritySchemes securityScheme, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2)
            : base(false, null, false)
        {
            TxOptions = txOptions;
            NodeId = nodeId;
            Data = data;
            CmdClass = cmdClass;
            Cmd = cmd;
            IntervalMs = intervalMs;
            TimeoutMs = timeoutMs;
            SecurityScheme = securityScheme;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
        }

        public NoiseDataExOperation(byte nodeId, byte[] data, TransmitOptions txOptions, int intervalMs, SecuritySchemes securityScheme, TransmitSecurityOptions txSecOptions, TransmitOptions2 txOptions2)
            : base(false, null, false)
        {
            TxOptions = txOptions;
            NodeId = nodeId;
            Data = data;
            IntervalMs = intervalMs;
            SecurityScheme = securityScheme;
            TxSecOptions = txSecOptions;
            TxOptions2 = txOptions2;
        }

        SendDataExOperation sendFirst;
        ActionCompletedUnit sendCompleted;
        RequestDataExOperation requestFirst;
        ActionCompletedUnit requestCompleted;

        protected override void CreateWorkflow()
        {
            if (CmdClass == 0)
            {
                ActionUnits.Add(new StartActionUnit(null, 0, sendFirst));
                ActionUnits.Add(sendCompleted);
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(null, 0, requestFirst));
                ActionUnits.Add(requestCompleted);
            }
        }

        protected override void CreateInstance()
        {
            sendFirst = new SendDataExOperation(NodeId, Data, TxOptions, TxSecOptions, SecurityScheme, TxOptions2);
            sendFirst.SubstituteSettings = SubstituteSettings;
            sendCompleted = new ActionCompletedUnit(sendFirst, OnSendCompleted);

            requestFirst = new RequestDataExOperation(0, NodeId, Data, TxOptions, TxSecOptions, SecurityScheme, TxOptions2, CmdClass, Cmd, TimeoutMs);
            requestFirst.SubstituteSettings = SubstituteSettings;
            requestCompleted = new ActionCompletedUnit(requestFirst, OnRequestCompleted);
        }

        private void OnSendCompleted(ActionCompletedUnit tu)
        {
            OnOperationCompletedBefore(tu);
            SendDataExOperation op = new SendDataExOperation(NodeId, Data, TxOptions, TxSecOptions, SecurityScheme, TxOptions2);
            OnOperationCompletedAfter(tu, sendCompleted, op);
        }

        private void OnRequestCompleted(ActionCompletedUnit tu)
        {
            OnOperationCompletedBefore(tu);
            RequestDataExOperation op = new RequestDataExOperation(0, NodeId, Data, TxOptions, TxSecOptions, SecurityScheme, TxOptions2, CmdClass, Cmd, TimeoutMs);
            OnOperationCompletedAfter(tu, requestCompleted, op);
        }

        private void OnOperationCompletedBefore(ActionCompletedUnit tu)
        {
            if (IntervalMs > 0)
                Thread.Sleep(IntervalMs);
            SpecificResult.TotalCount++;
            if (!tu.Action.IsStateCompleted)
                SpecificResult.FailCount++;
        }

        private void OnOperationCompletedAfter(ActionCompletedUnit taskUnit, ActionCompletedUnit completedTaskUnit, ApiOperation op)
        {
            op.SubstituteSettings = SubstituteSettings;
            op.NewToken();
            taskUnit.SetNextActionItems(op);
        }

        public NoiseDataResult SpecificResult
        {
            get { return (NoiseDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new NoiseDataResult();
        }
    }
}
