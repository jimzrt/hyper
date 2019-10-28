using System;
using System.Linq;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text.RegularExpressions;
#if NETCOREAPP
using System.Runtime.InteropServices;
#endif

namespace ZWave.Layers.Transport
{
    public class SerialPortProvider : ISerialPortProvider, IDisposable
    {
        public const uint BUFFER_SIZE = 512;

        private readonly ISerialPortProvider _interalProvider;

        public SerialPortProvider()
        {
#if NETCOREAPP
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _interalProvider = new SerialPortProviderWindows();
            }
            else
            {
                _interalProvider = new SerialPortProviderWindows();// SerialPortProviderLinux();
            }
#else
            _interalProvider = new SerialPortProviderWindows();
#endif
        }

        public string PortName { get { return _interalProvider.PortName; } }
        public bool IsOpen { get { return _interalProvider.IsOpen; } }

        public bool Open(string portName, int baudRate, PInvokeParity parity, int dataBits, PInvokeStopBits stopBits)
        {
            return _interalProvider.Open(portName, baudRate, parity, dataBits, stopBits);
        }

        public int Read(byte[] buffer, int bufferLen)
        {
            return _interalProvider.Read(buffer, bufferLen);
        }

        public byte[] ReadExisting()
        {
            return _interalProvider.ReadExisting();
        }

        public int Write(byte[] buffer, int bufferLen)
        {
            return _interalProvider.Write(buffer, bufferLen);
        }

        public void Close()
        {
            _interalProvider.Close();
        }

        public void Dispose()
        {
            ((IDisposable)_interalProvider).Dispose();
        }

#if NETCOREAPP
        public static string[] GetPortNames()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return System.IO.Ports.SerialPort.GetPortNames();
            }
            else
            {
                return SerialPortProviderLinux.EnumConnectedUZBSticks();
            }
        }
#endif
    }

    public class SerialPortProviderWindows : ISerialPortProvider, IDisposable
    {
        private SerialPort _port;

        private string _portName;
        public string PortName
        {
            get
            {
                return _portName;
            }
        }

        public bool IsOpen
        {
            get
            {
                return _port != null && _port.IsOpen;
            }
        }

        public bool Open(string portName, int baudRate, PInvokeParity parity, int dataBits, PInvokeStopBits stopBits)
        {
            if (!IsOpen)
            {
                _portName = portName;
                _port = new SerialPort(_portName, baudRate, (Parity)(int)parity, dataBits, (StopBits)(int)stopBits);
                bool ret = false;
                try
                {
                    _port.Open();
                    ret = true;
                }
                catch (ArgumentException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                return ret;
            }
            return false;
        }

        public int Read(byte[] buffer, int bufferLen)
        {
            if (IsOpen)
            {
                int ret = -1;
                try
                {
                    ret = _port.Read(buffer, 0, bufferLen);
                }
                catch (TimeoutException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (IOException)
                {
                }
                catch (OperationCanceledException)
                {

                }
                return ret;
            }
            return -1;
        }

        public byte[] ReadExisting()
        {
            try
            {
                var bytesString = _port.ReadExisting();
                if (!string.IsNullOrEmpty(bytesString))
                {
                    return System.Text.Encoding.UTF8.GetBytes(bytesString);
                }
            }
            catch (InvalidOperationException)
            {
            }
            return null;
        }

        public int Write(byte[] buffer, int bufferLen)
        {
            if (IsOpen)
            {
                int ret = -1;
                try
                {
                    _port.Write(buffer, 0, bufferLen);
                    ret = bufferLen;
                }
                catch (IOException ioex)
                {
                    //OperationException.Throw(ioex.Message);
                }
                catch (InvalidOperationException)
                {
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                }
                return ret;
            }
            return -1;
        }

        public void Close()
        {
            if (IsOpen)
            {
                try
                {
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    _port.Close();
                    Thread.Sleep(200);
                }
                catch (IOException)
                {

                }
                _portName = null;
            }
        }

        public void Dispose()
        {
            ((IDisposable)_port).Dispose();
        }
    }

#if NETCOREAPP
    public class SerialPortProviderLinux : ISerialPortProvider
    {
        private static bool Is64Bit { get { return Environment.Is64BitProcess; } }

        #region DllImports

        [DllImport("basictrans32", EntryPoint = "SetLogLevel", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static void SetLogLevel32(byte logLevel);

        [DllImport("basictrans64", EntryPoint = "SetLogLevel", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static void SetLogLevel64(byte logLevel);

        public static void SetLogLevel(byte logLevel)
        {
            if (Is64Bit)
            {
                SetLogLevel64(logLevel);
            }
            else
            {
                SetLogLevel32(logLevel);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct USB_DEVICE_ADDRESS
        {
            public byte busNo;
            public byte deviceAddress;
        }

        [DllImport("basictrans32", EntryPoint = "EnumUZBSticks", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U4)]
        private extern static uint EnumUZBSticks32([In, Out] USB_DEVICE_ADDRESS[] lpDevices);

        [DllImport("basictrans64", EntryPoint = "EnumUZBSticks", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U4)]
        private extern static uint EnumUZBSticks64([In, Out] USB_DEVICE_ADDRESS[] lpDevices);

        private static uint EnumUZBSticks(USB_DEVICE_ADDRESS[] lpDevices)
        {
            return Is64Bit ? EnumUZBSticks64(lpDevices) : EnumUZBSticks32(lpDevices);
        }

        [DllImport("basictrans32", EntryPoint = "OpenSerialPort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool OpenSerialPort32(byte busNo, byte addressNo);

        [DllImport("basictrans64", EntryPoint = "OpenSerialPort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool OpenSerialPort64(byte busNo, byte addressNo);

        private static bool OpenSerialPort(byte busNo, byte addressNo)
        {
            //SetLogLevel(7);
            return Is64Bit ?
                OpenSerialPort64(busNo, addressNo) :
                OpenSerialPort32(busNo, addressNo);
        }

        [DllImport("basictrans32", EntryPoint = "CloseSerialPort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static void CloseSerialPort32(byte busNo, byte addressNo);

        [DllImport("basictrans64", EntryPoint = "CloseSerialPort", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static void CloseSerialPort64(byte busNo, byte addressNo);

        private static void CloseSerialPort(byte busNo, byte addressNo)
        {
            if (Is64Bit)
            {
                CloseSerialPort64(busNo, addressNo);
            }
            else
            {
                CloseSerialPort32(busNo, addressNo);
            }
        }

        [DllImport("basictrans32", EntryPoint = "SerialPortRead", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static int SerialPortRead32(byte busNo,
            byte addressNo,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pData,
            uint dataLen,
            uint timeout);

        [DllImport("basictrans64", EntryPoint = "SerialPortRead", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static int SerialPortRead64(byte busNo,
            byte addressNo,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pData,
            uint dataLen,
            uint timeout);

        private static int SerialPortRead(byte busNo, byte addressNo, byte[] pData, uint dataLen, uint timeout)
        {
            return Is64Bit ?
                SerialPortRead64(busNo, addressNo, pData, dataLen, timeout) :
                SerialPortRead32(busNo, addressNo, pData, dataLen, timeout);
        }

        [DllImport("basictrans32", EntryPoint = "SerialPortWrite", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static int SerialPortWrite32(byte busNo,
            byte addressNo,
            byte[] pData,
            uint dataLen,
            uint timeout);

        [DllImport("basictrans64", EntryPoint = "SerialPortWrite", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private extern static int SerialPortWrite64(byte busNo,
            byte addressNo,
            byte[] pData,
            uint dataLen,
            uint timeout);

        private static int SerialPortWrite(byte busNo, byte addressNo, byte[] pData, uint dataLen, uint timeout)
        {
            return Is64Bit ?
                SerialPortWrite64(busNo, addressNo, pData, dataLen, timeout) :
                SerialPortWrite32(busNo, addressNo, pData, dataLen, timeout);
        }

        [DllImport("basictrans32", EntryPoint = "SerialPortIsOpen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool SerialPortIsOpen32(byte busNo, byte addressNo);

        [DllImport("basictrans64", EntryPoint = "SerialPortIsOpen", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool SerialPortIsOpen64(byte busNo, byte addressNo);

        private static bool SerialPortIsOpen(byte busNo, byte addressNo)
        {
            return Is64Bit ? SerialPortIsOpen64(busNo, addressNo) : SerialPortIsOpen32(busNo, addressNo);
        }

        #endregion

        private byte _busNo;
        private byte _addressNo;

        private string _portName;
        public string PortName
        {
            get
            {
                return _portName;
            }
        }

        public bool IsOpen
        {
            get
            {
                return SerialPortIsOpen(_busNo, _addressNo);
            }
        }

        /// <summary>
        /// Open serial port on Linux-like OSs
        /// </summary>
        /// <param name="portName">Name of serial port to be opened</param>
        /// <param name="baudRate">Ignored</param>
        /// <param name="parity">Ignored</param>
        /// <param name="dataBits">Ignored</param>
        /// <param name="stopBits">Ignored</param>
        /// <returns>true if port opening succeeded</returns>
        public bool Open(string portName, int baudRate, PInvokeParity parity, int dataBits, PInvokeStopBits stopBits)
        {
            _portName = portName;
            var nums = Regex.Matches(_portName, @"\d{3}");
            if (nums.Count == 2 &&
                byte.TryParse(nums[0].Value, out _busNo) &&
                byte.TryParse(nums[1].Value, out _addressNo)
                )
            {
                return OpenSerialPort(_busNo, _addressNo);
            }
            return false;
        }

        public int Read(byte[] buffer, int bufferLen)
        {
            if (IsOpen)
            {
                return SerialPortRead(_busNo, _addressNo, buffer, (uint)bufferLen, 0);
            }
            return -1;
        }

        public byte[] ReadExisting()
        {
            if (IsOpen)
            {
                var buffer = new byte[512];
                var readLen = SerialPortRead(_busNo, _addressNo, buffer, (uint)buffer.Length, 500);
                if (readLen != -1)
                {
                    return buffer.Take(readLen).ToArray();
                }
            }
            return null;
        }

        public int Write(byte[] buffer, int bufferLen)
        {
            if (IsOpen)
            {
                return SerialPortWrite(_busNo, _addressNo, buffer, (uint)bufferLen, 0);
            }
            return -1;
        }

        public void Close()
        {
            if (IsOpen)
            {
                CloseSerialPort(_busNo, _addressNo);
                _portName = null;
            }
        }

        public static string[] EnumConnectedUZBSticks()
        {
            var count = EnumUZBSticks(null);
            if (count > 0)
            {
                var devices = new USB_DEVICE_ADDRESS[count];
                EnumUZBSticks(devices);
                return devices.Select(addr => $"{addr.busNo:D3}_{addr.deviceAddress:D3}").ToArray();
            }
            return new string[] { };
        }
    }
#endif
}