using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AldurSoft.WurmApi.Impl.Utility;
using AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl.Heuristics;

namespace AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl
{
    public class WurmLogsHistory : IWurmLogsHistory
    {
        private readonly IThreadGuard threadGuard;
        private readonly LogsScannerFactory logsScannerFactory;

        public WurmLogsHistory(IWurmApiDataContext dataContext, IWurmLogFiles wurmLogFiles, ILogger logger,
            IThreadGuard threadGuard)
        {
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.threadGuard = threadGuard;
            logsScannerFactory = new LogsScannerFactory(
                new LogFileParserFactory(new ParsingHelper(), logger),
                new LogFileStreamReaderFactory(),
                new MonthlyLogFileHeuristicsFactory(
                    dataContext,
                    wurmLogFiles,
                    new MonthlyHeuristicsExtractorFactory(new ParsingHelper(), new LogFileStreamReaderFactory(), logger)),
                wurmLogFiles,
                logger);
        }

        public virtual async Task<IList<LogEntry>> Scan(LogSearchParameters logSearchParameters)
        {
            threadGuard.ValidateCurrentThread();
            var scanner = logsScannerFactory.Create(logSearchParameters);
            var results = await scanner.ExtractEntries();
            return results;
        }
    }
}
