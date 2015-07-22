using System;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.Logs.Parsing;
using AldurSoft.WurmApi.Wurm.Logs.Reading;

namespace AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics
{
    public class MonthlyHeuristicsExtractorFactory
    {
        private readonly ParsingHelper parsingHelper;
        private readonly LogFileStreamReaderFactory logFileStreamReaderFactory;
        private readonly ILogger logger;

        public MonthlyHeuristicsExtractorFactory(
            ParsingHelper parsingHelper,
            LogFileStreamReaderFactory logFileStreamReaderFactory,
            ILogger logger)
        {
            if (parsingHelper == null) throw new ArgumentNullException("parsingHelper");
            if (logFileStreamReaderFactory == null) throw new ArgumentNullException("logFileStreamReaderFactory");
            if (logger == null) throw new ArgumentNullException("logger");
            this.parsingHelper = parsingHelper;
            this.logFileStreamReaderFactory = logFileStreamReaderFactory;
            this.logger = logger;
        }

        public MonthlyHeuristicsExtractor Create(LogFileInfo logFileInfo)
        {
            return new MonthlyHeuristicsExtractor(logFileInfo, parsingHelper, logFileStreamReaderFactory, logger);
        }
    }
}
