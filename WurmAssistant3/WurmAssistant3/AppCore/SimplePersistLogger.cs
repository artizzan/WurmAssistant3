using AldurSoft.SimplePersist;

using NLog;

using LogLevel = NLog.LogLevel;

namespace AldurSoft.WurmAssistant3.AppCore
{
    class SimplePersistLogger : ISimplePersistLogger
    {
        private readonly Logger logger;

        public SimplePersistLogger(string category)
        {
            logger = LogManager.GetLogger(category);
        }

        public void Log(string message, Severity severity)
        {
            switch (severity)
            {
                case Severity.Debug:
                    logger.Debug(message);
                    break;
                case Severity.Error:
                    logger.Error(message);
                    break;
                case Severity.Warning:
                    logger.Warn(message);
                    break;
                default:
                    logger.Log(
                        LogLevel.Error,
                        () =>
                        string.Format("Logged event with unknown Severity of {0}, message: {1}.", severity, message));
                    break;
            }
        }
    }
}
