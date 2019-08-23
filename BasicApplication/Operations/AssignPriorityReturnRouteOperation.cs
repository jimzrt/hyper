using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x4F | bSrcNodeID | bDstNodeID | PriorityRoute | funcID
    /// ZW->HOST: RES | 0x4F | retVal
    /// ZW->HOST: REQ | 0x4F | funcID | bStatus
    /// </summary>
    public class AssignPriorityReturnRouteOperation : CallbackApiOperation
    {
        public byte[] PriorityRoute { get; set; }
        public byte Source { get; set; }
        public byte Destination { get; set; }
        public byte RouteSpeed { get; set; }
        public AssignPriorityReturnRouteOperation(byte source, byte destination,
            byte priorityRoute0, byte priorityRoute1, byte priorityRoute2, byte priorityRoute3, byte routespeed)
            : base(CommandTypes.CmdZWaveAssignPriorityReturnRoute)
        {
            PriorityRoute = new[] { priorityRoute0, priorityRoute1, priorityRoute2, priorityRoute3 };
            Source = source;
            Destination = destination;
            RouteSpeed = routespeed;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[PriorityRoute.Length + 3];
            ret[0] = Source;
            ret[1] = Destination;
            for (int i = 0; i < PriorityRoute.Length; i++)
            {
                ret[i + 2] = PriorityRoute[i];
            }
            ret[ret.Length - 1] = RouteSpeed;
            return ret;
        }

        protected override void OnCallbackInternal(DataReceivedUnit ou)
        {
            if (ou.DataFrame.Payload != null && ou.DataFrame.Payload.Length > 1)
            {
                SpecificResult.RetStatus = ou.DataFrame.Payload[1];
            }
        }

        public AssignReturnRouteResult SpecificResult
        {
            get { return (AssignReturnRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new AssignReturnRouteResult();
        }
    }

}

/*
ApiMessage message;
ApiHandler handler;

protected override void CreateWorkflow()
{
    ActionUnits.Add(new DataReceivedUnit(null, null, message, 1000));
    ActionUnits.Add(new DataReceivedUnit(handler, OnReceived));
}

protected override void CreateInstance()
{
    message = new ApiMessage(CommandTypes.CmdZWaveAssignPriorityReturnRoute, Source, Destination);
    message.AddData(LastWorkingRoute);
    message.AddData(RouteSpeed);


    handler = new ApiHandler(CommandTypes.CmdZWaveAssignPriorityReturnRoute);
    handler.AddConditions(new ByteIndex(Destination));
    handler.AddConditions(new ByteIndex(Source));
}

private void OnReceived(OperationUnit ou)
{
    byte[] ret = ou.CommandHandler.DataFrame.Payload;
    SpecificResult.Repeaters = new byte[ret.Length - 1];
    Array.Copy(ret, 1, SpecificResult.Repeaters, 0, SpecificResult.Repeaters.Length);
    SetStateCompleted(ou);
}

public AssignPriorityReturnRouteResult SpecificResult
{
    get { return (AssignPriorityReturnRouteResult)Result; }
}

protected override ActionResult CreateOperationResult()
{
    return new AssignPriorityReturnRouteResult();
}
}

public class AssignPriorityReturnRouteResult : ActionResult
{
public byte[] Repeaters { get; set; }
}

*/
