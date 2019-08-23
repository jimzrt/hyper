using System;
using Utils;

namespace ZWave
{
    public class TimeInterval : IActionCase, IActionItem
    {
        public bool IsHandled { get; set; }
        public int Id { get; set; }
        public int ActionId { get; set; }
        public DateTime ExpireDateTime = DateTime.MinValue;
        public Action<IActionItem> CompletedCallback { get; set; }
        private ActionBase _parentAction;
        public ActionBase ParentAction
        {
            get { return _parentAction; }
            set { _parentAction = value; }
        }

        protected int mIntervalMs;
        public int IntervalMs
        {
            get
            {
                return GetValue();
            }
        }
        protected TimeInterval(int id)
        {
            Id = id;
        }

        public TimeInterval(int id, int intervalMs)
            : this(id)
        {
            mIntervalMs = intervalMs;
        }

        protected virtual int GetValue()
        {
            return mIntervalMs;
        }

        internal bool CheckExpired(DateTime currentDate)
        {
            return ExpireDateTime < currentDate;
        }

        public override string ToString()
        {
            return Tools.FormatStr("{0}; {1}ms", GetType().Name, GetValue());
        }
    }

    public class RandomTimeInterval : TimeInterval
    {
        public int MinIntervalMs { get; set; }
        public int MaxIntervalMs { get; set; }
        private Random rnd = new Random();

        public RandomTimeInterval(int id, int minIntervalMs, int maxIntervalMs)
            : base(id)
        {
            MinIntervalMs = minIntervalMs;
            MaxIntervalMs = maxIntervalMs;
        }

        protected override int GetValue()
        {
            mIntervalMs = rnd.Next(MinIntervalMs, MaxIntervalMs);
            return mIntervalMs;
        }
    }
}
