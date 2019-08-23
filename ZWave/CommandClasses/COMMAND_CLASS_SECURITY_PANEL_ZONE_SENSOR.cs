using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR
    {
        public const byte ID = 0x2F;
        public const byte VERSION = 1;
        public class COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR_INSTALLED_REPORT
        {
            public const byte ID = 0x02;
            public byte zoneNumber;
            public byte numberOfSensors;
            public static implicit operator COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR_INSTALLED_REPORT(byte[] data)
            {
                COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR_INSTALLED_REPORT ret = new COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR_INSTALLED_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.zoneNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.numberOfSensors = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR_INSTALLED_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR.ID);
                ret.Add(ID);
                ret.Add(command.zoneNumber);
                ret.Add(command.numberOfSensors);
                return ret.ToArray();
            }
        }
        public class SECURITY_PANEL_ZONE_SENSOR_TYPE_GET
        {
            public const byte ID = 0x03;
            public byte zoneNumber;
            public byte sensorNumber;
            public static implicit operator SECURITY_PANEL_ZONE_SENSOR_TYPE_GET(byte[] data)
            {
                SECURITY_PANEL_ZONE_SENSOR_TYPE_GET ret = new SECURITY_PANEL_ZONE_SENSOR_TYPE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.zoneNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_PANEL_ZONE_SENSOR_TYPE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR.ID);
                ret.Add(ID);
                ret.Add(command.zoneNumber);
                ret.Add(command.sensorNumber);
                return ret.ToArray();
            }
        }
        public class SECURITY_PANEL_ZONE_SENSOR_TYPE_REPORT
        {
            public const byte ID = 0x04;
            public byte zoneNumber;
            public byte sensorNumber;
            public byte zwaveAlarmType;
            public static implicit operator SECURITY_PANEL_ZONE_SENSOR_TYPE_REPORT(byte[] data)
            {
                SECURITY_PANEL_ZONE_SENSOR_TYPE_REPORT ret = new SECURITY_PANEL_ZONE_SENSOR_TYPE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.zoneNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmType = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_PANEL_ZONE_SENSOR_TYPE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR.ID);
                ret.Add(ID);
                ret.Add(command.zoneNumber);
                ret.Add(command.sensorNumber);
                ret.Add(command.zwaveAlarmType);
                return ret.ToArray();
            }
        }
        public class SECURITY_PANEL_ZONE_SENSOR_INSTALLED_GET
        {
            public const byte ID = 0x01;
            public byte zoneNumber;
            public static implicit operator SECURITY_PANEL_ZONE_SENSOR_INSTALLED_GET(byte[] data)
            {
                SECURITY_PANEL_ZONE_SENSOR_INSTALLED_GET ret = new SECURITY_PANEL_ZONE_SENSOR_INSTALLED_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.zoneNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_PANEL_ZONE_SENSOR_INSTALLED_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR.ID);
                ret.Add(ID);
                ret.Add(command.zoneNumber);
                return ret.ToArray();
            }
        }
        public class SECURITY_PANEL_ZONE_SENSOR_STATE_GET
        {
            public const byte ID = 0x05;
            public byte zoneNumber;
            public byte sensorNumber;
            public static implicit operator SECURITY_PANEL_ZONE_SENSOR_STATE_GET(byte[] data)
            {
                SECURITY_PANEL_ZONE_SENSOR_STATE_GET ret = new SECURITY_PANEL_ZONE_SENSOR_STATE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.zoneNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorNumber = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_PANEL_ZONE_SENSOR_STATE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR.ID);
                ret.Add(ID);
                ret.Add(command.zoneNumber);
                ret.Add(command.sensorNumber);
                return ret.ToArray();
            }
        }
        public class SECURITY_PANEL_ZONE_SENSOR_STATE_REPORT
        {
            public const byte ID = 0x06;
            public byte zoneNumber;
            public byte sensorNumber;
            public byte zwaveAlarmType;
            public byte zwaveAlarmEvent;
            public byte eventParameters;
            public static implicit operator SECURITY_PANEL_ZONE_SENSOR_STATE_REPORT(byte[] data)
            {
                SECURITY_PANEL_ZONE_SENSOR_STATE_REPORT ret = new SECURITY_PANEL_ZONE_SENSOR_STATE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.zoneNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorNumber = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmType = data.Length > index ? data[index++] : (byte)0x00;
                    ret.zwaveAlarmEvent = data.Length > index ? data[index++] : (byte)0x00;
                    ret.eventParameters = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](SECURITY_PANEL_ZONE_SENSOR_STATE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR.ID);
                ret.Add(ID);
                ret.Add(command.zoneNumber);
                ret.Add(command.sensorNumber);
                ret.Add(command.zwaveAlarmType);
                ret.Add(command.zwaveAlarmEvent);
                ret.Add(command.eventParameters);
                return ret.ToArray();
            }
        }
    }
}

