using System;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace AldursLab.Deprec.Core.AppFramework.Operations
{
    public abstract class OperationBase<TResult> : IOperationWithResult<TResult>
    {
        private readonly Logger logger;

        private string currentStateOverride;
        private readonly Guid operationId = Guid.NewGuid();

        Stopwatch stopwatch = null;

        private TResult result;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private int executing = 0;

        protected OperationBase()
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

                    result = OnExecute();
                    PerfEnd();
                    if (Status != OperationStatus.Cancelled)
                    {
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
            if (logger.IsEnabled(OperationBaseConfig.LogLevel))
            {
                Log("End execute");
            }
            cancellationTokenSource.Dispose();
        }

        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        public Percent Progress { get; protected set; }

        public bool IndeterminateProgress { get; protected set; }

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

        protected abstract TResult OnExecute();

        /// <summary>
        /// Allows to handle any errors thrown by executing operation.
        /// Return true to indicate, that error is handled and operation succeeded,
        /// otherwise operation transitions into faulted state.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        protected virtual bool OnError(Exception exception)
        {
            return false;
        }

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
        /// If this method is not called and operation execution is done, 
        /// it will be flagged as Succeeded.
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

    abstract class OperationBase : OperationBase<Nothing>
    {
    }
}