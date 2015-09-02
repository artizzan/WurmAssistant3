using System;
using JetBrains.Annotations;
using NLog;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Infrastructure.Logging
{
    public class Logger : ILogger, WurmApi.ILogger
    {
        readonly ILogMessageProcessor logMessageProcessor;
        readonly NLog.Logger logger;

        public Logger([NotNull] ILogMessageProcessor logMessageProcessor, string category = null)
        {
            if (logMessageProcessor == null) throw new ArgumentNullException("logMessageProcessor");
            this.logMessageProcessor = logMessageProcessor;
            if (category == null)
                category = string.Empty;
            logger = LogManager.GetLogger(category);
        }

        public void Log(WurmApi.LogLevel level, string message, object source, Exception exception)
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
                logMessageProcessor.Handle(nlogLevel, message, exception);
            }
        }

        LogLevel Convert(WurmApi.LogLevel wurmApiLevel)
        {
            switch (wurmApiLevel)
            {
                case WurmApi.LogLevel.Diag:
                    return LogLevel.Trace;
                case WurmApi.LogLevel.Error:
                    return LogLevel.Error;
                case WurmApi.LogLevel.Fatal:
                    return LogLevel.Fatal;
                case WurmApi.LogLevel.Info:
                    return LogLevel.Info;
                case WurmApi.LogLevel.Warn:
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
                logMessageProcessor.Handle(LogLevel.Error, message, null);
            }
        }

        public void Error(Exception exception, string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(exception, message);
                logMessageProcessor.Handle(LogLevel.Error, message, exception);
            }
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
                logMessageProcessor.Handle(LogLevel.Info, message, null);
            }
        }

        public void Info(Exception exception, string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(exception, message);
                logMessageProcessor.Handle(LogLevel.Info, message, exception);
            }
        }
    }
}