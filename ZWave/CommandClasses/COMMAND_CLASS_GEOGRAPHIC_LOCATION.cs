using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_GEOGRAPHIC_LOCATION
    {
        public const byte ID = 0x8C;
        public const byte VERSION = 1;
        public class GEOGRAPHIC_LOCATION_GET
        {
            public const byte ID = 0x02;
            public static implicit operator GEOGRAPHIC_LOCATION_GET(byte[] data)
            {
                GEOGRAPHIC_LOCATION_GET ret = new GEOGRAPHIC_LOCATION_GET();
                return ret;
            }
            public static implicit operator byte[](GEOGRAPHIC_LOCATION_GET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_GEOGRAPHIC_LOCATION.ID);
                ret.Add(ID);
                return ret.ToArray();
            }
        }
        public class GEOGRAPHIC_LOCATION_REPORT
        {
            public const byte ID = 0x03;
            public byte longitudeDegrees;
            public struct Tproperties1
            {
                private byte _value;
                public byte longitudeMinutes
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte longSign
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public byte latitudeDegrees;
            public struct Tproperties2
            {
                private byte _value;
                public byte latitudeMinutes
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte latSign
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public static implicit operator GEOGRAPHIC_LOCATION_REPORT(byte[] data)
            {
                GEOGRAPHIC_LOCATION_REPORT ret = new GEOGRAPHIC_LOCATION_REPORT();
                if (data != null)
                {
                    int index = 2;
                    ret.longitudeDegrees = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.latitudeDegrees = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GEOGRAPHIC_LOCATION_REPORT command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_GEOGRAPHIC_LOCATION.ID);
                ret.Add(ID);
                ret.Add(command.longitudeDegrees);
                ret.Add(command.properties1);
                ret.Add(command.latitudeDegrees);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
        public class GEOGRAPHIC_LOCATION_SET
        {
            public const byte ID = 0x01;
            public byte longitudeDegrees;
            public struct Tproperties1
            {
                private byte _value;
                public byte longitudeMinutes
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte longSign
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public byte latitudeDegrees;
            public struct Tproperties2
            {
                private byte _value;
                public byte latitudeMinutes
                {
                    get { return (byte)(_value >> 0 & 0x7F); }
                    set { _value &= 0xFF - 0x7F; _value += (byte)(value << 0 & 0x7F); }
                }
                public byte latSign
                {
                    get { return (byte)(_value >> 7 & 0x01); }
                    set { _value &= 0xFF - 0x80; _value += (byte)(value << 7 & 0x80); }
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
            public static implicit operator GEOGRAPHIC_LOCATION_SET(byte[] data)
            {
                GEOGRAPHIC_LOCATION_SET ret = new GEOGRAPHIC_LOCATION_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.longitudeDegrees = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.latitudeDegrees = data.Length > index ? data[index++] : (byte)0x00;
                    ret.properties2 = data.Length > index ? data[index++] : (byte)0x00;
                }
                return ret;
            }
            public static implicit operator byte[](GEOGRAPHIC_LOCATION_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_GEOGRAPHIC_LOCATION.ID);
                ret.Add(ID);
                ret.Add(command.longitudeDegrees);
                ret.Add(command.properties1);
                ret.Add(command.latitudeDegrees);
                ret.Add(command.properties2);
                return ret.ToArray();
            }
        }
    }
}

