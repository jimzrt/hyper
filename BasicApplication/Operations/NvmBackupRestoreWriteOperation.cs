using System.Collections.Generic;
using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class NvmBackupRestoreWriteOperation : RequestApiOperation
    {
        byte Length;
        int Offset;
        byte[] Data;
        public NvmBackupRestoreWriteOperation(byte length, int offset, byte[] data)
            : base(CommandTypes.CmdZWaveNVMBackupRestore, true)
        {
            Length = length;
            Offset = offset;
            Data = data;
        }

        protected override byte[] CreateInputParameters()
        {
            byte offsetMSB = (byte)(Offset >> 8);
            byte offsetLSB = (byte)Offset;
            List<byte> request = new List<byte>();
            request.Add(0x02/*Write*/);
            request.Add(Length);
            request.Add(offsetMSB);
            request.Add(offsetLSB);
            request.AddRange(Data);
            return request.ToArray();
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            if (res != null && res.Length > 2)
            {
                SpecificResult.Status = (NvmBackupRestoreStatuses)res[0];
            }
            base.SetStateCompleted(ou);
        }

        public NvmBackupRestoreWriteResult SpecificResult
        {
            get { return (NvmBackupRestoreWriteResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new NvmBackupRestoreWriteResult();
        }
    }

    public class NvmBackupRestoreWriteResult : ActionResult
    {
        public NvmBackupRestoreStatuses Status { get; set; }
    }
}
