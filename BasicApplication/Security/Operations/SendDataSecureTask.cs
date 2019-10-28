using System;
using System.Linq;
using System.Threading;
using Utils;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SendDataSecureTask : ApiOperation
    {
        #region Timeouts
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        public static int NONCE_REQUEST_INCLUSION_TIMER = 10000;
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        public static int NONCE_REQUEST_TIMER = 20000;
        #endregion

        protected TransmitOptions TxOptions { get; set; }
        internal byte[] CommandToSecureSend { get; private set; }
        internal int DataDelay { get; set; }
        internal byte NodeId { get; private set; }
        internal byte? TestNodeId { get; set; }
        private readonly SecurityS0CryptoProvider _securityS0CryptoProvider;
        private readonly SecurityManagerInfo _securityManagerInfo;

        public Action<ActionUnit> OnHandledCallback { get; set; }
        internal SendDataSecureTask(SecurityManagerInfo securityManagerInfo, SecurityS0CryptoProvider securityS0CryptoProvider, byte nodeId, byte[] data, TransmitOptions txOptions)
            : base(false, null, false)
        {
            _securityManagerInfo = securityManagerInfo;
            _securityS0CryptoProvider = securityS0CryptoProvider;
            SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            CommandToSecureSend = data;
            NodeId = nodeId;
            TxOptions = txOptions;
        }

        private RequestDataOperation requestNonce;
        private SendDataOperation sendEncData;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnStart, 0, requestNonce));
            ActionUnits.Add(new ActionCompletedUnit(requestNonce, OnNonceReport, sendEncData));
            ActionUnits.Add(new ActionCompletedUnit(sendEncData, OnSendEncData));
        }

        protected override void CreateInstance()
        {
            requestNonce = new RequestDataOperation(0, NodeId,
                new COMMAND_CLASS_SECURITY.SECURITY_NONCE_GET(), TxOptions,
                new COMMAND_CLASS_SECURITY.SECURITY_NONCE_REPORT(), 2, NONCE_REQUEST_TIMER);
            if (_securityManagerInfo.IsInclusion)
                requestNonce.Token.Reset(NONCE_REQUEST_INCLUSION_TIMER);
            requestNonce.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);

            sendEncData = new SendDataOperation(TestNodeId ?? NodeId, null, TxOptions);
            sendEncData.SubstituteSettings.SetFlag(SubstituteFlags.DenyTransportService | SubstituteFlags.DenySecurity);
        }

        protected override void SetStateFailed(ActionUnit ou)
        {
            base.SetStateFailed(ou);
        }

        private void OnStart(StartActionUnit ou)
        {
            if (_securityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.NonceGet))
            {
                Thread.Sleep(_securityManagerInfo.DelaysS0[SecurityS0Delays.NonceGet]);
            }
        }

        private void OnNonceReport(ActionCompletedUnit ou)
        {
            AddTraceLogItems(requestNonce.SpecificResult.TraceLog);
            SpecificResult.TransmitStatus = (requestNonce.Result as TransmitResult).TransmitStatus;
            if (requestNonce.Result.State == ActionStates.Completed)
            {
                COMMAND_CLASS_SECURITY.SECURITY_NONCE_REPORT cmd = requestNonce.SpecificResult.Command;
                "NONCE REPORT {0}"._DLOG(requestNonce.SpecificResult.Command.GetHex());
                if (cmd.nonceByte != null && cmd.nonceByte.Count == 8)
                {
                    byte[] msg = _securityS0CryptoProvider.Encrypt(0, CommandToSecureSend, _securityManagerInfo.Network.NodeId, NodeId, cmd.nonceByte.ToArray());
                    sendEncData.Data = msg;
                    if (DataDelay > 0)
                    {
                        Thread.Sleep(DataDelay);
                    }
                    else if (_securityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.Command))
                    {
                        Thread.Sleep(_securityManagerInfo.DelaysS0[SecurityS0Delays.Command]);
                    }
                }
                else
                    SetStateFailed(ou);
            }
            else
                SetStateFailed(ou);
        }

        private void OnSendEncData(ActionCompletedUnit ou)
        {
            AddTraceLogItems(sendEncData.SpecificResult.TraceLog);
            if (sendEncData.Result.State == ActionStates.Completed)
            {
                SpecificResult.TransmitStatus = (sendEncData.Result as TransmitResult).TransmitStatus;
                SetStateCompleted(ou);
            }
            else
                SetStateFailed(ou);
        }

        public SendDataResult SpecificResult
        {
            get { return (SendDataResult)Result; }
        }

        protected override ActionResult CreateOperationResult()
        {
            return new SendDataResult();
        }
    }
}
