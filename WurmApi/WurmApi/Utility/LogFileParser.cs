using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Utility
{
    public class LogFileParser
    {
        private readonly ParsingHelper parsingHelper;
        private readonly ILogger logger;

        public LogFileParser(ParsingHelper parsingHelper, ILogger logger)
        {
            if (parsingHelper == null) throw new ArgumentNullException("parsingHelper");
            if (logger == null) throw new ArgumentNullException("logger");
            this.parsingHelper = parsingHelper;
            this.logger = logger;
        }

        /// <summary>
        /// Parses lines with expectation, that they all come from single day indicated by originDate.
        /// In other words, the first line MUST be the first entry on a given day.
        /// </summary>
        /// <returns></returns>
        public IList<LogEntry> ParseLinesForDay(IReadOnlyList<string> lines, DateTime originDate, LogFileInfo logFileInfo)
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
                var lineStamp = parsingHelper.TryParseTimestampFromLogLine(line);
                if (lineStamp == TimeSpan.MinValue)
                {
                    // bad timestamp may indicate corrupted file, special unforseen case 
                    // or log file from when "log timestamps" was disabled in game.
                    HandleUnparseableTimestamp(logFileInfo, line, result);
                    continue;
                }

                //if (originDate == new DateTime(2012, 8, 26) || originDate == new DateTime(2012, 8, 27))
                //{
                //    var debug = true;
                //}

                if (lineStamp < currentLineStamp)
                {
                    LogTimestampDiscrepancy(logFileInfo, result, line);
                }
                else
                {
                    currentLineStamp = lineStamp;
                }

                string source = parsingHelper.TryParseSourceFromLogLine(line);
                string content = parsingHelper.TryParseContentFromLogLine(line);

                LogEntry entry = new LogEntry()
                {
                    Timestamp = originDate + currentLineStamp,
                    Content = content,
                    Source = source
                };

                result.Add(entry);
            }

            return result;
        }

        public IList<LogEntry> ParseLinesFromLogsScan(IReadOnlyList<string> lines, DateTime linesStamp)
        {
            List<LogEntry> result = new List<LogEntry>(lines.Count);
            foreach (var line in lines)
            {
                if (IsLoggingStartedLine(line))
                {
                    continue;
                }
                string source = parsingHelper.TryParseSourceFromLogLine(line);
                string content = parsingHelper.TryParseContentFromLogLine(line);

                LogEntry entry = new LogEntry()
                {
                    Timestamp = linesStamp,
                    Content = content,
                    Source = source
                };
                result.Add(entry);
            }
            return result;
        }

        private bool IsLoggingStartedLine(string line)
        {
            return parsingHelper.IsLoggingStartedLine(line);
        }

        private void HandleUnparseableTimestamp(LogFileInfo logFileInfo, string line, List<LogEntry> result)
        {
            Log(
                string.Format(
                    "Parsing timestamp from log line was not possible. Appending contents to previous logged line. Line contents: {0}",
                    line),
                logFileInfo);

            LogEntry lastEntry = result.LastOrDefault();
            if (lastEntry != null)
            {
                lastEntry.Content += line;
            }
            else
            {
                Log("Appending impossible, no entries parsed yet.", logFileInfo);
            }
        }

        private void LogTimestampDiscrepancy(LogFileInfo logFileInfo, List<LogEntry> result, string line)
        {
            string lastLineContents;
            LogEntry lastEntry = result.LastOrDefault();
            if (lastEntry != null)
            {
                lastLineContents = lastEntry.ToString();
            }
            else
            {
                lastLineContents = "No entries parsed yet";
            }

            Log(
                string.Format(
                    "Parsed line has earlier timestamp, than last parsed line. Overriding this timestamp with stamp of last parsed line. Line contents: [{0}], Last entry data: [{1}]",
                    line,
                    lastLineContents),
                logFileInfo);
        }

        private static void AssertOriginDate(DateTime originDate)
        {
            DateTime validationDate = new DateTime(originDate.Year, originDate.Month, originDate.Day);
            if (validationDate != originDate)
            {
                throw new InvalidOperationException(string.Format("originDate must start at midnight, actual: {0}", originDate));
            }
        }

        private void Log(string message, LogFileInfo logFileInfo)
        {
            logger.Log(LogLevel.Warn, string.Format("{0}, Log file: [{1}]", message, logFileInfo.FullPath), this, null);
        }
    }

    public class LogFileParserFactory
    {
        private readonly ParsingHelper parsingHelper;
        private readonly ILogger logger;

        public LogFileParserFactory(ParsingHelper parsingHelper, ILogger logger)
        {
            if (parsingHelper == null) throw new ArgumentNullException("parsingHelper");
            if (logger == null) throw new ArgumentNullException("logger");
            this.parsingHelper = parsingHelper;
            this.logger = logger;
        }

        public LogFileParser Create()
        {
            return new LogFileParser(parsingHelper, logger);
        }
    }
}
