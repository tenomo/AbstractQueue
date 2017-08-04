namespace AbstractQueue
{
    /// <summary>
    /// Status which the determinate task.
    /// </summary>
    public enum QueueTaskStatus
    {
        /// <summary>
        /// Task created and wating.
        /// </summary>
        Created,
        /// <summary>
        /// Task in procces.
        /// </summary>
        InProcces,
        /// <summary>
        /// Failed executed task.
        /// </summary>
        Failed,
        /// <summary>
        /// Success executed task.
        /// </summary>
        Success,
        /// <summary>
        ///  Not procces task.
        /// </summary>
        NotProccess = Failed | Success | Created
    }
}