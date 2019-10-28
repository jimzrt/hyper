using System;
using Utils;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x92 | bNodeID
    /// ZW->HOST: RES | 0x92 | bNodeID | retVal | repeater0 | repeater1 | repeater2 | repeater3 | routespeed
    /// </summary>

    public class GetPriorityRouteOperation : ApiOperation
    {
        public byte Destination { get; set; }
        public GetPriorityRouteOperation(byte destination)
            : base(true, CommandTypes.CmdZWaveGetPriorityRoute, false)
        {
            Destination = destination;
        }

        private ApiMessage message;
        private ApiHandler handler;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 1000, message));
            ActionUnits.Add(new DataReceivedUnit(handler, OnReceived));
        }

        protected override void CreateInstance()
        {
            message = new ApiMessage(CommandTypes.CmdZWaveGetPriorityRoute, Destination);

            handler = new ApiHandler(CommandTypes.CmdZWaveGetPriorityRoute);
            handler.AddConditions(new ByteIndex(Destination));
        }

        private void OnReceived(DataReceivedUnit ou)
        {
            byte[] ret = ou.DataFrame.Payload;
            SpecificResult.NodeId = ret[0];
            SpecificResult.RetVal = (ret[1] == 0x01) ? true : false;
            SpecificResult.PriorityRoute = new byte[ret.Length - 3];
            SpecificResult.RouteSpeed = ret[ret.Length - 1];
            Array.Copy(ret, 2, SpecificResult.PriorityRoute, 0, SpecificResult.PriorityRoute.Length);
            SetStateCompleted(ou);
        }

        public GetPriorityRouteResult SpecificResult
        {
            get { return (GetPriorityRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetPriorityRouteResult();
        }
    }

    public class GetPriorityRouteResult : ActionResult
    {
        public byte NodeId { get; set; }
        public bool RetVal { get; set; }
        public byte[] PriorityRoute { get; set; }
        public byte RouteSpeed { get; set; }
    }


}
