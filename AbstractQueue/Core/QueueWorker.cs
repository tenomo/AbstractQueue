using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using AbstractQueue.Infrastructure;
using AbstractQueue.QueueData.Entities;

namespace AbstractQueue.Core
{
   internal class QueueWorker : ITaskExecutionObserve
    {
        /// <summary>
        /// Queue worker id.
         /// </summary>
        internal int Id { get; set; }
        /// <summary>
        /// Concrete executer
        /// </summary>
        private readonly AbstractTaskExecuter Executer;

        private bool isTryHandleError = false;
        private int attemptMaxCount;

        public int AttemptMaxCount
        {
            get { return attemptMaxCount; }
            private set
            {
                if (value > 1)
                    isTryHandleError = true;
                else if (value < 0)
                {
                    throw new ArgumentException("AttemptMaxCount must be more 0");
                }

                attemptMaxCount = value;
            }
        }

        /// <summary>
        /// QueueTasks WorkerTaskStore.
        /// </summary>
        internal TaskStore.TaskStore WorkerTaskStore {   get; private set; }

        internal bool InProccess { get; private set; }

        /// <summary>
        /// Current queue name
        /// </summary>
        private readonly string queueName;

     

          private QueueTask currentTask;

        internal QueueTask CurrentTask
        {
            get { return currentTask; }
            private set { currentTask = value; }
        }

        private TaskStore.TaskStore BuildTaskStore()
        {
            var taskstore = new TaskStore.TaskStore();
            taskstore.SuccessExecuteTaskEvent += ExecutedTaskEvent;
             taskstore.FailedExecuteTaskEvent += ExecutedTaskEvent;


            taskstore.SuccessExecuteTaskEvent += OnSuccessExecuteTaskEvent;
            taskstore.InProccesTaskEvent += OnInProccesTaskEvent;
            taskstore.FailedExecuteTaskEvent += OnFailedExecuteTaskEvent;
            return taskstore;
        }
            

        public QueueWorker(AbstractTaskExecuter executer, string queueName, int attemptMaxCount = 0)
        {
            this.WorkerTaskStore = BuildTaskStore();
          this.TryStartTask();
            this.Executer = executer;
            this.AttemptMaxCount = attemptMaxCount;
            this.queueName = queueName;
            SetStatusFree();
        }

        internal int CountHandleFailed
        {
            get { return attemptMaxCount; }
            private set
            {
                if (value > 1)
                    isTryHandleError = true;
                else if (value < 0)
                {
                    throw new ArgumentException("AttemptMaxCount must be more 0");
                }
                attemptMaxCount = value;
            }
        }


        /// <summary>
        /// Executed queueTask handler.
        /// </summary>
        /// <param name="queueTask"></param>
        private void ExecutedTaskEvent(QueueTask queueTask)
        {
            SetStatusFree();
            TryStartNextTask();
        }


        /// <summary>
        /// Try execute task.
        /// </summary>
        internal void TryStartTask()
        { 
            var isCan =     IsCanExecuteTask( );
            if (!isCan)
            {
                Logger.Log("wtf");  
                SetStatusFree();
                return;
            }
            
            SetStatusBusy();

            UpAttempt(currentTask);
            WorkerTaskStore.SetProccesStatus(currentTask);

            new TaskFactory().StartNew(() =>
            {
                this.WorkerTaskStore = BuildTaskStore(); // Rebuild task store for new thread. 
                try
                {
                    Logger.Log("Start task");  
                    Executer.Execute(currentTask);
                    WorkerTaskStore.SetSuccessStatus(currentTask);
                }
                catch
                {
                    WorkerTaskStore.SetFailedStatus(currentTask);
                }
            });
        }


        private void SetStatusBusy()
        {
            InProccess = true;
        }
        private void SetStatusFree()
        {
            InProccess = false;
        }
   

        /// <summary>
        /// Try execute task.
        /// </summary>
        private void TryStartNextTask()
        {
            var isCan = IsCanExecuteTask();
            if (!isCan)
            {
                Logger.Log("wtf"); Executer.Execute(currentTask);
                SetStatusFree();
                return;
            }

            
          SetStatusBusy();
            UpAttempt(currentTask);
            WorkerTaskStore.SetProccesStatus(currentTask);
            try
            {
                Logger.Log("Start task"); Executer.Execute(currentTask);
                WorkerTaskStore.SetSuccessStatus(currentTask);
            }
            catch
            {
                WorkerTaskStore.SetFailedStatus(currentTask);
            }
        }


        /// <summary>
        /// Check executeble queueTask and return boolean value and queueTask workerId.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private bool IsCanExecuteTask()
        {
          
            var task = WorkerTaskStore.GetAll().FirstOrDefault(each => CheckStatus(each)  && each.QueueName == queueName );
  Logger.Log(queueName);
            Logger.Log((task == null).ToString());

            var isCan = task != null;

            if (isCan)
            {


                if (CheckStatus(task) &&
                    CheckOnAttemptLimit(task))
                {
                    currentTask = task;
                    currentTask.TaskIndexInQueue = this.Id;
                    WorkerTaskStore.Update(currentTask);

                    return isCan;
                }

                else
                {
                    currentTask = null;
                    SetStatusFree();
                    return false;
                }
            }
            currentTask = null;
            SetStatusFree();
            return false; 
        }



        /// <summary>
        /// Check task status.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckStatus(QueueTask task)
        {
            
            var _isHandleFailed = this.isTryHandleError;
            if (_isHandleFailed)
            {

                return  (task?.QueueTaskStatus == QueueTaskStatus.Created || task?.QueueTaskStatus == QueueTaskStatus.Failed);
            }
            else
                return task?.QueueTaskStatus == QueueTaskStatus.Created;
        }

        /// <summary>
        /// Сheck the task's attempts of execution limit.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool CheckOnAttemptLimit(QueueTask task)
        {
            var countHandleFailed = CountHandleFailed;
            if (task == null || isTryHandleError == false)
                return true;
            return task.QueueTaskStatus == QueueTaskStatus.Failed && task.Attempt <= countHandleFailed;
        }

        /// <summary>
        /// Iterate the execution task attempt. 
        /// </summary>
        /// <param name="task"></param>
        private void UpAttempt(QueueTask task)
        {
            task.Attempt++;
            WorkerTaskStore.Update(task);
        }

        public event Action<QueueTask> SuccessExecuteTaskEvent;
        public event Action<QueueTask> FailedExecuteTaskEvent;
        public event Action<QueueTask> InProccesTaskEvent;

        protected virtual void OnSuccessExecuteTaskEvent(QueueTask obj)
        {
            SuccessExecuteTaskEvent?.Invoke(obj);
        }

        protected virtual void OnFailedExecuteTaskEvent(QueueTask obj)
        {
            FailedExecuteTaskEvent?.Invoke(obj);
        }

        protected virtual void OnInProccesTaskEvent(QueueTask obj)
        {
            InProccesTaskEvent?.Invoke(obj);
        }
    }
}
