namespace AbstractQueue
{
   public enum ETaskStatus
    {
        Created,
        InProcces,
        Failed,
        Success,
        NotProcces = Failed | Success | InProcces
    }
}