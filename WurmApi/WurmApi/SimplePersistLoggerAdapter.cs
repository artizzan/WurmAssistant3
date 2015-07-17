using System;

using AldurSoft.SimplePersist;

namespace AldurSoft.WurmApi
{
    class SimplePersistLoggerAdapter : ISimplePersistLogger
    {
        private readonly ILogger logger;

        public SimplePersistLoggerAdapter(ILogger logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            this.logger = logger;
        }

        public void Log(string message, Severity severity)
        {
            logger.Log(GetLogLevel(severity), message, "WurmApi");
        }

        private LogLevel GetLogLevel(Severity severity)
        {
            switch (severity)
            {
                case Severity.Debug:
                    return LogLevel.Diag;
                case Severity.Error:
                    return LogLevel.Error;
                case Severity.Warning:
                    return LogLevel.Warn;
                default:
                    throw new NotImplementedException("Translation not available for severity: " + severity);
            }
        }
    }
}