namespace ZWave.BasicApplication.Enums
{
    public enum SetSucReturnValues : int
    {
        /// <summary>
        /// 
        /// </summary>
        SucUndefined = 0x00,
        /// <summary>
        /// ZWaveSUC_SET_SUCCEEDED
        /// </summary>
        SucSetSucceeded = 0x05,
        /// <summary>
        /// ZWaveSUC_SET_FAILED
        /// </summary>
        SucSetFailed = 0x06
    }
}
