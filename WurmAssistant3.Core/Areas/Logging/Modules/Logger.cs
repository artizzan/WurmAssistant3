using System;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;
using NLog;
using ILogger = AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts.ILogger;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Modules
{
    public class Logger : ILogger, AldursLab.WurmApi.IWurmApiLogger
    {
        readonly ILogMessageHandler logMessageHandler;
        readonly string category;
        readonly NLog.Logger logger;

        public Logger([NotNull] ILogMessageHandler logMessageHandler, string category = null)
        {
            if (logMessageHandler == null) throw new ArgumentNullException("logMessageHandler");
            this.logMessageHandler = logMessageHandler;
            if (category == null)
                category = string.Empty;
            this.category = category;
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
                logMessageHandler.HandleEvent(nlogLevel, message, exception, category);
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
                logMessageHandler.HandleEvent(LogLevel.Error, message, null, category);
            }
        }

        public void Error(Exception exception, string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(exception, message);
                logMessageHandler.HandleEvent(LogLevel.Error, message, exception, category);
            }
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
                logMessageHandler.HandleEvent(LogLevel.Info, message, null, category);
            }
        }

        public void Info(Exception exception, string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(exception, message);
                logMessageHandler.HandleEvent(LogLevel.Info, message, exception, category);
            }
        }

        public void Warn(string message)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message);
                logMessageHandler.HandleEvent(LogLevel.Warn, message, null, category);
            }
        }

        public void Warn(Exception exception, string message)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(exception, message);
                logMessageHandler.HandleEvent(LogLevel.Warn, message, exception, category);
            }
        }

        public void Debug(string message)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
                logMessageHandler.HandleEvent(LogLevel.Debug, message, null, category);
            }
        }
    }
}