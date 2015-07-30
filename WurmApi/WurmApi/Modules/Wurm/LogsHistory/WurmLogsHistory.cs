using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.PersistentObjects;
using AldursLab.PersistentObjects.FlatFiles;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    public class WurmLogsHistory : IWurmLogsHistory, IDisposable
    {
        readonly LogScansRunner runner;

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

            runner = new LogScansRunner(logsScannerFactory, persistentLibrary, logger);
        }

        public virtual async Task<IList<LogEntry>> Scan(LogSearchParameters logSearchParameters)
        {
            var result = await runner.Run(logSearchParameters, CancellationToken.None).ConfigureAwait(false);
            return result.LogEntries;
        }

        public virtual async Task<IList<LogEntry>> Scan(LogSearchParameters logSearchParameters, CancellationToken cancellationToken)
        {
            var result = await runner.Run(logSearchParameters, cancellationToken).ConfigureAwait(false);
            return result.LogEntries;
        }

        public void Dispose()
        {
            runner.Dispose();
        }
    }
}
