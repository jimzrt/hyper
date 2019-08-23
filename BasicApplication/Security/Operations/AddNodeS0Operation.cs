using System.Collections.Generic;
using System.Threading;
using Utils;
using ZWave.BasicApplication.Enums;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Devices;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class AddNodeS0Operation : ApiOperation
    {
        #region Timeouts
        /// <summary>
        /// Time from CMD sent to CMD received (CMD - any command except NONCE commands)
        /// </summary>
        public static int CMD_TIMEOUT = 10000;
        #endregion
        private SecurityManagerInfo SecurityManagerInfo { get; set; }
        internal AddNodeS0Operation(SecurityManagerInfo securityManagerInfo)
            : base(false, null, false)
        {
            SecurityManagerInfo = securityManagerInfo;
        }

        RequestDataOperation requestScheme;
        SendDataOperation sendNetworkKeySet;
        ExpectDataOperation expectNetworkKeyVerify;
        RequestDataOperation requestSchemeInherit;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnAddNodeCompleted, 0, requestScheme));
            ActionUnits.Add(new ActionCompletedUnit(requestScheme, OnSchemeReport, expectNetworkKeyVerify, sendNetworkKeySet));
            ActionUnits.Add(new ActionCompletedUnit(sendNetworkKeySet, OnNetworkKeySet));
            ActionUnits.Add(new ActionCompletedUnit(expectNetworkKeyVerify, OnNetworkKeyVerify));
            ActionUnits.Add(new ActionCompletedUnit(requestSchemeInherit, OnSchemeInheritReport));
        }

        protected override void CreateInstance()
        {
            requestScheme = new RequestDataOperation(0, 0,
                new COMMAND_CLASS_SECURITY.SECURITY_SCHEME_GET() { supportedSecuritySchemes = SecurityManagerInfo.SecuritySchemeInGetS0 },
                SecurityManagerInfo.TxOptions,
                new COMMAND_CLASS_SECURITY.SECURITY_SCHEME_REPORT(), 2, CMD_TIMEOUT);
            requestScheme.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);

            sendNetworkKeySet = new SendDataOperation(0, null, SecurityManagerInfo.TxOptions);

            expectNetworkKeyVerify = new ExpectDataOperation(0, 0, new COMMAND_CLASS_SECURITY.NETWORK_KEY_VERIFY(), 2, CMD_TIMEOUT);

            requestSchemeInherit = new RequestDataOperation(0, 0,
                new COMMAND_CLASS_SECURITY.SECURITY_SCHEME_INHERIT() { supportedSecuritySchemes = SecurityManagerInfo.SecuritySchemeInInheritS0 },
                SecurityManagerInfo.TxOptions,
                new COMMAND_CLASS_SECURITY.SECURITY_SCHEME_REPORT(), 2, CMD_TIMEOUT);
        }

        protected void SetStateCompletedSecurityFailed(ActionUnit ou)
        {
            SecurityManagerInfo.IsInclusion = false;
            SecurityManagerInfo.Network.ResetSecuritySchemes(SpecificResult.Id);
            SecurityManagerInfo.Network.SetSecuritySchemesSpecified(SpecificResult.Id);
            SetStateCompleted(ou);
        }

        protected override void SetStateCompleted(ActionUnit ou)
        {
            SecurityManagerInfo.IsInclusion = false;
            base.SetStateCompleted(ou);
        }

        protected override void SetStateFailed(ActionUnit ou)
        {
            SecurityManagerInfo.IsInclusion = false;
            SecurityManagerInfo.Network.ResetSecuritySchemes(SpecificResult.Id);
            base.SetStateFailed(ou);
        }

        private byte _initiateNodeId;

        private void OnAddNodeCompletedInternal(StartActionUnit tu)
        {
            SpecificResult.Id = _initiateNodeId;

            var rniRes = SpecificResult.FindInnerResults<RequestNodeInfoResult>();
            if (rniRes != null && rniRes.Count > 0)
            {
                SpecificResult.CommandClasses = rniRes[0].CommandClasses;
                SpecificResult.IsSlave = rniRes[0].Basic > 2;
                SpecificResult.Basic = rniRes[0].Basic;
                SpecificResult.Generic = rniRes[0].Generic;
                SpecificResult.Specific = rniRes[0].Specific;
                SecurityManagerInfo.Network.SetCommandClasses(new NodeTag(SpecificResult.Id), SpecificResult.CommandClasses);
            }
            SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
            requestScheme.DestNodeId = SpecificResult.Id;
            if (SecurityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.SchemeGet))
            {
                Thread.Sleep(SecurityManagerInfo.DelaysS0[SecurityS0Delays.SchemeGet]);
            }
        }

        public void SetInclusionControllerInitiateParameters(byte initiateNodeId)
        {
            _initiateNodeId = initiateNodeId;
        }

        private void OnAddNodeCompleted(StartActionUnit tu)
        {
            SecurityManagerInfo.IsInclusion = true;
            if ((ParentAction as ActionSerialGroup) != null)
            {
                ActionSerialGroup actionGroup = (ActionSerialGroup)ParentAction;
                if (_initiateNodeId > 0)
                {
                    OnAddNodeCompletedInternal(tu);
                }
                else
                {
                    ActionResult agRes = ParentAction.Result;
                    AddTraceLogItems(agRes.InnerResults[0].TraceLog);
                    if (agRes.InnerResults[0].State == ActionStates.Completed)
                    {
                        AddRemoveNodeResult arnRes = agRes.InnerResults[0] as AddRemoveNodeResult;
                        SpecificResult.Id = arnRes.Id;
                        SpecificResult.Basic = arnRes.Basic;
                        SpecificResult.Generic = arnRes.Generic;
                        SpecificResult.Specific = arnRes.Specific;
                        SpecificResult.AddRemoveNodeStatus = arnRes.AddRemoveNodeStatus;
                        if (arnRes.CommandClasses == null)
                        {
                            var rniRes = agRes.FindInnerResults<RequestNodeInfoResult>();
                            if (rniRes != null && rniRes.Count > 0)
                            {
                                SpecificResult.CommandClasses = rniRes[0].CommandClasses;
                                SpecificResult.IsSlave = rniRes[0].Basic > 2;
                            }
                        }
                        else
                        {
                            SpecificResult.CommandClasses = arnRes.CommandClasses;
                            SpecificResult.IsSlave = arnRes.IsSlave;
                        }

                        SecurityManagerInfo.Network.SetCommandClasses(new NodeTag(SpecificResult.Id), SpecificResult.CommandClasses);

                        if (actionGroup.Actions != null && actionGroup.Actions.Length > 0 && actionGroup.Actions[0] is ReplaceFailedNodeOperation)
                        {
                            SecurityManagerInfo.Network.ResetSecuritySchemes(SpecificResult.Id);
                        }

                        if (SpecificResult.SubstituteStatus == SubstituteStatuses.None &&
                            (!SecurityManagerInfo.CheckIfSupportSecurityCC ||
                            (SpecificResult.CommandClasses != null && SpecificResult.CommandClasses.Contains(COMMAND_CLASS_SECURITY.ID))))
                        {
                            if (SecurityManagerInfo.Network.HasSecurityScheme(SpecificResult.Id, SecuritySchemes.S0) ||
                                arnRes.AddRemoveNodeStatus == AddRemoveNodeStatuses.Replicated)
                            {
                                SetStateCompleted(tu);
                                SpecificResult.SubstituteStatus = SubstituteStatuses.Done;
                            }
                            else
                            {
                                if (SpecificResult.tmpSkipS0)
                                {
                                    SetStateCompleted(tu);
                                }
                                else
                                {
                                    SpecificResult.SubstituteStatus = SubstituteStatuses.Failed;
                                    requestScheme.DestNodeId = SpecificResult.Id;
                                    if (SecurityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.SchemeGet))
                                    {
                                        Thread.Sleep(SecurityManagerInfo.DelaysS0[SecurityS0Delays.SchemeGet]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (SpecificResult.SubstituteStatus != SubstituteStatuses.Done &&
                                SecurityManagerInfo.CheckIfSupportSecurityCC &&
                                SpecificResult.CommandClasses != null &&
                                !SpecificResult.CommandClasses.Contains(COMMAND_CLASS_SECURITY.ID))
                            {
                                SecurityManagerInfo.Network.ResetSecuritySchemes(SpecificResult.Id);
                            }
                            SetStateCompleted(tu);
                        }
                    }
                    else
                        SetStateFailed(tu);
                }
            }
            else
                SetStateFailed(tu);
        }

        private void OnSchemeReport(ActionCompletedUnit ou)
        {
            if (requestScheme.Result.State == ActionStates.Completed)
            {
                SecurityManagerInfo.Network.SetSecuritySchemes(SpecificResult.Id, SecuritySchemeSet.S0);
                COMMAND_CLASS_SECURITY.SECURITY_SCHEME_REPORT command = requestScheme.SpecificResult.Command;
                //TO# 07122 accourding to SDS12652-11 section 3.29.2.3, an including controller must not verify the reported scheme.
                sendNetworkKeySet.NodeId = SpecificResult.Id;
                expectNetworkKeyVerify.SrcNodeId = SpecificResult.Id;
                var networkKey = SecurityManagerInfo.GetActualNetworkKey(SecuritySchemes.S0);
                if (SecurityManagerInfo.TestNetworkKeyS0InSet != null)
                {
                    networkKey = SecurityManagerInfo.TestNetworkKeyS0InSet;
                }
                sendNetworkKeySet.Data = new COMMAND_CLASS_SECURITY.NETWORK_KEY_SET() { networkKeyByte = new List<byte>(networkKey) };
                SecurityManagerInfo.ActivateNetworkKeyS0Temp();
                if (SecurityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.NetworkKeySet))
                {
                    sendNetworkKeySet.DataDelay = SecurityManagerInfo.DelaysS0[SecurityS0Delays.NetworkKeySet];
                }
            }
            else
                SetStateCompletedSecurityFailed(ou);
        }

        private void OnNetworkKeySet(ActionCompletedUnit ou)
        {
            SecurityManagerInfo.ActivateNetworkKeyS0();
        }

        private void OnNetworkKeyVerify(ActionCompletedUnit ou)
        {
            if (expectNetworkKeyVerify.Result.State == ActionStates.Completed)
            {
                if (SpecificResult.IsSlave)
                {
                    SetStateCompletedSecurityDone(ou);
                }
                else
                {
                    requestSchemeInherit.DestNodeId = SpecificResult.Id;
                    ou.SetNextActionItems(requestSchemeInherit);
                    if (SecurityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.SchemeInherit))
                    {
                        requestSchemeInherit.DataDelay = SecurityManagerInfo.DelaysS0[SecurityS0Delays.SchemeInherit];
                    }
                }
            }
            else
                SetStateCompletedSecurityFailed(ou);
        }

        private void OnSchemeInheritReport(ActionCompletedUnit ou)
        {
            if (requestSchemeInherit.Result.State == ActionStates.Completed)
            {
                SetStateCompletedSecurityDone(ou);
            }
            else
            {
                SetStateCompletedSecurityFailed(ou);
            }
        }

        private void SetStateCompletedSecurityDone(ActionCompletedUnit ou)
        {
            SpecificResult.SubstituteStatus = SubstituteStatuses.Done;
            SpecificResult.SecuritySchemes = SecuritySchemeSet.S0;
            SecurityManagerInfo.Network.SetSecuritySchemes(SpecificResult.Id, SecuritySchemeSet.S0);
            SecurityManagerInfo.Network.SetSecuritySchemesSpecified(SpecificResult.Id);
            SetStateCompleted(ou);
        }

        public override string AboutMe()
        {
            if (ParentAction != null)
                return string.Format("Id={0}, Security={1}", SpecificResult.Id, SpecificResult.SubstituteStatus);
            else
                return "";
        }

        public AddRemoveNodeResult SpecificResult
        {
            get { return (AddRemoveNodeResult)ParentAction.Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new AddRemoveNodeResult();
        }
    }
}
