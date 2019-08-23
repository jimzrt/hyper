using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class TypeLibraryOperation : RequestApiOperation
    {
        public TypeLibraryOperation()
            : base(CommandTypes.CmdZWaveTypeLibrary, false)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return null;
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.Library = (Libraries)((DataReceivedUnit)ou).DataFrame.Payload[0];
            base.SetStateCompleted(ou);
        }

        public TypeLibraryResult SpecificResult
        {
            get { return (TypeLibraryResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new TypeLibraryResult();
        }
    }

    public class TypeLibraryResult : ActionResult
    {
        public Libraries Library { get; set; }
    }
}
