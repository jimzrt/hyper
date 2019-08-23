using System;
using Utils.UI.Enums;

namespace Utils.UI.Logging
{
    public class LogPacket
    {
        public DateTime Timestamp { get; set; }

        public Dyes Dye { get; set; }

        public string Text { get; set; }

        public bool IsBold { get; set; }

        public bool IsFixed { get; set; }

        public bool IsLineBreak { get; set; }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Text); }
        }

        public LogPacket(string text, Dyes dye, bool isBold, bool isFixed, bool isLineBreak)
        {
            Timestamp = DateTime.Now;
            Dye = dye;
            Text = text;
            IsBold = isBold;
            IsFixed = isFixed;
            IsLineBreak = isLineBreak;
        }
    }
}
