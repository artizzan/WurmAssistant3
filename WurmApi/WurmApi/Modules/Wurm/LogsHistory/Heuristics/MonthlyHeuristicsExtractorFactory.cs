using System;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    public class MonthlyHeuristicsExtractorFactory
    {
        private readonly LogFileStreamReaderFactory logFileStreamReaderFactory;
        private readonly ILogger logger;

        public MonthlyHeuristicsExtractorFactory(
            LogFileStreamReaderFactory logFileStreamReaderFactory,
            ILogger logger)
        {
            if (logFileStreamReaderFactory == null) throw new ArgumentNullException("logFileStreamReaderFactory");
            if (logger == null) throw new ArgumentNullException("logger");
            this.logFileStreamReaderFactory = logFileStreamReaderFactory;
            this.logger = logger;
        }

        public MonthlyHeuristicsExtractor Create(LogFileInfo logFileInfo)
        {
            return new MonthlyHeuristicsExtractor(logFileInfo, logFileStreamReaderFactory, logger);
        }
    }
}
