using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetLearnModeControllerOperation : ApiOperation
    {
        public static int TIMEOUT = 60000;

        internal LearnModes Mode { get; set; }
        internal int TimeoutMs { get; set; }
        public NodeStatuses LearnStatus { get; set; }
        internal Action<NodeStatuses> NodeStatusCallback { get; set; }
        public SetLearnModeControllerOperation(LearnModes mode, Action<NodeStatuses> nodeStatusCallback, int timeoutMs)
            : base(true, CommandTypes.CmdZWaveSetLearnMode, true)
        {
            Mode = mode;
            TimeoutMs = timeoutMs;
            NodeStatusCallback = nodeStatusCallback;
            if (TimeoutMs <= 0)
                TimeoutMs = TIMEOUT;
        }

        private ApiMessage cmZWaveSetLearnModeStart;
        private ApiMessage cmZWaveExploreRequestInclusion;
        private ApiMessage cmZWaveExploreRequestExclusion;
        protected ApiMessage cmZWaveSetLearnModeStop;

        private ApiHandler chZWaveSetLearnModeStarted;
        private ApiHandler chZWaveSetLearnModeDone;
        private ApiHandler chZWaveSetLearnModeFailed;


        protected override void CreateWorkflow()
        {
            if (Mode == LearnModes.LearnModeDisable)
            {
                ActionUnits.Add(new StartActionUnit(SetStateCompleting, TimeoutMs, cmZWaveSetLearnModeStop));
            }
            else
            {
                if ((Mode & LearnModes.LearnModeNWE) == LearnModes.LearnModeNWE)
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, cmZWaveSetLearnModeStart, cmZWaveExploreRequestExclusion, new RandomTimeInterval(0, 1000, 3000)));
                    ActionUnits.Add(new TimeElapsedUnit(0, OnTimeElapsed, 0, cmZWaveExploreRequestExclusion, new RandomTimeInterval(0, 1000, 3000)));
                }
                else if ((Mode & LearnModes.LearnModeNWI) == LearnModes.LearnModeNWI)
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, cmZWaveSetLearnModeStart, cmZWaveExploreRequestInclusion, new RandomTimeInterval(0, 1000, 3000)));
                    ActionUnits.Add(new TimeElapsedUnit(0, OnTimeElapsed, 0, cmZWaveExploreRequestInclusion, new RandomTimeInterval(0, 1000, 3000)));
                }
                else
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, cmZWaveSetLearnModeStart));
                }
                ActionUnits.Add(new DataReceivedUnit(chZWaveSetLearnModeStarted, OnStarted));
                ActionUnits.Add(new DataReceivedUnit(chZWaveSetLearnModeFailed, SetStateFailing, cmZWaveSetLearnModeStop));
                ActionUnits.Add(new DataReceivedUnit(chZWaveSetLearnModeDone, OnDone, cmZWaveSetLearnModeStop));

                ActionUnitStop = new ActionUnit(cmZWaveSetLearnModeStop);
            }
        }

        protected override void CreateInstance()
        {
            cmZWaveSetLearnModeStart = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)Mode });
            cmZWaveSetLearnModeStart.SetSequenceNumber(SequenceNumber);

            cmZWaveSetLearnModeStop = new ApiMessage(SerialApiCommands[0], new byte[] { (byte)LearnModes.LearnModeDisable });
            cmZWaveSetLearnModeStop.SetSequenceNumber(SequenceNumber);

            cmZWaveExploreRequestInclusion = new ApiMessage(CommandTypes.CmdZWaveExploreRequestInclusion);
            cmZWaveExploreRequestInclusion.SetSequenceNumber(SequenceNumber);

            cmZWaveExploreRequestExclusion = new ApiMessage(CommandTypes.CmdZWaveExploreRequestExclusion);
            cmZWaveExploreRequestExclusion.SetSequenceNumber(SequenceNumber);

            chZWaveSetLearnModeStarted = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            chZWaveSetLearnModeStarted.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.LearnReady));

            chZWaveSetLearnModeDone = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            chZWaveSetLearnModeDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.Done));

            chZWaveSetLearnModeFailed = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            chZWaveSetLearnModeFailed.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)NodeStatuses.Failed));
        }

        private void OnTimeElapsed(TimeElapsedUnit ou)
        {

        }

        private void OnStarted(DataReceivedUnit ou)
        {
            LearnStatus = (NodeStatuses)ou.DataFrame.Payload[1];
            NodeStatusCallback(LearnStatus);
        }

        private void OnDone(DataReceivedUnit ou)
        {
            LearnStatus = (NodeStatuses)ou.DataFrame.Payload[1];
            NodeStatusCallback(LearnStatus);
            if (ou.DataFrame.Payload.Length > 2)
            {
                SpecificResult.NodeId = ou.DataFrame.Payload[2];
            }
            base.SetStateCompleting(ou);
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
