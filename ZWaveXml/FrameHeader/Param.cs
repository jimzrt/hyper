using System.Xml.Serialization;

namespace ZWave.Xml.FrameHeader
{
    public partial class Param
    {

        private Header _parentHeaderField;
        [XmlIgnore]
        public Header ParentHeader
        {
            get
            {
                return _parentHeaderField;
            }
            set
            {
                _parentHeaderField = value;
                RaisePropertyChanged("ParentHeader");
            }
        }

        private Param _parentParamField;
        [XmlIgnore]
        public Param ParentParam
        {
            get
            {
                return _parentParamField;
            }
            set
            {
                _parentParamField = value;
                RaisePropertyChanged("ParentParam");
            }
        }

        [XmlIgnore]
        public ItemReference ItemRef { get; set; }

        [XmlIgnore]
        public ListReference ListRef { get; set; }

        [XmlIgnore]
        public Param PreviousVariableParam { get; set; }
    }
}
