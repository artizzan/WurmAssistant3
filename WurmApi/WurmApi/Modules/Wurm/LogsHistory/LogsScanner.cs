using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory
{
    class LogsScanner
    {
        readonly IWurmLogFiles wurmLogFiles;
        readonly MonthlyLogFilesHeuristics monthlyHeuristics;
        readonly LogFileStreamReaderFactory streamReaderFactory;
        readonly ILogger logger;
        readonly LogFileParserFactory logFileParserFactory;

        readonly LogSearchParameters logSearchParameters;
        readonly JobCancellationManager cancellationManager;

        public LogsScanner(
            [NotNull] LogSearchParameters logSearchParameters, 
            [NotNull] JobCancellationManager cancellationManager,
            [NotNull] IWurmLogFiles wurmLogFiles,
            [NotNull] MonthlyLogFilesHeuristics monthlyHeuristics,
            [NotNull] LogFileStreamReaderFactory streamReaderFactory,
            [NotNull] ILogger logger,
            [NotNull] LogFileParserFactory logFileParserFactory)
        {
            if (logSearchParameters == null) throw new ArgumentNullException("logSearchParameters");
            if (cancellationManager == null) throw new ArgumentNullException("cancellationManager");
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (monthlyHeuristics == null) throw new ArgumentNullException("monthlyHeuristics");
            if (streamReaderFactory == null) throw new ArgumentNullException("streamReaderFactory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (logFileParserFactory == null) throw new ArgumentNullException("logFileParserFactory");
            this.logSearchParameters = logSearchParameters;
            this.cancellationManager = cancellationManager;
            this.wurmLogFiles = wurmLogFiles;
            this.monthlyHeuristics = monthlyHeuristics;
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
        public ScanResult Scan()
        {
            var man = this.wurmLogFiles.GetForCharacter(logSearchParameters.CharacterName);
            LogFileInfo[] logFileInfos =
                man.GetLogFiles(logSearchParameters.DateFrom, logSearchParameters.DateTo).ToArray();

            cancellationManager.ThrowIfCancelled();

            CharacterMonthlyLogHeuristics characterHeuristics = monthlyHeuristics.GetForCharacter(logSearchParameters.CharacterName);

            cancellationManager.ThrowIfCancelled();

            var result = GetEntries(logFileInfos, characterHeuristics);
            return new ScanResult(result);
        }

        private IList<LogEntry> GetEntries(
            IEnumerable<LogFileInfo> logFileInfos,
            CharacterMonthlyLogHeuristics heuristicsFileMap)
        {
            var parsingHelper = logFileParserFactory.Create();

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

                cancellationManager.ThrowIfCancelled();
            }
            return result;
        }

        private void ParseMonthlyFile(
            CharacterMonthlyLogHeuristics heuristicsFileMap,
            LogFileInfo logFileInfo,
            List<LogEntry> result,
            LogFileParser logFileParser)
        {
            var heuristics = heuristicsFileMap.GetFullHeuristicsForMonth(logFileInfo);
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

                cancellationManager.ThrowIfCancelled();
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
    }
}