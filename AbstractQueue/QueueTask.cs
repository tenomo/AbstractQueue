using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbstractQueue
{
    [Serializable]
    public sealed class QueueTask :  IQueueName
    {
        /// <summary>
        /// QueueTask id.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

         
        public int TaskIdInQueue { get; set; }

        /// <summary>
        ///  Queue name which the belongs task.
        /// </summary>
        public string QueueName { get;   set; }

        /// <summary>
        /// QueueTask status.
        /// </summary>
        public QueueTaskStatus QueueTaskStatus { get; internal set; }



       

        /// <summary>
        /// QueueTask type for determinate the executer(custom value).
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// QueueTask body as Json for execution.
        /// </summary>
        public string Body { get; set; }

       

        /// <summary>
        /// Date of creation task.
        /// </summary>
        public DateTime CreationDate;

        /// <summary>
        /// Date of executed task.
        /// </summary>
        public DateTime ExecutedDate;


      private  readonly object mLock = new object();

        private byte _attempt = 0;


        public byte Attempt
        {
            get
            {
                lock (mLock)
                {
                      return _attempt;
                }
              
            }
            set
            {

                lock (mLock)
                {
                    if (value < 0)
                        throw new InvalidOperationException(
                            $"Failed {value} attempt, task must be have failed status, check logic");

                    _attempt = value;
                }
              
            }
        }

        public static  QueueTask Create(byte type, string body )
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
