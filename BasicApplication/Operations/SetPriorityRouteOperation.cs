using System;
using Utils;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x93 | bnodeID | repeater0| repeater1| repeater2| repeater3| routerspeed
    /// ZW->HOST: RES | 0x93 | bnodeID | retVal
    /// </summary>
    public class SetPriorityRouteOperation : ApiOperation
    {
        public byte[] PriorityRoute { get; set; }
        public byte Destination { get; set; }
        public byte RouteSpeed { get; set; }
        public SetPriorityRouteOperation(byte destination, byte repeater0, byte repeater1, byte repeater2, byte repeater3, byte routespeed)
            : base(true, CommandTypes.CmdZWaveSetPriorityRoute, false)
        {
            PriorityRoute = new[] { repeater0, repeater1, repeater2, repeater3 };
            Destination = destination;
            RouteSpeed = routespeed;
        }

        ApiMessage message;
        ApiHandler handler;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 1000, message));
            ActionUnits.Add(new DataReceivedUnit(handler, OnReceived));
        }

        protected override void CreateInstance()
        {
            message = new ApiMessage(CommandTypes.CmdZWaveSetPriorityRoute, Destination);
            message.AddData(PriorityRoute);
            message.AddData(RouteSpeed);


            handler = new ApiHandler(CommandTypes.CmdZWaveSetPriorityRoute);
            handler.AddConditions(new ByteIndex(Destination));
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            byte[] ret = ou.DataFrame.Payload;
            SpecificResult.PriorityRoute = new byte[ret.Length - 1];
            Array.Copy(ret, 1, SpecificResult.PriorityRoute, 0, SpecificResult.PriorityRoute.Length);
            SetStateCompleted(ou);
        }

        public SetPriorityRouteResult SpecificResult
        {
            get { return (SetPriorityRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SetPriorityRouteResult();
        }
    }

    public class SetPriorityRouteResult : ActionResult
    {
        public byte[] PriorityRoute { get; set; }
    }


}
