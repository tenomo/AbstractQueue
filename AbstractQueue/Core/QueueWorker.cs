using System;
using System.Linq;
using System.Threading.Tasks;
using AbstractQueue.QueueData.Entities;
using AbstractQueue.TaskStore;

namespace AbstractQueue.Core
{
   internal class QueueWorker //: ITaskExecutionObserver
    {
        /// <summary>
        /// Queue worker Id.
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
            Infrastructure.TaskExecutionObserver.Kernal.FailedExecuteTaskEvent += ExecutedTaskEvent;
            Infrastructure.TaskExecutionObserver.Kernal.SuccessExecuteTaskEvent += ExecutedTaskEvent;
            Infrastructure.TaskExecutionObserver.Kernal.InProccesTaskEvent += delegate(ITaskStore store, QueueTask task)
            {
                this.WorkerTaskStore = BuildTaskStore();
            };
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
        private void ExecutedTaskEvent(ITaskStore obj ,QueueTask e)
        {
            if (obj.Id == this.WorkerTaskStore.Id  )
            {  
                SetStatusFree();
                TryStartNextTask();
            }
        }


        /// <summary>
        /// Try execute task.
        /// </summary>
        internal void TryStartTask()
        { 
            var isCan =     IsCanExecuteTask( );
            if (!isCan)
            {
                SetStatusFree();
                return;
            }
            else
            {

                SetStatusBusy();
                UpExecutionAttempt(currentTask);
                WorkerTaskStore.SetProccesStatus(currentTask);

                new TaskFactory().StartNew(() =>
                {
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

        }

        /// <summary>
        /// Try execute task.
        /// </summary>
        private void TryStartNextTask()
        {
              this.WorkerTaskStore = BuildTaskStore();
            var isCan = IsCanExecuteTask();
            if (!isCan)
            {
                SetStatusFree();
                return;
            }
            else
            {
                SetStatusBusy();
                UpExecutionAttempt(currentTask);
                WorkerTaskStore.SetProccesStatus(currentTask);
                try
                {
                    Executer.Execute(currentTask);
                    WorkerTaskStore.SetSuccessStatus(currentTask);
                }
                catch
                {
                    WorkerTaskStore.SetFailedStatus(currentTask);
                }
            }
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
        /// Check executeble queueTask and return boolean value and queueTask workerId.
        /// </summary>
        /// <param name="isCan"></param>
        /// <param name="index"></param>
        private bool IsCanExecuteTask()
        {
           var task = WorkerTaskStore.GetAll().FirstOrDefault(each => CheckStatus(each)  && each.QueueName == queueName ); 
             
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
            if (task == null)
                return false;
            if (isTryHandleError   && task.QueueTaskStatus == QueueTaskStatus.Failed &&
                task.Attempt <= countHandleFailed)
                return true;

            return   task.QueueTaskStatus == QueueTaskStatus.Created;


        }

        /// <summary>
        /// Iterate the execution task attempt. 
        /// </summary>
        /// <param name="task"></param>
        private void UpExecutionAttempt(QueueTask task)
        {
            task.Attempt++;
            WorkerTaskStore.Update(task);
        } 
    }
}
