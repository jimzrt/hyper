using System.Collections.Generic;
using ZWave.Enums;
using ZWave.Layers.Frame;

namespace ZWave.Layers
{
    public interface ISubstituteManager
    {
        SubstituteIncomingFlags Id { get; }
        CustomDataFrame SubstituteIncoming(CustomDataFrame packet, out ActionBase additionalAction /*Optional*/, out ActionBase completeAction /*Optional*/);
        void OnIncomingSubstituted(CustomDataFrame dataFrameOri, CustomDataFrame dataFrameSub, List<ActionHandlerResult> ahResults);
        ActionBase SubstituteAction(ActionBase runningOperation);
        List<ActionToken> GetRunningActionTokens();
        void AddRunningActionToken(ActionToken token);
        void RemoveRunningActionToken(ActionToken token);
        void SetDefault();
        void Suspend();
        void Resume();
        bool IsActive { get; }
    }
}
