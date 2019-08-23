using System.Collections.Generic;
using System.Xml.Serialization;
using Utils;

namespace ZWave.Xml.FrameHeader
{
    public partial class Validation
    {
        [XmlIgnore]
        public DataIndex DataIndex { get; set; }

        private bool _isFound;
        internal void Initialize(Header header)
        {
            DataIndex = new DataIndex { IndexInData = -1 };
            ProcessHeader(header.BaseHeaderDefinition);
            ProcessHeader(header);
        }

        private byte _bytesCount;
        private void ProcessHeader(BaseHeader header)
        {
            if (header != null && !_isFound)
            {
                foreach (var item in header.Param)
                {
                    if (item.Bits % 8 == 0)
                    {
                        _bytesCount = (byte)(item.Bits / 8);
                        if (item.Size > 0)
                        {
                            _bytesCount = (byte)(_bytesCount * item.Size);
                        }
                        DataIndex.OffsetInData = 0x00;
                        DataIndex.MaskInData = 0xFF;
                        DataIndex.IndexInData = (byte)(DataIndex.IndexInData + _bytesCount);
                        if (item.Param1 != null && item.Param1.Count > 0)
                        {
                            foreach (var prm1 in item.Param1)
                            {
                                DataIndex.MaskInData = Tools.GetMaskFromBits(prm1.Bits, DataIndex.OffsetInData);
                                string[] tokens = ParamName.Split('.');
                                if (tokens.Length == 2)
                                {
                                    if (tokens[0] == item.Name && tokens[1] == prm1.Name)
                                    {
                                        _isFound = true;
                                        return;
                                    }
                                }
                                DataIndex.OffsetInData += prm1.Bits;
                            }
                        }
                    }
                    if (item.Name == ParamName)
                    {
                        _isFound = true;
                        return;
                    }
                }
            }
        }

        private void ProcessHeader(Header header)
        {
            if (header != null && !_isFound)
            {
                foreach (var item in header.Param)
                {
                    if (item.Bits % 8 == 0)
                    {
                        _bytesCount = (byte)(item.Bits / 8);
                        if (item.Size > 0)
                        {
                            _bytesCount = (byte)(_bytesCount * item.Size);
                        }
                        DataIndex.OffsetInData = 0x00;
                        DataIndex.MaskInData = 0xFF;
                        DataIndex.IndexInData = (byte)(DataIndex.IndexInData + _bytesCount);
                        if (item.Param1 != null && item.Param1.Count > 0)
                        {
                            foreach (var prm1 in item.Param1)
                            {
                                DataIndex.MaskInData = Tools.GetMaskFromBits(prm1.Bits, DataIndex.OffsetInData);
                                string[] tokens = ParamName.Split('.');
                                if (tokens.Length == 2)
                                {
                                    if (tokens[0] == item.Name && tokens[1] == prm1.Name)
                                    {
                                        _isFound = true;
                                        return;
                                    }
                                }
                                DataIndex.OffsetInData += prm1.Bits;
                            }
                        }
                    }
                    if (item.Name == ParamName)
                    {
                        _isFound = true;
                        return;
                    }
                }
            }
        }
    }

    public class DataIndex
    {
        public int IndexInData { get; set; }
        public byte MaskInData { get; set; }
        public byte OffsetInData { get; set; }
    }

    public class ValidationLeaf
    {
        public Validation Rule { get; set; }
        public Header Header { get; set; }
        public Dictionary<byte, IList<ValidationLeaf>> Results { get; set; }
    }
}
