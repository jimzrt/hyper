using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Utils;

namespace ZWave.Xml.FrameHeader
{
    public partial class Header
    {
        [XmlIgnore]
        public BaseHeader BaseHeaderDefinition { get; set; }

        internal void Initialize(BaseHeader baseHeader)
        {
            BaseHeaderDefinition = baseHeader;
            ProcessDataIndexes();
            FillDataIndexes();
        }

        private void FillDataIndexes()
        {
            if (IsMulticast)
            {
                ListReference lRef = (ListReference)Destination.Item;
                lRef.IndexOfSize = new DataIndex();
                ProcessHeader(lRef.IndexOfSize, lRef.SizeRef);
                lRef.Index = new DataIndex();
                ProcessHeader(lRef.Index, lRef.Ref);
            }
            else
            {
                ItemReference iRef = (ItemReference)Destination.Item;
                iRef.Index = new DataIndex();
                ProcessHeader(iRef.Index, iRef.Ref);
            }
            if (IsRouted)
            {
                Hops.Index = new DataIndex();
                ProcessHeader(Hops.Index, Hops.Ref);
                Repeaters.IndexOfSize = new DataIndex();
                ProcessHeader(Repeaters.IndexOfSize, Repeaters.SizeRef);
                Repeaters.Index = new DataIndex();
                ProcessHeader(Repeaters.Index, Repeaters.Ref);
            }
        }

        private static byte GetLength(byte[] store, Param param)
        {
            int ret = 0;
            if (param.ItemRef != null)
            {
                ItemReference iref = param.ItemRef;
                if (iref.IndexOfOpt == null ||
                    iref.IndexOfOpt != null && (store[iref.IndexOfOpt.IndexInData] & iref.IndexOfOpt.MaskInData) >> iref.IndexOfOpt.OffsetInData > 0)
                {
                    int bCount = param.Bits / 8 * (param.Size > 0 ? param.Size : 1);
                    if (bCount >= 0)
                    {
                        ret += iref.Index.IndexInData + bCount;
                    }
                }
                else
                {
                    ret += iref.Index.IndexInData;
                }
            }
            else
            {
                ListReference lref = param.ListRef;
                if (lref.IndexOfOpt == null ||
                    lref.IndexOfOpt != null && (store[lref.IndexOfOpt.IndexInData] & lref.IndexOfOpt.MaskInData) >> lref.IndexOfOpt.OffsetInData > 0)
                {
                    if (param.PreviousVariableParam != null)
                    {
                        byte prevLen = (byte)(GetLength(store, param.PreviousVariableParam) + lref.Index.IndexInData);
                        if (store.Length > prevLen - 1 + lref.IndexOfSize.IndexInData)
                        {
                            int len = (store[prevLen - 1 + lref.IndexOfSize.IndexInData] & lref.IndexOfSize.MaskInData) >> lref.IndexOfSize.OffsetInData;
                            if (len >= 0)
                            {
                                int bCount = param.Bits / 8 * len + (param.SizeCorrectionSpecified ? param.SizeCorrection : 0);
                                if (bCount >= 0)
                                {
                                    ret += lref.Index.IndexInData + bCount;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (store.Length > lref.IndexOfSize.IndexInData)
                        {
                            int len = (store[lref.IndexOfSize.IndexInData] & lref.IndexOfSize.MaskInData) >> lref.IndexOfSize.OffsetInData;
                            if (len >= 0)
                            {
                                int bCount = param.Bits / 8 * len + (param.SizeCorrectionSpecified ? param.SizeCorrection : 0);
                                if (bCount >= 0)
                                {
                                    ret += lref.Index.IndexInData + bCount;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ret += lref.Index.IndexInData;
                }
            }
            if (param.PreviousVariableParam != null)
            {
                ret += GetLength(store, param.PreviousVariableParam);
            }
            if (ret < 0 || ret > 0xFF)
                ret = 0;
            return (byte)ret;
        }

        public byte GetLength(byte[] store, int dataLength)
        {
            byte ret = GetLength(store, Param.Last());
            if (ret > dataLength)
            {
                ret = 0;
            }
            return ret;
        }

        public byte GetHops(byte[] store, int dataLength)
        {
            byte ret = 0;
            if (IsRouted)
            {
                if (Hops != null && dataLength > Hops.Index.IndexInData)
                {
                    int tmp = (store[Hops.Index.IndexInData] & Hops.Index.MaskInData) >> Hops.Index.OffsetInData;
                    if (tmp > 0 && tmp < 0xFF)
                        ret = (byte)tmp;
                }
            }
            return ret;
        }

        public byte GetRoutesCount(byte[] store, int dataLength)
        {
            byte ret = 0;
            if (IsRouted)
            {
                if (Repeaters != null && dataLength > Repeaters.IndexOfSize.IndexInData)
                {
                    int tmp = (store[Repeaters.IndexOfSize.IndexInData] & Repeaters.IndexOfSize.MaskInData) >> Repeaters.IndexOfSize.OffsetInData;
                    if (tmp > 0 && tmp < 0xFF)
                        ret = (byte)tmp;
                }
            }
            return ret;
        }

        public byte[] GetRepeaters(byte[] store, int dataLength)
        {
            byte[] ret = null;
            if (IsRouted)
            {
                if (Repeaters != null && dataLength > Repeaters.IndexOfSize.IndexInData)
                {
                    int len = (store[Repeaters.IndexOfSize.IndexInData] & Repeaters.IndexOfSize.MaskInData) >> Repeaters.IndexOfSize.OffsetInData;
                    if (len > 0 && dataLength > len + Repeaters.Index.IndexInData)
                    {
                        ret = new byte[len];
                        for (int i = 0; i < len; i++)
                        {
                            ret[i] = store[Repeaters.Index.IndexInData + i];
                        }
                    }
                }
            }
            return ret;
        }

        public byte[] GetDestination(byte[] store, int dataLength)
        {
            byte[] ret = null;
            if (IsMulticast)
            {
                ListReference lref = (ListReference)Destination.Item;
                int len = (store[lref.IndexOfSize.IndexInData] & lref.IndexOfSize.MaskInData) >> lref.IndexOfSize.OffsetInData;
                if (len > 0 && dataLength > len + lref.Index.IndexInData)
                {
                    IList<byte> tmp = new List<byte>();
                    byte nodeIdx = 0;
                    for (int i = 0; i < len; i++)
                    {
                        byte availabilityMask = store[lref.Index.IndexInData + i];
                        for (byte bit = 0; bit < 8; bit++)
                        {
                            nodeIdx++;
                            if ((availabilityMask & (1 << bit)) > 0)
                            {
                                tmp.Add(nodeIdx);
                            }
                        }
                    }
                    if (tmp.Count > 0)
                        ret = tmp.ToArray();
                }
            }
            else
            {
                ItemReference iref = (ItemReference)Destination.Item;
                ret = new byte[1];
                ret[0] = store[iref.Index.IndexInData];
            }
            return ret;
        }

        private void ProcessHeader(DataIndex dataIndex, string paramName)
        {
            dataIndex.IndexInData = BaseHeaderDefinition.Length;
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
                    dataIndex.IndexInData = dataIndex.IndexInData + bytesCount;
                }
            }
        }

        private Param _prevVariableParam;
        private void ProcessDataIndexes()
        {
            _prevVariableParam = null;
            byte indexInData = BaseHeaderDefinition.Length;
            foreach (var item in Param)
            {
                if (item.Bits % 8 == 0)
                {
                    if (string.IsNullOrEmpty(item.SizeRef))
                    {
                        byte bytesCount = (byte)(item.Bits / 8);
                        if (item.Size > 0)
                        {
                            bytesCount = (byte)(bytesCount * item.Size);
                        }
                        byte offsetInData = 0x00;
                        byte maskInData = 0xFF;

                        item.PreviousVariableParam = _prevVariableParam;
                        item.ItemRef = new ItemReference
                        {
                            Index = new DataIndex
                            {
                                IndexInData = indexInData,
                                MaskInData = maskInData,
                                OffsetInData = offsetInData
                            },
                            IndexOfOpt = GetDataIndex(item.OptRef)
                        };
                        if (item.Param1 != null && item.Param1.Count > 0)
                        {
                            foreach (var prm1 in item.Param1)
                            {
                                maskInData = Tools.GetMaskFromBits(prm1.Bits, offsetInData);
                                prm1.PreviousVariableParam = _prevVariableParam;
                                prm1.ItemRef = new ItemReference
                                {
                                    Index = new DataIndex
                                    {
                                        IndexInData = indexInData,
                                        MaskInData = maskInData,
                                        OffsetInData = offsetInData
                                    },
                                    IndexOfOpt = item.ItemRef.IndexOfOpt
                                };
                                offsetInData += prm1.Bits;
                            }
                        }
                        indexInData = (byte)(indexInData + bytesCount);
                        if (item.ItemRef.IndexOfOpt != null)
                        {
                            indexInData = 0;
                            _prevVariableParam = item;
                        }
                    }
                    else
                    {
                        item.PreviousVariableParam = _prevVariableParam;
                        item.ListRef = new ListReference
                        {
                            Index = new DataIndex { IndexInData = indexInData },
                            IndexOfOpt = GetDataIndex(item.OptRef),
                            IndexOfSize = GetDataIndex(item.SizeRef)
                        };
                        indexInData = 0;
                        _prevVariableParam = item;
                    }

                }
            }
        }

        private DataIndex GetDataIndex(string paramName)
        {
            if (!string.IsNullOrEmpty(paramName))
            {
                foreach (var item in Param)
                {
                    DataIndex ret;
                    if (item.Param1 != null && item.Param1.Count > 0)
                    {
                        foreach (var prm1 in item.Param1)
                        {
                            string[] tokens = paramName.Split('.');
                            if (tokens.Length == 2)
                            {
                                if (tokens[0] == item.Name && tokens[1] == prm1.Name)
                                {
                                    ret = prm1.ItemRef.Index;
                                    return ret;
                                }
                            }
                        }
                    }
                    if (item.Name == paramName)
                    {
                        ret = item.ItemRef.Index;
                        return ret;
                    }
                }
            }
            return null;
        }
    }
}
