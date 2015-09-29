using System;
using AldursLab.Essentials.Eventing;
using AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts;
using JetBrains.Annotations;
using ILogger = AldursLab.WurmAssistant3.Core.Areas.Logging.Contracts.ILogger;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Core.Areas.Logging.Modules
{
    public class LoggingManager : ILoggerFactory, IWurmApiLoggerFactory, ILogMessageFlow, ILogMessageHandler, ILoggingConfig
    {
        readonly IThreadMarshaller threadMarshaller;
        readonly Logger factoryLogger;

        LoggingConfig loggingConfig;

        public LoggingManager([NotNull] IThreadMarshaller threadMarshaller)
        {
            if (threadMarshaller == null) throw new ArgumentNullException("threadMarshaller");
            this.threadMarshaller = threadMarshaller;
            factoryLogger = new Logger(this, "LoggingManager");
            loggingConfig = new LoggingConfig();
        }

        public void Setup(string logOutputDirFullPath)
        {
            loggingConfig.Setup(logOutputDirFullPath);
        }

        public int ErrorCount { get; private set; }
        public int WarnCount { get; private set; }
        public event EventHandler<EventArgs> ErrorCountChanged;
        public event EventHandler<LogMessageEventArgs> EventLogged;

        ILogger ILoggerFactory.Create(string category)
        {
            return new Logger(this, category);
        }

        AldursLab.WurmApi.ILogger IWurmApiLoggerFactory.Create()
        {
            return new Logger(this, "WurmApi");
        }

        public void HandleEvent(LogLevel nlogLevel, string message, Exception exception, string category)
        {
            if (nlogLevel == LogLevel.Error || nlogLevel == LogLevel.Fatal || nlogLevel == LogLevel.Warn)
            {
                if (nlogLevel == LogLevel.Warn) WarnCount++; else ErrorCount++;

                try
                {
                    OnErrorCountChanged();
                }
                catch (Exception handlingException)
                {
                    // calling logger directly to avoid enless loop of this error
                    factoryLogger.Error(handlingException, "Error at OnErrorCountChanged");
                }
            }

            var handler = EventLogged;
            if (handler != null)
            {
                var args = new LogMessageEventArgs(nlogLevel, message, exception, category);
                try
                {
                    threadMarshaller.Marshal(() => handler(this, args));
                }
                catch (Exception handlingException)
                {
                    // calling logger directly to avoid endless loop of this error
                    factoryLogger.Error(handlingException, "Error during log event forwarding");
                }
            }
        }

        protected virtual void OnErrorCountChanged()
        {
            var handler = ErrorCountChanged;
            if (handler != null)
            {
                threadMarshaller.Marshal(() =>
                    handler(this, EventArgs.Empty));
            }
        }

        public string GetCurrentReadableLogFileFullPath()
        {
            return loggingConfig.GetCurrentReadableLogFileFullPath();
        }

        public string GetCurrentVerboseLogFileFullPath()
        {
            return loggingConfig.GetCurrentVerboseLogFileFullPath();
        }
    }
}
