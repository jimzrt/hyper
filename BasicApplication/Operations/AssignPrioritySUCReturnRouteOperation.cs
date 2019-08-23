using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x58 | bSrcNodeID | PriorityRoute | funcID
    /// ZW->HOST: RES | 0x58 | retVal
    /// ZW->HOST: REQ | 0x58 | funcID | bStatus
    /// </summary>
    /// 
    public class AssignPrioritySucReturnRouteOperation : CallbackApiOperation
    {
        public byte[] Repeaters { get; set; }
        public byte Source { get; set; }
        public byte RouteSpeed { get; set; }
        public AssignPrioritySucReturnRouteOperation(byte source, byte repeater0, byte repeater1, byte repeater2, byte repeater3, byte routespeed)
            : base(CommandTypes.CmdZWaveAssignPrioritySucReturnRoute)
        {
            Repeaters = new[] { repeater0, repeater1, repeater2, repeater3 };
            Source = source;
            RouteSpeed = routespeed;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] ret = new byte[Repeaters.Length + 2];
            ret[0] = Source;
            for (int i = 0; i < Repeaters.Length; i++)
            {
                ret[i + 1] = Repeaters[i];
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

        public AssignSucReturnRouteResult SpecificResult
        {
            get { return (AssignSucReturnRouteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new AssignSucReturnRouteResult();
        }
    }
}
