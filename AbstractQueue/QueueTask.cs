using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbstractQueue
{
    [Serializable]
    public sealed class QueueTask : IExecutedTask, IQueueName
    {
        /// <summary>
        /// QueueTask id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Queue id which the belongs task [Autoincrement]
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QueueId { get; set; }

        /// <summary>
        ///  Queue name which the belongs task.
        /// </summary>
        public string QueueName { get; set; }

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
        /// QueueTask body as Json for execution.
        /// </summary>
        public string Body { get; set; }

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
                    throw new InvalidOperationException(
                        $"Failed {value} attempt, task must be have failed status, check logic");
                _attempt = value;
            }
        }

        public QueueTask(byte type, string body, string queueName)
        {
            QueueTaskStatus = QueueTaskStatus.Created;
            Type = type;
            Body = body;
            CreationDate = DateTime.Now;
            QueueName = queueName;
        }


        private void OnExecutedTask(QueueTaskStatus taskStatus)
        {
            if (taskStatus == QueueTaskStatus.Failed || taskStatus == QueueTaskStatus.Success)
                ExecutedTask?.Invoke(this);

        }
    }
}
