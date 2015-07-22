using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Persistence.DataModel.LogsHistoryModel;
using AldurSoft.WurmApi.Wurm.Logs.Parsing;

namespace AldurSoft.WurmApi.Wurm.Logs.Searching.WurmLogsHistoryModule.Heuristics.MonthlyDataBuilders
{
    class DataBuilder : IMonthlyHeuristicsDataBuilder
    {
        private readonly string logFileName;
        private readonly ParsingHelper parsingHelper;
        private readonly DateTime today;
        private readonly ILogger logger;

        private Dictionary<int, WurmLogMonthlyFileHeuristics> dayToHeuristicsMap;

        private Dictionary<int, WurmLogMonthlyFileHeuristics> DayToHeuristicsMap
        {
            get
            {
                if (dayToHeuristicsMap == null)
                {
                    dayToHeuristicsMap = new Dictionary<int, WurmLogMonthlyFileHeuristics>();
                    dayToHeuristicsMap[1] = new WurmLogMonthlyFileHeuristics();
                }
                return dayToHeuristicsMap;
            }
        }

        private DateTime ProcessedLogDate { get; set; }
        private bool ThisLogIsForCurrentMonth { get; set; }
        private int PreviousDay { get; set; }
        private int CurrentDay { get; set; }
        private int DayCountInThisLogMonth { get; set; }
        private int LineCounter { get; set; }
        private TimeSpan LastLogLineStamp { get; set; }

        private bool resultTaken = false;

        private bool firstLoggingStartedFound = false;

        private bool NoDataGathered
        {
            get
            {
                if (DayToHeuristicsMap.Keys.Count == 1)
                {
                    var heuristic = DayToHeuristicsMap[1];
                    if (heuristic.FilePositionInBytes == 0 && heuristic.LinesCount == 0)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public DataBuilder(string logFileName, ParsingHelper parsingHelper, DateTime today, ILogger logger)
        {
            if (logFileName == null)
                throw new ArgumentNullException("logFileName");
            if (parsingHelper == null)
                throw new ArgumentNullException("parsingHelper");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.logFileName = logFileName;
            this.parsingHelper = parsingHelper;
            this.today = today;
            this.logger = logger;

            var logDate = parsingHelper.GetDateFromLogFileName(logFileName);
            if (logDate.LogSavingType != LogSavingType.Monthly)
            {
                throw new WurmApiException("This builder can only be used for monthly log files, actual file name: " + logFileName);
            }
            ProcessedLogDate = logDate.DateTime;
            DayCountInThisLogMonth = parsingHelper.GetDaysInMonthForLogFile(logFileName);
            ThisLogIsForCurrentMonth = ProcessedLogDate.Year == today.Year && ProcessedLogDate.Month == today.Month;
            LineCounter = 1;
        }

        public void ProcessLine(string line, long lastReadLineStartPosition)
        {
            AssertResultNotTaken();

            LineCounter++;
            try
            {
                if (line.StartsWith("Logging started", StringComparison.Ordinal))
                {
                    var dayStamp = parsingHelper.GetDateFromLogFileLoggingStarted(line);
                    if (dayStamp.Day > CurrentDay)
                    {
                        CurrentDay = dayStamp.Day;
                        LastLogLineStamp = TimeSpan.Zero;
                        AdvanceDays(line, lastReadLineStartPosition);
                    }
                    else if (firstLoggingStartedFound && dayStamp.Day < CurrentDay)
                    {
                        //seems the logs have invalid timestamps, we need to flag for rollback
                        RollbackHeuristics(dayStamp.Day);
                    }
                    firstLoggingStartedFound = true;
                }
                else
                {
                    var lineStamp = parsingHelper.GetTimestampFromLogLine(line);
                    if (lineStamp < LastLogLineStamp
                        && parsingHelper.AreMoreThanOneHourAppartOnSameDay(lineStamp, LastLogLineStamp))
                    {
                        CurrentDay++;
                        LastLogLineStamp = TimeSpan.Zero;
                        AdvanceDays(line, lastReadLineStartPosition);
                    }
                    else
                    {
                        LastLogLineStamp = lineStamp;
                    }
                }
            }
            catch (WurmApiException exception)
            {
                // ignore this line
                logger.Log(
                    LogLevel.Warn,
                    string.Format("Unexpected exception while parsing line: {0}", line),
                    exception,
                    this);
            }
        }

        private void RollbackHeuristics(int actualCurrentDay)
        {
            // remove heuristics since this day (including this day)

            var overflows = DayToHeuristicsMap.Where(pair => pair.Key > actualCurrentDay).OrderBy(pair => pair.Key).ToList();
            int overflowLineCount = LineCounter;
            if (overflows.Any())
            {
                overflowLineCount += overflows.Sum(pair => pair.Value.LinesCount);
            }
            foreach (var keyValuePair in overflows)
            {
                dayToHeuristicsMap.Remove(keyValuePair.Key);
            }
            LineCounter = overflowLineCount;

            CurrentDay = actualCurrentDay;
            PreviousDay = CurrentDay - 1;
        }

        public void Complete(long finalPositionInLogFile)
        {
            AssertResultNotTaken();

            if (NoDataGathered)
            {
                return;
            }

            var lastDayProcessed = DayToHeuristicsMap.Keys.Max();
            WurmLogMonthlyFileHeuristics lastDayInfo = DayToHeuristicsMap[lastDayProcessed];

            if (ThisLogIsForCurrentMonth)
            {
                if (lastDayProcessed == today.Day)
                {
                    // we dont want limiting todays line counter
                    lastDayInfo.LinesCount = int.MaxValue;
                    LineCounter = 0;
                }
                else
                {
                    lastDayInfo.LinesCount = LineCounter;
                    LineCounter = 0;
                }

                for (int i = lastDayProcessed + 1; i <= today.Day; i++)
                {
                    if (i == today.Day)
                    {
                        //today has not yet ended, do not set a line limit
                        DayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                        {
                            DayOfMonth = i,
                            FilePositionInBytes = finalPositionInLogFile,
                            LinesCount = int.MaxValue
                        };
                    }
                    else
                    {
                        //this day is past, it has no lines
                        DayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                        {
                            DayOfMonth = i,
                            FilePositionInBytes = finalPositionInLogFile,
                            LinesCount = 0
                        };
                    }
                }
            }
            else
            {
                lastDayInfo.LinesCount = LineCounter;
                LineCounter = 0;
                for (int i = lastDayProcessed + 1; i <= DayCountInThisLogMonth; i++)
                {
                    //this day is past, it has no lines
                    DayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                    {
                        DayOfMonth = i,
                        FilePositionInBytes = finalPositionInLogFile,
                        LinesCount = 0
                    };
                }
            }
        }

        public HeuristicsExtractionResult GetResult()
        {
            if (!resultTaken)
            {
                if (NoDataGathered)
                {
                    throw new WurmApiException("Empty results, may indicate malformed file data. File name: " + this.logFileName);
                }
                resultTaken = true;
            }

            var heuristics = DayToHeuristicsMap.ToDictionary(pair => pair.Key, pair => pair.Value);
            return new HeuristicsExtractionResult() { LogDate = ProcessedLogDate, Heuristics = heuristics };
        }

        private void AdvanceDays(string line, long lastReadLineStartPosition)
        {
            WurmLogMonthlyFileHeuristics lastDayInfo = DayToHeuristicsMap[DayToHeuristicsMap.Keys.Max()];
            lastDayInfo.LinesCount = LineCounter - 1;
            LineCounter = 1;

            // add file positions for new days
            for (int i = PreviousDay + 1; i <= CurrentDay; i++)
            {
                DayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                {
                    DayOfMonth = i,
                    FilePositionInBytes = lastReadLineStartPosition,
                };
            }
            PreviousDay = CurrentDay;
        }

        private void AssertResultNotTaken()
        {
            if (resultTaken)
            {
                throw new InvalidOperationException("Result already taken");
            }
        }
    }
}