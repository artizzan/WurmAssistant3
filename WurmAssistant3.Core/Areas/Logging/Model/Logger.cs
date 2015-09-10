using System;
using JetBrains.Annotations;
using NLog;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Model
{
    public class Logger : ILogger, AldursLab.WurmApi.ILogger
    {
        readonly ILogMessageHandler logMessageHandler;
        readonly NLog.Logger logger;

        public Logger([NotNull] ILogMessageHandler logMessageHandler, string category = null)
        {
            if (logMessageHandler == null) throw new ArgumentNullException("logMessageHandler");
            this.logMessageHandler = logMessageHandler;
            if (category == null)
                category = string.Empty;
            logger = LogManager.GetLogger(category);
        }

        public void Log(AldursLab.WurmApi.LogLevel level, string message, object source, Exception exception)
        {
            var nlogLevel = Convert(level);
            if (logger.IsEnabled(nlogLevel))
            {
                string prefix = string.Empty;
                if (source != null)
                {
                    var stringSource = source as string;
                    if (!string.IsNullOrEmpty(stringSource))
                    {
                        prefix += "." + stringSource;
                    }
                    else
                    {
                        prefix += source.GetType().Name;
                    }
                }

                message = "WurmApi > " + prefix + ": " + message;

                logger.Log(nlogLevel, exception, message);
                logMessageHandler.HandleEvent(nlogLevel, message, exception);
            }
        }

        LogLevel Convert(AldursLab.WurmApi.LogLevel wurmApiLevel)
        {
            switch (wurmApiLevel)
            {
                case AldursLab.WurmApi.LogLevel.Diag:
                    return LogLevel.Trace;
                case AldursLab.WurmApi.LogLevel.Error:
                    return LogLevel.Error;
                case AldursLab.WurmApi.LogLevel.Fatal:
                    return LogLevel.Fatal;
                case AldursLab.WurmApi.LogLevel.Info:
                    return LogLevel.Info;
                case AldursLab.WurmApi.LogLevel.Warn:
                    return LogLevel.Warn;
                default:
                    return LogLevel.Info;
            }
        }

        public void Error(string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(message);
                logMessageHandler.HandleEvent(LogLevel.Error, message, null);
            }
        }

        public void Error(Exception exception, string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(exception, message);
                logMessageHandler.HandleEvent(LogLevel.Error, message, exception);
            }
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
                logMessageHandler.HandleEvent(LogLevel.Info, message, null);
            }
        }

        public void Info(Exception exception, string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(exception, message);
                logMessageHandler.HandleEvent(LogLevel.Info, message, exception);
            }
        }
    }
}