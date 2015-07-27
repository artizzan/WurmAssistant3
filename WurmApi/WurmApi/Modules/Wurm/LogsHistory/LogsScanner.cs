using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    public class LogsScanner
    {
        private readonly LogSearchParameters logSearchParameters;
        private readonly IWurmLogFiles wurmLogFiles;
        private readonly MonthlyLogFileHeuristicsFactory heuristicsFactory;
        private readonly LogFileStreamReaderFactory streamReaderFactory;
        private readonly ILogger logger;
        private readonly LogFileParserFactory logFileParserFactory;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(4,4);

        public LogsScanner(
            LogSearchParameters logSearchParameters,
            IWurmLogFiles wurmLogFiles,
            MonthlyLogFileHeuristicsFactory heuristicsFactory,
            LogFileStreamReaderFactory streamReaderFactory,
            ILogger logger,
            LogFileParserFactory logFileParserFactory)
        {
            if (wurmLogFiles == null)
                throw new ArgumentNullException("wurmLogFiles");
            if (heuristicsFactory == null)
                throw new ArgumentNullException("heuristicsFactory");
            if (streamReaderFactory == null)
                throw new ArgumentNullException("streamReaderFactory");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (logFileParserFactory == null)
                throw new ArgumentNullException("logFileParserFactory");
            this.logSearchParameters = logSearchParameters;
            this.wurmLogFiles = wurmLogFiles;
            this.heuristicsFactory = heuristicsFactory;
            this.streamReaderFactory = streamReaderFactory;
            this.logger = logger;
            this.logFileParserFactory = logFileParserFactory;
        }

        /// <summary>
        /// Extracts all entries spanning given scan parameters. 
        /// Extraction is performed on a separate thread.
        /// Returned entries are in descending timestamp order.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<LogEntry>> ExtractEntries()
        {
            LogFileInfo[] logFileInfos =
                this.wurmLogFiles.TryGetLogFiles(logSearchParameters).ToArray();

            IEnumerable<LogFileInfo> monthlyFiles = logFileInfos.Where(info => info.LogFileDate.LogSavingType == LogSavingType.Monthly);
            HeuristicsFileMap heuristicsFileMap = new HeuristicsFileMap();
            CharacterMonthlyLogHeuristics characterHeuristics = heuristicsFactory.Create(logSearchParameters.CharacterName);
            foreach (var monthlyFile in monthlyFiles)
            {
                MonthlyFileHeuristics monthlyHeuristics = await characterHeuristics.GetFullHeuristicsForMonthAsync(monthlyFile);
                heuristicsFileMap.Add(monthlyFile, monthlyHeuristics);
            }

            return await ExtractOnWorkerThread(logFileInfos, heuristicsFileMap);
        }

        private async Task<IList<LogEntry>> ExtractOnWorkerThread(
            IEnumerable<LogFileInfo> logFileInfos,
            HeuristicsFileMap heuristicsFileMap)
        {
            try
            {
                await semaphore.WaitAsync();
                var parsingHelper = logFileParserFactory.Create();
                return await Task.Factory.StartNew(
                    () =>
                        {
                            List<LogEntry> result = new List<LogEntry>();
                            IOrderedEnumerable<LogFileInfo> orderedLogFileInfos =
                                logFileInfos.OrderBy(info => info.LogFileDate.DateTime);

                            foreach (LogFileInfo logFileInfo in orderedLogFileInfos)
                            {
                                if (logFileInfo.LogFileDate.LogSavingType == LogSavingType.Monthly)
                                {
                                    ParseMonthlyFile(heuristicsFileMap, logFileInfo, result, parsingHelper);
                                }
                                else if (logFileInfo.LogFileDate.LogSavingType == LogSavingType.Daily)
                                {
                                    ParseDailyFile(logFileInfo, result, parsingHelper);
                                }
                                else
                                {
                                    logger.Log(
                                        LogLevel.Warn,
                                        string.Format(
                                            "LogsScanner encountered and skipped file with unsupported saving type, type: {0}, file: {1}",
                                            logFileInfo.LogFileDate.LogSavingType,
                                            logFileInfo.FullPath),
                                        this,
                                        null);
                                }
                            }
                            return result;
                        });
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void ParseMonthlyFile(
            HeuristicsFileMap heuristicsFileMap,
            LogFileInfo logFileInfo,
            List<LogEntry> result,
            LogFileParser logFileParser)
        {
            var heuristics = heuristicsFileMap.Get(logFileInfo);
            var dayToSearchFrom = GetMinDayToSearchFrom(logSearchParameters.DateFrom, logFileInfo.LogFileDate.DateTime);
            var dayToSearchTo = GetMaxDayToSearchUpTo(logSearchParameters.DateTo, logFileInfo.LogFileDate.DateTime);
            for (int day = dayToSearchFrom; day <= dayToSearchTo; day++)
            {
                var thisEntryDate = new DateTime(
                    logFileInfo.LogFileDate.DateTime.Year,
                    logFileInfo.LogFileDate.DateTime.Month,
                    day,
                    0,
                    0,
                    0);
                var thisDayHeuristics = heuristics.GetForDay(day);
                int currentLineIndex = 0;
                List<string> allLines = new List<string>();
                using (
                    var reader = streamReaderFactory.Create(
                        logFileInfo.FullPath,
                        thisDayHeuristics.StartPositionInBytes))
                {
                    string currentLine;
                    while ((currentLine = reader.TryReadNextLine()) != null)
                    {
                        currentLineIndex++;
                        if (currentLineIndex > thisDayHeuristics.LinesLength)
                            break;
                        allLines.Add(currentLine);
                    }
                }
                IEnumerable<LogEntry> parsedLines = logFileParser.ParseLinesForDay(allLines, thisEntryDate, logFileInfo);
                result.AddRange(parsedLines);
            }
        }

        private int GetMaxDayToSearchUpTo(DateTime to, DateTime logDateTime)
        {
            if (to.Year == logDateTime.Year && to.Month == logDateTime.Month)
            {
                return to.Day;
            }
            else
            {
                return DateTime.DaysInMonth(logDateTime.Year, logDateTime.Month);
            }
        }

        private int GetMinDayToSearchFrom(DateTime frm, DateTime logDateTime)
        {
            if (frm.Year == logDateTime.Year && frm.Month == logDateTime.Month)
            {
                return frm.Day;
            }
            else
            {
                return 1;
            }
        }

        private void ParseDailyFile(LogFileInfo logFileInfo, List<LogEntry> result, LogFileParser logFileParser)
        {
            List<string> allLines = new List<string>();
            using (var reader = streamReaderFactory.Create(logFileInfo.FullPath))
            {
                string currentLine;
                while ((currentLine = reader.TryReadNextLine()) != null)
                {
                    allLines.Add(currentLine);
                }
            }

            IEnumerable<LogEntry> parsedLines = logFileParser.ParseLinesForDay(
                allLines,
                logFileInfo.LogFileDate.DateTime,
                logFileInfo);
            result.AddRange(parsedLines);
        }

        class HeuristicsFileMap
        {
            readonly Dictionary<LogFileInfo, MonthlyFileHeuristics> fileToHeuristicsMap = new Dictionary<LogFileInfo, MonthlyFileHeuristics>();

            public void Add(LogFileInfo logFileInfo, MonthlyFileHeuristics heuristics)
            {
                fileToHeuristicsMap[logFileInfo] = heuristics;
            }

            public MonthlyFileHeuristics Get(LogFileInfo logFileInfo)
            {
                return fileToHeuristicsMap[logFileInfo];
            }
        }
    }
}