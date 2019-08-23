using System;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class AddNodeOperation : ApiOperation, IAddRemoveNode
    {
        public static int TIMEOUT = 60000;
        private bool IsModeStopEnabled { get; set; }
        public byte[] NodesBefore { get; set; }
        public byte[] DskValue { get; set; }
        public int GrantSchemesValue { get; set; }
        public byte[] Args { get; set; }
        internal Modes InitMode { get; private set; }
        internal int TimeoutMs { get; set; }
        public Action<NodeStatuses> NodeStatusCallback { get; set; }
        public AddNodeOperation(Modes mode, Action<NodeStatuses> nodeStatusCallback, int timeoutMs, params byte[] args)
            : this(mode, nodeStatusCallback, true, timeoutMs, args)
        { }
        public AddNodeOperation(Modes mode, Action<NodeStatuses> nodeStatusCallback, bool isNodeStopEnabled, int timeoutMs, params byte[] args)
           : base(true, CommandTypes.CmdZWaveAddNodeToNetwork, true)
        {
            IsModeStopEnabled = isNodeStopEnabled;
            IsExclusive = IsModeStopEnabled;
            Args = args;
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
        private ApiMessage messageStopDone;

        private ApiHandler handlerLearnReady;
        private ApiHandler handlerFailed;              // code=7
        private ApiHandler handlerNodeFound;
        protected ApiHandler handlerAddingController;
        protected ApiHandler handlerAddingSlave;
        private ApiHandler handlerProtocolDone;        // code=5
        protected ApiHandler handlerDone;                // code=6 the slaves return this callback for messageStop and for messageStopDone
        private ApiHandler handlerNotPrimary;                // code=23

        protected override void CreateWorkflow()
        {
            if ((InitMode & ~Modes.FlagModes) != Modes.NodeStop || (InitMode & ~Modes.FlagModes) != Modes.NodeStopFailed)
            {
                ActionUnits.Add(new StartActionUnit(OnStart, TimeoutMs, messageStart));
                ActionUnits.Add(new DataReceivedUnit(handlerLearnReady, OnLearnReady));
                ActionUnits.Add(new DataReceivedUnit(handlerNodeFound, OnNodeFound));
                ActionUnits.Add(new DataReceivedUnit(handlerAddingController, OnAddingController));
                ActionUnits.Add(new DataReceivedUnit(handlerAddingSlave, OnAddingSlave));
                if (IsModeStopEnabled)
                {
                    ActionUnits.Add(new DataReceivedUnit(handlerFailed, OnFailed, messageStopDone));
                    ActionUnits.Add(new DataReceivedUnit(handlerProtocolDone, OnProtocolDone, messageStop));
                    ActionUnits.Add(new DataReceivedUnit(handlerDone, OnDone, messageStopDone));
                    ActionUnits.Add(new DataReceivedUnit(handlerNotPrimary, OnNotPrimary, messageStopDone));

                    ActionUnitStop = new ActionUnit(messageStopDone);
                }
                else
                {
                    ActionUnits.Add(new DataReceivedUnit(handlerFailed, OnFailed));
                    ActionUnits.Add(new DataReceivedUnit(handlerProtocolDone, OnDone));
                    ActionUnits.Add(new DataReceivedUnit(handlerNotPrimary, OnNotPrimary));
                }
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(SetStateCompleting, 0, messageStart));
            }
        }

        protected override void CreateInstance()
        {
            if (Args != null && Args.Length > 0)
            {
                byte[] args = new byte[Args.Length + 2];
                args[0] = (byte)InitMode;
                Array.Copy(Args, 0, args, 2, Args.Length);
                messageStart = new ApiMessage(SerialApiCommands[0], args);
                messageStart.SequenceNumberCustomIndex = 3;
            }
            else
            {
                messageStart = new ApiMessage(SerialApiCommands[0], (byte)InitMode);
            }

            if ((InitMode & ~Modes.FlagModes) != Modes.NodeStop || (InitMode & ~Modes.FlagModes) != Modes.NodeStopFailed)
            {
                messageStart.SetSequenceNumber(SequenceNumber);
            }
            else
            {
                messageStart.SetSequenceNumber(0);
            }

            messageStop = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Modes.NodeStop });
            messageStop.SetSequenceNumber(SequenceNumber);

            messageStopFailed = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Modes.NodeStopFailed });
            messageStopFailed.SetSequenceNumber(0); //NULL funcID = 0

            messageStopDone = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Modes.NodeStop });
            messageStopDone.SetSequenceNumber(0); //NULL funcID = 0

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

        private void OnStart(StartActionUnit ou)
        {
            //Controller.ResetNodeStatusSignals();
        }

        private void OnNodeFound(DataReceivedUnit ou)
        {
            ParseHandler(ou);
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

        protected virtual void OnDone(DataReceivedUnit ou)
        {
            ParseHandler(ou);
            base.SetStateCompleting(ou);
        }

        protected void ParseHandler(DataReceivedUnit ou)
        {
            byte[] res = ou.DataFrame.Payload;
            SequenceNumber = res[0];
            NodeStatus = (NodeStatuses)res[1];
            if (NodeStatus == NodeStatuses.AddingRemovingController || NodeStatus == NodeStatuses.AddingRemovingSlave)
            {
                if (NodeStatus == NodeStatuses.AddingRemovingSlave)
                    SpecificResult.IsSlave = true; //set isController bit
                if (res.Length > 2)
                    SpecificResult.Id = res[2];
                if (res.Length > 4)
                    SpecificResult.Basic = res[4];
                if (res.Length > 5)
                    SpecificResult.Generic = res[5];
                if (res.Length > 6)
                    SpecificResult.Specific = res[6];
                if (res.Length > 7)
                    SpecificResult.CommandClasses = res.Skip(7).TakeWhile(x => x != 0xEF).ToArray();

                if (NodesBefore != null && NodesBefore.Contains(SpecificResult.Id))
                {
                    SpecificResult.AddRemoveNodeStatus = AddRemoveNodeStatuses.Replicated;
                }
                else
                {
                    SpecificResult.AddRemoveNodeStatus = AddRemoveNodeStatuses.Added;
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

    public class AddRemoveNodeResult : ActionResult
    {
        public byte Id { get; set; }
        public byte Basic { get; set; }
        public byte Generic { get; set; }
        public byte Specific { get; set; }
        public byte[] CommandClasses { get; set; }
        public bool IsSlave { get; set; }
        public SubstituteStatuses SubstituteStatus { get; set; }
        public byte RoleType { get; set; }
        public AddRemoveNodeStatuses AddRemoveNodeStatus { get; set; }
        public SecuritySchemes[] SecuritySchemes { get; set; }
        internal bool tmpSkipS0 { get; set; }
    }

    public interface IAddRemoveNode
    {
        byte[] DskValue { get; set; }
        int GrantSchemesValue { get; set; }
        AddRemoveNodeResult SpecificResult { get; }
        Action<NodeStatuses> NodeStatusCallback { get; set; }
    }
}
