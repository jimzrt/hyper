using System;
using System.Collections.Generic;
using System.Text;
using ZWave;
using ZWave.BasicApplication;
using ZWave.BasicApplication.Enums;

namespace BasicApplication_netcore.Operations
{
    public class WriteNVRamOperation : RequestApiOperation
    {
        private ushort Offset { get; set; }
        private byte Length { get; set; }
        private byte[] Buffer { get; set; }
        public WriteNVRamOperation(ushort offset, byte length, byte[] buffer)
            : base(CommandTypes.CmdNVMExtWrite, false)
        {
            Offset = offset;
            Length = length;
            Buffer = buffer;
        }

        protected override byte[] CreateInputParameters()
        {
            byte[] input = new byte[5 + 128];
            byte[] param = new byte[] { (byte)(Offset >> 16), (byte)(Offset >> 8), (byte)(Offset & 0xFF), (byte)(Length >> 8), (byte)(Length & 0xFF) };

            System.Buffer.BlockCopy(param, 0, input, 0, param.Length);
            System.Buffer.BlockCopy(Buffer, 0, input, param.Length, Buffer.Length);

            return input;
          //  return new byte[] { (byte)(Offset >> 16), (byte)(Offset >> 8), (byte)(Offset & 0xFF), (byte)(Length >> 8), (byte)(Length & 0xFF) };
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
           // SpecificResult.RetValue = ((DataReceivedUnit)ou).DataFrame.Payload;
            base.SetStateCompleted(ou);
        }

        public WriteNVRamResult SpecificResult
        {
            get { return (WriteNVRamResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new WriteNVRamResult();
        }
    }

    public class WriteNVRamResult : ActionResult
    {
    }
}
