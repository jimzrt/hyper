using System;
using Utils.Events;

namespace ZWave.Layers.Session
{
    public class SessionLayer : ISessionLayer
    {
        public bool SuppressDebugOutput { get; set; }
        public event EventHandler<EventArgs<ActionToken>> ActionChanged;
        public string LogEntryPointClass { get; set; }
        public string LogPrefix { get; set; }
        public ISessionClient CreateClient()
        {
            SessionClient sessionClient = new SessionClient(ActionChangeCallback)
            {
                LogEntryPointClass = LogEntryPointClass,
                SuppressDebugOutput = SuppressDebugOutput
            };
            sessionClient.RunComponentsDefault();
            return sessionClient;
        }

        protected void ActionChangeCallback(ActionToken actionToken)
        {
            if (ActionChanged != null)
                ActionChanged(this, new EventArgs<ActionToken>(actionToken));
        }
    }
}
