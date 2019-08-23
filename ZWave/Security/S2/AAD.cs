using System.Collections.Generic;

namespace ZWave.Security
{
    // Additional Authenticated Data.
    public class AAD
    {
        public byte SenderNodeId { get; set; }
        public byte ReceiverNodeId { get; set; }
        public byte[] HomeId { get; set; }
        public ushort PayloadLength { get; set; }
        public byte SequenceNumber { get; set; }
        public byte StatusByte { get; set; }
        public byte[] ExtensionData { get; set; }

        public static implicit operator byte[](AAD aad)
        {
            var data = new List<byte>();
            data.Add(aad.SenderNodeId);
            data.Add(aad.ReceiverNodeId);
            data.AddRange(aad.HomeId);
            data.Add((byte)(aad.PayloadLength >> 8));
            data.Add((byte)aad.PayloadLength);
            data.Add(aad.SequenceNumber);
            data.Add(aad.StatusByte);
            if (aad.ExtensionData != null)
            {
                data.AddRange(aad.ExtensionData);
            }
            return data.ToArray();
        }
    }
}
