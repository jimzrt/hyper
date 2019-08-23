using System.Collections.Generic;

namespace ZWave.CommandClasses
{
    public class COMMAND_CLASS_METER_TBL_CONFIG
    {
        public const byte ID = 0x3C;
        public const byte VERSION = 1;
        public class METER_TBL_TABLE_POINT_ADM_NO_SET
        {
            public const byte ID = 0x01;
            public struct Tproperties1
            {
                private byte _value;
                public byte numberOfCharacters
                {
                    get { return (byte)(_value >> 0 & 0x1F); }
                    set { _value &= 0xFF - 0x1F; _value += (byte)(value << 0 & 0x1F); }
                }
                public byte reserved
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
            public IList<byte> meterPointAdmNumberCharacter = new List<byte>();
            public static implicit operator METER_TBL_TABLE_POINT_ADM_NO_SET(byte[] data)
            {
                METER_TBL_TABLE_POINT_ADM_NO_SET ret = new METER_TBL_TABLE_POINT_ADM_NO_SET();
                if (data != null)
                {
                    int index = 2;
                    ret.properties1 = data.Length > index ? data[index++] : (byte)0x00;
                    ret.meterPointAdmNumberCharacter = new List<byte>();
                    for (int i = 0; i < ret.properties1.numberOfCharacters; i++)
                    {
                        ret.meterPointAdmNumberCharacter.Add(data.Length > index ? data[index++] : (byte)0x00);
                    }
                }
                return ret;
            }
            public static implicit operator byte[](METER_TBL_TABLE_POINT_ADM_NO_SET command)
            {
                List<byte> ret = new List<byte>();
                ret.Add(COMMAND_CLASS_METER_TBL_CONFIG.ID);
                ret.Add(ID);
                ret.Add(command.properties1);
                if (command.meterPointAdmNumberCharacter != null)
                {
                    foreach (var tmp in command.meterPointAdmNumberCharacter)
                    {
                        ret.Add(tmp);
                    }
                }
                return ret.ToArray();
            }
        }
    }
}

