using System;
using System.Linq;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// HOST->ZW: REQ | 0x60 | NodeID
    /// ZW->HOST: RES | 0x60 | retVal
    /// 
    /// ZW->HOST: REQ | 0x49 | bStatus | bNodeID | bLen | basic | generic | specific | commandclasses[ ]
    /// </summary>
    public class RequestNodeInfoOperation : ApiOperation
    {
        internal byte NodeId { get; set; }
        int _timeoutMs;

        public RequestNodeInfoOperation(byte nodeId)
            : this(nodeId, 5000)
        {
        }

        public RequestNodeInfoOperation(byte nodeId, int timeoutMs)
            : base(true, CommandTypes.CmdZWaveRequestNodeInfo, false)
        {
            NodeId = nodeId;
            _timeoutMs = timeoutMs;
        }

        private ApiMessage messageRequest;
        private ApiHandler handlerResponseTrue;
        private ApiHandler handlerResponseFalse;

        private ApiHandler handlerNodeInfoReceived;
        private ApiHandler handlerNodeInfoReqDone;
        private ApiHandler handlerNodeInfoReqFailed;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(null, 5000, messageRequest));
            //ActionUnits.Add(new DataReceivedUnit(handlerResponseTrue, null));
            ActionUnits.Add(new DataReceivedUnit(handlerResponseFalse, SetStateFailed));
            //ActionUnits.Add(new DataReceivedUnit(handlerNodeInfoReqDone, null));
            ActionUnits.Add(new DataReceivedUnit(handlerNodeInfoReqFailed, SetStateFailed));
            ActionUnits.Add(new DataReceivedUnit(handlerNodeInfoReceived, SetStateCompleted));
        }

        protected override void CreateInstance()
        {
            messageRequest = new ApiMessage(CommandTypes.CmdZWaveRequestNodeInfo, NodeId);


            handlerResponseTrue = new ApiHandler(CommandTypes.CmdZWaveRequestNodeInfo);
            handlerResponseTrue.AddConditions(new ByteIndex(0x01));

            handlerResponseFalse = new ApiHandler(CommandTypes.CmdZWaveRequestNodeInfo);
            handlerResponseFalse.AddConditions(new ByteIndex(0x00));

            handlerNodeInfoReceived = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationControllerUpdate);
            handlerNodeInfoReceived.AddConditions(new ByteIndex((byte)ControllerUpdateStatuses.NodeInfoReceived), new ByteIndex(NodeId));

            handlerNodeInfoReqDone = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationControllerUpdate);
            handlerNodeInfoReqDone.AddConditions(new ByteIndex((byte)ControllerUpdateStatuses.NodeInfoReqDone), new ByteIndex(NodeId));

            handlerNodeInfoReqFailed = new ApiHandler(FrameTypes.Request, CommandTypes.CmdApplicationControllerUpdate);
            handlerNodeInfoReqFailed.AddConditions(new ByteIndex((byte)ControllerUpdateStatuses.NodeInfoReqFailed));
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            byte[] res = ((DataReceivedUnit)ou).DataFrame.Payload;
            if (res[2] > 0)
            {
                try
                {
                    SpecificResult.NodeId = NodeId;
                    SpecificResult.NodeInfo = new byte[res[2]];
                    Array.Copy(res, 3, SpecificResult.NodeInfo, 0, SpecificResult.NodeInfo.Length);
                    if (SpecificResult.NodeInfo.Length > 0)
                    {
                        SpecificResult.Basic = SpecificResult.NodeInfo[0];
                    }
                    if (SpecificResult.NodeInfo.Length > 1)
                    {
                        SpecificResult.Generic = SpecificResult.NodeInfo[1];
                    }
                    if (SpecificResult.NodeInfo.Length > 2)
                    {
                        SpecificResult.Specific = SpecificResult.NodeInfo[2];
                    }
                    if (SpecificResult.NodeInfo.Length > 3)
                    {
                        SpecificResult.CommandClasses = SpecificResult.NodeInfo.Skip(3).TakeWhile(x => x != 0xEF).ToArray();
                    }
                }
                catch (Exception)
                { }
            }
            base.SetStateCompleted(ou);
        }

        public RequestNodeInfoResult SpecificResult
        {
            get { return (RequestNodeInfoResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new RequestNodeInfoResult();
        }
    }

    public class RequestNodeInfoResult : ActionResult
    {
        public byte NodeId { get; set; }
        public byte[] NodeInfo { get; set; }
        public byte Basic { get; set; }
        public byte Generic { get; set; }
        public byte Specific { get; set; }
        public byte[] CommandClasses { get; set; }
        public byte[] SecureCommandClasses { get; set; }
        public SecuritySchemes[] SecuritySchemes { get; set; }

        internal void CopyTo(RequestNodeInfoResult res)
        {
            res.NodeId = NodeId;
            res.NodeInfo = NodeInfo;
            res.Basic = Basic;
            res.Generic = Generic;
            res.Specific = Specific;
            res.CommandClasses = CommandClasses;
            res.SecureCommandClasses = SecureCommandClasses;
            res.SecuritySchemes = SecuritySchemes;
        }
    }
}
