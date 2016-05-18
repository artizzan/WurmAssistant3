using System;
using JetBrains.Annotations;
using NLog;

namespace AldursLab.WurmAssistant3.Areas.Logging.Contracts
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogMessageEventArgs(LogLevel level, string message, Exception exception, string category)
        {
            Level = level;
            Message = message ?? string.Empty;
            Exception = exception;
            Category = category;
        }

        public DateTime Timestamp { get; } = DateTime.Now;

        public NLog.LogLevel Level { get; private set; }

        public string Message { get; private set; }

        [CanBeNull]
        public Exception Exception { get; private set; }

        public string Category { get; private set; }
    }
}