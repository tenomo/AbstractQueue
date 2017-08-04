using System; 

namespace AbstractQueue
{
  public  class QueueTask
    {
        private static int _testId;
        /// <summary>
        /// QueueTask id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// QueueTask status.
        /// </summary>
        public QueueTaskStatus QueueTaskStatus { get; internal set; }



        public void SetFailed()
        {
            QueueTaskStatus = QueueTaskStatus.Failed;
            OnExecutedTask(QueueTaskStatus);
        }

        public void SetSuccess()
        {
            QueueTaskStatus = QueueTaskStatus.Success;
            OnExecutedTask(QueueTaskStatus);
        }

        /// <summary>
        /// QueueTask type for determinate the executer(custom value).
        /// </summary>
        public byte Type { get; set; }
        /// <summary>
        /// QueueTask body for execution.
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// Ovserve about executed the task.
        /// </summary>
        public event Action<QueueTask> ExecutedTask;

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
                if (QueueTaskStatus != QueueTaskStatus.Failed)
                    throw new InvalidOperationException($"Failed {value} attempt, task must be have failed status, check logic");
                _attempt = value;
            }
        }

        public QueueTask(byte type, byte[] body)
        {
            QueueTaskStatus = QueueTaskStatus.Created;
            Type = type;
            Body = body;
            CreationDate = DateTime.Now;

            _testId++;

            Id = _testId; // clear ( id must be generate on db)
        }


        private   void OnExecutedTask(  QueueTaskStatus taskStatus)
        {
            if (taskStatus == QueueTaskStatus.Failed || taskStatus == QueueTaskStatus.Success)
                ExecutedTask?.Invoke(this);
         
        }
    }
}
