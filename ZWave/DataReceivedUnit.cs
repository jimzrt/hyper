using System;
using System.Collections.Generic;
using ZWave.Layers.Frame;

namespace ZWave
{
    public class DataReceivedUnit : ActionUnit
    {
        public CustomDataFrame DataFrame
        {
            get { return ((CommandHandler)ActionHandler).DataFrame; }
        }

        public DataReceivedUnit(CommandHandler handler, Action<DataReceivedUnit> func, int timeoutMs, params IActionItem[] items)
        {
            ActionHandler = handler;
            if (func != null)
                Func = x => func((DataReceivedUnit)x);
            TimeoutMs = timeoutMs;
            if (items != null)
                ActionItems = new List<IActionItem>(items);
        }

        public DataReceivedUnit(CommandHandler handler, Action<DataReceivedUnit> func, params IActionItem[] items) :
            this(handler, func, 0, items)
        {

        }
    }
}
