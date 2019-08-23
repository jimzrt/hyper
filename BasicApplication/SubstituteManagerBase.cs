using System;
using System.Collections.Generic;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;
using ZWave.Layers;
using ZWave.Layers.Frame;

namespace ZWave.BasicApplication
{
    public abstract class SubstituteManagerBase : ISubstituteManager
    {
        #region ISubstituteManager Members

        public SubstituteIncomingFlags Id
        {
            get { return GetId(); }
        }

        public bool IsActive { get; protected set; }

        protected abstract SubstituteIncomingFlags GetId();

        public SubstituteManagerBase()
        {
            IsActive = true;
        }

        public static bool TryParseCommand(CustomDataFrame packet, out byte destNodeId, out byte srcNodeId, out int lenIndex, out byte[] cmdData)
        {
            bool ret = false;
            cmdData = null;
            srcNodeId = 0;
            destNodeId = 0;
            lenIndex = 0;
            if (packet != null && packet.Data != null && packet.Data.Length > 1 &&
                (packet.Data[1] == (byte)CommandTypes.CmdApplicationCommandHandler || packet.Data[1] == (byte)CommandTypes.CmdApplicationCommandHandler_Bridge))
            {
                int srcIndex = 3;
                lenIndex = 4;
                int destIndex = -1;
                byte[] frameData = packet.Data;
                if (frameData[1] == (byte)CommandTypes.CmdApplicationCommandHandler_Bridge)
                {
                    destIndex = 3;
                    srcIndex = 4;
                    lenIndex = 5;
                }
                if (frameData.Length > lenIndex + 2)
                {
                    srcNodeId = frameData[srcIndex];
                    if (destIndex > 0)
                    {
                        destNodeId = frameData[destIndex];
                    }
                    cmdData = new byte[frameData[lenIndex]];
                    Array.Copy(frameData, lenIndex + 1, cmdData, 0, cmdData.Length);
                    if (cmdData.Length > 2)
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }

        public CustomDataFrame SubstituteIncoming(CustomDataFrame packet, out ActionBase additionalAction, out ActionBase completeAction)
        {
            CustomDataFrame ret = packet;
            additionalAction = null;
            completeAction = null;
            byte[] cmdData;
            byte srcNodeId;
            byte destNodeId;
            int lenIndex;
            if (TryParseCommand(packet, out destNodeId, out srcNodeId, out lenIndex, out cmdData))
            {
                ret = SubstituteIncomingInternal(packet, destNodeId, srcNodeId, cmdData, lenIndex, out additionalAction, out completeAction);
            }
            return ret;
        }

        protected virtual CustomDataFrame SubstituteIncomingInternal(CustomDataFrame packet, byte destNodeId, byte srcNodeId, byte[] cmdData, int lenIndex, out ActionBase additionalAction, out ActionBase completeAction)
        {
            additionalAction = null;
            completeAction = null;
            return null;
        }

        protected CustomDataFrame CreateNewFrame(CustomDataFrame packet, byte[] newFrameData)
        {
            var ret = new DataFrame(packet.SessionId, packet.DataFrameType, false, false, packet.SystemTimeStamp);
            byte[] buffer = DataFrame.CreateFrameBuffer(newFrameData);
            ret.SetBuffer(buffer, 0, buffer.Length);
            ret.SubstituteIncomingFlags = packet.SubstituteIncomingFlags | GetId();
            ret.Extensions = packet.Extensions;
            return ret;
        }

        protected CustomDataFrame CreateNewFrame(CustomDataFrame packet, byte[] newFrameData, int cmdLength)
        {
            var ret = new DataFrame(packet.SessionId, packet.DataFrameType, false, false, packet.SystemTimeStamp);
            byte[] buffer = DataFrame.CreateFrameBuffer(newFrameData);
            ret.SetBuffer(buffer, cmdLength);
            ret.SubstituteIncomingFlags = packet.SubstituteIncomingFlags | GetId();
            return ret;
        }

        public virtual void OnIncomingSubstituted(CustomDataFrame dataFrameOri, CustomDataFrame dataFrameSub, List<ActionHandlerResult> ahResults)
        {

        }

        public ActionBase SubstituteAction(ActionBase action)
        {
            ActionBase ret = null;

            ApiOperation apiAction = action as ApiOperation;
            if (apiAction != null)
            {
                Action<IActionItem> completedCallback = action.CompletedCallback;
                ActionToken token = action.Token;
                ActionResult result = action.Result;
                ActionBase parent = action.ParentAction;
                int actionId = action.Id;

                ret = SubstituteActionInternal(apiAction);

                if (ret != null)
                {
                    ret.Id = actionId;
                    ret.ParentAction = parent;
                    ret.Token = token;
                    if (ret.CompletedCallback == null)
                    {
                        ret.CompletedCallback = completedCallback;
                    }
                    else
                    {
                        var newCompletedCallback = ret.CompletedCallback;
                        ret.CompletedCallback = (x) =>
                        {
                            newCompletedCallback(x);
                            completedCallback(x);
                        };
                    }
                }
            }
            return ret;
        }

        public virtual ActionBase SubstituteActionInternal(ApiOperation action)
        {
            ActionBase ret = null;
            return ret;
        }

        public virtual List<ActionToken> GetRunningActionTokens()
        {
            return null;
        }

        public virtual void AddRunningActionToken(ActionToken token)
        {
        }

        public virtual void RemoveRunningActionToken(ActionToken token)
        {
        }

        public virtual void SetDefault()
        {
        }

        public virtual void Suspend()
        {
            IsActive = false;
        }

        public virtual void Resume()
        {
            IsActive = true;
        }

        #endregion
    }
}
