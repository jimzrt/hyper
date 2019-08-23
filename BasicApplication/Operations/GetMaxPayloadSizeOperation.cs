namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x0B | 0x10
    /// ZW->HOST: RES | 0x0B | 0x10 | maxPayloadSize
    /// ZW->HOST: RES | 0x0B | 0x00 | 0x10
    /// </summary>
    public class GetMaxPayloadSizeOperation : SerialApiSetupOperation
    {
        public const byte ARGUMENT = 0x10;

        public GetMaxPayloadSizeOperation()
            : base(ARGUMENT)
        {
            //SerialApiSetup
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            base.SetStateCompleted(ou);
            var res = base.SpecificResult.RetValue;
            if (res != null && res.Length > 1)
            {
                if (res[0] == 0x10) // if the subfunction is supported
                    (SpecificResult as GetMaxPayloadSizeResult).MaxPayloadSize = base.SpecificResult.RetValue[1];
                else
                {
                    //SERIAL_API_SETUP_CMD_SUPPORTED
                    (SpecificResult as GetMaxPayloadSizeResult).MaxPayloadSize = 0;
                }
            }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetMaxPayloadSizeResult();
        }
    }

    public class GetMaxPayloadSizeResult : SerialApiSetupResult
    {
        public byte MaxPayloadSize { get; set; }
    }
}
