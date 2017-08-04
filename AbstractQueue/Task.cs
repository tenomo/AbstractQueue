using System; 

namespace AbstractQueue
{
  public  class Task
    {
        private static int _testId;
        public int Id { get; set; }
        public ETaskStatus ETaskStatus { get; set; }
        public int TaskType { get; set; }
        public byte[] TaskBody { get; set; }
        public event Action<Task> ExecutedTask;

        public DateTime CreationDate;

        public DateTime ExecutedDate;
        public Task(int taskType, byte[] taskBody)
        {
            ETaskStatus = ETaskStatus.Created;
            TaskType = taskType;
            TaskBody = taskBody;
            CreationDate = DateTime.Now;

            _testId++;

            Id = _testId;
        }


    }
}
