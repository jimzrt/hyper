using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication
{
    public abstract class ApiOperation : ActionBase
    {
        public CommandTypes[] SerialApiCommands { get; private set; }
        public SubstituteSettings SubstituteSettings { get; set; }

        public ApiOperation(bool isExclusive, CommandTypes[] serialApiCommands, bool isSequenceNumberRequired)
            : base(isExclusive)
        {
            SerialApiCommands = serialApiCommands;
            IsSequenceNumberRequired = isSequenceNumberRequired;
            SubstituteSettings = new SubstituteSettings();
        }

        public ApiOperation(bool isExclusive, CommandTypes serialApiCommand, bool isSequenceNumberRequired)
            : base(isExclusive)
        {
            SerialApiCommands = new CommandTypes[] { serialApiCommand };
            IsSequenceNumberRequired = isSequenceNumberRequired;
            SubstituteSettings = new SubstituteSettings();
        }
    }
}
