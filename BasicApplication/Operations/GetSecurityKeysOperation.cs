using System.Linq;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Security;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x9A
    /// ZW->HOST: RES | 0x9A | securityKeys_bitmask
    /// </summary>

    public class GetSecurityKeysOperation : RequestApiOperation
    {
        public GetSecurityKeysOperation()
            : base(CommandTypes.CmdZWaveSecuritySetup, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { 0x00 };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            if (res != null && res.Length > 2)
            {
                SpecificResult.SecurityKeysMask = res[2];
                SpecificResult.SecuritySchemes = SecurityManagerInfo.ConvertToSecuritySchemes((NetworkKeyS2Flags)SpecificResult.SecurityKeysMask);
            }
            base.SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            if (SpecificResult.SecuritySchemes != null)
            {
                return $"{SpecificResult.SecuritySchemes.Select(z => z.ToString()).DefaultIfEmpty().Aggregate((x, y) => x + "," + y)}";
            }
            return string.Empty;
        }

        public GetSecurityKeysResult SpecificResult
        {
            get { return (GetSecurityKeysResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new GetSecurityKeysResult();
        }
    }

    public class GetSecurityKeysResult : ActionResult
    {
        public byte SecurityKeysMask { get; set; }
        public SecuritySchemes[] SecuritySchemes { get; set; }
    }


}
