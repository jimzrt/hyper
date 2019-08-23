using Utils;
using ZWave.Enums;
using ZWave.Layers.Transport;

namespace ZWave.Layers
{
    public class SerialPortDataSource : IDataSource
    {
        public string SourceName { get; set; }
        public string Alias { get; set; }
        public string Args { get; set; }
        public BaudRates BaudRate { get; set; }
        public PInvokeStopBits StopBits { get; set; }
        public bool IsSigmaDesignsUsbProgrammingDriver { get; set; }
        public bool IsUzbDriver { get; set; }
        public SerialPortDataSource()
        {
            StopBits = PInvokeStopBits.One;
            BaudRate = BaudRates.Rate_115200;
        }

        public SerialPortDataSource(string sourceName)
            : this(sourceName, BaudRates.Rate_115200, PInvokeStopBits.One)
        {
        }

        public SerialPortDataSource(string sourceName, BaudRates baudRate)
            : this(sourceName, baudRate, PInvokeStopBits.One)
        {
        }

        public SerialPortDataSource(string sourceName, BaudRates baudRate, PInvokeStopBits stopBits)
        {
            SourceName = sourceName;
            BaudRate = baudRate;
            StopBits = stopBits;
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(SourceName);
        }

        public static AutoProgDeviceTypes GetAutoProgType(string portName)
        {
            AutoProgDeviceTypes ret = AutoProgDeviceTypes.NONE;
#if !NETCOREAPP
            var portInfo = ComputerSystemHardwareHelper.GetWin32PnPEntityClassSerialPortDevice(portName);
#else
            Win32PnPEntityClass portInfo = null;
#endif
            if (portInfo != null)
            {
                ret = AutoProgDeviceTypes.UART;
                if (portInfo.Description.ToUpper().Contains("Sigma Designs ZWave programming interface".ToUpper()))
                {
                    if (portInfo.HardwareId == "0001")
                        ret = AutoProgDeviceTypes.SD_USB_0001;
                    else
                        ret = AutoProgDeviceTypes.SD_USB_0000;
                }
                else
                {
                    if (portInfo.Description.ToUpper().Contains("UZB") || portInfo.Description.ToUpper().Contains("ZCOM"))
                    {
                        if (portInfo.HardwareId == "0001")
                            ret = AutoProgDeviceTypes.UZB_0001;
                        else
                            ret = AutoProgDeviceTypes.UZB_0000;
                    }
                }
            }
            ret.ToString()._DLOG();
            return ret;
        }

        public override string ToString()
        {
            return SourceName;
        }
    }

    public enum AutoProgDeviceTypes
    {
        NONE,
        UART,
        UZB_0000,
        UZB_0001,
        SD_USB_0000,
        SD_USB_0001
    }


    public class SerialPortProgrammerDataSource : SerialPortDataSource
    {
        public SerialPortProgrammerDataSource(string sourceName)
            : base(sourceName, BaudRates.Rate_115200)
        {
#if !NETCOREAPP
            var device = ComputerSystemHardwareHelper.GetWin32PnPEntityClassSerialPortDevice(sourceName);
#else
            Win32PnPEntityClass device = null;
#endif
            if (device != null && device.Caption != null && device.Caption.Contains("Sigma Designs"))
            {
                StopBits = PInvokeStopBits.One;
                IsSigmaDesignsUsbProgrammingDriver = true;
            }
            else if (device != null && device.Caption != null && (device.Caption.Contains("UZB") || device.Caption.Contains("ZCOM")))
            {
                IsUzbDriver = true;
            }
            else
            {
                StopBits = PInvokeStopBits.Two; //UART
            }
        }
    }

}
