using System;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class NvmBackupRestoreReadOperation : RequestApiOperation
    {
        byte Length;
        int Offset;
        public NvmBackupRestoreReadOperation(byte length, int offset)
            : base(CommandTypes.CmdZWaveNVMBackupRestore, true)
        {
            Length = length;
            Offset = offset;
        }

        protected override byte[] CreateInputParameters()
        {
            byte offsetMSB = (byte)(Offset >> 8);
            byte offsetLSB = (byte)Offset;
            return new byte[] { 0x01/*Read*/, Length, offsetMSB, offsetLSB };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            if (res != null && res.Length > 2)
            {
                SpecificResult.Status = (NvmBackupRestoreStatuses)res[0];
                byte dataLength = res[1];
                SpecificResult.Data = new byte[dataLength];
                Array.Copy(res, 4, SpecificResult.Data, 0, dataLength);
            }
            base.SetStateCompleted(ou);
        }

        public NvmBackupRestoreReadResult SpecificResult
        {
            get { return (NvmBackupRestoreReadResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new NvmBackupRestoreReadResult();
        }
    }

    public class NvmBackupRestoreReadResult : ActionResult
    {
        public NvmBackupRestoreStatuses Status { get; set; }
        public byte[] Data { get; set; }
    }
}
