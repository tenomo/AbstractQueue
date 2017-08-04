namespace AbstractQueue
{
   public enum ETaskStatus
    {
        Created,
        InProcces,
        Failed,
        Success,
        NotProccess = Failed | Success | InProcces
    }
}