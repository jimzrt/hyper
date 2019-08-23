using ZWave.BasicApplication.Enums;

namespace ZWave.BasicApplication.Operations
{
    /// <summary>
    /// To send a FUNC_ID_PM_STAY_AWAKE cmd send 0xD7 <powerLockType> <timeout value>
    /// powerLockType 0 for radio powerLock and 1 for IOPowerLock
    /// timeout is 32 bit value in ms first byte is MSB and last is LSB
    /// </summary>
    public class PowerMgmtStayAwakeOperation : ApiOperation
    {
        private readonly byte _powerLockType;
        private readonly int _powerTimeoutMs;
        private readonly int timeoutMs = 2000;
        public PowerMgmtStayAwakeOperation(byte powerLockType, int powerTimeoutMs)
            : base(true, CommandTypes.CmdPowerMgmtStayAwake, false)
        {
            _powerLockType = powerLockType;
            _powerTimeoutMs = powerTimeoutMs;
        }

        ApiMessage message;

        protected override void CreateWorkflow()
        {
            ActionUnits.Add(new StartActionUnit(SetStateCompleting, timeoutMs, message));
        }

        protected override void CreateInstance()
        {
            message = new ApiMessage(CommandTypes.CmdPowerMgmtStayAwake,
                _powerLockType,
                (byte)(_powerTimeoutMs >> 24),
                (byte)(_powerTimeoutMs >> 16),
                (byte)(_powerTimeoutMs >> 8),
                (byte)_powerTimeoutMs);
        }
    }
}
