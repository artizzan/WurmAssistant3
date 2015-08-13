using System;
using NLog;

namespace AldursLab.WurmAssistant3.Core.Logging
{
    public interface ILogger
    {
        void Error(string message);
        void Error(Exception exception, string message);
        void Info(string message);
        void Info(Exception exception, string message);
    }

    public class Logger : ILogger, WurmApi.ILogger
    {
        readonly NLog.Logger logger;

        public Logger(string category = null)
        {
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
                    if (stringSource != null)
                    {
                        prefix += "." + stringSource;
                    }
                    else
                    {
                        prefix += "." + source.GetType().Name;
                    }
                }

                logger.Log(nlogLevel, exception, prefix + ": " + message);
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
            }
        }

        public void Error(Exception exception, string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(exception, message);
            }
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
            }
        }

        public void Info(Exception exception, string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(exception, message);
            }
        }
    }
}