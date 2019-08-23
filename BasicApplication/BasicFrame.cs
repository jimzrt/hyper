namespace ZWave.BasicApplication
{
    public class BasicFrame
    {
        public BasicFrame(DataFrame dataFrame)
        {
            Command = dataFrame.ToString();
        }

        public string Command { get; set; }
    }
}
