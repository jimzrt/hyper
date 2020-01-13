namespace hyper.Helper
{
    internal class MultiSensorType
    {
        public enum SensorType
        {
            TEMP = 0x01,
            LUMINANCE = 0x03,
            POWER = 0x04,
            HUMIDITY = 0x05,
            CO2 = 0x11,
            UV = 0x1B
        }
    }
}