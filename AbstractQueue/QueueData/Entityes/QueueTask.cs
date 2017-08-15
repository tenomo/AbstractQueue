using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AbstractQueue.Core;

namespace AbstractQueue.QueueData.Entities
{
    [Serializable]
    public sealed class QueueTask
    {
        /// <summary>
        /// QueueTasks Id.
        /// </summary>
        //[Key]
        // public   int Id { get; set; }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        public int TaskIndexInQueue { get; set; }

        /// <summary>
        ///  Queue name which the belongs task.
        /// </summary>
        // [Index]
        public string QueueName { get; set; }

        /// <summary>
        /// QueueTasks status.
        /// </summary>
        [Index]
        public QueueTaskStatus QueueTaskStatus { get; internal set; }

        /// <summary>
        /// QueueTasks type for determinate the executer(custom value).
        /// </summary>
        public byte? Type { get; set; }

        /// <summary>
        /// QueueTasks body as Json for execution.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Date of creation task.
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Date of executed task.
        /// </summary>
        public DateTime? ExecutedDate { get; set; }

        private byte _attempt;

        public byte Attempt
        {
            get { return _attempt; }
            set
            {
                if (value < 0)
                    throw new InvalidOperationException(
                        $"Failed {value} attempt, task must be have failed status, check logic");
                _attempt = value;
            }
        }

        public static QueueTask Create(byte type, string body)
        {
            return new QueueTask()
            {
                Id = Guid.NewGuid().ToString(),
                QueueTaskStatus = QueueTaskStatus.Created,
                Type = type,
                Body = body,
                CreationDate = DateTime.Now,
            };
        }
    }
}
