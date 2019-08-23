using System;
using System.Collections.Generic;

namespace ZWave
{
    public class TimeElapsedUnit : ActionUnit
    {
        public TimeElapsedUnit(int timeIntervalId, Action<TimeElapsedUnit> func, int timeoutMs, params IActionItem[] items)
        {
            ActionHandler = new TimeElapsedHandler(timeIntervalId);
            if (func != null)
                Func = x => func((TimeElapsedUnit)x);
            TimeoutMs = timeoutMs;
            if (items != null)
                ActionItems = new List<IActionItem>(items);
        }
    }
}
