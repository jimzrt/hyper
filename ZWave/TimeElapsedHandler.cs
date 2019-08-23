namespace ZWave
{
    public class TimeElapsedHandler : ActionHandler
    {
        public int Id { get; set; }

        public TimeElapsedHandler(int id)
        {
            Id = id;
        }

        public override bool WaitingFor(IActionCase actionCase)
        {
            bool ret = false;
            if (actionCase is TimeInterval)
            {
                TimeInterval receivedValue = (TimeInterval)actionCase;
                if (Id == receivedValue.Id)
                    ret = true;
            }
            return ret;
        }
    }
}
