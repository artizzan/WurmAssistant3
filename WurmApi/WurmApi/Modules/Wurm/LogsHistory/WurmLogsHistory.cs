using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    public class WurmLogsHistory : IWurmLogsHistory, IDisposable
    {
        readonly QueuedJobsSyncRunner<LogSearchParameters, ScanResult> runner;

        public WurmLogsHistory([NotNull] IWurmLogFiles wurmLogFiles,
            [NotNull] ILogger logger, string heuristicsDataDirectory)
        {
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (logger == null) throw new ArgumentNullException("logger");

            var persistentLibrary = new PersistentCollectionsLibrary(new FlatFilesPersistenceStrategy(heuristicsDataDirectory));
            var heuristicsCollection = persistentLibrary.GetCollection("heuristics");

            var logFileStreamReaderFactory = new LogFileStreamReaderFactory();
            var logsScannerFactory = new LogsScannerFactory(
                new LogFileParserFactory(logger),
                logFileStreamReaderFactory,
                new MonthlyLogFilesHeuristics(
                    heuristicsCollection,
                    wurmLogFiles,
                    new MonthlyHeuristicsExtractorFactory(logFileStreamReaderFactory, logger)),
                wurmLogFiles,
                logger);

            runner = new QueuedJobsSyncRunner<LogSearchParameters, ScanResult>(new ScanJobExecutor(logsScannerFactory, persistentLibrary, logger), logger);
        }

        public virtual async Task<IList<LogEntry>> ScanAsync(LogSearchParameters logSearchParameters)
        {
            var result = await runner.Run(logSearchParameters, CancellationToken.None).ConfigureAwait(false);
            return result.LogEntries;
        }

        public IList<LogEntry> Scan(LogSearchParameters logSearchParameters)
        {
            return TaskHelper.UnwrapSingularAggegateException(() => ScanAsync(logSearchParameters).Result);
        }

        public virtual async Task<IList<LogEntry>> ScanAsync(LogSearchParameters logSearchParameters, CancellationToken cancellationToken)
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
