using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.JobRunning
{
    sealed class TaskManager : IDisposable
    {
        readonly ILogger logger;

        Task runner;
        volatile bool stop = false;
        readonly HashSet<TaskHandle> tasks = new HashSet<TaskHandle>();
        readonly object locker = new object();

        public TaskManager([NotNull] ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
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

    class TaskHandle
    {
        readonly Action action;
        DateTimeOffset lastInvoke = DateTimeOffset.MinValue;
        int triggered;

        public TaskHandle([NotNull] Action action, [NotNull] string description, TimeSpan? minimumDelay = null)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (description == null) throw new ArgumentNullException("description");
            this.action = action;
            MinimumDelay = minimumDelay.HasValue ? MinimumDelay : TimeSpan.Zero;
            Description = description;
        }

        internal void TryExecute()
        {
            var now = Time.Get.LocalNowOffset;
            if (triggered == 1 && lastInvoke < now - MinimumDelay)
            {
                Interlocked.Exchange(ref triggered, 0);
                try
                {
                    action();
                    lastInvoke = now;
                }
                catch (Exception)
                {
                    // if action failed, retry on next run
                    Interlocked.Exchange(ref triggered, 1);
                    throw;
                }
            }
        }

        public TimeSpan MinimumDelay { get; private set; }
        public string Description { get; private set; }

        public void Trigger()
        {
            Interlocked.Exchange(ref triggered, 1);
        }
    }
}
