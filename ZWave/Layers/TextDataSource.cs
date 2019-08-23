using System.IO;
using Utils;

namespace ZWave.Layers
{
    public class TextDataSource : IDataSource
    {
        public string SourceName { get; private set; }
        public string Args { get; private set; }
        public string Alias { get; private set; }
        public TextWriter Writer { get; private set; }
        public TextReader Reader { get; private set; }
        public TextDataSource(TextWriter writer, TextReader reader)
        {
            Writer = writer;
            Reader = reader;
        }

        public bool Validate()
        {
            return true;
        }

        public override string ToString()
        {
            return SourceName;
        }
    }
}
