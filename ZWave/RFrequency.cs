namespace ZWave
{
    public class RFrequency
    {
        public byte Channels { get; set; }
        public string Name { get; set; }
        public RFrequency(byte channels, string name)
        {
            Channels = channels;
            Name = name;
        }
    }
}
