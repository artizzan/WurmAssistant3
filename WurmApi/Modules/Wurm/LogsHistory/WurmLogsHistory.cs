using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.PersistentObjects.FlatFiles;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory
{
    class WurmLogsHistory : IWurmLogsHistory, IDisposable
    {
        readonly QueuedJobsSyncRunner<LogSearchParameters, ScanResult> runner;

        public WurmLogsHistory([NotNull] IWurmLogFiles wurmLogFiles, [NotNull] IWurmApiLogger logger,
            [NotNull] string heuristicsDataDirectory, [NotNull] LogFileStreamReaderFactory logFileStreamReaderFactory,
            [NotNull] IWurmApiConfig wurmApiConfig)
        {
            if (wurmLogFiles == null) throw new ArgumentNullException(nameof(wurmLogFiles));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (heuristicsDataDirectory == null) throw new ArgumentNullException(nameof(heuristicsDataDirectory));
            if (logFileStreamReaderFactory == null) throw new ArgumentNullException(nameof(logFileStreamReaderFactory));
            if (wurmApiConfig == null) throw new ArgumentNullException(nameof(wurmApiConfig));

            IPersistenceStrategy persistenceStrategy = new FlatFilesPersistenceStrategy(heuristicsDataDirectory);
 
            var persistentLibrary =
                new PersistentCollectionsLibrary(persistenceStrategy,
                    new PersObjErrorHandlingStrategy(logger));
            var heuristicsCollection = persistentLibrary.GetCollection("heuristics");

            var logsScannerFactory = new LogsScannerFactory(
                new LogFileParserFactory(logger),
                logFileStreamReaderFactory,
                new MonthlyLogFilesHeuristics(
                    heuristicsCollection,
                    wurmLogFiles,
                    new MonthlyHeuristicsExtractorFactory(logFileStreamReaderFactory, logger, wurmApiConfig)),
                wurmLogFiles,
                logger,
                wurmApiConfig);

            runner =
                new QueuedJobsSyncRunner<LogSearchParameters, ScanResult>(
                    new ScanJobExecutor(logsScannerFactory, persistentLibrary, logger),
                    logger);
        }

        public async Task<IList<LogEntry>> ScanAsync(LogSearchParameters logSearchParameters)
        {
            var result = await runner.Run(logSearchParameters, CancellationToken.None).ConfigureAwait(false);
            return result.LogEntries;
        }

        public IList<LogEntry> Scan(LogSearchParameters logSearchParameters)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => ScanAsync(logSearchParameters).Result);
        }

        public async Task<IList<LogEntry>> ScanAsync(LogSearchParameters logSearchParameters,
            CancellationToken cancellationToken)
        {
            var result = await runner.Run(logSearchParameters, cancellationToken).ConfigureAwait(false);
            return result.LogEntries;
        }

        public IList<LogEntry> Scan(LogSearchParameters logSearchParameters, CancellationToken cancellationToken)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => ScanAsync(logSearchParameters, cancellationToken).Result);
        }

        public void Dispose()
        {
            runner.Dispose();
        }
    }
}
