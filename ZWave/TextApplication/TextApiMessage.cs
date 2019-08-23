namespace ZWave.TextApplication
{
    public class TextApiMessage : CommandMessage
    {
        public TextApiMessage(params byte[] inputParameters)
        {
            AddData(inputParameters);
        }
    }
}
