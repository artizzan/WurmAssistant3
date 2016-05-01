using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;
using AldursLab.WurmApi.Utility;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.MonthlyDataBuilders
{
    class DataBuilderV2 : IMonthlyHeuristicsDataBuilder
    {
        private readonly string logFileName;
        private readonly DateTime today;
        private readonly IWurmApiLogger logger;

        private DateTime ProcessedLogDate { get; set; }
        private bool ThisLogIsForCurrentMonth { get; set; }
        private int DayCountInThisLogMonth { get; set; }
        private TimeSpan LastLogLineStamp { get; set; }

        readonly List<Record> records = new List<Record>();
        private long finalPositionInLogFile;

        public DataBuilderV2(string logFileName, DateTime today, IWurmApiLogger logger)
        {
            if (logFileName == null)
                throw new ArgumentNullException(nameof(logFileName));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.logFileName = logFileName;
            this.today = today;
            this.logger = logger;

            var logDate = ParsingHelper.GetDateFromLogFileName(logFileName);
            if (logDate.LogSavingType != LogSavingType.Monthly)
            {
                throw new WurmApiException("This builder can only be used for monthly log files, actual file name: " + logFileName);
            }

            ProcessedLogDate = logDate.DateTime;
            DayCountInThisLogMonth = ParsingHelper.GetDaysInMonthForLogFile(logFileName);
            ThisLogIsForCurrentMonth = ProcessedLogDate.Year == today.Year && ProcessedLogDate.Month == today.Month;
        }

        public void ProcessLine(string line, long lastReadLineStartPosition)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    var currentDay = records.Any() ? records.Last().Day : 0;
                    records.Add(new Record(currentDay, lastReadLineStartPosition));
                }
                else if (line.StartsWith("Logging started", StringComparison.Ordinal))
                {
                    var lastDay = records.Any() ? records.Last().Day : 0;
                    var dayStamp = ParsingHelper.GetDateFromLogFileLoggingStarted(line);

                    ReadjustPreviousRecords(lastDay, dayStamp);

                    records.Add(new Record(dayStamp.Day, lastReadLineStartPosition));
                    LastLogLineStamp = TimeSpan.Zero;
                }
                else
                {
                    var currentDay = records.Any() ? records.Last().Day : 0;
                    var lineStamp = ParsingHelper.GetTimestampFromLogLine(line);

                    if (OverflowsToNextDay(lineStamp))
                    {
                        LastLogLineStamp = TimeSpan.Zero;
                        if (!(OverflowsBeyondToday(currentDay + 1) || OverflowsBeyondMaxMonth(currentDay + 1)))
                        {
                            currentDay++;
                        }
                    }
                    else
                    {
                        LastLogLineStamp = lineStamp;
                    }

                    records.Add(new Record(currentDay, lastReadLineStartPosition));
                }
            }
            catch (WurmApiException exception)
            {
                // this line must be added to records, because lastReadLineStartPosition 
                var currentDay = records.Any() ? records.Last().Day : 0;
                records.Add(new Record(currentDay, lastReadLineStartPosition));
                // ignore this line
                logger.Log(
                    LogLevel.Warn,
                    string.Format("Unexpected exception while parsing line: {0}", line),
                    this,
                    exception);
            }
        }

        private bool OverflowsBeyondToday(int day)
        {
            return ThisLogIsForCurrentMonth && day > today.Day;
        }

        private bool OverflowsBeyondMaxMonth(int currentDay)
        {
            return currentDay > DayCountInThisLogMonth;
        }

        private bool OverflowsToNextDay(TimeSpan lineStamp)
        {
            return lineStamp < LastLogLineStamp
                   && ParsingHelper.AreMoreThanOneHourAppartOnSameDay(lineStamp, LastLogLineStamp);
        }

        private void ReadjustPreviousRecords(int lastDay, DateTime dayStamp)
        {
            if (lastDay > dayStamp.Day)
            {
                for (int i = records.Count - 1; i >= 0; i--)
                {
                    var iRecord = records[i];
                    if (iRecord.Day > dayStamp.Day)
                    {
                        records[i] = new Record(dayStamp.Day, iRecord.FilePositionInBytes);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void Complete(long finalPositionInLogFile)
        {
            this.finalPositionInLogFile = finalPositionInLogFile;

            var lastDay = records.Any() ? records.Last().Day : 0;
            if (ThisLogIsForCurrentMonth && lastDay < today.Day)
            {
                records.Add(new Record(today.Day, finalPositionInLogFile));
            }
        }

        public HeuristicsExtractionResult GetResult()
        {
            if (!records.Any() || records.All(record => record.Day == 0))
            {
                throw new WurmApiException("Empty results, may indicate malformed file data. File name: " + logFileName);
            }

            var dayToHeuristicsMap = new Dictionary<int, WurmLogMonthlyFileHeuristics>();

            FillDaysBeforeFirstFoundDay(dayToHeuristicsMap);
            FillDaysBasedOnRecords(dayToHeuristicsMap);
            FillDaysAfterLastFoundDay(dayToHeuristicsMap);

            return new HeuristicsExtractionResult() { LogDate = ProcessedLogDate, Heuristics = dayToHeuristicsMap };
        }

        private void FillDaysBeforeFirstFoundDay(Dictionary<int, WurmLogMonthlyFileHeuristics> dayToHeuristicsMap)
        {
            var firstDay = records.First().Day;
            for (int i = 1; i < firstDay; i++)
            {
                dayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                {
                    DayOfMonth = i,
                    FilePositionInBytes = 0,
                    LinesCount = 0
                };
            }
        }

        private void FillDaysBasedOnRecords(Dictionary<int, WurmLogMonthlyFileHeuristics> dayToHeuristicsMap)
        {
            var day = records.First().Day;
            var maxDay = CalculateMaxDay();
            int lineCounter = 0;
            long dayBeginFilePosition = 0;

            foreach (var record in records)
            {
                if (record.Day > day)
                {
                    FinalizeCurrentDay(dayToHeuristicsMap, day, lineCounter, dayBeginFilePosition);
                    FillGaps(dayToHeuristicsMap, day, record.Day, record.FilePositionInBytes);

                    dayBeginFilePosition = record.FilePositionInBytes;
                    day = record.Day;
                    lineCounter = 0;
                }
                lineCounter++;
            }

            FinalizeCurrentDay(dayToHeuristicsMap, day, lineCounter, dayBeginFilePosition, true);

        }

        private int CalculateMaxDay()
        {
            if (ThisLogIsForCurrentMonth)
            {
                return today.Day;
            }
            else
            {
                return DayCountInThisLogMonth;
            }
        }

        private void FinalizeCurrentDay(
            Dictionary<int, WurmLogMonthlyFileHeuristics> dayToHeuristicsMap,
            int day,
            int lineCounter,
            long dayBeginFilePosition,
            bool endOfFile = false)
        {
            if (endOfFile)
            {
                if (ThisLogIsForCurrentMonth && day == today.Day)
                {
                    lineCounter = int.MaxValue;
                }
            }

            dayToHeuristicsMap[day] = new WurmLogMonthlyFileHeuristics()
            {
                DayOfMonth = day,
                FilePositionInBytes = dayBeginFilePosition,
                LinesCount = lineCounter
            };
        }

        private void FillGaps(Dictionary<int, WurmLogMonthlyFileHeuristics> dayToHeuristicsMap, int lastDay, int dayNow, long filePositionInBytes)
        {
            for (int i = lastDay + 1; i < dayNow; i++)
            {
                dayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                {
                    DayOfMonth = i,
                    FilePositionInBytes = filePositionInBytes,
                    LinesCount = 0
                };
            }
        }

        private void FillDaysAfterLastFoundDay(Dictionary<int, WurmLogMonthlyFileHeuristics> dayToHeuristicsMap)
        {
            var lastRecordDay = records.Last().Day;
            int maxDay = DayCountInThisLogMonth;

            if (ThisLogIsForCurrentMonth)
            {
                maxDay = today.Day;
            }

            for (int i = lastRecordDay + 1; i <= maxDay; i++)
            {
                dayToHeuristicsMap[i] = new WurmLogMonthlyFileHeuristics()
                {
                    DayOfMonth = i,
                    FilePositionInBytes = finalPositionInLogFile,
                    LinesCount = 0
                };
            }
        }
    }

    struct Record
    {
        public readonly int Day;
        public readonly long FilePositionInBytes;

        public Record(int day, long filePositionInBytes)
            : this()
        {
            Day = day;
            FilePositionInBytes = filePositionInBytes;
        }
    }
}