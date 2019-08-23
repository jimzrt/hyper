using System;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// ZW->HOST: REQ | 0x49 | bStatus | bNodeID | bLen | basic | generic | specific | commandclasses[ ]
    /// 
    /// ApplicationControllerUpdate via the Serial API also have the possibility for receiving 
    /// the status UPDATE_STATE_NODE_INFO_REQ_FAILED, which means that a node did not acknowledge 
    /// a ZW_RequestNodeInfo call.
    /// </summary>
    public class ApplicationControllerUpdateOperation : ApiOperation
    {
        public Action<ApplicationControllerUpdateResult> UpdateCallback { get; set; }
        public ApplicationControllerUpdateOperation(Action<ApplicationControllerUpdateResult> updateCallback)
            : base(false, CommandTypes.CmdApplicationControllerUpdate, false)
        {
            UpdateCallback = updateCallback;
        }

        private ApiHandler applicationControllerUpdateHandler;
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 0));
            ActionUnits.Add(new DataReceivedUnit(applicationControllerUpdateHandler, OnReceived));
        }

        protected override void CreateInstance()
        {
            applicationControllerUpdateHandler = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationControllerUpdate);
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            FillReceived(ou.DataFrame.Payload);
            UpdateCallback(SpecificResult);
        }

        private void FillReceived(byte[] payload)
        {
            if (payload.Length > 0)
            {
                SpecificResult.Status = (ControllerUpdateStatuses)payload[0];
            }
            if (payload.Length > 1)
            {
                SpecificResult.NodeId = payload[1];
            }
            SpecificResult.Payload = payload;
        }

        public ApplicationControllerUpdateResult SpecificResult
        {
            get { return (ApplicationControllerUpdateResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ApplicationControllerUpdateResult();
        }
    }


    public class ExpectControllerUpdateOperation : ApplicationControllerUpdateOperation
    {
        private int TimeoutMs { get; set; }
        private ControllerUpdateStatuses UpdateStatus { get; set; }
        public ExpectControllerUpdateOperation(ControllerUpdateStatuses updateStatus, int timeoutMs)
            : base(null)
        {
            UpdateStatus = updateStatus;
            TimeoutMs = timeoutMs;
            UpdateCallback = OnCallback1;
        }

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, TimeoutMs));
            base.CreateWorkflow();
        }

        private void OnCallback1(ApplicationControllerUpdateResult result)
        {
            if (result.Status == UpdateStatus)
                SetStateCompleted(null);
        }
    }

    public class ApplicationControllerUpdateResult : ActionResult
    {
        public ControllerUpdateStatuses Status { get; set; }
        public byte NodeId { get; set; }
        public byte[] Payload { get; set; }
    }
}
