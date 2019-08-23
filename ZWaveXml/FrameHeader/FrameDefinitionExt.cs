using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Utils;

namespace ZWave.Xml.FrameHeader
{
    public partial class FrameDefinition
    {
        private readonly Dictionary<byte, IList<ValidationLeaf>> _frameHeaderRules = new Dictionary<byte, IList<ValidationLeaf>>();
        [XmlIgnore]
        public Dictionary<byte, byte> FrequencyHeaders = new Dictionary<byte, byte>();

        public static FrameDefinition Load(string path)
        {
            FrameDefinition ret = null;
            XmlSerializer zwFrameXmlSerializer = new XmlSerializer(typeof(FrameDefinition));
            XmlReader reader = XmlReader.Create(path);
            try
            {
                ret = (FrameDefinition)zwFrameXmlSerializer.Deserialize(reader);
            }
            catch (InvalidOperationException)
            {
            }
            finally
            {
                reader.Close();
            }
            if (ret != null)
            {
                ret.InitFrameDefinition();
                ret.AssignParentProperties();
            }
            return ret;
        }

        private void AssignParentProperties()
        {
            foreach (var item in Header)
            {
                if (item.Param != null)

                    foreach (var par in item.Param)
                    {
                        par.ParentHeader = item;
                        if (par.Param1 != null && par.Param1.Count > 0)
                            foreach (var par1 in par.Param1)
                            {
                                par1.ParentParam = par;
                                par1.ParentHeader = item;
                            }
                    }
            }
        }

        private void InitFrameDefinition()
        {
            _frameHeaderRules.Clear();
            FrequencyHeaders.Clear();
            foreach (var item in BaseHeader)
            {
                if (RadioFrequency != null)
                {
                    foreach (var f in RadioFrequency)
                    {
                        if (f.BaseHeader == item.Key)
                        {
                            FrequencyHeaders.Add(f.Code, item.Key);
                        }
                    }
                }
                //foreach (var f in item.Frequencies)
                //{
                //    mFrequencyHeaders.Add(f, item.Key);
                //}
                _frameHeaderRules.Add(item.Key, new List<ValidationLeaf>());
                item.Initialize();
            }
            foreach (var item in Header)
            {
                if (item.Validation != null && item.Validation.Count > 0)
                {
                    item.Initialize(BaseHeader.FirstOrDefault(x => x.Key == item.BaseHeaderKey));
                    foreach (var val in item.Validation)
                    {
                        val.Initialize(item);
                    }
                    InsertRules(item.Validation, item);
                }
            }
        }


        private void InsertRules(IEnumerable<Validation> validations, Header header)
        {
            IEnumerator<Validation> valsNew = validations.GetEnumerator();
            if (valsNew.MoveNext())
            {
                ValidationLeaf existingVl = _frameHeaderRules[header.BaseHeaderDefinition.Key].FirstOrDefault(x => AreEqualRules(x.Rule, valsNew.Current));
                if (existingVl != null)
                {
                    InsertRules(valsNew, header, existingVl);
                }
                else
                {
                    ValidationLeaf newLeaf = new ValidationLeaf
                    {
                        Rule = valsNew.Current,
                        Results = new Dictionary<byte, IList<ValidationLeaf>>()
                    };
                    ProcessLeaves(valsNew, header, newLeaf);
                    _frameHeaderRules[header.BaseHeaderDefinition.Key].Add(newLeaf);
                }
            }

        }

        private static void InsertRules(IEnumerator<Validation> valsNew, Header header, ValidationLeaf existingVl)
        {
            byte key = Tools.GetByte(valsNew.Current.ParamHexValue);
            if (existingVl.Results.ContainsKey(key))
            {
                if (valsNew.MoveNext())
                {
                    IList<ValidationLeaf> children = existingVl.Results[key];
                    ValidationLeaf child = children.FirstOrDefault(x => AreEqualRules(x.Rule, valsNew.Current));
                    if (child != null)
                    {
                        InsertRules(valsNew, header, child);
                    }
                    else
                    {
                        //parent has another child with same key
                        ValidationLeaf newLeaf = new ValidationLeaf
                        {
                            Rule = valsNew.Current,
                            Results = new Dictionary<byte, IList<ValidationLeaf>>()
                        };
                        ProcessLeaves(valsNew, header, newLeaf);
                        children.Add(newLeaf);
                    }
                }
                else
                {
                    foreach (var item in existingVl.Results[key])
                    {
                        item.Header = header;
                    }
                }
            }
            else
            {
                // same rule with new key
                ProcessLeaves(valsNew, header, existingVl);
            }

        }

        private static void ProcessLeaves(IEnumerator<Validation> valsNew, Header header, ValidationLeaf rootLeaf)
        {
            byte parentKey = Tools.GetByte(valsNew.Current.ParamHexValue);
            ValidationLeaf parentVl = rootLeaf;
            while (valsNew.MoveNext())
            {
                ValidationLeaf vl = new ValidationLeaf
                {
                    Rule = valsNew.Current,
                    Results = new Dictionary<byte, IList<ValidationLeaf>>()
                };
                IList<ValidationLeaf> vlList = new List<ValidationLeaf>();
                vlList.Add(vl);
                parentVl.Results.Add(parentKey, vlList);
                parentVl = vl;
                parentKey = Tools.GetByte(valsNew.Current.ParamHexValue);
            }
            ValidationLeaf headerLeaf = new ValidationLeaf
            {
                Header = header,
                Rule = null,
                Results = new Dictionary<byte, IList<ValidationLeaf>>()
            };
            IList<ValidationLeaf> lvl = new List<ValidationLeaf>();
            lvl.Add(headerLeaf);
            parentVl.Results.Add(parentKey, lvl);
        }



        private static bool AreEqualRules(Validation val1, Validation val2)
        {
            return val1.DataIndex.IndexInData == val2.DataIndex.IndexInData && val1.DataIndex.MaskInData == val2.DataIndex.MaskInData && val1.DataIndex.OffsetInData == val2.DataIndex.OffsetInData;
        }

        public bool ParseHeaderWithCrc(byte[] data, byte crcBytes, byte frequency, out Header header)
        {
            header = null;
            bool ret = false;
            if (data.Length > 7 + crcBytes && CrcOk(data, data[7], crcBytes) && FrequencyHeaders.ContainsKey(frequency))
            {
                ret = true;
                ValidationLeaf vl = GetHeader(data, _frameHeaderRules[FrequencyHeaders[frequency]]);
                if (vl != null)
                {
                    if (vl.Header != null)
                        header = vl.Header;
                }
            }
            return ret;
        }



        private static ValidationLeaf GetHeader(byte[] data, IList<ValidationLeaf> vLeaves)
        {
            ValidationLeaf ret = null;

            foreach (var item in vLeaves)
            {
                if (item.Header != null)
                {
                    //System.Diagnostics.Debug.Write("{" + item.Header.Name + "}");
                    ret = item;
                }
            }

            if (ret == null)
            {
                foreach (var item in vLeaves)
                {
                    if (data.Length > item.Rule.DataIndex.IndexInData)
                    {
                        byte val = (byte)((data[item.Rule.DataIndex.IndexInData] & item.Rule.DataIndex.MaskInData) >> item.Rule.DataIndex.OffsetInData);
                        if (item.Results.ContainsKey(val))
                        {
                            //System.Diagnostics.Debug.Write(item.Rule.ParamName + ":TRUE->");
                            ret = GetHeader(data, item.Results[val]);
                            if (ret != null)
                                break;
                        }
                    }
                }
            }
            else
            {
                while (ret.Rule != null)
                {
                    if (data.Length > ret.Rule.DataIndex.IndexInData)
                    {
                        byte val = (byte)((data[ret.Rule.DataIndex.IndexInData] & ret.Rule.DataIndex.MaskInData) >> ret.Rule.DataIndex.OffsetInData);
                        if (ret.Results.ContainsKey(val))
                        {
                            //System.Diagnostics.Debug.Write(ret.Rule.ParamName + ":TRUE->");
                            ValidationLeaf deep = GetHeader(data, ret.Results[val]);
                            if (deep != null)
                                ret = deep;
                        }
                        else
                        {
                            //System.Diagnostics.Debug.Write(ret.Rule.ParamName + ":FALSE->");
                            break;
                        }
                    }
                    else
                        break;

                }
            }
            return ret;
        }

        public void ProcessParametersTextValue(IList<ParamValue> paramValues)
        {
            foreach (var item in paramValues)
            {
                if (item.InnerValues != null)
                {
                    ProcessParametersTextValue(item.InnerValues);
                }
                IEnumerable<string> range = ParameterToString(item.ByteValueList, item.ParamDefinition.Type);
                foreach (var str in range)
                {
                    item.TextValueList.Add(str);
                }
            }
        }

        private static IEnumerable<string> ParameterToString(IList<byte> list, zwParamType type)
        {
            IList<string> ret = new List<string>();
            switch (type)
            {
                case zwParamType.CHAR:
                    try
                    {
                        ret.Add(Encoding.ASCII.GetString(list.ToArray(), 0, list.Count));
                    }
                    catch (Exception dex)
                    {
                        dex.Message._EXLOG();
                    }
                    break;
                case zwParamType.HEX:
                    ret.Add(Tools.GetHex(list));
                    break;
                case zwParamType.NUMBER:
                    if (list.Count <= 4) //int
                    {
                        int tmp = 0;
                        switch (list.Count)
                        {
                            case 1:
                                tmp = list[0];
                                break;
                            case 2:
                                tmp = (list[0] << 8) + list[1];
                                break;
                            case 3:
                                tmp = (list[0] << 16) + (list[1] << 8) + list[2];
                                break;
                            case 4:
                                tmp = (list[0] << 24) + (list[1] << 16) + (list[2] << 8) + list[3];
                                break;
                        }
                        if (tmp < 99)
                            ret.Add(tmp.ToString(""));
                        else
                            ret.Add(tmp.ToString());
                    }
                    else // as HEX
                    {
                        ret.Add(Tools.GetHex(list));
                    }
                    break;
                case zwParamType.NODE_NUMBER:
                    var first = true;
                    var sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        if (!first)
                        {
                            sb.Append(", ");
                        }
                        first = false;
                        sb.Append(item.ToString(""));
                    }
                    ret.Add(sb.ToString());
                    break;
                case zwParamType.BITMASK:
                    for (int i = 0; i < list.Count; i++)
                    {

                        byte maskByte = list[i];
                        if (maskByte == 0)
                        {
                            continue;
                        }
                        byte bitMask = 0x01;
                        const byte bitOffset = 0x01; //nodes starting from 1 in mask bytes array
                        for (int j = 0; j < 8; j++)
                        {
                            if ((bitMask & maskByte) != 0)
                            {
                                byte nodeId = (byte)(i * 8 + j + bitOffset);
                                ret.Add(nodeId.ToString(""));
                            }
                            bitMask <<= 1;
                        }
                    }
                    break;
                case zwParamType.BOOLEAN:
                    if (list.Count > 0)
                    {
                        ret.Add(list[0] > 0 ? "true" : "false");
                    }
                    else
                        ret.Add("false");
                    break;
            }
            return ret;
        }

        private const uint Poly = 0x1021;          /* crc-ccitt mask */
        private static void update_crc(ushort ch, ref ushort crc)
        {
            ushort i;
            ushort v = 0x80;
            for (i = 0; i < 8; i++)
            {
                ushort xorFlag;
                if ((crc & 0x8000) != 0)
                {
                    xorFlag = 1;
                }
                else
                {
                    xorFlag = 0;
                }
                crc = (ushort)(crc << 1);

                if ((ch & v) != 0)
                {
                    crc = (ushort)(crc + 1);
                }

                if (xorFlag != 0)
                {
                    crc = (ushort)(crc ^ Poly);
                }
                v = (ushort)(v >> 1);
            }
        }
        private static void augment_message_for_crc(ref ushort crc)
        {
            ushort i;

            for (i = 0; i < 16; i++)
            {
                ushort xorFlag;
                if ((crc & 0x8000) != 0)
                {
                    xorFlag = 1;
                }
                else
                {
                    xorFlag = 0;
                }
                crc = (ushort)(crc << 1);

                if (xorFlag != 0)
                {
                    crc = (ushort)(crc ^ Poly);
                }
            }
        }
        private static bool CrcOk(byte[] data, byte length, byte crcBytes)
        {
            bool ret = false;
            switch (crcBytes)
            {
                case 1:
                    if (data.Length == length)
                    {
                        byte sum = 0xFF;
                        byte temp = data[length - 1];
                        for (int i = 0; i < length - 1; i++)
                            sum ^= data[i];
                        if (sum == temp)
                        {
                            ret = true;
                        }
                    }
                    break;
                case 2:
                    if (data.Length == length)
                    {
                        ushort sum = 0xFFFF;
                        for (int i = 0; i < length - 2; i++)
                        {
                            update_crc(data[i], ref sum);
                        }
                        augment_message_for_crc(ref sum);

                        var temp = (ushort)((data[length - 2] << 8) + data[length - 1]);
                        if (sum == temp)
                        {
                            ret = true;
                        }
                    }
                    break;
            }
            return ret;
        }
    }
}
