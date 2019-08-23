namespace ZWave.BasicApplication
{
    public class InsistentAction : ApiOperation
    {
        private ActionBase _action;
        private int _maxAttemptsCount;
        private int _interval;
        public InsistentAction(ActionBase action, int maxAttemptsCount, int interval)
            : base(false, null, false)
        {
            _action = action;
            _maxAttemptsCount = maxAttemptsCount;
            _interval = interval;
        }

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 3000, _action));
            ActionUnits.Add(new ActionCompletedUnit(_action, OnActionCompleted, new TimeInterval(Id, _interval)));
            ActionUnits.Add(new TimeElapsedUnit(Id, null, 0, _action));
        }

        protected override void CreateInstance()
        {

        }

        private void OnActionCompleted(ActionUnit au)
        {
            if (_action.Result || _maxAttemptsCount == 0)
            {
                SetStateCompleted(au);
            }
            else
            {
                _maxAttemptsCount--;
                _action.NewToken();
            }
        }
    }
}
