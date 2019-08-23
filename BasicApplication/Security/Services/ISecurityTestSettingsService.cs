namespace ZWave.BasicApplication.Security
{
    public interface ISecurityTestSettingsService
    {
        void ActivateTestPropertiesForFrame(SecurityS2TestFrames testFrameType, ApiOperation apiOperation);
    }
}