using System;
using JetBrains.Annotations;
using NLog;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    public class Logger : ILogger, AldursLab.WurmApi.IWurmApiLogger
    {
        readonly ILogMessageDump logMessageDump;
        readonly string category;
        readonly NLog.Logger logger;

        public Logger([NotNull] ILogMessageDump logMessageDump, string category = null)
        {
            if (logMessageDump == null) throw new ArgumentNullException(nameof(logMessageDump));
            this.logMessageDump = logMessageDump;
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
                logMessageDump.HandleEvent(nlogLevel, message, exception, category);
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
                logMessageDump.HandleEvent(LogLevel.Error, message, null, category);
            }
        }

        public void Error(Exception exception, string message)
        {
            if (logger.IsErrorEnabled)
            {
                logger.Error(exception, message);
                logMessageDump.HandleEvent(LogLevel.Error, message, exception, category);
            }
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
                logMessageDump.HandleEvent(LogLevel.Info, message, null, category);
            }
        }

        public void Info(Exception exception, string message)
        {
            if (logger.IsInfoEnabled)
            {
                logger.Info(exception, message);
                logMessageDump.HandleEvent(LogLevel.Info, message, exception, category);
            }
        }

        public void Warn(string message)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message);
                logMessageDump.HandleEvent(LogLevel.Warn, message, null, category);
            }
        }

        public void Warn(Exception exception, string message)
        {
            if (logger.IsWarnEnabled)
            {
                logger.Warn(exception, message);
                logMessageDump.HandleEvent(LogLevel.Warn, message, exception, category);
            }
        }

        public void Debug(string message)
        {
            if (logger.IsDebugEnabled)
            {
                logger.Debug(message);
                logMessageDump.HandleEvent(LogLevel.Debug, message, null, category);
            }
        }
    }
}