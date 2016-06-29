using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using AldursLab.Essentials.Eventing;
using AldursLab.Essentials.Extensions.DotNet.Collections.Concurrent;
using AldursLab.WurmAssistant3.Areas.Core;
using JetBrains.Annotations;
using ILogger = AldursLab.WurmAssistant3.Areas.Logging.ILogger;
using LogLevel = NLog.LogLevel;

namespace AldursLab.WurmAssistant3.Areas.Logging
{
    [KernelBind(BindingHint.Singleton)]
    public sealed class LoggingManager : ILoggerFactory, IWurmApiLoggerFactory, ILogMessageSteam, ILogMessageDump, ILoggingConfig
    {
        readonly IThreadMarshaller threadMarshaller;
        readonly Logger factoryLogger;

        readonly LoggingConfig loggingConfig;
        readonly ConcurrentQueue<LogMessageEventArgs> missedEvents = new ConcurrentQueue<LogMessageEventArgs>();

        public LoggingManager([NotNull] IThreadMarshaller threadMarshaller,
            [NotNull] IWurmAssistantDataDirectory wurmAssistantDataDirectory)
        {
            if (threadMarshaller == null) throw new ArgumentNullException(nameof(threadMarshaller));
            if (wurmAssistantDataDirectory == null) throw new ArgumentNullException(nameof(wurmAssistantDataDirectory));
            this.threadMarshaller = threadMarshaller;

            var logOutputDirFullPath = Path.Combine(wurmAssistantDataDirectory.DirectoryPath, "Logs");
            loggingConfig = new LoggingConfig(logOutputDirFullPath);

            factoryLogger = new Logger(this, "LoggingManager");
        }

        public int ErrorCount { get; private set; }
        public int WarnCount { get; private set; }
        public event EventHandler<EventArgs> ErrorCountChanged;
        public event EventHandler<LogMessageEventArgs> EventLogged;

        public IEnumerable<LogMessageEventArgs> ConsumeMissedMessages()
        {
            return missedEvents.DequeueAll();
        }

        ILogger ILoggerFactory.Create(string category)
        {
            return new Logger(this, category);
        }

        AldursLab.WurmApi.IWurmApiLogger IWurmApiLoggerFactory.Create()
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

            var args = new LogMessageEventArgs(nlogLevel, message, exception, category);

            var handler = EventLogged;
            if (handler != null)
            {
                
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
            else
            {
                // necessary because there is a small delay before LogView is resolved.
                AddToMissedEvents(args);
            }
        }

        void AddToMissedEvents(LogMessageEventArgs args)
        {
            // avoid accidental "memory leak"
            if (missedEvents.Count > 50000)
            {
                LogMessageEventArgs throwaway;
                missedEvents.TryDequeue(out throwaway);
            }
            missedEvents.Enqueue(args);
        }

        void OnErrorCountChanged()
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

        public string LogsDirectoryFullPath => loggingConfig.LogsDirectoryFullPath;
    }
}
