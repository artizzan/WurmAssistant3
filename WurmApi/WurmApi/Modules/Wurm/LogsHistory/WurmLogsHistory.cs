using System.Collections.Generic;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    public class WurmLogsHistory : IWurmLogsHistory
    {
        private readonly LogsScannerFactory logsScannerFactory;

        public WurmLogsHistory(IWurmApiDataContext dataContext, IWurmLogFiles wurmLogFiles, ILogger logger)
        {
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
            var scanner = logsScannerFactory.Create(logSearchParameters);
            var results = await scanner.ExtractEntries();
            return results;
        }
    }
}
