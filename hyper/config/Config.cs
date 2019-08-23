using System;
using System.Collections.Generic;
using System.Text;

namespace hyper.config
{
    public class ConfigItemList
    {
        public List<ConfigItem> configItems;
    }

    public class ConfigItem
    {
        public string deviceName;
        public int manufacturerId;
        public int productTypeId;
        public Dictionary<byte, byte> groups = new Dictionary<byte, byte>();
        public Dictionary<string, ushort> config = new Dictionary<string, ushort>();
        public int wakeup;

    }

    public class GroupConfig
    {
        public int identifier;
        public int member;
    }

    public class ParameterConfig
    {
        public int parameter;
        public int value;
    }

}