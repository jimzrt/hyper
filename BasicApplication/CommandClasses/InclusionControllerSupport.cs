using ZWave.BasicApplication.Operations;
using ZWave.CommandClasses;
using ZWave.Enums;

namespace ZWave.BasicApplication.CommandClasses
{
    public class InclusionControllerSupport : ApiAchOperation
    {
        public InclusionControllerSupport()
            : base(0, 0, null)
        {
        }
    }

    public class InclusionController
    {
        public class Initiate : RequestDataOperation
        {
            private static readonly byte[] _dataToSend = new COMMAND_CLASS_INCLUSION_CONTROLLER.INITIATE();
            public byte NodeId { get; private set; }
            public byte StepId { get; private set; }

            public Initiate(byte srcNodeId, byte destNodeId, TransmitOptions txOptions, int timeoutMs)
                : base(srcNodeId, destNodeId, _dataToSend, txOptions, new COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE(), 2, timeoutMs)
            {
            }

            public void SetCommandParameters(byte nodeId, byte stepId)
            {
                NodeId = nodeId;
                StepId = stepId;
                var internalData = (COMMAND_CLASS_INCLUSION_CONTROLLER.INITIATE)Data;
                internalData.nodeId = nodeId;
                internalData.stepId = stepId;
                Data = internalData;
            }
        }

        public class Complete : SendDataOperation
        {
            private static readonly byte[] _dataToSend = new COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE();
            public byte StatusComplete { get; private set; }
            public byte StepId { get; private set; }

            public Complete(byte nodeId, TransmitOptions txOptions)
                : base(nodeId, _dataToSend, txOptions)
            {
            }

            public void SetCommandParameters(byte statusComplete, byte stepId)
            {
                StatusComplete = statusComplete;
                StepId = stepId;
                var internalData = (COMMAND_CLASS_INCLUSION_CONTROLLER.COMPLETE)Data;
                internalData.status = statusComplete;
                internalData.stepId = stepId;
                Data = internalData;
            }
        }
    }
}
