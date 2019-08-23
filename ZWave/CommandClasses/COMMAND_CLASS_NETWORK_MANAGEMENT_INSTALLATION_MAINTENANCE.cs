using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE
    {
        public const byte ID = 0x67;
        public const byte VERSION = 1;
        public class PRIORITY_ROUTE_SET
        {
            public const byte ID = 0x01;
            public byte nodeid;
            public byte repeater1;
            public byte repeater2;
            public byte repeater3;
            public byte repeater4;
            public byte speed;
            public static implicit operator PRIORITY_ROUTE_SET(byte[] data)
            {
                PRIORITY_ROUTE_SET ret = new PRIORITY_ROUTE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.speed = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](PRIORITY_ROUTE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE.ID);
                ret.Add(ID);
                ret.Add(command.nodeid);
                ret.Add(command.repeater1);
                ret.Add(command.repeater2);
                ret.Add(command.repeater3);
                ret.Add(command.repeater4);
                ret.Add(command.speed);
                return ret.ToArray();
            }
        }
        public class PRIORITY_ROUTE_GET
        {
            public const byte ID = 0x02;
            public byte nodeid;
            public static implicit operator PRIORITY_ROUTE_GET(byte[] data)
            {
                PRIORITY_ROUTE_GET ret = new PRIORITY_ROUTE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](PRIORITY_ROUTE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE.ID);
                ret.Add(ID);
                ret.Add(command.nodeid);
                return ret.ToArray();
            }
        }
        public class PRIORITY_ROUTE_REPORT
        {
            public const byte ID = 0x03;
            public byte nodeid;
            public byte type;
            public byte repeater1;
            public byte repeater2;
            public byte repeater3;
            public byte repeater4;
            public byte speed;
            public static implicit operator PRIORITY_ROUTE_REPORT(byte[] data)
            {
                PRIORITY_ROUTE_REPORT ret = new PRIORITY_ROUTE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.type = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.repeater4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.speed = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](PRIORITY_ROUTE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE.ID);
                ret.Add(ID);
                ret.Add(command.nodeid);
                ret.Add(command.type);
                ret.Add(command.repeater1);
                ret.Add(command.repeater2);
                ret.Add(command.repeater3);
                ret.Add(command.repeater4);
                ret.Add(command.speed);
                return ret.ToArray();
            }
        }
        public class STATISTICS_GET
        {
            public const byte ID = 0x04;
            public byte nodeid;
            public static implicit operator STATISTICS_GET(byte[] data)
            {
                STATISTICS_GET ret = new STATISTICS_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](STATISTICS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE.ID);
                ret.Add(ID);
                ret.Add(command.nodeid);
                return ret.ToArray();
            }
        }
        public class STATISTICS_REPORT
        {
            public const byte ID = 0x05;
            public byte nodeid;
            public class TSTATISTICS
            {
                public byte type;
                public byte length;
                public IList<byte> value = new List<byte>();
            }
            public List<TSTATISTICS> statistics = new List<TSTATISTICS>();
            public static implicit operator STATISTICS_REPORT(byte[] data)
            {
                STATISTICS_REPORT ret = new STATISTICS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.nodeid = data.Length > index ? data[index++] : (byte)0x00;
                    ret.statistics = new List<TSTATISTICS>();
                    while (data.Length - 0 > index)
                    {
                        TSTATISTICS tmp = new TSTATISTICS();
                        tmp.type = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.length = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.value = new List<byte>();
                        for (int i = 0; i < tmp.length; i++)
                        {
                            tmp.value.Add(data.Length > index ? data[index++] : (byte)0x00);
                        }
                        ret.statistics.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](STATISTICS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE.ID);
                ret.Add(ID);
                ret.Add(command.nodeid);
                if (command.statistics != null)
                {
                    foreach (var item in command.statistics)
                    {
                        ret.Add(item.type);
                        ret.Add(item.length);
                        if (item.value != null)
                        {
                            foreach (var tmp in item.value)
                            {
                                ret.Add(tmp);
                            }
                        }
                    }
                }
                return ret.ToArray();
            }
        }
        public class STATISTICS_CLEAR
        {
            public const byte ID = 0x06;
            public static implicit operator STATISTICS_CLEAR(byte[] data)
            {
                STATISTICS_CLEAR ret = new STATISTICS_CLEAR();
                return ret;
            }
            public static implicit operator byte[](STATISTICS_CLEAR command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
    }
}

