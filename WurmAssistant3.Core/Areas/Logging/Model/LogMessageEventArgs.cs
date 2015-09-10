using System;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    public class LogMessageEventArgs : EventArgs
    {
        public LogMessageEventArgs(NLog.LogLevel level, string message, Exception exception)
        {
            Level = level;
            Message = message;
            Exception = exception;
        }

        public NLog.LogLevel Level { get; private set; }

        [CanBeNull]
        public string Message { get; private set; }

        [CanBeNull]
        public Exception Exception { get; private set; }
    }
}