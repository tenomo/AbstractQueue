using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbstractQueue
{
  [Serializable]
    public  sealed class QueueTask 
  {
      /// <summary>
      /// QueueTasks id.
      /// </summary>

    
       [Key]
        public   int Id { get; set; }

         
        public int TaskIndexInQueue { get; set; }

        /// <summary>
        ///  Queue name which the belongs task.
        /// </summary>
        public string QueueName { get;   set; }

        /// <summary>
        /// QueueTasks status.
        /// </summary>
        public QueueTaskStatus QueueTaskStatus { get; internal set; }

        

        /// <summary>
        /// QueueTasks type for determinate the executer(custom value).
        /// </summary>
        public byte Type { get; set; }

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

      //[NotMapped]
      //internal TaskStore.TaskStore TaskStore { get; set; } 
      public static QueueTask Create(byte type, string body)
        {
            return new QueueTask()
            {
                QueueTaskStatus = QueueTaskStatus.Created,
                Type = type,
                Body = body,
                CreationDate = DateTime.Now,
            };
        }



    }
}
