using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_MANUFACTURER_SPECIFIC
    {
        public const byte ID = 0x72;
        public const byte VERSION = 1;
        public class MANUFACTURER_SPECIFIC_GET
        {
            public const byte ID = 0x04;
            public static implicit operator MANUFACTURER_SPECIFIC_GET(byte[] data)
            {
                MANUFACTURER_SPECIFIC_GET ret = new MANUFACTURER_SPECIFIC_GET();
                return ret;
            }
            public static implicit operator byte[](MANUFACTURER_SPECIFIC_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MANUFACTURER_SPECIFIC.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class MANUFACTURER_SPECIFIC_REPORT
        {
            public const byte ID = 0x05;
            public byte[] manufacturerId = new byte[2];
            public byte[] productTypeId = new byte[2];
            public byte[] productId = new byte[2];
            public static implicit operator MANUFACTURER_SPECIFIC_REPORT(byte[] data)
            {
                MANUFACTURER_SPECIFIC_REPORT ret = new MANUFACTURER_SPECIFIC_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.manufacturerId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.productTypeId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                    ret.productId = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](MANUFACTURER_SPECIFIC_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_MANUFACTURER_SPECIFIC.ID);
                ret.Add(ID);
                ret.Add(command.manufacturerId[0]);
                ret.Add(command.manufacturerId[1]);
                ret.Add(command.productTypeId[0]);
                ret.Add(command.productTypeId[1]);
                ret.Add(command.productId[0]);
                ret.Add(command.productId[1]);
                return ret.ToArray();
            }
        }
    }
}

