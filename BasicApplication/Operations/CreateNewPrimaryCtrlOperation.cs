using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class CreateNewPrimaryCtrlOperation : ApiOperation
    {
        public static int TIMEOUT = 60000;

        public CreatePrimaryModes InitMode { get; set; }
        internal int TimeoutMs { get; set; }
        internal Action<NodeStatuses> NodeStatusCallback { get; set; }
        public CreateNewPrimaryCtrlOperation(CreatePrimaryModes mode, Action<NodeStatuses> nodeStatusCallback, int timeoutMs)
            : base(true, CommandTypes.CmdZWaveCreateNewPrimary, true)
        {
            InitMode = mode;
            TimeoutMs = timeoutMs;
            NodeStatusCallback = nodeStatusCallback;
            if (TimeoutMs <= 0)
                TimeoutMs = TIMEOUT;
        }

        public NodeStatuses NodeStatus { get; set; }

        private ApiMessage messageStart;
        private ApiMessage messageStop;
        private ApiMessage messageStopFailed;

        private ApiHandler handlerLearnReady;
        private ApiHandler handlerFailed;              // code=7
        private ApiHandler handlerNodeFound;
        private ApiHandler handlerAddingController;
        private ApiHandler handlerAddingSlave;
        private ApiHandler handlerProtocolDone;        // code=5
        private ApiHandler handlerDone;                // code=6 
        private ApiHandler handlerNotPrimary;                // code=23


        protected override void CreateWorkflow()
        {
            if (InitMode == CreatePrimaryModes.Start)
            {
                ActionUnits.Add(new StartActionUnit(null, TimeoutMs, messageStart));
                ActionUnits.Add(new DataReceivedUnit(handlerLearnReady, OnLearnReady));
                ActionUnits.Add(new DataReceivedUnit(handlerFailed, OnFailed, messageStopFailed));
                ActionUnits.Add(new DataReceivedUnit(handlerNodeFound, OnNodeFound));
                ActionUnits.Add(new DataReceivedUnit(handlerAddingController, OnAddingController));
                ActionUnits.Add(new DataReceivedUnit(handlerAddingSlave, OnAddingSlave));
                ActionUnits.Add(new DataReceivedUnit(handlerProtocolDone, OnProtocolDone, messageStop));
                ActionUnits.Add(new DataReceivedUnit(handlerDone, OnDone));
                ActionUnits.Add(new DataReceivedUnit(handlerNotPrimary, OnNotPrimary));
            }
            else if (InitMode == CreatePrimaryModes.Stop)
            {
                ActionUnits.Add(new StartActionUnit(null, 0, messageStop));
                ActionUnits.Add(new DataReceivedUnit(handlerDone, OnDone));
            }
            else
                base.SetStateCompleted(null);
        }

        protected override void CreateInstance()
        {
            messageStart = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)InitMode });
            messageStart.SetSequenceNumber(SequenceNumber);

            messageStop = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)CreatePrimaryModes.Stop });
            messageStop.SetSequenceNumber(SequenceNumber);

            messageStopFailed = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)CreatePrimaryModes.StopFailed });
            messageStopFailed.SetSequenceNumber(0);//remove SequenceNumber (use zero) from payload

            handlerProtocolDone = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerProtocolDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.ProtocolDone));

            handlerDone = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.Done));

            handlerNodeFound = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerNodeFound.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.NodeFound));

            handlerAddingController = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerAddingController.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.AddingRemovingController));

            handlerAddingSlave = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerAddingSlave.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.AddingRemovingSlave));

            handlerLearnReady = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerLearnReady.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.LearnReady));

            handlerFailed = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerFailed.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.Failed));

            handlerNotPrimary = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerNotPrimary.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.NotPrimary));
        }

        private void OnNotPrimary(DataReceivedUnit ou)
        {
            ParseHandler(ou);
            base.SetStateFailed(ou);
        }

        private void OnNodeFound(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnFailed(DataReceivedUnit ou)
        {
            ParseHandler(ou);
            base.SetStateFailed(ou);
        }

        private void OnAddingController(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnAddingSlave(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnLearnReady(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnProtocolDone(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnDone(DataReceivedUnit ou)
        {
            //((ApiHandler)ou.CommandHandler).ReceivedFrame.IsHandled = true;
            ParseHandler(ou);
            base.SetStateCompleted(ou);
        }

        protected void ParseHandler(DataReceivedUnit ou)
        {
            byte[] res = ou.DataFrame.Payload;
            SequenceNumber = res[0];
            NodeStatus = (NodeStatuses)res[1];

            if (NodeStatus == NodeStatuses.AddingRemovingController || NodeStatus == NodeStatuses.AddingRemovingSlave)
            {
                if (res.Length > 2)
                    SpecificResult.Id = res[2];

                byte nodeInfoLength = 0;

                if (res.Length > 3)
                    nodeInfoLength = res[3];

                for (int i = 0; i < nodeInfoLength; i++)
                {
                    byte value = res[4 + i];

                    if (nodeInfoLength > 3)
                        SpecificResult.CommandClasses = new byte[nodeInfoLength - 3];

                    switch (i)
                    {
                        case 0:
                            SpecificResult.Basic = value;
                            break;
                        case 1:
                            SpecificResult.Generic = value;
                            break;
                        case 2:
                            SpecificResult.Specific = value;
                            break;
                        default:
                            SpecificResult.CommandClasses[i - 3] = value;
                            break;
                    }
                }
            }
        }

        public AddRemoveNodeResult SpecificResult
        {
            get { return (AddRemoveNodeResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new AddRemoveNodeResult();
        }
    }
}
