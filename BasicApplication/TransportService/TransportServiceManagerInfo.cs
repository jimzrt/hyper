using System;
using ZWave.Enums;

namespace ZWave.BasicApplication.TransportService
{
    public class TransportServiceManagerInfo
    {
        public const int REQUEST_MISSING_SEGMENT_TIMEOUT = 800;

        public byte NodeId { get; set; }
        public TransmitOptions TxOptions { get; private set; }

        private Func<byte, bool> mSendDataSubstitutionCallback;
        public Func<byte, bool> SendDataSubstitutionCallback
        {
            get
            {
                return mSendDataSubstitutionCallback;
            }
            internal set
            {
                mSendDataSubstitutionCallback = value;
            }
        }

        public TransportServiceManagerInfo(int maxSegmentSize, TransmitOptions txOptions, Func<byte, bool> sendDataSubstitutionCallback)
        {
            MaxCmdSize = maxSegmentSize;
            TxOptions = txOptions;
            SendDataSubstitutionCallback = sendDataSubstitutionCallback;
        }

        private byte _sessionId = 1;
        internal byte SessionId { get { return _sessionId; } }
        public byte GenerateSessionId()
        {
            return _sessionId++;
        }

        private ExpirableValue<int?> _testOffset;
        public ExpirableValue<int?> TestOffset
        {
            get
            {
                return _testOffset ?? (_testOffset = new ExpirableValue<int?>());
            }
        }

        private ExpirableValue<byte[]> _testSubsequentFragmentCRC16;
        public ExpirableValue<byte[]> TestSubsequentFragmentCRC16
        {
            get
            {
                return _testSubsequentFragmentCRC16 ?? (_testSubsequentFragmentCRC16 = new ExpirableValue<byte[]>());
            }
        }

        public void SetTestSubsequentFragmentCRC16(byte[] crc, int offset, int timesUsed)
        {
            TestSubsequentFragmentCRC16.Reset(crc, timesUsed);
            TestOffset.Reset(offset, timesUsed);
        }

        private ExpirableValue<byte[]> _testFirstFragmentCRC16;
        public ExpirableValue<byte[]> TestFirstFragmentCRC16
        {
            get
            {
                return _testFirstFragmentCRC16 ?? (_testFirstFragmentCRC16 = new ExpirableValue<byte[]>());
            }
        }

        public void SetTestFirstFragmentCRC16(byte[] crc, int timesUsed)
        {
            TestFirstFragmentCRC16.Reset(crc, timesUsed);
        }

        private ExpirableValue<byte?> _testSegmentCompleteCmdSessionId;
        public ExpirableValue<byte?> TestSegmentCompleteCmdSessionId
        {
            get
            {
                return _testSegmentCompleteCmdSessionId ?? (_testSegmentCompleteCmdSessionId = new ExpirableValue<byte?>());
            }
        }

        public void SetTestSegmentCompleteCmdSessionId(byte? sessionId, int timesUsed)
        {
            TestSegmentCompleteCmdSessionId.Reset(sessionId, timesUsed);
        }

        private ExpirableValue<bool?> _testNeedToIgnoreFirstSegment;
        public ExpirableValue<bool?> TestNeedToIgnoreFirstSegment
        {
            get
            {
                return _testNeedToIgnoreFirstSegment ?? (_testNeedToIgnoreFirstSegment = new ExpirableValue<bool?>());
            }
        }

        public void SetTestIgnoreFirstSegment(int times)
        {
            TestNeedToIgnoreFirstSegment.Reset(true, times);
        }

        private ExpirableValue<bool?> _testNeedToIgnoreSubsequentSegment;
        public ExpirableValue<bool?> TestNeedToIgnoreSubsequentSegment
        {
            get
            {
                return _testNeedToIgnoreSubsequentSegment ?? (_testNeedToIgnoreSubsequentSegment = new ExpirableValue<bool?>());
            }
        }

        public void SetTestIgnoreSubsequentSegment(int offset, int times)
        {
            TestOffset.Reset(offset, times);
            TestNeedToIgnoreSubsequentSegment.Reset(true, times);
        }

        public int MaxCmdSize { get; private set; }
        public void SetMaxCmdSize(int maxCmdSize)
        {
            MaxCmdSize = maxCmdSize;
        }
    }

    public class ExpirableValue<T>
    {
        private T _value;
        private int _counter;

        public ExpirableValue()
        {
            _value = default(T);
            _counter = 0;
        }

        public ExpirableValue(T value, int timesUsed)
        {
            Reset(value, timesUsed);
        }

        public void Reset(T value, int timesUsed)
        {
            _value = value;
            _counter = timesUsed;
        }

        public T PullValue()
        {
            if (_counter > 0)
            {
                _counter--;
                return _value;
            }
            return default(T);
        }

        public T PullValue(Func<T, bool> predicate)
        {
            if (predicate(_value))
            {
                return PullValue();
            }
            return default(T);
        }

        public bool CanBeUsed
        {
            get { return _counter > 0; }
        }
    }
}
