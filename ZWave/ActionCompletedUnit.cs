using System;
using System.Collections.Generic;

namespace ZWave
{
    public class ActionCompletedUnit : ActionUnit
    {
        public ActionBase Action
        {
            get { return ((ActionCompletedHandler)ActionHandler).Action; }
        }

        public ActionCompletedUnit(ActionBase action, Action<ActionCompletedUnit> func, params IActionItem[] items)
        {
            ActionHandler = new ActionCompletedHandler(action);

            if (func != null)
            {
                Func = x => func((ActionCompletedUnit)x);
            }

            if (items != null)
            {
                ActionItems = new List<IActionItem>(items);
            }
        }
    }
}
