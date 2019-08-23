using System;
using Utils.Events;

namespace ZWave.Layers
{
    /// <summary>
    /// Provides the features required to support communication sessions.
    /// </summary>
    public interface ISessionLayer
    {
        bool SuppressDebugOutput { get; set; }
        ISessionClient CreateClient();
        event EventHandler<EventArgs<ActionToken>> ActionChanged;
        /// <summary>
        /// Add debug info about line number in the specified class in the stack trace
        /// </summary>
    }
}
