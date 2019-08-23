namespace Utils
{
    public class BaseWin32DeviceClass
    {
        #region Private Fields

        #endregion
        #region Public Properties

        public string HardwareId { get; set; }

        /// <summary>
        /// Gets the availability.
        /// </summary>
        /// <value>The availability.</value>
        public ushort? Availability { get; set; }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption { get; set; }

        /// <summary>
        /// Gets the config manager error code.
        /// </summary>
        /// <value>The config manager error code.</value>
        public uint? ConfigManagerErrorCode { get; set; }

        /// <summary>
        /// Gets the config manager user config.
        /// </summary>
        /// <value>The config manager user config.</value>
        public bool? ConfigManagerUserConfig { get; set; }

        /// <summary>
        /// Gets the name of the creation class.
        /// </summary>
        /// <value>The name of the creation class.</value>
        public string CreationClassName { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the device ID.
        /// </summary>
        /// <value>The device ID.</value>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the PNP device ID.
        /// </summary>
        /// <value>The PNP device ID.</value>
        public string PnpDeviceId { get; set; }

        /// <summary>
        /// Gets the power management capabilities.
        /// </summary>
        /// <value>The power management capabilities.</value>
        public ushort[] PowerManagementCapabilities { get; set; }

        /// <summary>
        /// Gets the power management supported.
        /// </summary>
        /// <value>The power management supported.</value>
        public bool? PowerManagementSupported { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status { get; set; }

        /// <summary>
        /// Gets the status info.
        /// </summary>
        /// <value>The status info.</value>
        public ushort? StatusInfo { get; set; }

        /// <summary>
        /// Gets the name of the system creation class.
        /// </summary>
        /// <value>The name of the system creation class.</value>
        public string SystemCreationClassName { get; set; }

        /// <summary>
        /// Gets the name of the system.
        /// </summary>
        /// <value>The name of the system.</value>
        public string SystemName { get; set; }

        #endregion
        public override string ToString()
        {
            if (Description != null)
            {
                return string.Format("{0} - {1}", Caption, Description);
            }
            return string.Format("{0}", Caption);
        }
    }
}
