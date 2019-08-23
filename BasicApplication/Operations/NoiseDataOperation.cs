using System.Threading;
using ZWave.Enums;
namespace ZWave.BasicApplication.Operations
{
    public class NoiseDataOperation : ApiOperation
    {
        protected TransmitOptions TxOptions { get; set; }
        internal byte NodeId { get; set; }
        internal byte[] Data { get; set; }
        internal byte CmdClass { get; private set; }
        internal byte Cmd { get; private set; }
        internal int IntervalMs { get; private set; }
        internal int TimeoutMs { get; private set; }

        public NoiseDataOperation(byte nodeId, byte[] data, TransmitOptions txOptions, byte cmdClass, byte cmd, int intervalMs, int timeoutMs)
            : base(false, null, false)
        {
            TxOptions = txOptions;
            NodeId = nodeId;
            Data = data;
            CmdClass = cmdClass;
            Cmd = cmd;
            IntervalMs = intervalMs;
            TimeoutMs = timeoutMs;
        }

        public NoiseDataOperation(byte nodeId, byte[] data, TransmitOptions txOptions, int intervalMs)
            : base(false, null, false)
        {
            TxOptions = txOptions;
            NodeId = nodeId;
            Data = data;
            IntervalMs = intervalMs;
        }

        SendDataOperation sendData;
        RequestDataOperation requestData;

        protected override void CreateWorkflow()
        {
            if (CmdClass == 0)
            {
                ActionUnits.Add(new StartActionUnit(null, 0, sendData));
                ActionUnits.Add(new ActionCompletedUnit(sendData, OnSendCompleted, sendData));
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(null, 0, requestData));
                ActionUnits.Add(new ActionCompletedUnit(requestData, OnRequestCompleted, requestData));
            }
        }

        protected override void CreateInstance()
        {
            sendData = new SendDataOperation(NodeId, Data, TxOptions);
            sendData.SubstituteSettings = SubstituteSettings;

            requestData = new RequestDataOperation(0, NodeId, Data, TxOptions, new[] { CmdClass, Cmd }, 2, TimeoutMs);
            requestData.SubstituteSettings = SubstituteSettings;
        }

        private void OnSendCompleted(ActionCompletedUnit tu)
        {
            OnOperationCompletedBefore(tu);
            sendData.NewToken();
        }

        private void OnRequestCompleted(ActionCompletedUnit tu)
        {
            OnOperationCompletedBefore(tu);
            requestData.NewToken();
        }

        private void OnOperationCompletedBefore(ActionCompletedUnit tu)
        {
            if (IntervalMs > 0)
                Thread.Sleep(IntervalMs);
            SpecificResult.TotalCount++;
            if (!tu.Action.IsStateCompleted)
                SpecificResult.FailCount++;
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

    public class NoiseDataResult : ActionResult
    {
        public int TotalCount { get; set; }
        public int FailCount { get; set; }
    }
}
