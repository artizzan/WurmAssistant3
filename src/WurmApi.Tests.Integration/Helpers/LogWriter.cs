using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AldursLab.WurmApi.Tests.Integration.Helpers
{
    class LogWriter
    {
        readonly DateTime logDate;
        readonly bool isMonthly;
        readonly string logFilePath;

        public LogWriter(string logFilePath, DateTime logDate, bool isMonthly)
        {
            this.logDate = logDate;
            this.isMonthly = isMonthly;
            if (logFilePath == null) throw new ArgumentNullException(nameof(logFilePath));
            this.logFilePath = logFilePath;

            string directoryPath = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        public void WriteSection(
            ICollection<LogEntry> contents,
            bool addLoggingStarted = false)
        {
            if (contents == null) throw new ArgumentNullException(nameof(contents));
            if (!contents.Any()) throw new ArgumentException("contents empty");

            if (addLoggingStarted)
            {
                WriteLoggingStarted(contents.First().Timestamp);
            }

            List<string> lines = new List<string>();
            foreach (var logEntry in contents)
            {
                ValidateEntry(logEntry);
                var t = "[" + logEntry.Timestamp.ToString("HH:mm:ss") + "] ";
                var src = !string.IsNullOrEmpty(logEntry.Source) ? "<" + logEntry.Source + "> " : string.Empty;
                var line = string.Format("{0}{1}{2}", t, src, logEntry.Content);
                lines.Add(line);
            }
            WriteText(lines.ToArray());
        }

        private void ValidateEntry(LogEntry logEntry)
        {
            if (isMonthly && logEntry.Timestamp.Year != logDate.Year && logEntry.Timestamp.Month != logDate.Month)
            {
                throw new InvalidOperationException("timestamp datepart mismatch for file " + logFilePath);
            }
            else if (!isMonthly && logEntry.Timestamp.Date != logDate.Date)
            {
                throw new InvalidOperationException("timestamp datepart mismatch for file " + logFilePath);
            }
        }

        private void WriteLoggingStarted(DateTime timestamp)
        {
            WriteText(string.Format("Logging started {0}", timestamp.ToString("yyyy-MM-dd")));
        }

        private void WriteText(params string[] contents)
        {
            File.AppendAllLines(logFilePath, contents);
        }
    }
}
