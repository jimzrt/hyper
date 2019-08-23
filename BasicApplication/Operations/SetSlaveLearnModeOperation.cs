using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetSlaveLearnModeOperation : ApiOperation
    {
        public static int TIMEOUT = 60000;

        internal SlaveLearnModes Mode { get; private set; }
        internal byte NodeId { get; private set; }
        private byte TxOptions;
        internal int TimeoutMs { get; private set; }
        internal Action<AssignStatuses> AssignStatusCallback { get; private set; }
        public SetSlaveLearnModeOperation(byte nodeId, SlaveLearnModes mode, Action<AssignStatuses> assignStatusCallback, int timeoutMs)
            : base(true, new CommandTypes[] { CommandTypes.CmdZWaveSetSlaveLearnMode, CommandTypes.CmdZWaveSendSlaveNodeInfo }, true)
        {
            Mode = mode;
            NodeId = nodeId;
            TimeoutMs = timeoutMs;
            AssignStatusCallback = assignStatusCallback;
            if (TimeoutMs <= 0)
                TimeoutMs = TIMEOUT;
        }

        public SetSlaveLearnModeOperation(byte nodeId, SlaveLearnModes mode, Action<AssignStatuses> assignStatusCallback, TransmitOptions txOptions, int timeoutMs)
            : base(true, new CommandTypes[] { CommandTypes.CmdZWaveSetSlaveLearnMode, CommandTypes.CmdZWaveSendSlaveNodeInfo }, true)
        {
            Mode = mode;
            NodeId = nodeId;
            TxOptions = (byte)txOptions;
        }

        private ApiMessage cmStart;
        private ApiMessage cmStop;
        private ApiHandler chOk;
        private ApiHandler chFailed;

        private ApiHandler chAssignComplete;
        private ApiHandler chAssignNodeIdDone;
        private ApiHandler chAssignRangeInfoUpdate;

        private ApiMessage cmSlaveNodeInfo;

        protected override void CreateWorkflow()
        {
            if (Mode == SlaveLearnModes.VirtualSlaveLearnModeAdd || Mode == SlaveLearnModes.VirtualSlaveLearnModeRemove)
            {
                ActionUnits.Add(new StartActionUnit(OnStart, 5000, cmStart));
                ActionUnits.Add(new DataReceivedUnit(chOk, null, TimeoutMs));
                ActionUnits.Add(new DataReceivedUnit(chFailed, SetStateFailed));
                ActionUnits.Add(new DataReceivedUnit(chAssignNodeIdDone, OnAssignNodeIdDone));
            }
            else if (Mode == SlaveLearnModes.VirtualSlaveLearnModeDisable)
            {
                ActionUnits.Add(new StartActionUnit(null, 5000, cmStop));
                ActionUnits.Add(new DataReceivedUnit(chOk, SetStateCompleted));
                ActionUnits.Add(new DataReceivedUnit(chFailed, SetStateFailed));
            }
            else
            {
                ActionUnits.Add(new StartActionUnit(OnStart, 5000, cmStart));
                ActionUnits.Add(new DataReceivedUnit(chOk, null, TimeoutMs, cmSlaveNodeInfo));
                ActionUnits.Add(new DataReceivedUnit(chFailed, SetStateFailed));
                ActionUnits.Add(new DataReceivedUnit(chAssignComplete, OnAssignComplete));
                ActionUnits.Add(new DataReceivedUnit(chAssignNodeIdDone, OnAssignNodeIdDone));
            }
        }

        protected override void CreateInstance()
        {
            cmStart = new ApiMessage(SerialApiCommands[0], NodeId, (byte)Mode);
            cmStart.SetSequenceNumber(SequenceNumber);

            cmStop = new ApiMessage(SerialApiCommands[0], NodeId, (byte)SlaveLearnModes.VirtualSlaveLearnModeDisable);
            cmStop.SetSequenceNumber(SequenceNumber);

            cmSlaveNodeInfo = new ApiMessage(SerialApiCommands[1], NodeId, 0xFF, TxOptions);
            cmSlaveNodeInfo.SetSequenceNumber(SequenceNumber);

            chOk = new ApiHandler(SerialApiCommands[0]);
            chOk.AddConditions(new ByteIndex(0x01));

            chFailed = new ApiHandler(SerialApiCommands[0]);
            chFailed.AddConditions(new ByteIndex(0x00));

            chAssignComplete = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            chAssignComplete.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)AssignStatuses.AssignComplete));

            chAssignNodeIdDone = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            chAssignNodeIdDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)AssignStatuses.AssignNodeIdDone));

            chAssignRangeInfoUpdate = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            chAssignRangeInfoUpdate.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)AssignStatuses.AssignRangeInfoUpdate));

        }

        private void OnStart(StartActionUnit ou)
        {

        }

        private void OnAssignNodeIdDone(DataReceivedUnit ou)
        {
            SpecificResult.NodeId = ou.DataFrame.Payload[3];
            SetStateCompleted(ou);
        }

        private void OnAssignComplete(DataReceivedUnit ou)
        {
            SpecificResult.NodeId = ou.DataFrame.Payload[3];
            SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            return string.Format("Id={0}", SpecificResult.NodeId);
        }

        public SetLearnModeResult SpecificResult
        {
            get { return (SetLearnModeResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SetLearnModeResult();
        }
    }
}
