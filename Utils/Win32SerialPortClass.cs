namespace Utils
{
    public class Win32SerialPortClass : BaseWin32DeviceClass
    {
        #region Private Fields

        #endregion
        #region Public Properties
        /// <summary>
        /// Gets the binary.
        /// </summary>
        /// <value>The binary.</value>
        public bool? Binary { get; set; }

        /// <summary>
        /// Gets the max baud rate.
        /// </summary>
        /// <value>The max baud rate.</value>
        public uint? MaxBaudRate { get; set; }

        /// <summary>
        /// Gets the maximum size of the input buffer.
        /// </summary>
        /// <value>The maximum size of the input buffer.</value>
        public uint? MaximumInputBufferSize { get; set; }

        /// <summary>
        /// Gets the maximum size of the output buffer.
        /// </summary>
        /// <value>The maximum size of the output buffer.</value>
        public uint? MaximumOutputBufferSize { get; set; }

        /// <summary>
        /// Gets the OS auto discovered.
        /// </summary>
        /// <value>The OS auto discovered.</value>
        public bool? OsAutoDiscovered { get; set; }

        /// <summary>
        /// Gets the type of the provider.
        /// </summary>
        /// <value>The type of the provider.</value>
        public string ProviderType { get; set; }

        /// <summary>
        /// Gets the settable baud rate.
        /// </summary>
        /// <value>The settable baud rate.</value>
        public bool? SettableBaudRate { get; set; }

        /// <summary>
        /// Gets the settable data bits.
        /// </summary>
        /// <value>The settable data bits.</value>
        public bool? SettableDataBits { get; set; }

        /// <summary>
        /// Gets the settable flow control.
        /// </summary>
        /// <value>The settable flow control.</value>
        public bool? SettableFlowControl { get; set; }

        /// <summary>
        /// Gets the settable parity.
        /// </summary>
        /// <value>The settable parity.</value>
        public bool? SettableParity { get; set; }

        /// <summary>
        /// Gets the settable parity check.
        /// </summary>
        /// <value>The settable parity check.</value>
        public bool? SettableParityCheck { get; set; }

        /// <summary>
        /// Gets the settable RLSD.
        /// </summary>
        /// <value>The settable RLSD.</value>
        public bool? SettableRlsd { get; set; }

        /// <summary>
        /// Gets the settable stop bits.
        /// </summary>
        /// <value>The settable stop bits.</value>
        public bool? SettableStopBits { get; set; }

        /// <summary>
        /// Gets the supports 16 bit mode.
        /// </summary>
        /// <value>The supports 16 bit mode.</value>
        public bool? Supports16BitMode { get; set; }

        /// <summary>
        /// Gets the supports DTRDSR.
        /// </summary>
        /// <value>The supports DTRDSR.</value>
        public bool? SupportsDtrdsr { get; set; }

        /// <summary>
        /// Gets the supports elapsed timeouts.
        /// </summary>
        /// <value>The supports elapsed timeouts.</value>
        public bool? SupportsElapsedTimeouts { get; set; }

        /// <summary>
        /// Gets the supports int timeouts.
        /// </summary>
        /// <value>The supports int timeouts.</value>
        public bool? SupportsIntTimeouts { get; set; }

        /// <summary>
        /// Gets the supports parity check.
        /// </summary>
        /// <value>The supports parity check.</value>
        public bool? SupportsParityCheck { get; set; }

        /// <summary>
        /// Gets the supports RLSD.
        /// </summary>
        /// <value>The supports RLSD.</value>
        public bool? SupportsRlsd { get; set; }

        /// <summary>
        /// Gets the supports RTSCTS.
        /// </summary>
        /// <value>The supports RTSCTS.</value>
        public bool? SupportsRtscts { get; set; }

        /// <summary>
        /// Gets the supports special characters.
        /// </summary>
        /// <value>The supports special characters.</value>
        public bool? SupportsSpecialCharacters { get; set; }

        /// <summary>
        /// Gets the supports XOnXOff.
        /// </summary>
        /// <value>The supports XOnXOff.</value>
        public bool? SupportsXonXOff { get; set; }

        /// <summary>
        /// Gets the supports XOnXOffSet.
        /// </summary>
        /// <value>The supports XOnXOffSet.</value>
        public bool? SupportsXonXOffSet { get; set; }

        #endregion
    }
}
