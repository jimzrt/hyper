namespace ZWave.BasicApplication.Enums
{
    public enum SlaveLearnModes
    {
        /// <summary>
        /// Disable SlaveLearnMode (disable possibility to add/remove Virtual Slave nodes)
        /// Allowed when bridge is a primary controller, an inclusion controller or a
        /// secondary controller
        /// </summary>
        VirtualSlaveLearnModeDisable = 0,
        /// <summary>
        /// Enable SlaveLearnMode - Enable possibility for including/excluding a Virtual
        /// Slave node by an external primary/inclusion controller Allowed when bridge
        /// is an inclusion controller or a secondary controller
        /// </summary>
        VirtualSlaveLearnModeEnable = 1,
        /// <summary>
        /// Add new Virtual Slave node if possible Allowed when bridge is a primary or
        /// an inclusion controller Slave Learn function done when Callback function
        /// returns ASSIGN_NODEID_DONE
        /// </summary>
        VirtualSlaveLearnModeAdd = 2,
        /// <summary>
        /// Remove existing Virtual Slave node Allowed when bridge is a primary or an
        /// inclusion controller Slave Learn function done when Callback function returns
        /// ASSIGN_NODEID_DONE
        /// </summary>
        VirtualSlaveLearnModeRemove = 3
    }
}
