namespace ZWave
{
    public class ActionHandler
    {
        public HandlerStates State { get; set; }
        public virtual bool WaitingFor(IActionCase actionCase)
        {
            return false;
        }
    }
}
