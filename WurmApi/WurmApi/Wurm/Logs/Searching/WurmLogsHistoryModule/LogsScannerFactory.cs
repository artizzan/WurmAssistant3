using System;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.Logs.Parsing;
using AldurSoft.WurmApi.Wurm.Logs.Reading;
using AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics;

namespace AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule
{
    public class LogsScannerFactory
    {
        private readonly LogFileParserFactory logFileParserFactory;
        private readonly LogFileStreamReaderFactory streamReaderFactory;
        private readonly MonthlyLogFileHeuristicsFactory heuristicsFactory;
        private readonly IWurmLogFiles wurmLogFiles;
        private readonly ILogger logger;

        public LogsScannerFactory(
            LogFileParserFactory logFileParserFactory,
            LogFileStreamReaderFactory streamReaderFactory,
            MonthlyLogFileHeuristicsFactory heuristicsFactory,
            IWurmLogFiles wurmLogFiles,
            ILogger logger)
        {
            if (logFileParserFactory == null)
                throw new ArgumentNullException("logFileParserFactory");
            if (streamReaderFactory == null)
                throw new ArgumentNullException("streamReaderFactory");
            if (heuristicsFactory == null)
                throw new ArgumentNullException("heuristicsFactory");
            if (wurmLogFiles == null)
                throw new ArgumentNullException("wurmLogFiles");
            if (logger == null)
                throw new ArgumentNullException("logger");
            this.logFileParserFactory = logFileParserFactory;
            this.streamReaderFactory = streamReaderFactory;
            this.heuristicsFactory = heuristicsFactory;
            this.wurmLogFiles = wurmLogFiles;
            this.logger = logger;
        }

        /// <summary>
        /// Extracts all lines matching scan parameters.
        /// </summary>
        /// <returns></returns>
        public LogsScanner Create(LogSearchParameters logSearchParameters)
        {
            return new LogsScanner(
                logSearchParameters,
                wurmLogFiles,
                heuristicsFactory,
                streamReaderFactory,
                logger,
                logFileParserFactory);
        }
    }
}