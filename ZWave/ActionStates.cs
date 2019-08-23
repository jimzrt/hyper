namespace ZWave
{
    public enum ActionStates
    {
        /// <summary>
        /// Operation is created
        /// </summary>
        None,
        /// <summary>
        /// Operation is currently running. Running operation receives incoming data.
        /// </summary>
        Running,
        /// <summary>
        /// Operation is completed. Completed operation is removed from SessionLayer processing queue.
        /// </summary>
        Completed,
        Completing,
        /// <summary>
        /// Operation is failed. Failed operation is removed from SessionLayer processing queue.
        /// </summary>
        Failed,
        Failing,
        /// <summary>
        /// Operation is expired. Expired operation is removed from SessionLayer processing queue.
        /// </summary>
        Expired,
        Expiring,
        /// <summary>
        /// Operation is cancelled. Cancelled operation is removed from SessionLayer processing queue.
        /// </summary>
        Cancelled,
        Cancelling
    }
}
