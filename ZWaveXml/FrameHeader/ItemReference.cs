using System.Xml.Serialization;

namespace ZWave.Xml.FrameHeader
{
    public partial class ItemReference
    {
        [XmlIgnore]
        public DataIndex IndexOfOpt { get; set; }
        [XmlIgnore]
        public DataIndex Index { get; set; }
    }
}
