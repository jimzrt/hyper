using Utils;
using ZWave.Enums;

namespace ZWave.Layers
{
    public class ConnectionInfo
    {
        public IDataSource DataSource { get; set; }
        public string Library { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public ApiTypes ApiType { get; set; }
    }
}
