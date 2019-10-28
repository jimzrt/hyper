using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication
{
    public abstract class RequestApiOperation : ApiOperation
    {
        public static int RET_TIMEOUT = 60000;

        internal int TimeoutMs { get; set; }
        public byte[] GetOutputParameters()
        {
            return handler.DataFrame.Data;
        }

        private readonly FrameTypes HandlerType;

        public RequestApiOperation(FrameTypes handlerType, CommandTypes command, bool isSequenceNumberRequired)
            : base(true, command, isSequenceNumberRequired)
        {
            TimeoutMs = RET_TIMEOUT;
            HandlerType = handlerType;
        }

        public RequestApiOperation(CommandTypes command, bool useSequenceNumber)
            : this(FrameTypes.Response, command, useSequenceNumber)
        {
        }

        public RequestApiOperation(CommandTypes command)
            : this(FrameTypes.Response, command, true)
        {
        }

        protected ApiMessage message;
        private ApiHandler handler;

        /// <summary>
        /// Fills OperationUnits list. Frame retransmittion timeout=2sec, so we waiting 7 seconds in operation
        /// </summary>
        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, TimeoutMs, message));
            ActionUnits.Add(new DataReceivedUnit(handler, SetStateCompleted));
        }

        protected override void CreateInstance()
        {
            message = new ApiMessage(SerialApiCommands[0], CreateInputParameters());
            if (IsSequenceNumberRequired)
                message.SetSequenceNumber(SequenceNumber);
            handler = new ApiHandler(HandlerType, SerialApiCommands[0]);
        }

        protected abstract byte[] CreateInputParameters();


    }
}
