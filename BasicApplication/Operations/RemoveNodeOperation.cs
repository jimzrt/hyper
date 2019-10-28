using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class RemoveNodeOperation : ApiOperation
    {
        public static int TIMEOUT = 60000;
        private readonly Modes mInitMode;
        private readonly int TimeoutMs;
        internal Action<NodeStatuses> NodeStatusCallback { get; set; }
        public RemoveNodeOperation(Modes mode, Action<NodeStatuses> nodeStatusCallback, int timeoutMS)
            : base(true, CommandTypes.CmdZWaveRemoveNodeFromNetwork, true)
        {
            mInitMode = mode;
            TimeoutMs = timeoutMS;
            NodeStatusCallback = nodeStatusCallback;
            if (TimeoutMs <= 0)
                TimeoutMs = TIMEOUT;
        }

        public NodeStatuses NodeStatus { get; set; }

        private ApiMessage messageStart;
        private ApiMessage messageStop;
        private ApiMessage messageStopFailed;
        private ApiMessage messageStopDone;

        private ApiHandler handlerLearnReady;
        private ApiHandler handlerFailed;
        private ApiHandler handlerNodeFound;
        private ApiHandler handlerRemovingController;
        private ApiHandler handlerRemovingSlave;
        private ApiHandler handlerDone;
        private ApiHandler handlerNotPrimary;                // code=23


        protected override void CreateWorkflow()
        {

            if (mInitMode != Modes.NodeStop)
            {
                //UniqueCategory = "AddingRemovingNode";
                ActionUnits.Add(new StartActionUnit(OnStart, TimeoutMs, messageStart));
                ActionUnits.Add(new DataReceivedUnit(handlerLearnReady, OnLearnReady));
                ActionUnits.Add(new DataReceivedUnit(handlerFailed, OnFailed, messageStopDone));
                ActionUnits.Add(new DataReceivedUnit(handlerNodeFound, OnNodeFound));
                ActionUnits.Add(new DataReceivedUnit(handlerRemovingSlave, OnRemovingSlave));
                ActionUnits.Add(new DataReceivedUnit(handlerRemovingController, OnRemovingController));
                ActionUnits.Add(new DataReceivedUnit(handlerDone, OnDone, messageStopDone));
                ActionUnits.Add(new DataReceivedUnit(handlerNotPrimary, OnNotPrimary, messageStopDone));

                ActionUnitStop = new ActionUnit(messageStop);
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(null, 0, messageStop));
                ActionUnits.Add(new DataReceivedUnit(handlerDone, OnDone, messageStopDone));
            }
        }

        protected override void CreateInstance()
        {
            messageStart = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)mInitMode });
            messageStart.SetSequenceNumber(SequenceNumber);

            messageStop = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Modes.NodeStop });
            messageStop.SetSequenceNumber(SequenceNumber);

            messageStopFailed = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Modes.NodeStopFailed });
            messageStopFailed.SetSequenceNumber(0); //NULL FuncId

            messageStopDone = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Modes.NodeStop });
            messageStopDone.SetSequenceNumber(0); //NULL FuncId

            handlerDone = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.Done));

            handlerNodeFound = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerNodeFound.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.NodeFound));

            handlerRemovingController = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerRemovingController.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.AddingRemovingController));

            handlerRemovingSlave = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerRemovingSlave.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.AddingRemovingSlave));

            handlerLearnReady = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerLearnReady.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.LearnReady));

            handlerFailed = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerFailed.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.Failed));

            handlerNotPrimary = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            handlerNotPrimary.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.NotPrimary));
        }

        private void OnStart(StartActionUnit ou)
        {
            //mController.ResetNodeStatusSignals();
        }

        private void OnNotPrimary(DataReceivedUnit ou)
        {
            ParseHandler(ou);
            base.SetStateFailing(ou);
        }

        private void OnFailed(DataReceivedUnit ou)
        {
            ParseHandler(ou);
            base.SetStateFailing(ou);
        }

        private void OnRemovingController(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnNodeFound(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnRemovingSlave(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnLearnReady(DataReceivedUnit ou)
        {
            ParseHandler(ou);
        }

        private void OnDone(DataReceivedUnit ou)
        {
            ParseHandler(ou);
            base.SetStateCompleting(ou);
        }

        private void ParseHandler(DataReceivedUnit ou)
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
            NodeStatusCallback(NodeStatus);
        }

        public override string AboutMe()
        {
            return string.Format("Id={0}", SpecificResult.Id);
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
