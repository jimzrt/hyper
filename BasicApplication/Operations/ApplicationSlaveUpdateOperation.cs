using System;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// The Z Wave protocol also calls ApplicationSlaveUpdate when a Node Information Frame has been received
    /// and the protocol is not in a state where it needs the node information.
    /// All slave libraries requires this function implemented within the System layer.
    /// 
    /// ZW->HOST: REQ | 0x49 | bStatus | bNodeID | bLen | basic | generic | specific | commandclasses[ ]
    /// </summary>
    [Obsolete]
    public class ApplicationSlaveUpdateOperation : ApiOperation
    {
        public Action<byte[]> UpdateCallback { get; set; }
        public ApplicationSlaveUpdateOperation()
            : base(false, CommandTypes.CmdApplicationControllerUpdate, false)
        {
        }

        private ApiHandler applicationSlaveUpdateHandler;
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 0));
            ActionUnits.Add(new DataReceivedUnit(applicationSlaveUpdateHandler, OnReceived));
        }

        protected override void CreateInstance()
        {
            applicationSlaveUpdateHandler = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ActionResult();
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            UpdateCallback(ou.DataFrame.Payload);
        }
    }
}
