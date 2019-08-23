using System;
using System.Collections.Generic;

namespace ZWave
{
    public class StartActionUnit : ActionUnit
    {
        public StartActionUnit(Action<StartActionUnit> func, int timeoutMs, params IActionItem[] items)
        {
            if (func != null)
            {
                Func = x => func((StartActionUnit)x);
            }

            TimeoutMs = timeoutMs;

            if (items != null && items.Length > 0)
            {
                ActionItems = new List<IActionItem>(items);
            }
        }
    }
}
