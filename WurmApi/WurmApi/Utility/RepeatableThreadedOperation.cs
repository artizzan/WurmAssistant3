using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Utility
{
    /// <summary>
    /// Maintains a dedicated thread to run job delegate at. Job is executed repeatedly in response to signals. 
    /// If signals arrive when job is already running, another execution is queued immediately.
    /// </summary>
    public sealed class RepeatableThreadedOperation : IDisposable
    {
        [CanBeNull]
        readonly IPublicEventMarshaller publicEventMarshaller;
        readonly Task task;
        Task delayedSignallingTask;
        volatile bool exit = false;
        readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        readonly TaskCompletionSource<bool> operationCompletedAtLeastOnceAwaiter = new TaskCompletionSource<bool>();

        /// <summary>
        /// </summary>
        /// <param name="job">
        /// Delegate to execute after receiving signals.
        /// </param>
        /// <param name="publicEventMarshaller">Optional thread marshaller of the events.</param>
        public RepeatableThreadedOperation([NotNull] Action job, IPublicEventMarshaller publicEventMarshaller = null)
        {
            this.publicEventMarshaller = publicEventMarshaller;

            if (job == null) throw new ArgumentNullException("job");
            task = new Task(() =>
            {
                while (true)
                {
                    autoResetEvent.WaitOne();
                    if (exit)
                    {
                        break;
                    }
                    try
                    {
                        job();
                        operationCompletedAtLeastOnceAwaiter.TrySetResult(true);
                    }
                    catch (Exception exception)
                    {
                        try
                        {
                            OnOperationFaulted(new ExceptionEventArgs(exception));
                        }
                        catch (Exception)
                        {
                            // nothing more to be done here
                        }
                    }
                    if (exit)
                    {
                        break;
                    }
                }
            }, TaskCreationOptions.LongRunning);
            task.Start();
        }

        /// <summary>
        /// Triggered if job delegate has thrown an unhandled exception.
        /// Should this handler throw unhandled exception and should it return to the underlying thread, it will be ignored.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> OperationError;

        /// <summary>
        /// Indicates, if operation had been successfully executed at least once.
        /// </summary>
        public bool OperationCompletedAtLeastOnce
        {
            get { return operationCompletedAtLeastOnceAwaiter.Task.IsCompleted; }
        }

        /// <summary>
        /// Task, that transitions to completion, when operation has been successfully executed for the first time.
        /// </summary>
        public Task OperationCompletedAtLeastOnceAwaiter
        {
            get
            {
                return operationCompletedAtLeastOnceAwaiter.Task;
            }
        }

        /// <summary>
        /// Synchronously waits for when operation is successfully executed for the first time
        /// </summary>
        /// <param name="timeout">Null - wait indefinitely</param>
        /// <exception cref="TimeoutException"></exception>
        public void WaitSynchronouslyForInitialOperation(TimeSpan? timeout = null)
        {
            if (timeout != null)
            {
                if (OperationCompletedAtLeastOnceAwaiter.Wait(timeout.Value))
                {
                    throw new TimeoutException();
                }
            }
            OperationCompletedAtLeastOnceAwaiter.Wait();
        }

        /// <summary>
        /// Signals to the operation, that it should execute.
        /// If signalled while operation is running, it will be queued for another execution.
        /// </summary>
        public void Signal()
        {
            autoResetEvent.Set();
        }

        /// <summary>
        /// Schedules signal to be sent after specified time interval.
        /// If signal is already scheduled, this method does nothing. 
        /// </summary>
        /// <param name="delay"></param>
        public void DelayedSignal(TimeSpan delay)
        {
            // preventing possible explosion in thread counts
            if (delayedSignallingTask != null && delayedSignallingTask.IsCompleted)
            {
                delayedSignallingTask = Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(delay);
                    Signal();
                }, TaskCreationOptions.LongRunning);
            }
        }

        public void Dispose()
        {
            // stop the operation
            exit = true;
            autoResetEvent.Set();
            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
                // task might be faulted, which is irrelevant for cleanup
            }
            task.Dispose();
        }

        void OnOperationFaulted(ExceptionEventArgs e)
        {
            var handler = OperationError;
            if (handler != null)
            {
                if (publicEventMarshaller != null)
                {
                    publicEventMarshaller.Marshal(() =>
                    {
                        handler(this, e);
                    });
                }
                else
                {
                    handler(this, e);
                }
            }
        }
    }
}
