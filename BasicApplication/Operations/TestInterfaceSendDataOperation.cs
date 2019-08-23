using System;
using Utils;

namespace ZWave.BasicApplication.Operations
{
    public class TestInterfaceSendDataOperation : ApiOperation
    {
        readonly int _timeoutMs = 0;
        private readonly byte[] _testInterfaceCmd;
        public byte[] TestInterfaceCmd
        {
            get { return _testInterfaceCmd; }
        }
        public TestInterfaceSendDataOperation(byte[] testInterfaceCmd, int timeoutMs)
            : base(true, null, true)
        {
            _testInterfaceCmd = testInterfaceCmd ?? new byte[0];
            _timeoutMs = timeoutMs;
        }

        ApiProgMessage message;
        ApiProgHandler handler;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, _timeoutMs, message));
            ActionUnits.Add(new DataReceivedUnit(handler, OnRetDataReceived));
        }

        protected override void CreateInstance()
        {
            message = new ApiProgMessage(_testInterfaceCmd);
            message.SetSequenceNumber(SequenceNumber);
            message.IsNoAck = true;
            handler = new ApiProgHandler(_testInterfaceCmd[0]);
        }

        private void OnRetDataReceived(DataReceivedUnit ou)
        {
            SpecificResult.ByteArray = ou.DataFrame.Payload;
            if (ou.DataFrame.Payload[0] == _testInterfaceCmd[0])
            {
                if (ou.DataFrame.Payload.Length >= 2)
                {
                    SpecificResult.ByteArray = new byte[ou.DataFrame.Payload.Length - 2];
                    Array.Copy(ou.DataFrame.Payload, 1, SpecificResult.ByteArray, 0, SpecificResult.ByteArray.Length);
                }
            }
            SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            if (_testInterfaceCmd != null)
            {
                if (SpecificResult.ByteArray != null)
                {
                    return string.Format("Data: {0} Ret: {1}", _testInterfaceCmd.GetHex(), SpecificResult.ByteArray.GetHex());
                }
                else
                {
                    return string.Format("Data: {0}", _testInterfaceCmd.GetHex());
                }
            }
            else
            {
                return string.Empty;
            }

        }

        public ReturnValueResult SpecificResult
        {
            get { return (ReturnValueResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ReturnValueResult();
        }
    }
}
