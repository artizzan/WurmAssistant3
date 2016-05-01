using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.JobRunning
{
    sealed class TaskManager : IDisposable
    {
        readonly IWurmApiLogger logger;

        readonly Task runner;
        volatile bool stop = false;
        readonly HashSet<TaskHandle> tasks = new HashSet<TaskHandle>();
        readonly object locker = new object();

        public TaskManager([NotNull] IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
            CycleDelayMillis = 100;
            runner = new Task(JobRunner, TaskCreationOptions.LongRunning);
            runner.Start();
        }

        void JobRunner()
        {
            while (true)
            {
                Thread.Sleep(CycleDelayMillis);
                if (stop) return;
                TaskHandle[] allTasks;

                lock (locker)
                {
                    allTasks = tasks.ToArray();
                }

                foreach (var taskHandle in allTasks)
                {
                    try
                    {
                        taskHandle.TryExecute();
                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Error, "Error during task execution: " + taskHandle.Description, this, exception);
                    }
                }
            }
        }

        public static int CycleDelayMillis { get; set; }

        public void Add(TaskHandle taskHandle)
        {
            lock (locker)
            {
                tasks.Add(taskHandle);
            }
        }

        public void Remove(TaskHandle taskHandle)
        {
            lock (locker)
            {
                tasks.Remove(taskHandle);
            }
        }

        public void Dispose()
        {
            stop = true;
            if (runner.Wait(10000))
            {
                runner.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        ~TaskManager()
        {
            stop = true;
        }
    }
}
