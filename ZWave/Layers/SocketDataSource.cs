using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using Utils;

namespace ZWave.Layers
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SoketSourceTypes
    {
        [Description("Z/IP")]
        ZIP = 0,
        [Description("Serial API")]
        TCP
    }

    public class SocketDataSource : IDataSource
    {
        public SoketSourceTypes Type { get; set; }
        public string SourceName { get; set; }
        public string Args { get; set; }
        public string Alias { get; set; }
        public int Port { get; set; }
        public bool CanReconnect { get; set; }

        public SocketDataSource()
        {
        }

        public SocketDataSource(string sourceName, int port)
            : this(sourceName, port, null)
        {
        }

        public SocketDataSource(string sourceName, string alias, string args)
            : this(sourceName, 0, args)
        {
            Alias = alias;
        }


        public SocketDataSource(string sourceName, int port, string args)
        {
            Type = string.IsNullOrEmpty(args) ? SoketSourceTypes.TCP : SoketSourceTypes.ZIP;
            SourceName = sourceName;
            Port = port;
            Args = args;
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(SourceName) && Port != 0;
        }

        public override string ToString()
        {
            return string.Concat(SourceName, ":", Port);
        }

        public static bool TryParse(string addressString, out string ipAddress, out int port)
        {
            ipAddress = null;
            port = -1;
            var addressParts = Regex.Split(addressString, @"[\s:]+");
            if (addressParts.Length != 2)
            {
                return false;
            }
            IPAddress temp;
            if (!IPAddress.TryParse(addressParts[0], out temp))
            {
                return false;
            }
            ipAddress = addressParts[0];
            if (!int.TryParse(addressParts[1], out port))
            {
                return false;
            }
            return true;
        }
    }
}
