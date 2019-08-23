using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace ZWave.Security
{
    public enum MpanUsageStates
    {
        Used = 0,
        Mos,
        MosReported
    }

    public class MpanContainer
    {
        private MpanUsageStates _mpanUsageState;
        public bool IsMosState { get { return _mpanUsageState == MpanUsageStates.Mos; } }

        public bool IsMosReportedState { get { return _mpanUsageState == MpanUsageStates.MosReported; } }

        public NodeGroupId NodeGroupId { get; private set; }

        public byte SequenceNumber { get; private set; }

        private HashSet<byte> _receiverGroupHandle;
        public byte[] ReceiverGroupHandle
        {
            get { return _receiverGroupHandle.ToArray(); }
        }

        public void SetReceiverGroupHandle(byte[] receivers)
        {
            _receiverGroupHandle = new HashSet<byte>(receivers);
        }

        private BigInteger _mpanState;
        public byte[] MpanState { get { return _mpanState.GetBytes(); } }

        private DateTime _receivedTimeStamp;
        public DateTime ReceivedTimeStamp { get { return _receivedTimeStamp; } }

        public MpanContainer(NodeGroupId nodeGroupId, byte[] mpanState, byte sequenceNumber, byte[] receiverGroupHandle)
        {
            SequenceNumber = sequenceNumber;
            NodeGroupId = nodeGroupId;
            SetMpanState(mpanState ?? new byte[16]);
            if (!receiverGroupHandle.IsNullOrEmpty())
                _receiverGroupHandle = new HashSet<byte>(receiverGroupHandle);
            else
                _receiverGroupHandle = new HashSet<byte>();
        }

        public void IncrementMpanState()
        {
            if (IsMosState)
                throw new InvalidOperationException("container is in MOS state");

            _mpanState.Increment();
        }

        public void DecrementMpanState()
        {
            if (IsMosState)
                throw new InvalidOperationException("container is in MOS state");

            _mpanState.Decrement();
        }

        public void SetMpanState(byte[] mpanState)
        {
            if (IsMosState)
                throw new InvalidOperationException("container is in MOS state");

            _mpanState = new BigInteger(mpanState);
            _mpanUsageState = MpanUsageStates.Used;
            SetReceivedTimeStamp();
        }

        public byte UpdateSequenceNumber()
        {
            if (IsMosState)
                throw new InvalidOperationException("container is in MOS state");

            return ++SequenceNumber;
        }

        public void SetSequenceNumber(byte sequenceNumber)
        {
            SequenceNumber = sequenceNumber;
        }

        public void SetMosState(bool isMos)
        {
            _mpanUsageState = isMos ? MpanUsageStates.Mos : MpanUsageStates.Used;
        }

        public void SetMosStateReported()
        {
            if (_mpanUsageState != MpanUsageStates.Mos)
                throw new InvalidOperationException("container must be in MOS state");

            _mpanUsageState = MpanUsageStates.MosReported;
        }

        public bool DestNodesEquals(byte[] nodeIds)
        {
            return _receiverGroupHandle != null && _receiverGroupHandle.SetEquals(nodeIds);
        }

        public void SetReceivedTimeStamp()
        {
            _receivedTimeStamp = DateTime.Now;
        }
    }
}
