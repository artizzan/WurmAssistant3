using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldurSoft.WurmApi.Tests.Helpers
{
    class LogWriter
    {
        private readonly DateTime logDate;
        private readonly bool isMonthly;
        private readonly string logFilePath;

        public LogWriter(string logFilePath, DateTime logDate, bool isMonthly)
        {
            this.logDate = logDate;
            this.isMonthly = isMonthly;
            if (logFilePath == null) throw new ArgumentNullException("logFilePath");
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
            if (contents == null) throw new ArgumentNullException("contents");
            if (!contents.Any()) throw new ArgumentException("contents empty");

            if (addLoggingStarted)
            {
                WriteLoggingStarted(contents.First().Timestamp);
            }

            List<string> lines = new List<string>();
            foreach (var logEntry in contents)
            {
                ValidateEntry(logEntry);
                var t = "[" + logEntry.Timestamp.ToString("hh:MM:ss") + "] ";
                var src = logEntry.Source != null ? "<" + logEntry.Source + "> " : string.Empty;
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
