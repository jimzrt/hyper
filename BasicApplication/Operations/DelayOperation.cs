namespace ZWave.BasicApplication.Operations
{
    public class DelayOperation : ActionBase
    {
        private readonly int _timeoutMs;

        public DelayOperation(int timeoutMs)
            : base(false)
        {
            _timeoutMs = timeoutMs;
        }

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 0, new TimeInterval(Id, _timeoutMs)));
            ActionUnits.Add(new TimeElapsedUnit(Id, SetStateCompleted, 0));
        }

        protected override void CreateInstance()
        {

        }
    }
}
