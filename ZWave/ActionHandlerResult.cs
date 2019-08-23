using System;
using System.Collections.Generic;

namespace ZWave
{
    public class ActionHandlerResult
    {
        public ActionBase Parent { get; set; }
        public Action<bool> NextFramesCompletedCallback { get; set; }

        private List<IActionItem> mNextActions = new List<IActionItem>();
        public List<IActionItem> NextActions
        {
            get { return mNextActions; }
            set { mNextActions = value; }
        }

        public ActionHandlerResult(ActionBase parent)
        {
            Parent = parent;
        }
    }
}
