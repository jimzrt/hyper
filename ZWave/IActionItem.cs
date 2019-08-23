using System;

namespace ZWave
{
    public interface IActionItem
    {
        Action<IActionItem> CompletedCallback { get; set; }
        ActionBase ParentAction { get; set; }
    }
}
