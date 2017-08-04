using System; 

namespace AbstractQueue
{
  public  class Task
    {
        private static int _testId;
        /// <summary>
        /// Task id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Task status.
        /// </summary>
        public ETaskStatus ETaskStatus { get; set; }
        /// <summary>
        /// Task type for the determinate executer(custom value).
        /// </summary>
        public byte TaskType { get; set; }
        /// <summary>
        /// Task body for execution.
        /// </summary>
        public byte[] TaskBody { get; set; }

        /// <summary>
        /// Ovserve about executed the task.
        /// </summary>
        public event Action<Task> ExecutedTask;

        /// <summary>
        /// Date of creation task.
        /// </summary>
        public DateTime CreationDate;

        /// <summary>
        /// Date of executed task.
        /// </summary>
        public DateTime ExecutedDate;

        private byte _attempt;


        public byte Attempt
        {
            get { return _attempt; }
            set
            {
                if (ETaskStatus != ETaskStatus.Failed)
                    throw new InvalidOperationException($"Failed {value} attempt, task must be have failed status, check logic");
                _attempt = value;
            }
        }

        public Task(byte taskType, byte[] taskBody)
        {
            ETaskStatus = ETaskStatus.Created;
            TaskType = taskType;
            TaskBody = taskBody;
            CreationDate = DateTime.Now;

            _testId++;

            Id = _testId; // clear ( id must be generate on db)
        }


    }
}
