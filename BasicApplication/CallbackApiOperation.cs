using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication
{
    public abstract class CallbackApiOperation : ApiOperation
    {
        public static int RET_TIMEOUT = 7000;
        public static int CALLBACK_TIMEOUT = 60000;

        //internal Action<ActionUnit> OnHandledCallback { get; set; }
        public CallbackApiOperation(CommandTypes command)
            : base(true, command, true)
        {
        }

        private ApiMessage Message;
        private ApiHandler HandlerOk;
        private ApiHandler HandlerFailed;
        private ApiHandler CallbackHandler;

        protected override void CreateWorkflow()
        {
            isHandled = false;
            ActionUnits.Add(new StartActionUnit(null, RET_TIMEOUT, Message));
            ActionUnits.Add(new DataReceivedUnit(HandlerFailed, OnFailed));
            ActionUnits.Add(new DataReceivedUnit(HandlerOk, OnHandled, CALLBACK_TIMEOUT));
            ActionUnits.Add(new DataReceivedUnit(CallbackHandler, OnCallback));
        }

        protected override void CreateInstance()
        {
            Message = new ApiMessage(SerialApiCommands[0], CreateInputParameters());

            Message.SetSequenceNumber(SequenceNumber);

            HandlerOk = new ApiHandler(SerialApiCommands[0]);
            HandlerOk.AddConditions(new ByteIndex(0x01));
            HandlerFailed = new ApiHandler(SerialApiCommands[0]);
            HandlerFailed.AddConditions(new ByteIndex(0x00));
            CallbackHandler = new ApiHandler(FrameTypes.Request, SerialApiCommands[0]);
            CallbackHandler.AddConditions(new ByteIndex[]
            {
                new ByteIndex(SequenceNumber),
            });
        }

        private void OnFailed(DataReceivedUnit ou)
        {
            SetStateFailed(ou);
        }

        protected bool isHandled = false;
        private void OnHandled(DataReceivedUnit ou)
        {
            if (!isHandled)
            {
                isHandled = true;
                //if (OnHandledCallback != null)
                //{
                //    OnHandledCallback(ou);
                //}
            }
        }

        protected virtual void OnCallback(DataReceivedUnit ou)
        {
            OnCallbackInternal(ou);
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                //TransmitStatuses ts = (TransmitStatuses)ou.DataFrame.Payload[1];
                //if (isHandled && (ts == TransmitStatuses.CompleteOk || ts == TransmitStatuses.CompleteNoAcknowledge || ts == TransmitStatuses.CompleteOk))
                //    SetStateCompleted(ou);
                //else
                //    SetStateFailed(ou);
                SetStateCompleted(ou);
            }
            else
                SetStateFailed(ou);
        }

        protected virtual void OnCallbackInternal(DataReceivedUnit ou)
        {

        }

        protected override ActionResult CreateOperationResult()
        {
            return new ActionResult();
        }

        protected abstract byte[] CreateInputParameters();
    }
}
