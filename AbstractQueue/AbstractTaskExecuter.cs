namespace AbstractQueue
{

    public abstract class AbstractTaskExecuter
    {

        /// <summary>
        /// Execute task.
        /// </summary>
        /// <param name="task"></param>
        public abstract void Execute(Task task);

        /// <summary>
        /// Set task status after executed.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="status"></param>
        protected void SetTaskStatus(Task task, ETaskStatus status)
        {
            task.ETaskStatus = status;
        }
    }
}
