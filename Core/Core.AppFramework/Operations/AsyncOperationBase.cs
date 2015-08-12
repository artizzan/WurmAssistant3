using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace AldursLab.Deprec.Core.AppFramework.Operations
{
    public abstract class AsyncOperationBase<TResult> : IAsyncOperation, IAsyncOperationWithResult<TResult>
    {
        private readonly Logger logger;

        private string currentStateOverride;
        private readonly Guid operationId = Guid.NewGuid();

        Stopwatch stopwatch = null;

        private readonly TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        private TResult result;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private int executing = 0;

        protected AsyncOperationBase()
        {
            Type type = this.GetType();
            logger = LogManager.GetLogger(type.FullName);
            Progress = 0;
            IndeterminateProgress = true;
            Status = OperationStatus.NotStarted;
            Error = NoException.Instance;
        }

        #region Public API

        public OperationStatus Status { get; private set; }

        public bool Finished
        {
            get
            {
                return Status == OperationStatus.Faulted || Status == OperationStatus.Succeeded
                       || Status == OperationStatus.Cancelled;
            }
        }

        public bool Running
        {
            get { return Status == OperationStatus.Running; }
        }

        public Exception Error { get; private set; }

        public string CurrentState
        {
            get
            {
                if (currentStateOverride != null)
                {
                    return currentStateOverride;
                }
                else
                {
                    return GetCurrentStateString();
                }
            }
        }

        public void Execute()
        {
            var executed = Interlocked.Exchange(ref executing, 1);
            if (executed == 1)
            {
                if (logger.IsEnabled(OperationBaseConfig.LogLevel))
                {
                    Log("This operation is already being executed, returning.");
                }
                return;
            }
            ExecuteAsync();
        }

        private async void ExecuteAsync()
        {
            if (logger.IsEnabled(OperationBaseConfig.LogLevel))
            {
                logger.Log(OperationBaseConfig.LogLevel, () => "Begin execute");
            }
            try
            {
                PerfBegin();
                if (Status != OperationStatus.Cancelled)
                {
                    Status = OperationStatus.Running;
                    result = await OnExecuteAsync();
                    if (Status != OperationStatus.Cancelled)
                    {
                        Progress = 100;
                        Status = OperationStatus.Succeeded;
                        if (logger.IsEnabled(OperationBaseConfig.LogLevel))
                        {
                            Log("Completed.");
                        }
                    }
                    else
                    {
                        if (logger.IsEnabled(OperationBaseConfig.LogLevel))
                        {
                            Log("Cancelled.");
                        }
                    }
                }
                else
                {
                    if (logger.IsEnabled(OperationBaseConfig.LogLevel))
                    {
                        Log("State is Cancelled, execute skipped.");
                    }
                }
                PerfEnd();
            }
            catch (Exception exception)
            {
                Error = exception;
                Status = OperationStatus.Faulted;
                if (logger.IsEnabled(OperationBaseConfig.LogLevel))
                {
                    Log("Execute error: " + exception.ToString());
                }
            }
            finally
            {
                taskCompletionSource.SetResult(true);
                if (logger.IsEnabled(OperationBaseConfig.LogLevel))
                {
                    Log("End execute");
                }
                
                cancellationTokenSource.Dispose();
            }
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public Percent Progress { get; protected set; }

        public bool IndeterminateProgress { get; protected set; }

        public async Task WhenFinishedAsync()
        {
            await taskCompletionSource.Task;
        }

        public TResult GetResult()
        {
            if (Status != OperationStatus.Succeeded)
            {
                throw new InvalidOperationException(
                    "Operation must be in succeeded state to obtain result, current status: " + Status);
            }
            return result;
        }

        public Guid OperationId
        {
            get { return operationId; }
        }

        #endregion

        #region Internal API

        /// <summary>
        /// Implement to code logic for this operation
        /// </summary>
        /// <returns></returns>
        protected abstract Task<TResult> OnExecuteAsync();

        protected CancellationToken CancellationToken
        {
            get { return cancellationTokenSource.Token; }
        }

        protected void OverrideCurrentStatus(string newStatus)
        {
            currentStateOverride = newStatus;
        }

        protected void ClearOverridenStatus()
        {
            currentStateOverride = null;
        }

        /// <summary>
        /// Informs operation engine, that operation has been successfully cancelled.
        /// </summary>
        protected void Cancelled()
        {
            Status = OperationStatus.Cancelled;
        }


        #endregion

        private string GetCurrentStateString()
        {
            switch (Status)
            {
                case OperationStatus.NotStarted:
                    return "Not started";
                case OperationStatus.Running:
                    return "Running";
                case OperationStatus.Succeeded:
                    return "Succeeded";
                case OperationStatus.Cancelled:
                    return "Cancelled";
                case OperationStatus.Faulted:
                    return "Faulted";
                default:
                    return "Unknown state of " + Status;
            }
        }

        private void PerfBegin()
        {
            if (logger.IsTraceEnabled)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
        }

        private void PerfEnd()
        {
            if (logger.IsTraceEnabled && stopwatch != null)
            {
                stopwatch.Stop();
                Log(string.Format("OnExecuteAsync took {0} milliseconds ({1} ticks)",
                    stopwatch.ElapsedMilliseconds,
                    stopwatch.ElapsedTicks),
                    OperationBaseConfig.LogLevel);
            }
        }

        protected void Log(string message, LogLevel logLevel = null)
        {
            logger.Log(
                logLevel ?? OperationBaseConfig.LogLevel,
                () => string.Format("[OID: {0}] {1}", OperationId, message));
        }

        protected void Log(Func<string> messageInvoker, LogLevel logLevel = null)
        {
            logger.Log(
                logLevel ?? OperationBaseConfig.LogLevel,
                () => string.Format("[OID: {0}] {1}", OperationId, messageInvoker()));
        }
    }

    abstract class AsyncOperationBase : AsyncOperationBase<Nothing>
    {
    }
}