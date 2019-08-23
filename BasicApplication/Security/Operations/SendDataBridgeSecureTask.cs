using System.Linq;
using System.Threading;
using Utils;
using ZWave.BasicApplication.Security;
using ZWave.CommandClasses;
using ZWave.Enums;

namespace ZWave.BasicApplication.Operations
{
    public class SendDataBridgeSecureTask : ApiOperation
    {
        #region Timeouts
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        const int NONCE_REQUEST_INCLUSION_TIMER = 10000;
        /// <summary>
        /// Nonce Request Timer
        /// </summary>
        const int NONCE_REQUEST_TIMER = 20000;
        #endregion

        protected TransmitOptions TxOptions { get; set; }
        internal byte[] CommandToSecureSend { get; private set; }
        internal int DataDelay { get; set; }
        internal byte SrcNodeId { get; private set; }
        internal byte DestNodeId { get; private set; }
        internal byte? TestNodeId { get; set; }
        private SecurityManagerInfo _securityManagerInfo;
        private SecurityS0CryptoProvider _securityS0CryptoProvider;

        internal SendDataBridgeSecureTask(SecurityManagerInfo securityManagerInfo, SecurityS0CryptoProvider securityS0CryptoProvider, byte srcNodeId, byte destNodeId, byte[] data, TransmitOptions txOptions)
            : base(false, null, false)
        {
            _securityManagerInfo = securityManagerInfo;
            _securityS0CryptoProvider = securityS0CryptoProvider;
            SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
            CommandToSecureSend = data;
            SrcNodeId = srcNodeId;
            DestNodeId = destNodeId;
            TxOptions = txOptions;
        }

        RequestDataOperation requestNonce;
        SendDataBridgeOperation sendEncData;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(OnStart, 0, requestNonce));
            ActionUnits.Add(new ActionCompletedUnit(requestNonce, OnNonceReport, sendEncData));
            ActionUnits.Add(new ActionCompletedUnit(sendEncData, OnSendEncData));
        }

        protected override void CreateInstance()
        {
            requestNonce = new RequestDataOperation(SrcNodeId, DestNodeId,
                new COMMAND_CLASS_SECURITY.SECURITY_NONCE_GET(), TxOptions,
                new COMMAND_CLASS_SECURITY.SECURITY_NONCE_REPORT(), 2, NONCE_REQUEST_TIMER);
            if (_securityManagerInfo.IsInclusion)
                requestNonce.Token.Reset(NONCE_REQUEST_INCLUSION_TIMER);
            requestNonce.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);

            sendEncData = new SendDataBridgeOperation(SrcNodeId, DestNodeId, null, TxOptions);
            sendEncData.SubstituteSettings.SetFlag(SubstituteFlags.DenySecurity);
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
            if (requestNonce.Result.State == ActionStates.Completed)
            {
                COMMAND_CLASS_SECURITY.SECURITY_NONCE_REPORT cmd = requestNonce.SpecificResult.Command;
                "NONCE REPORT {0}"._DLOG(requestNonce.SpecificResult.Command.GetHex());
                if (cmd.nonceByte != null && cmd.nonceByte.Count == 8)
                {
                    byte[] msg = _securityS0CryptoProvider.Encrypt(0, CommandToSecureSend, SrcNodeId, DestNodeId, cmd.nonceByte.ToArray());
                    sendEncData.Data = msg;
                    //sendEncData.OnHandledCallback = OnHandledCallback;
                    if (_securityManagerInfo.DelaysS0.ContainsKey(SecurityS0Delays.Command))
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
                SetStateCompleted(ou);
            }
            else
                SetStateFailed(ou);
        }

        protected override ActionResult CreateOperationResult()
        {
            return new ActionResult();
        }
    }
}
