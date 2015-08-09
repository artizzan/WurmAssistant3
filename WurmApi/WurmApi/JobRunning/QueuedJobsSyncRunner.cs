using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.JobRunning
{
    class QueuedJobsSyncRunner<TJobContext, TResult> : IDisposable
    {
        readonly JobExecutor<TJobContext, TResult> jobExecutor;

        readonly ConcurrentQueue<ScanJob> jobQueue =
            new ConcurrentQueue<ScanJob>();
        readonly AutoResetEvent jobSignaller = new AutoResetEvent(false);
        readonly Task searchJobTask;
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public QueuedJobsSyncRunner([NotNull] JobExecutor<TJobContext, TResult> jobExecutor, ILogger logger)
        {
            if (jobExecutor == null) throw new ArgumentNullException("jobExecutor");
            this.jobExecutor = jobExecutor;

            searchJobTask = new Task((token) =>
            {
                var internalCancellationToken = cancellationTokenSource.Token;
                while (true)
                {
                    var signalled = jobSignaller.WaitOne(jobExecutor.IdleJobTreshhold);
                    if (internalCancellationToken.IsCancellationRequested)
                    {
                        CancelAllRemainingJobs();
                        return;
                    }
                    if (!signalled)
                    {
                        try
                        {
                            jobExecutor.IdleJob(internalCancellationToken);
                        }
                        catch (Exception exception)
                        {
                            logger.Log(LogLevel.Error, "Regular job has thrown unhandled exception.", this,
                                exception);
                        }
                    }
                    ScanJob job;
                    while (jobQueue.TryDequeue(out job))
                    {
                        if (internalCancellationToken.IsCancellationRequested)
                        {
                            CancelAllRemainingJobs();
                            return;
                        }
                        try
                        {
                            var cancellationManager = new JobCancellationManager(internalCancellationToken,
                                job.ExternalCancellationToken);
                            var result = this.jobExecutor.Execute(job.JobContext, cancellationManager);
                            job.SetResult(result);
                        }
                        catch (Exception exception)
                        {
                            job.SetException(exception);
                        }
                    }
                    if (internalCancellationToken.IsCancellationRequested)
                    {
                        CancelAllRemainingJobs();
                        return;
                    }
                }
            }, TaskCreationOptions.LongRunning);
            searchJobTask.Start();
        }

        void CancelAllRemainingJobs()
        {
            ScanJob job;
            while (jobQueue.TryDequeue(out job))
            {
                job.JobHandle.SetCanceled();
            }
        }

        public async Task<TResult> Run(TJobContext jobContext, CancellationToken cancellationToken)
        {
            var job = new ScanJob(new TaskCompletionSource<TResult>(), jobContext, cancellationToken);
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new OperationCanceledException("Job runner is shutting down and cannot process this request.");
            }
            jobQueue.Enqueue(job);
            jobSignaller.Set();
            var result = await job.AwaitComplete().ConfigureAwait(false);
            return result;
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            jobSignaller.Set();
            // protect against unlikely race condition
            if (!searchJobTask.Wait(50))
            {
                jobSignaller.Set();
                searchJobTask.Wait();
            }
            GC.SuppressFinalize(this);
        }

        ~QueuedJobsSyncRunner()
        {
            try
            {
                cancellationTokenSource.Cancel();
                jobSignaller.Set();
            }
            catch (Exception)
            {
                // objects may be already disposed
            }
        }

        class ScanJob
        {
            public TaskCompletionSource<TResult> JobHandle { get; private set; }
            public TJobContext JobContext { get; private set; }
            public CancellationToken ExternalCancellationToken { get; private set; }

            public ScanJob(TaskCompletionSource<TResult> jobHandle, TJobContext jobContext, CancellationToken externalCancellationToken)
            {
                JobHandle = jobHandle;
                JobContext = jobContext;
                ExternalCancellationToken = externalCancellationToken;
            }

            public void SetResult(TResult results)
            {
                JobHandle.SetResult(results);
            }

            public async Task<TResult> AwaitComplete()
            {
                return await JobHandle.Task;
            }

            public void SetException(Exception exception)
            {
                JobHandle.SetException(exception);
            }
        }
    }

    /// <summary>
    /// Implements execution logic for single job.
    /// </summary>
    /// <typeparam name="TJobContext">Argument passed to job instance</typeparam>
    /// <typeparam name="TResult">Result of the job</typeparam>
    abstract class JobExecutor<TJobContext, TResult>
    {
        readonly TimeSpan defaultIdleJobTreshhold = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// Executes single job synchronously with other jobs and returns it's result.
        /// </summary>
        /// <param name="jobContext">Argument passed to job instance</param>
        /// <param name="jobCancellationManager">Manager for cancellation requests</param>
        /// <returns></returns>
        public abstract TResult Execute(TJobContext jobContext, JobCancellationManager jobCancellationManager);

        /// <summary>
        /// Job to execute, if queue is inactive for IdleJobTreshhold
        /// </summary>
        /// <param name="cancellationToken"></param>
        public virtual void IdleJob(CancellationToken cancellationToken) { }

        /// <summary>
        /// Time to wait, before IdleJob is triggered due to queue inactivity.
        /// Defaults to never.
        /// </summary>
        public virtual TimeSpan IdleJobTreshhold
        {
            get { return defaultIdleJobTreshhold; }
        }
    }
}