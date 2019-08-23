using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_IRRIGATION
    {
        public const byte ID = 0x6B;
        public const byte VERSION = 1;
        public class IRRIGATION_SYSTEM_INFO_GET
        {
            public const byte ID = 0x01;
            public static implicit operator IRRIGATION_SYSTEM_INFO_GET(byte[] data)
            {
                IRRIGATION_SYSTEM_INFO_GET ret = new IRRIGATION_SYSTEM_INFO_GET();
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_INFO_REPORT
        {
            public const byte ID = 0x02;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved1
                {
                    get { return (byte)(_value >> 1 & 0x03); }
                    set { _value &= 0xFF - 0x06; _value += (byte)(value << 1 & 0x06); }
                }
                public byte reserved2
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte reserved3
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte totalNumberOfValves;
            public byte totalNumberOfValveTables;
            public struct Tproperties2
            {
                private byte _value;
                public byte valveTableMaxSize
                {
                    get { return (byte)(_value >> 0 & 0x0F); }
                    set { _value &= 0xFF - 0x0F; _value += (byte)(value << 0 & 0x0F); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 4 & 0x0F); }
                    set { _value &= 0xFF - 0xF0; _value += (byte)(value << 4 & 0xF0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public static implicit operator IRRIGATION_SYSTEM_INFO_REPORT(byte[] data)
            {
                IRRIGATION_SYSTEM_INFO_REPORT ret = new IRRIGATION_SYSTEM_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.totalNumberOfValves = data.Length > index ? data[index++] : (byte)0x00;
                    ret.totalNumberOfValveTables = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.totalNumberOfValves);
                ret.Add(command.totalNumberOfValveTables);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_STATUS_GET
        {
            public const byte ID = 0x03;
            public static implicit operator IRRIGATION_SYSTEM_STATUS_GET(byte[] data)
            {
                IRRIGATION_SYSTEM_STATUS_GET ret = new IRRIGATION_SYSTEM_STATUS_GET();
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_STATUS_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_STATUS_REPORT
        {
            public const byte ID = 0x04;
            public byte systemVoltage;
            public byte sensorStatus;
            public struct Tproperties1
            {
                private byte _value;
                public byte flowSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte flowScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte flowPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public IList<byte> flowValue = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte pressureSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte pressureScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte pressurePrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> pressureValue = new List<byte>();
            public byte shutoffDuration;
            public byte systemErrorStatus;
            public struct Tproperties3
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
                }
                public static implicit operator Tproperties3(byte data)
                {
                    Tproperties3 ret = new Tproperties3();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties3 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties3 properties3;
            public byte valveId;
            public static implicit operator IRRIGATION_SYSTEM_STATUS_REPORT(byte[] data)
            {
                IRRIGATION_SYSTEM_STATUS_REPORT ret = new IRRIGATION_SYSTEM_STATUS_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.systemVoltage = data.Length > index ? data[index++] : (byte)0x00;
                    ret.sensorStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.flowValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.flowSize; i++)
                    {
                        ret.flowValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.pressureValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.pressureSize; i++)
                    {
                        ret.pressureValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.shutoffDuration = data.Length > index ? data[index++] : (byte)0x00;
                    ret.systemErrorStatus = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_STATUS_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.systemVoltage);
                ret.Add(command.sensorStatus);
                ret.Add(command.properties1);
                if (command.flowValue != null)
                {
                    foreach (var tmp in command.flowValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.pressureValue != null)
                {
                    foreach (var tmp in command.pressureValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.shutoffDuration);
                ret.Add(command.systemErrorStatus);
                ret.Add(command.properties3);
                ret.Add(command.valveId);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_CONFIG_SET
        {
            public const byte ID = 0x05;
            public byte masterValveDelay;
            public struct Tproperties1
            {
                private byte _value;
                public byte highPressureThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte highPressureThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte highPressureThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public IList<byte> highPressureThresholdValue = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte lowPressureThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte lowPressureThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte lowPressureThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> lowPressureThresholdValue = new List<byte>();
            public byte sensorPolarity;
            public static implicit operator IRRIGATION_SYSTEM_CONFIG_SET(byte[] data)
            {
                IRRIGATION_SYSTEM_CONFIG_SET ret = new IRRIGATION_SYSTEM_CONFIG_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.masterValveDelay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.highPressureThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.highPressureThresholdSize; i++)
                    {
                        ret.highPressureThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lowPressureThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.lowPressureThresholdSize; i++)
                    {
                        ret.lowPressureThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.sensorPolarity = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_CONFIG_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.masterValveDelay);
                ret.Add(command.properties1);
                if (command.highPressureThresholdValue != null)
                {
                    foreach (var tmp in command.highPressureThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.lowPressureThresholdValue != null)
                {
                    foreach (var tmp in command.lowPressureThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.sensorPolarity);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_CONFIG_GET
        {
            public const byte ID = 0x06;
            public static implicit operator IRRIGATION_SYSTEM_CONFIG_GET(byte[] data)
            {
                IRRIGATION_SYSTEM_CONFIG_GET ret = new IRRIGATION_SYSTEM_CONFIG_GET();
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_CONFIG_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_CONFIG_REPORT
        {
            public const byte ID = 0x07;
            public byte masterValveDelay;
            public struct Tproperties1
            {
                private byte _value;
                public byte highPressureThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte highPressureThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte highPressureThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public IList<byte> highPressureThresholdValue = new List<byte>();
            public struct Tproperties2
            {
                private byte _value;
                public byte lowPressureThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte lowPressureThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte lowPressureThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> lowPressureThresholdValue = new List<byte>();
            public byte sensorPolarity;
            public static implicit operator IRRIGATION_SYSTEM_CONFIG_REPORT(byte[] data)
            {
                IRRIGATION_SYSTEM_CONFIG_REPORT ret = new IRRIGATION_SYSTEM_CONFIG_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.masterValveDelay = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.highPressureThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties1.highPressureThresholdSize; i++)
                    {
                        ret.highPressureThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.lowPressureThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.lowPressureThresholdSize; i++)
                    {
                        ret.lowPressureThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.sensorPolarity = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_CONFIG_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.masterValveDelay);
                ret.Add(command.properties1);
                if (command.highPressureThresholdValue != null)
                {
                    foreach (var tmp in command.highPressureThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties2);
                if (command.lowPressureThresholdValue != null)
                {
                    foreach (var tmp in command.lowPressureThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.sensorPolarity);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_INFO_GET
        {
            public const byte ID = 0x08;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte valveId;
            public static implicit operator IRRIGATION_VALVE_INFO_GET(byte[] data)
            {
                IRRIGATION_VALVE_INFO_GET ret = new IRRIGATION_VALVE_INFO_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_INFO_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.valveId);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_INFO_REPORT
        {
            public const byte ID = 0x09;
            public struct Tproperties1
            {
                private byte _value;
                public byte master
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte connected
                {
                    get { return (byte)(_value >> 1 & 0x01); }
                    set { _value &= 0xFF - 0x02; _value += (byte)(value << 1 & 0x02); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 2 & 0x3F); }
                    set { _value &= 0xFF - 0xFC; _value += (byte)(value << 2 & 0xFC); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte valveId;
            public byte nominalCurrent;
            public byte valveErrorStatus;
            public static implicit operator IRRIGATION_VALVE_INFO_REPORT(byte[] data)
            {
                IRRIGATION_VALVE_INFO_REPORT ret = new IRRIGATION_VALVE_INFO_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nominalCurrent = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveErrorStatus = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_INFO_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.valveId);
                ret.Add(command.nominalCurrent);
                ret.Add(command.valveErrorStatus);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_CONFIG_SET
        {
            public const byte ID = 0x0A;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte valveId;
            public byte nominalCurrentHighThreshold;
            public byte nominalCurrentLowThreshold;
            public struct Tproperties2
            {
                private byte _value;
                public byte maximumFlowSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte maximumFlowScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte maximumFlowPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> maximumFlowValue = new List<byte>();
            public struct Tproperties3
            {
                private byte _value;
                public byte flowHighThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte flowHighThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte flowHighThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties3(byte data)
                {
                    Tproperties3 ret = new Tproperties3();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties3 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties3 properties3;
            public IList<byte> flowHighThresholdValue = new List<byte>();
            public struct Tproperties4
            {
                private byte _value;
                public byte flowLowThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte flowLowThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte flowLowThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties4(byte data)
                {
                    Tproperties4 ret = new Tproperties4();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties4 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties4 properties4;
            public IList<byte> flowLowThresholdValue = new List<byte>();
            public byte sensorUsage;
            public static implicit operator IRRIGATION_VALVE_CONFIG_SET(byte[] data)
            {
                IRRIGATION_VALVE_CONFIG_SET ret = new IRRIGATION_VALVE_CONFIG_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nominalCurrentHighThreshold = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nominalCurrentLowThreshold = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.maximumFlowValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.maximumFlowSize; i++)
                    {
                        ret.maximumFlowValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.flowHighThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties3.flowHighThresholdSize; i++)
                    {
                        ret.flowHighThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.flowLowThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties4.flowLowThresholdSize; i++)
                    {
                        ret.flowLowThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.sensorUsage = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_CONFIG_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.valveId);
                ret.Add(command.nominalCurrentHighThreshold);
                ret.Add(command.nominalCurrentLowThreshold);
                ret.Add(command.properties2);
                if (command.maximumFlowValue != null)
                {
                    foreach (var tmp in command.maximumFlowValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties3);
                if (command.flowHighThresholdValue != null)
                {
                    foreach (var tmp in command.flowHighThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties4);
                if (command.flowLowThresholdValue != null)
                {
                    foreach (var tmp in command.flowLowThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.sensorUsage);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_CONFIG_GET
        {
            public const byte ID = 0x0B;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte valveId;
            public static implicit operator IRRIGATION_VALVE_CONFIG_GET(byte[] data)
            {
                IRRIGATION_VALVE_CONFIG_GET ret = new IRRIGATION_VALVE_CONFIG_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_CONFIG_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.valveId);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_CONFIG_REPORT
        {
            public const byte ID = 0x0C;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte valveId;
            public byte nominalCurrentHighThreshold;
            public byte nominalCurrentLowThreshold;
            public struct Tproperties2
            {
                private byte _value;
                public byte maximumFlowSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte maximumFlowScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte maximumFlowPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties2(byte data)
                {
                    Tproperties2 ret = new Tproperties2();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties2 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties2 properties2;
            public IList<byte> maximumFlowValue = new List<byte>();
            public struct Tproperties3
            {
                private byte _value;
                public byte flowHighThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte flowHighThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte flowHighThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties3(byte data)
                {
                    Tproperties3 ret = new Tproperties3();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties3 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties3 properties3;
            public IList<byte> flowHighThresholdValue = new List<byte>();
            public struct Tproperties4
            {
                private byte _value;
                public byte flowLowThresholdSize
                {
                    get { return (byte)(_value >> 0 & 0x07); }
                    set { _value &= 0xFF - 0x07; _value += (byte)(value << 0 & 0x07); }
                }
                public byte flowLowThresholdScale
                {
                    get { return (byte)(_value >> 3 & 0x03); }
                    set { _value &= 0xFF - 0x18; _value += (byte)(value << 3 & 0x18); }
                }
                public byte flowLowThresholdPrecision
                {
                    get { return (byte)(_value >> 5 & 0x07); }
                    set { _value &= 0xFF - 0xE0; _value += (byte)(value << 5 & 0xE0); }
                }
                public static implicit operator Tproperties4(byte data)
                {
                    Tproperties4 ret = new Tproperties4();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties4 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties4 properties4;
            public IList<byte> flowLowThresholdValue = new List<byte>();
            public byte sensorUsage;
            public static implicit operator IRRIGATION_VALVE_CONFIG_REPORT(byte[] data)
            {
                IRRIGATION_VALVE_CONFIG_REPORT ret = new IRRIGATION_VALVE_CONFIG_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nominalCurrentHighThreshold = data.Length > index ? data[index++] : (byte)0x00;
                    ret.nominalCurrentLowThreshold = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.maximumFlowValue = new List<byte>();
                    for (int i = 0; i < ret.properties2.maximumFlowSize; i++)
                    {
                        ret.maximumFlowValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties3 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.flowHighThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties3.flowHighThresholdSize; i++)
                    {
                        ret.flowHighThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.properties4 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.flowLowThresholdValue = new List<byte>();
                    for (int i = 0; i < ret.properties4.flowLowThresholdSize; i++)
                    {
                        ret.flowLowThresholdValue.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                    ret.sensorUsage = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_CONFIG_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.valveId);
                ret.Add(command.nominalCurrentHighThreshold);
                ret.Add(command.nominalCurrentLowThreshold);
                ret.Add(command.properties2);
                if (command.maximumFlowValue != null)
                {
                    foreach (var tmp in command.maximumFlowValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties3);
                if (command.flowHighThresholdValue != null)
                {
                    foreach (var tmp in command.flowHighThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.properties4);
                if (command.flowLowThresholdValue != null)
                {
                    foreach (var tmp in command.flowLowThresholdValue)
                    {
                        ret.Add(tmp);
                    }
                }
                ret.Add(command.sensorUsage);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_RUN
        {
            public const byte ID = 0x0D;
            public struct Tproperties1
            {
                private byte _value;
                public byte masterValve
                {
                    get { return (byte)(_value >> 0 & 0x01); }
                    set { _value &= 0xFF - 0x01; _value += (byte)(value << 0 & 0x01); }
                }
                public byte reserved
                {
                    get { return (byte)(_value >> 1 & 0x7F); }
                    set { _value &= 0xFF - 0xFE; _value += (byte)(value << 1 & 0xFE); }
                }
                public static implicit operator Tproperties1(byte data)
                {
                    Tproperties1 ret = new Tproperties1();
                    ret._value = data;
                    return ret;
                }
                public static implicit operator byte(Tproperties1 prm)
                {
                    return prm._value;
                }
            }
            public Tproperties1 properties1;
            public byte valveId;
            public byte[] duration = new byte[2];
            public static implicit operator IRRIGATION_VALVE_RUN(byte[] data)
            {
                IRRIGATION_VALVE_RUN ret = new IRRIGATION_VALVE_RUN();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.valveId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.duration = new byte[]
                    {
                        data.Length > index ? data[index++] : (byte)0x00,
                        data.Length > index ? data[index++] : (byte)0x00
                    };
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_RUN command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                ret.Add(command.valveId);
                ret.Add(command.duration[0]);
                ret.Add(command.duration[1]);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_TABLE_SET
        {
            public const byte ID = 0x0E;
            public byte valveTableId;
            public class TVG1
            {
                public byte valveId;
                public byte[] duration = new byte[2];
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator IRRIGATION_VALVE_TABLE_SET(byte[] data)
            {
                IRRIGATION_VALVE_TABLE_SET ret = new IRRIGATION_VALVE_TABLE_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.valveTableId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    while (data.Length - 0 > index)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.valveId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.duration = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_TABLE_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.valveTableId);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.valveId);
                        ret.Add(item.duration[0]);
                        ret.Add(item.duration[1]);
                    }
                }
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_TABLE_GET
        {
            public const byte ID = 0x0F;
            public byte valveTableId;
            public static implicit operator IRRIGATION_VALVE_TABLE_GET(byte[] data)
            {
                IRRIGATION_VALVE_TABLE_GET ret = new IRRIGATION_VALVE_TABLE_GET();
                if (data != null)
                {
                    int index = 2;
                    ret.valveTableId = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_TABLE_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.valveTableId);
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_TABLE_REPORT
        {
            public const byte ID = 0x10;
            public byte valveTableId;
            public class TVG1
            {
                public byte valveId;
                public byte[] duration = new byte[2];
            }
            public List<TVG1> vg1 = new List<TVG1>();
            public static implicit operator IRRIGATION_VALVE_TABLE_REPORT(byte[] data)
            {
                IRRIGATION_VALVE_TABLE_REPORT ret = new IRRIGATION_VALVE_TABLE_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.valveTableId = data.Length > index ? data[index++] : (byte)0x00;
                    ret.vg1 = new List<TVG1>();
                    while (data.Length - 0 > index)
                    {
                        TVG1 tmp = new TVG1();
                        tmp.valveId = data.Length > index ? data[index++] : (byte)0x00;
                        tmp.duration = new byte[]
                        {
                            data.Length > index ? data[index++] : (byte)0x00,
                            data.Length > index ? data[index++] : (byte)0x00
                        };
                        ret.vg1.Add(tmp);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_TABLE_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.valveTableId);
                if (command.vg1 != null)
                {
                    foreach (var item in command.vg1)
                    {
                        ret.Add(item.valveId);
                        ret.Add(item.duration[0]);
                        ret.Add(item.duration[1]);
                    }
                }
                return ret.ToArray();
            }
        }
        public class IRRIGATION_VALVE_TABLE_RUN
        {
            public const byte ID = 0x11;
            public IList<byte> valveTableId = new List<byte>();
            public static implicit operator IRRIGATION_VALVE_TABLE_RUN(byte[] data)
            {
                IRRIGATION_VALVE_TABLE_RUN ret = new IRRIGATION_VALVE_TABLE_RUN();
                if (data != null)
                {
                    int index = 2;
                    ret.valveTableId = new List<byte>();
                    while (data.Length - 0 > index)
                    {
                        ret.valveTableId.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_VALVE_TABLE_RUN command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                if (command.valveTableId != null)
                {
                    foreach (var tmp in command.valveTableId)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
        public class IRRIGATION_SYSTEM_SHUTOFF
        {
            public const byte ID = 0x12;
            public byte duration;
            public static implicit operator IRRIGATION_SYSTEM_SHUTOFF(byte[] data)
            {
                IRRIGATION_SYSTEM_SHUTOFF ret = new IRRIGATION_SYSTEM_SHUTOFF();
                if (data != null)
                {
                    int index = 2;
                    ret.duration = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](IRRIGATION_SYSTEM_SHUTOFF command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_IRRIGATION.ID);
                ret.Add(ID);
                ret.Add(command.duration);
                return ret.ToArray();
            }
        }
    }
}

