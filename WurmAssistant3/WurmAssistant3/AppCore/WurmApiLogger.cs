using System;

using AldurSoft.WurmApi;

using NLog;

using LogLevel = NLog.LogLevel;

namespace AldurSoft.WurmAssistant3.AppCore
{
    class WurmApiLogger : ILogger
    {
        private readonly Logger logger;

        public WurmApiLogger(string category)
        {
            logger = LogManager.GetLogger(category);
        }

        public void Log(WurmApi.LogLevel level, string message, object source)
        {
            logger.Log(Convert(level), "{0}:{1}", message, source);
        }

        public void Log(WurmApi.LogLevel level, string message, Exception exception, object source)
        {
            var lvl = Convert(level);
            if (logger.IsEnabled(lvl))
            {
                logger.Log(lvl, string.Format("{0}:{1}", message, source), exception);
            }
        }

        private LogLevel Convert(WurmApi.LogLevel apiLevel)
        {
            switch (apiLevel)
            {
                case WurmApi.LogLevel.Diag:
                    return LogLevel.Debug;
                case WurmApi.LogLevel.Error:
                    return LogLevel.Error;
                case WurmApi.LogLevel.Fatal:
                    return LogLevel.Fatal;
                case WurmApi.LogLevel.Info:
                    return LogLevel.Info;
                case WurmApi.LogLevel.Warn:
                    return LogLevel.Warn;
                default:
                    logger.Error("Unknown logging level {0}, logging as Info", apiLevel);
                    return LogLevel.Info;
            }
        }
    }
}