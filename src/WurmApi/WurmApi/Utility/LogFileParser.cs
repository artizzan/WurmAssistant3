using System;
using System.Collections.Generic;
using System.Linq;

namespace AldursLab.WurmApi.Utility
{
    class LogFileParser
    {
        private readonly IWurmApiLogger logger;

        public LogFileParser(IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }

        /// <summary>
        /// Parses lines with expectation, that they all come from single day indicated by originDate.
        /// In other words, the first line MUST be the first entry on a given day.
        /// </summary>
        /// <returns></returns>
        public IList<LogEntry> ParseLinesForDay(IReadOnlyList<string> lines, DateTime originDate,
            LogFileInfo logFileInfo)
        {
            AssertOriginDate(originDate);

            List<LogEntry> result = new List<LogEntry>(lines.Count);
            TimeSpan currentLineStamp = TimeSpan.Zero;
            foreach (var line in lines)
            {
                // handle special types of lines
                if (IsLoggingStartedLine(line))
                {
                    // skip, is of no concern at this point
                    continue;
                }

                // handle timestamp
                var lineStamp = ParsingHelper.TryParseTimestampFromLogLine(line);
                if (lineStamp == TimeSpan.MinValue)
                {
                    // maybe just empty line (eg. happens on examining signs)
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    // bad timestamp may indicate corrupted file, special unforseen case 
                    // or log file from when "log timestamps" was disabled in game.
                    HandleUnparseableTimestamp(logFileInfo, line, result);
                    continue;
                }

                if (lineStamp < currentLineStamp)
                {
                    LogTimestampDiscrepancy(logFileInfo, result, line);
                }
                else
                {
                    currentLineStamp = lineStamp;
                }

                string source = ParsingHelper.TryParseSourceFromLogLine(line);
                string content = ParsingHelper.TryParseContentFromLogLine(line);

                LogEntry entry = new LogEntry(originDate + currentLineStamp, source, content, logFileInfo.PmRecipientNormalized);

                result.Add(entry);
            }

            return result;
        }

        public IList<LogEntry> ParseLinesFromLogsScan(IReadOnlyList<string> lines, DateTime dayStamp)
        {
            List<LogEntry> result = new List<LogEntry>(lines.Count);
            DateTime? previousStamp = null;
            foreach (var line in lines)
            {
                if (IsLoggingStartedLine(line))
                {
                    continue;
                }
                // maybe just empty line (eg. happens on examining signs)
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                TimeSpan? parsedTime = null;
                DateTime? currentStamp = null;
                try
                {
                    parsedTime = ParsingHelper.GetTimestampFromLogLine(line);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Warn, "Found log line with unparseable timestamp, line may be ignored.", this, exception);
                }
                
                string source = ParsingHelper.TryParseSourceFromLogLine(line);
                string content = ParsingHelper.TryParseContentFromLogLine(line);
                if (parsedTime.HasValue)
                {
                    currentStamp = new DateTime(dayStamp.Year,
                        dayStamp.Month,
                        dayStamp.Day,
                        parsedTime.Value.Hours,
                        parsedTime.Value.Minutes,
                        parsedTime.Value.Seconds);
                }
                else if (previousStamp.HasValue)
                {
                    currentStamp = previousStamp.Value;
                }

                if (currentStamp.HasValue)
                {
                    previousStamp = currentStamp;

                    LogEntry entry = new LogEntry(currentStamp.Value, source, content);
                    result.Add(entry);
                }
            }
            return result;
        }

        private bool IsLoggingStartedLine(string line)
        {
            return ParsingHelper.IsLoggingStartedLine(line);
        }

        private void HandleUnparseableTimestamp(LogFileInfo logFileInfo, string line, List<LogEntry> result)
        {
            Log(
                string.Format(
                    "Parsing timestamp from log line was not possible. Appending contents to previous logged line. Line contents: {0}",
                    line),
                logFileInfo);

            if (result.Any())
            {
                var lastIndex = result.Count - 1;
                var oldEntry = result[lastIndex];
                result[lastIndex] = new LogEntry(oldEntry.Timestamp, oldEntry.Source, oldEntry.Content + line);
            }
            else
            {
                Log("Appending impossible, no entries parsed yet.", logFileInfo);
            }
        }

        void LogTimestampDiscrepancy(LogFileInfo logFileInfo, List<LogEntry> result, string line)
        {
            var lastEntry = result.LastOrDefault();
            var lastLineContents = lastEntry?.ToString() ?? "No entries parsed yet";

            Log(
                string.Format(
                    "Parsed line has earlier timestamp, compared to last parsed line. Overriding this timestamp with stamp of last parsed line. Line contents: [{0}], Last entry data: [{1}]",
                    line,
                    lastLineContents),
                logFileInfo, 
                LogLevel.Info);
        }

        static void AssertOriginDate(DateTime originDate)
        {
            var validationDate = new DateTime(originDate.Year, originDate.Month, originDate.Day);
            if (validationDate != originDate)
            {
                throw new InvalidOperationException(string.Format("originDate must start at midnight, actual: {0}", originDate));
            }
        }

        void Log(string message, LogFileInfo logFileInfo, LogLevel level = LogLevel.Warn)
        {
            logger.Log(level, string.Format("{0}, Log file: [{1}]", message, logFileInfo.FullPath), this, null);
        }
    }

    class LogFileParserFactory
    {
        readonly IWurmApiLogger logger;

        public LogFileParserFactory(IWurmApiLogger logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }

        public LogFileParser Create()
        {
            return new LogFileParser(logger);
        }
    }
}
