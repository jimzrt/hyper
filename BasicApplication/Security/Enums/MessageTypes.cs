
namespace ZWave.BasicApplication.Security
{
    public enum MessageTypes : byte
    {
        SinglecastAll = 0x01,
        MulticastAll = 0x02,
        SinglecastWithSpan = 0x03,
        SinglecastWithMpan = 0x04,
        SinglecastWithMpanGrp = 0x05,
        SinglecastWithMos = 0x06
    }
}
