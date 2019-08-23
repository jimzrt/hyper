using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class NvmBackupRestoreCloseOperation : ControlNApiOperation
    {
        public NvmBackupRestoreCloseOperation()
            : base(CommandTypes.CmdZWaveNVMBackupRestore, true)
        {
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { 0x03/*Close*/};
        }
    }
}
