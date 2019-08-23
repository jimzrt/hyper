using System;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SetLearnModeSlaveOperation : ApiOperation
    {
        public static int TIMEOUT = 60000;

        internal LearnModes Mode { get; private set; }
        internal int TimeoutMs { get; private set; }
        public AssignStatuses AssignStatus { get; set; }
        internal Action<AssignStatuses> AssignStatusCallback { get; set; }
        public SetLearnModeSlaveOperation(LearnModes mode, Action<AssignStatuses> assignStatusCallback, int timeoutMs)
            : base(true, CommandTypes.CmdZWaveSetLearnMode, true)
        {
            Mode = mode;
            TimeoutMs = timeoutMs;
            AssignStatusCallback = assignStatusCallback;
            if (TimeoutMs <= 0)
                TimeoutMs = TIMEOUT;
        }

        private ApiMessage _setLearnModeStart;
        private ApiMessage _setLearnModeStop;
        private IActionItem _stopDelay;

        private ApiHandler _learnReady;
        private ApiHandler _assignComplete;
        private ApiHandler _assignNodeIdDone;
        private ApiHandler _assignRangeInfoUpdate;

        private ApiMessage _exploreRequestInclusion;
        private ApiMessage _exploreRequestExclusion;

        private ApiMessage _sendNodeInfo;
        private ApiHandler _sendNodeInfoRetFail;
        private ApiHandler _sendNodeInfoCallback;

        private bool _isAssignCompleteReceived;

        protected override void CreateWorkflow()
        {
            if (Mode == LearnModes.LearnModeDisable)
            {
                ActionUnits.Add(new StartActionUnit(SetStateCompleting, TimeoutMs, _setLearnModeStop));
            }
            else
            {
                if ((Mode & LearnModes.LearnModeNWE) == LearnModes.LearnModeNWE)
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, _setLearnModeStart, _sendNodeInfo, _exploreRequestExclusion, new RandomTimeInterval(0, 1000, 3000)));
                    ActionUnits.Add(new TimeElapsedUnit(0, null, 0, _exploreRequestExclusion, new RandomTimeInterval(0, 1000, 3000)));
                }
                else if ((Mode & LearnModes.LearnModeNWI) == LearnModes.LearnModeNWI)
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, _setLearnModeStart, _sendNodeInfo, _exploreRequestInclusion, new RandomTimeInterval(0, 1000, 3000)));
                    ActionUnits.Add(new TimeElapsedUnit(0, null, 0, _exploreRequestInclusion, new RandomTimeInterval(0, 1000, 3000)));
                }
                else if ((Mode & (LearnModes.LearnModeSmartStart | LearnModes.NetworkMask)) == (LearnModes.LearnModeSmartStart | LearnModes.NetworkMask))
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, _setLearnModeStart));
                }
                else if ((Mode & LearnModes.LearnModeSmartStart) == LearnModes.LearnModeSmartStart)
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, _setLearnModeStart, _exploreRequestInclusion, new RandomTimeInterval(0, 2000, 4000)));
                    ActionUnits.Add(new TimeElapsedUnit(0, null, 0, _exploreRequestInclusion, new RandomTimeInterval(0, 2000, 4000)));
                }
                else
                {
                    ActionUnits.Add(new StartActionUnit(null, TimeoutMs, _setLearnModeStart, new RandomTimeInterval(0, 500, 1000)));
                    ActionUnits.Add(new TimeElapsedUnit(0, null, 0, _sendNodeInfo));
                }
                ActionUnits.Add(new DataReceivedUnit(_learnReady, OnLearnReady));
                ActionUnits.Add(new DataReceivedUnit(_assignNodeIdDone, OnNodeIdDone));
                ActionUnits.Add(new DataReceivedUnit(_assignComplete, OnAssignComplete, _setLearnModeStop, _stopDelay));
                ActionUnits.Add(new TimeElapsedUnit(22, SetStateCompleted, 0, null));
                ActionUnits.Add(new DataReceivedUnit(_assignRangeInfoUpdate, OnRangeInfoUpdate));

                ActionUnitStop = new ActionUnit(_setLearnModeStop);
            }
        }

        protected override void CreateInstance()
        {
            _isAssignCompleteReceived = false;

            Mode = Mode == LearnModes.NodeAnyS2 ? LearnModes.LearnModeClassic : Mode;
            _setLearnModeStart = new ApiMessage(CommandTypes.CmdZWaveSetLearnMode, new byte[] { (byte)Mode });
            _setLearnModeStart.SetSequenceNumber(SequenceNumber);

            _setLearnModeStop = new ApiMessage(CommandTypes.CmdZWaveSetLearnMode, new byte[] { (byte)LearnModes.LearnModeDisable });
            _setLearnModeStop.SetSequenceNumber(0);

            _stopDelay = new TimeInterval(22, 200);

            _learnReady = new ApiHandler(FrameTypes.Response, CommandTypes.CmdZWaveSetLearnMode);
            _learnReady.AddConditions(new ByteIndex(0x01));

            _assignComplete = new ApiHandler(FrameTypes.Request, CommandTypes.CmdZWaveSetLearnMode);
            _assignComplete.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)AssignStatuses.AssignComplete));

            _assignNodeIdDone = new ApiHandler(FrameTypes.Request, CommandTypes.CmdZWaveSetLearnMode);
            _assignNodeIdDone.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)AssignStatuses.AssignNodeIdDone));

            _assignRangeInfoUpdate = new ApiHandler(FrameTypes.Request, CommandTypes.CmdZWaveSetLearnMode);
            _assignRangeInfoUpdate.AddConditions(ByteIndex.AnyValue, new ByteIndex((byte)AssignStatuses.AssignRangeInfoUpdate));

            _exploreRequestInclusion = new ApiMessage(CommandTypes.CmdZWaveExploreRequestInclusion);
            _exploreRequestInclusion.SetSequenceNumber(SequenceNumber);
            _exploreRequestInclusion.IsNoAck = true;

            _exploreRequestExclusion = new ApiMessage(CommandTypes.CmdZWaveExploreRequestExclusion);
            _exploreRequestExclusion.SetSequenceNumber(SequenceNumber);
            _exploreRequestExclusion.IsNoAck = true;

            _sendNodeInfo = new ApiMessage(CommandTypes.CmdZWaveSendNodeInformation, 0xFF, (byte)TransmitOptions.TransmitOptionNone);
            _sendNodeInfo.SetSequenceNumber(SequenceNumber);

            _sendNodeInfoRetFail = new ApiHandler(FrameTypes.Response, CommandTypes.CmdZWaveSendNodeInformation);
            _sendNodeInfoRetFail.AddConditions(new ByteIndex(0x00));

            _sendNodeInfoCallback = new ApiHandler(FrameTypes.Request, CommandTypes.CmdZWaveSendNodeInformation);
            _sendNodeInfoCallback.AddConditions(ByteIndex.AnyValue, ByteIndex.AnyValue);
        }

        private void OnLearnReady(ActionUnit ou)
        {
            if (_isAssignCompleteReceived)
            {
                SetStateCompleted(ou);
            }
        }

        private void OnNodeIdDone(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload.Length > 2)
            {
                SpecificResult.NodeId = ou.DataFrame.Payload[2];
            }
            AssignStatus = (AssignStatuses)ou.DataFrame.Payload[1];
            AssignStatusCallback(AssignStatus);
        }

        private void OnRangeInfoUpdate(DataReceivedUnit ou)
        {
            AssignStatus = (AssignStatuses)ou.DataFrame.Payload[1];
            AssignStatusCallback(AssignStatus);
        }

        private void OnAssignComplete(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload.Length > 2)
            {
                SpecificResult.NodeId = ou.DataFrame.Payload[2];
            }
            AssignStatus = (AssignStatuses)ou.DataFrame.Payload[1];
            AssignStatusCallback(AssignStatus);
            _isAssignCompleteReceived = true;
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

    public class SetLearnModeResult : ActionResult
    {
        public byte NodeId { get; set; }
        public SecuritySchemes[] SecuritySchemes { get; set; }
        public SubstituteStatuses SubstituteStatus { get; set; }
        public LearnModeStatuses LearnModeStatus { get; set; }
    }

    public enum LearnModeStatuses
    {
        None,
        Added,
        Removed,
        Changed,
        Replicated
    }
}
