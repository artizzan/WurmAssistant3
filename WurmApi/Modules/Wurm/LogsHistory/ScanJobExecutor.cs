using System;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.PersistentObjects;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory
{
    class ScanJobExecutor : JobExecutor<LogSearchParameters, ScanResult>
    {
        readonly LogsScannerFactory logsScannerFactory;
        readonly PersistentCollectionsLibrary persistentLibrary;
        readonly IWurmApiLogger logger;

        public ScanJobExecutor([NotNull] LogsScannerFactory logsScannerFactory,
            [NotNull] PersistentCollectionsLibrary persistentLibrary, [NotNull] IWurmApiLogger logger)
        {
            if (logsScannerFactory == null) throw new ArgumentNullException(nameof(logsScannerFactory));
            if (persistentLibrary == null) throw new ArgumentNullException(nameof(persistentLibrary));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logsScannerFactory = logsScannerFactory;
            this.persistentLibrary = persistentLibrary;
            this.logger = logger;
        }

        public override ScanResult Execute(LogSearchParameters logSearchParameters, JobCancellationManager jobCancellationManager)
        {
            try
            {
                var scanner = logsScannerFactory.Create(logSearchParameters, jobCancellationManager);
                var results = scanner.Scan();

                try
                {
                    persistentLibrary.SaveChanged();
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error, "Error at saving persistent data for WurmLogsHistory", this, exception);
                }

                return results;
            }
            catch (Exception exception)
            {
                var canceledException = exception as OperationCanceledException;
                if (canceledException == null)
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Error during scan job execution, search params: {0}", logSearchParameters),
                        this,
                        exception);
                }
                throw;
            }
        }
    }
}