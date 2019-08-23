using System.Xml.Serialization;
using Utils;

namespace ZWave.Xml.FrameHeader
{
    public partial class BaseHeader
    {
        internal void Initialize()
        {
            FillDataIndexes();
        }

        private void FillDataIndexes()
        {
            Calculatelength();// base header have no optional or list parameters
            HomeId.Index = new DataIndex();
            ProcessHeader(HomeId.Index, HomeId.Ref);
            Source.Index = new DataIndex();
            ProcessHeader(Source.Index, Source.Ref);
            SequenceNumber.Index = new DataIndex();
            ProcessHeader(SequenceNumber.Index, SequenceNumber.Ref);
            HeaderType.Index = new DataIndex();
            ProcessHeader(HeaderType.Index, HeaderType.Ref);
            IsLTX.Index = new DataIndex();
            ProcessHeader(IsLTX.Index, IsLTX.Ref);
        }

        [XmlIgnore]
        public byte Length { get; set; }

        public byte[] GetHomeId(byte[] store, int dataLength)
        {
            byte[] ret = null;
            if (dataLength > 4 + HomeId.Index.IndexInData)
            {
                ret = new byte[4];
                ret[0] = store[HomeId.Index.IndexInData];
                ret[1] = store[HomeId.Index.IndexInData + 1];
                ret[2] = store[HomeId.Index.IndexInData + 2];
                ret[3] = store[HomeId.Index.IndexInData + 3];
            }
            return ret;
        }

        public byte GetSource(byte[] store, int dataLength)
        {
            if (dataLength > Source.Index.IndexInData)
                return store[Source.Index.IndexInData];
            return 0;
        }

        public int GetSequenceNumber(byte[] store, int dataLength)
        {
            if (dataLength > SequenceNumber.Index.IndexInData)
                return (store[SequenceNumber.Index.IndexInData] & SequenceNumber.Index.MaskInData) >> SequenceNumber.Index.OffsetInData;
            return -1;
        }

        public bool GetIsLtx(byte[] store, int dataLength)
        {
            if (dataLength > IsLTX.Index.IndexInData)
                return (store[IsLTX.Index.IndexInData] & IsLTX.Index.MaskInData) >> IsLTX.Index.OffsetInData > 0;
            return false;
        }

        public byte GetHeaderType(byte[] store, int dataLength)
        {
            if (dataLength > HeaderType.Index.IndexInData)
                return (byte)((store[HeaderType.Index.IndexInData] & HeaderType.Index.MaskInData) >> HeaderType.Index.OffsetInData);
            return 0;
        }

        private void Calculatelength()
        {
            foreach (var item in Param)
            {
                if (item.Bits % 8 == 0)
                {
                    var bytesCount = (byte)(item.Bits / 8);
                    if (item.Size > 0)
                    {
                        bytesCount = (byte)(bytesCount * item.Size);
                    }
                    Length = (byte)(Length + bytesCount);
                }
            }
        }

        private void ProcessHeader(DataIndex dataIndex, string paramName)
        {
            foreach (var item in Param)
            {
                if (item.Bits % 8 == 0)
                {
                    var bytesCount = (byte)(item.Bits / 8);
                    if (item.Size > 0)
                    {
                        bytesCount = (byte)(bytesCount * item.Size);
                    }
                    dataIndex.OffsetInData = 0x00;
                    dataIndex.MaskInData = 0xFF;
                    if (item.Param1 != null && item.Param1.Count > 0)
                    {
                        foreach (var prm1 in item.Param1)
                        {
                            dataIndex.MaskInData = Tools.GetMaskFromBits(prm1.Bits, dataIndex.OffsetInData);
                            string[] tokens = paramName.Split('.');
                            if (tokens.Length == 2)
                            {
                                if (tokens[0] == item.Name && tokens[1] == prm1.Name)
                                {
                                    return;
                                }
                            }
                            dataIndex.OffsetInData += prm1.Bits;
                        }
                    }
                    else
                    {
                        if (item.Name == paramName)
                        {
                            return;
                        }
                    }
                    dataIndex.IndexInData = (byte)(dataIndex.IndexInData + bytesCount);
                }
            }
        }
    }
}
