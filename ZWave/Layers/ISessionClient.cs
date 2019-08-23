using System;
using System.Collections.Generic;

namespace ZWave.Layers
{
    public interface ISessionClient : IDisposable
    {
        byte SessionId { get; set; }
        bool IsHandleFrameEnabled { get; set; }
        void SetFuncId(byte value);
        Func<ActionHandlerResult, bool> SendFramesCallback { get; set; }
        Func<ActionBase, ActionBase> PostSubstituteAction { get; set; }
        void HandleActionCase(IActionCase actionCase);
        ActionToken ExecuteAsync(IActionItem action);
        void RunComponents();
        void RunComponentsDefault();
        void Cancel(Type type);
        void Cancel(ActionToken token);
        void ProcessNext(ActionHandlerResult ahResult);
        List<ISubstituteManager> GetSubstituteManagers();
        ISubstituteManager GetSubstituteManager(Type type);
        void AddSubstituteManager(ISubstituteManager sm, params ActionBase[] actions);
        void ClearSubstituteManagers();
        void TokenExpired(ActionToken actionToken);
    }
}
