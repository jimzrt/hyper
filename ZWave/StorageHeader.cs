using System;
using System.Collections.Generic;
using System.Text;

namespace ZWave
{
    /// <summary>
    /// Storage Header 
    /// | 4 bytes header Version | 4 bytes EncodingCode | 512 bytes CommentText |
    /// | 1 byte APItype | 1 byte Number of Sessions | 1 byte SessionId | 1 byte COM name length | N bytes COM name
    /// | 1 byte number of Freq | | 1 byte Freq channels 1 byte Freq code | 1 byte Freq name length | N bytes Freq name
    /// </summary>
    public class StorageHeader
    {
        public const int STORAGE_HEADER_SIZE = 2048;
        /// <summary>
        /// 100 - initial version
        /// 101 - added frequency dictionary, changed APIType as bitmask (00000ipb)
        /// 102 - added APIType - TEXT 
        /// 103 - added end of trace address
        /// </summary>
        public const int STORAGE_LATEST_VERSION = 103;

        private const int COMMENT_BYTES = 512;

        public byte[] Buffer { get; private set; }
        public StorageHeader()
            : this(null) { }

        public StorageHeader(params byte[] buffer)
        {
            Buffer = new byte[STORAGE_HEADER_SIZE];
            Sessions = new Dictionary<byte, string>();
            Frequencies = new Dictionary<byte, RFrequency>();
            if (buffer == null || buffer.Length < STORAGE_HEADER_SIZE)
            {
                Version = STORAGE_LATEST_VERSION;
                Comment = "";
                byte a = 0;
                int b = 0;
                a = (byte)(1 + b);
            }
            else
            {
                Array.Copy(buffer, Buffer, STORAGE_HEADER_SIZE);
                if (IsValid)
                {
                    int index = 521;
                    byte sessionsCount = Buffer[index];
                    index++;
                    for (int i = 0; i < sessionsCount; i++)
                    {
                        byte sessionId = Buffer[index];
                        index++;
                        int length = Buffer[index];
                        index++;
                        string str = Encoding.Unicode.GetString(Buffer, index, length);
                        index += length;
                        Sessions.Add(sessionId, str);
                    }
                    byte frequenciesCount = Buffer[index];
                    index++;
                    for (int i = 0; i < frequenciesCount; i++)
                    {
                        byte freq = Buffer[index];
                        index++;
                        byte ch = Buffer[index];
                        index++;
                        int length = Buffer[index];
                        index++;
                        string str = Encoding.Unicode.GetString(Buffer, index, length);
                        index += length;
                        Frequencies.Add(freq, new RFrequency(ch, str));
                    }
                }
            }
        }

        public void UpdateSourceAndFrequensiesToBuffer()
        {
            int index = 521;
            Buffer[index] = (byte)Sessions.Count;
            index++;
            foreach (var item in Sessions)
            {
                Buffer[index] = item.Key;
                index++;
                int length = Encoding.Unicode.GetByteCount(item.Value);
                Buffer[index] = (byte)length;
                index++;
                Array.Copy(Encoding.Unicode.GetBytes(item.Value), 0, Buffer, index, length);
                index += length;
            }
            Buffer[index] = (byte)Frequencies.Count;
            index++;
            foreach (var item in Frequencies)
            {
                Buffer[index] = item.Key;
                index++;
                Buffer[index] = item.Value.Channels;
                index++;
                int length = Encoding.Unicode.GetByteCount(item.Value.Name);
                Buffer[index] = (byte)length;
                index++;
                Array.Copy(Encoding.Unicode.GetBytes(item.Value.Name), 0, Buffer, index, length);
                index += length;
            }
            Array.Copy(BitConverter.GetBytes(CalculateCRC()), 0, Buffer, STORAGE_HEADER_SIZE - 2, 2);
        }

        /// <summary>
        /// Gets or sets the version. First 4 bytes in Buffer.
        /// </summary>
        /// <value>The version.</value>
        public int Version
        {
            get
            {
                return BitConverter.ToInt32(Buffer, 0);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), Buffer, 4);
            }
        }

        /// <summary>
        /// Gets or sets the version. 8 bytes in Buffer before CRC16.
        /// </summary>
        /// <value>The version.</value>
        public long TraceTotalLength
        {
            get
            {
                return BitConverter.ToInt64(Buffer, STORAGE_HEADER_SIZE - 10);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Buffer, STORAGE_HEADER_SIZE - 10, 8);
            }
        }

        /// <summary>
        /// Type of the API: 
        /// 0 - Zniffer API; 
        /// 1 - Basic API;
        /// 2 - Programmer API.
        /// 3 - Zip API.
        /// 7 - Text
        /// </summary>
        public byte ApiType
        {
            get
            {
                return Buffer[520];
            }
            set
            {
                Buffer[520] = value;
            }
        }

        public Dictionary<byte, string> Sessions { get; private set; }
        public Dictionary<byte, RFrequency> Frequencies { get; private set; }

        public bool IsValid
        {
            get
            {
                return CalculateCRC() == BitConverter.ToUInt16(Buffer, STORAGE_HEADER_SIZE - 2);
            }
        }

        private const uint POLY = 0x1021;          /* crc-ccitt mask */
        private static void update_crc(ushort ch, ref ushort crc)
        {
            ushort i, v, xor_flag;
            v = 0x80;
            for (i = 0; i < 8; i++)
            {
                if ((crc & 0x8000) != 0)
                {
                    xor_flag = 1;
                }
                else
                {
                    xor_flag = 0;
                }
                crc = (ushort)(crc << 1);

                if ((ch & v) != 0)
                {
                    crc = (ushort)(crc + 1);
                }

                if (xor_flag != 0)
                {
                    crc = (ushort)(crc ^ POLY);
                }
                v = (ushort)(v >> 1);
            }
        }
        private static void augment_message_for_crc(ref ushort crc)
        {
            ushort i, xor_flag;

            for (i = 0; i < 16; i++)
            {
                if ((crc & 0x8000) != 0)
                {
                    xor_flag = 1;
                }
                else
                {
                    xor_flag = 0;
                }
                crc = (ushort)(crc << 1);

                if (xor_flag != 0)
                {
                    crc = (ushort)(crc ^ POLY);
                }
            }
        }
        private ushort CalculateCRC()
        {
            ushort sum = 0xFFFF;
            for (int i = 0; i < STORAGE_HEADER_SIZE - 2; i++)
            {
                update_crc(Buffer[i], ref sum);
            }
            augment_message_for_crc(ref sum);
            return sum;
        }

        public int TextEncoding
        {
            get
            {
                return BitConverter.ToInt32(Buffer, 4);
            }
            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, Buffer, 4, 4);
            }
        }

        public string Comment
        {
            get
            {
                UnicodeEncoding enc = new UnicodeEncoding();
                string comment = enc.GetString(Buffer, 8, COMMENT_BYTES);
                if (comment != null)
                {
                    comment = comment.TrimEnd(new char[] { (char)0x00 });
                }
                return comment;
            }
            set
            {
                UnicodeEncoding enc = new UnicodeEncoding();
                const int maxChars = COMMENT_BYTES / 2 - 2;
                enc.GetBytes(value, 0, value.Length < maxChars ? value.Length : maxChars, Buffer, 8);
            }
        }
    }
}
