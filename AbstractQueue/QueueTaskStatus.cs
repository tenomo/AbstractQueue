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
        Created = 0,
        /// <summary>
        /// Task in procces.
        /// </summary>
        InProcces = 1,
        /// <summary>
        /// Failed executed task.
        /// </summary>
        Failed = 3,
        /// <summary>
        /// Success executed task.
        /// </summary>
        Success = 2,
        /// <summary>
        ///  Not procces task.
        /// </summary>
        NotProccess = Failed | Success | Created,

        TryFailInProcces =  InProcces  

       
    }
}