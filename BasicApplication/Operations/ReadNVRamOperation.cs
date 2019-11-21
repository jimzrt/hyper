using System;
using System.Collections.Generic;
using System.Text;
using ZWave;
using ZWave.BasicApplication;
using ZWave.BasicApplication.Enums;

namespace BasicApplication_netcore.Operations
{
    public class ReadNVRamOperation : RequestApiOperation
    {
        private ushort Offset { get; set; }
        private byte Length { get; set; }
        public ReadNVRamOperation(ushort offset, byte length)
            : base(CommandTypes.CmdNVMExtRead, false)
        {
            Offset = offset;
            Length = length;
        }

        protected override byte[] CreateInputParameters()
        {
            return new byte[] { (byte)(Offset >> 16), (byte)(Offset >> 8), (byte)(Offset & 0xFF), (byte)(Length >> 8), (byte)(Length & 0xFF) };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SpecificResult.RetValue = ((DataReceivedUnit)ou).DataFrame.Payload;
            base.SetStateCompleted(ou);
        }

        public ReadNVRamResult SpecificResult
        {
            get { return (ReadNVRamResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ReadNVRamResult();
        }
    }

    public class ReadNVRamResult : ActionResult
    {
        public byte[] RetValue { get; set; }
    }
}
