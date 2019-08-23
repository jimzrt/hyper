using System.Xml.Serialization;

namespace ZWave.Xml.FrameHeader
{
    public partial class ListReference
    {
        [XmlIgnore]
        public DataIndex IndexOfOpt { get; set; }
        [XmlIgnore]
        public DataIndex IndexOfSize { get; set; }
        [XmlIgnore]
        public DataIndex Index { get; set; }
    }
}
