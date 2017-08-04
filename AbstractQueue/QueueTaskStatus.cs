namespace AbstractQueue
{
   public enum QueueTaskStatus
    {
        Created,
        InProcces,
        Failed,
        Success,
        NotProccess = Failed | Success | InProcces
    }
}