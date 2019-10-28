using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class VersionOperation : RequestApiOperation
    {
        private readonly bool _isNoAck;
        public VersionOperation()
            : this(false)
        {
        }

        public VersionOperation(bool isNoAck)
            : base(ZWave.BasicApplication.Enums.CommandTypes.CmdZWaveGetVersion)
        {
            _isNoAck = isNoAck;
            TimeoutMs = 200;
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void CreateInstance()
        {
            base.CreateInstance();
            message.IsNoAck = _isNoAck;
            message.IsSequenceNumberRequired = false;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            System.Text.UTF7Encoding utf = new System.Text.UTF7Encoding();
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            if (res.Length > 6 + 6)
            {
                byte length = 0;
                for (length = 12; length > 0; length--)
                {
                    if (res[length - 1] != 0)
                        break;
                }
                if (length > 0)
                {
                    SpecificResult.Version = utf.GetString(res, 0, length);
                }
                else
                {
                    SpecificResult.Version = "";
                }
                SpecificResult.Library = (Libraries)res[12];
            }
            base.SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            return string.Format("{0} {1}", SpecificResult.Library, SpecificResult.Version);
        }

        public VersionResult SpecificResult
        {
            get { return (VersionResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new VersionResult();
        }
    }

    public class VersionResult : ActionResult
    {
        public string Version { get; set; }
        public Libraries Library { get; set; }
    }
}
