using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    class LogScansRunner : IDisposable
    {
        readonly LogsScannerFactory logsScannerFactory;
        readonly PersistentCollectionsLibrary persistentLibrary;
        readonly ILogger logger;

        readonly ConcurrentQueue<ScanJob> jobQueue =
            new ConcurrentQueue<ScanJob>();
        readonly AutoResetEvent jobSignaller = new AutoResetEvent(false);
        readonly Task searchJobTask;
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public LogScansRunner([NotNull] LogsScannerFactory logsScannerFactory, [NotNull] PersistentCollectionsLibrary persistentLibrary,
            [NotNull] ILogger logger)
        {
            if (logsScannerFactory == null) throw new ArgumentNullException("logsScannerFactory");
            if (persistentLibrary == null) throw new ArgumentNullException("persistentLibrary");
            if (logger == null) throw new ArgumentNullException("logger");
            this.logsScannerFactory = logsScannerFactory;
            this.persistentLibrary = persistentLibrary;
            this.logger = logger;
            searchJobTask = new Task((token) =>
            {
                var internalCancellationToken = cancellationTokenSource.Token;
                while (jobSignaller.WaitOne())
                {
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
                            var scanner = this.logsScannerFactory.Create(job.SearchParameters, cancellationManager);
                            var results = scanner.Scan();
                            job.SetResult(results);

                            SavePersistentData();
                        }
                        catch (InternalJobCancellationException)
                        {
                            // cancel all jobs and exit the job runner
                            job.SetCancelled();
                            CancelAllRemainingJobs();
                            return;
                        }
                        catch (ExternalJobCancellationException)
                        {
                            // cancel just this job
                            job.SetCancelled();
                            
                        }
                        catch (Exception exception)
                        {
                            logger.Log(LogLevel.Error,
                                string.Format("Error during log scan, search params: {0}", job.SearchParameters), this,
                                exception);
                            job.SetException(exception);
                        }
                    }
                }
            }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
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

        void SavePersistentData()
        {
            try
            {
                persistentLibrary.SaveChanged();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at saving persistent data for WurmLogsHistory", this, exception);
            }
        }

        public async Task<ScanResult> Run(LogSearchParameters searchParameters, CancellationToken cancellationToken)
        {
            var job = new ScanJob(new TaskCompletionSource<ScanResult>(), searchParameters, cancellationToken);
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
            searchJobTask.Wait();
        }

        class ScanJob
        {
            public TaskCompletionSource<ScanResult> JobHandle { get; private set; }
            public LogSearchParameters SearchParameters { get; private set; }
            public CancellationToken ExternalCancellationToken { get; private set; }

            public ScanJob(TaskCompletionSource<ScanResult> jobHandle, LogSearchParameters searchParameters, CancellationToken externalCancellationToken)
            {
                JobHandle = jobHandle;
                SearchParameters = searchParameters;
                ExternalCancellationToken = externalCancellationToken;
            }

            public void SetResult(ScanResult results)
            {
                JobHandle.SetResult(results);
            }

            public async Task<ScanResult> AwaitComplete()
            {
                return await JobHandle.Task;
            }

            public void SetException(Exception exception)
            {
                JobHandle.SetException(exception);
            }

            public void SetCancelled()
            {
                JobHandle.SetCanceled();
            }
        }
    }

    [Serializable]
    class InternalJobCancellationException : Exception
    {
        public InternalJobCancellationException()
        {
        }

        protected InternalJobCancellationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    class ExternalJobCancellationException : Exception
    {
        public ExternalJobCancellationException()
        {
        }

        protected ExternalJobCancellationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}