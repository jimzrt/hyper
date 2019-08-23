using System;
using System.Collections.Generic;

namespace ZWave
{
    public class ActionUnit
    {
        public int TimeoutMs { get; protected set; }
        public bool IsCancelled { get; set; }
        public string Name { get; set; }

        public ActionHandler ActionHandler { get; set; }
        public Action<ActionUnit> Func { get; set; }
        public List<IActionItem> ActionItems { get; protected set; }

        public ActionUnit(params IActionItem[] actionItems)
            : this(null, actionItems)
        {
        }

        public ActionUnit(Action<ActionUnit> func, params IActionItem[] actionItems)
        {
            Func = func;
            if (actionItems != null && actionItems.Length > 0)
            {
                ActionItems = new List<IActionItem>(actionItems);
            }
            else
            {
                ActionItems = new List<IActionItem>();
            }

        }

        public void SetNextActionItems(params IActionItem[] actionItems)
        {
            lock (this)
            {
                if (actionItems != null && actionItems.Length > 0)
                {
                    ActionItems = new List<IActionItem>(actionItems);
                }
                else if (ActionItems != null)
                {
                    ActionItems.Clear();
                }
            }
        }

        public void AddNextActionItems(params IActionItem[] actionItems)
        {
            lock (this)
            {
                if (ActionItems == null)
                {
                    SetNextActionItems(actionItems);
                }
                else
                {
                    ActionItems.AddRange(actionItems);
                }
            }
        }

        public void AddFirstNextActionItems(params IActionItem[] actionItems)
        {
            lock (this)
            {
                if (ActionItems == null)
                {
                    SetNextActionItems(actionItems);
                }
                else
                {
                    ActionItems.InsertRange(0, actionItems);
                }
            }
        }

        public virtual bool TryHandle(IActionCase actionCase)
        {
            return ActionHandler != null && ActionHandler.State != HandlerStates.Handled && ActionHandler.WaitingFor(actionCase);
        }
    }
}
