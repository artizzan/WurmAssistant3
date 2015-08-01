using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.Essentials;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class CharacterLogsMonitorEngine : IHandle<CharacterLogFilesAddedOrRemoved>
    {
        readonly CharacterName characterName;
        readonly ILogger logger;
        readonly SingleFileMonitorFactory singleFileMonitorFactory;
        readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        private DateTimeOffset lastRefresh;

        readonly Dictionary<string, SingleFileMonitor> monitors = new Dictionary<string, SingleFileMonitor>();
        readonly HashSet<string> loggedUnknownFiles = new HashSet<string>();

        readonly object locker = new object();
        volatile bool initialRebuild = true;

        public CharacterLogsMonitorEngine(
            [NotNull] CharacterName characterName,
            [NotNull] ILogger logger,
            [NotNull] SingleFileMonitorFactory singleFileMonitorFactory,
            [NotNull] IWurmCharacterLogFiles wurmCharacterLogFiles, 
            [NotNull] IInternalEventAggregator internalEventAggregator)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (logger == null) throw new ArgumentNullException("logger");
            if (singleFileMonitorFactory == null) throw new ArgumentNullException("singleFileMonitorFactory");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            if (internalEventAggregator == null) throw new ArgumentNullException("internalEventAggregator");
            this.characterName = characterName;
            this.logger = logger;
            this.singleFileMonitorFactory = singleFileMonitorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;

            lastRefresh = Time.Get.LocalNowOffset;

            internalEventAggregator.Subscribe(this);

            lock (locker)
            {
                RebuildAllMonitors();
            }
        }

        public void Handle(CharacterLogFilesAddedOrRemoved message)
        {
            if (message.CharacterName == characterName)
            {
                lock (locker)
                {
                    RebuildAllMonitors();
                }
            }
        }

        internal IEnumerable<MonitorEvents> RefreshAndGetNewEvents()
        {
            lock (locker)
            {
                var currentDate = Time.Get.LocalNowOffset;
                CheckForLargeRefreshDelay(currentDate);

                List<MonitorEvents> eventsList = new List<MonitorEvents>();
                foreach (var monitor in monitors.Values)
                {
                    var newEvents = monitor.GetNewEvents();
                    if (newEvents.Count > 0)
                    {
                        var container = new MonitorEvents(monitor.LogFileInfo);
                        container.AddEvents(newEvents);
                        eventsList.Add(container);
                    }
                }

                lastRefresh = currentDate;

                return eventsList;
            }
        }

        void RebuildAllMonitors()
        {
            var ir = initialRebuild;
            var timeNow = Time.Get.LocalNow;
            RemoveOldMonitors(timeNow);
            var files = wurmCharacterLogFiles.GetLogFiles(timeNow, timeNow);
            foreach (var file in files)
            {
                if (ShouldFileBeMonitored(file, timeNow))
                {
                    if (!monitors.ContainsKey(file.FullPath))
                    {
                        SingleFileMonitor monitor;
                        // when initializing engine, we ignore events already in files
                        // when adding engines for new files, we want to always read from start of file
                        if (ir)
                        {
                            monitor = singleFileMonitorFactory.Create(file);
                            initialRebuild = false;
                        }
                        else
                        {
                            monitor = singleFileMonitorFactory.Create(file, 0L);
                        }

                        monitors.Add(file.FullPath, monitor);
                    }
                }
            }
        }

        private void RemoveOldMonitors(DateTime now)
        {
            foreach (var monitor in monitors.Values.ToArray())
            {
                if (!ShouldFileBeMonitored(monitor.LogFileInfo, now))
                {
                    monitors.Remove(monitor.LogFileInfo.FullPath);
                }
            }
        }

        private bool ShouldFileBeMonitored(LogFileInfo file, DateTime localNow)
        {
            if (file.LogFileDate.LogSavingType == LogSavingType.Daily)
            {
                return (file.LogFileDate.DateTime.Year == localNow.Year
                        && file.LogFileDate.DateTime.Month == localNow.Month
                        && file.LogFileDate.DateTime.Day == localNow.Day);
            }
            else if (file.LogFileDate.LogSavingType == LogSavingType.Monthly)
            {
                return (file.LogFileDate.DateTime.Year == localNow.Year
                        && file.LogFileDate.DateTime.Month == localNow.Month);
            }
            else
            {
                if (!loggedUnknownFiles.Contains(file.FullPath))
                {
                    logger.Log(LogLevel.Warn, "CharacterLogsMonitorEngine found file with unknown saving type: " + file.FullPath, this, null);
                    loggedUnknownFiles.Add(file.FullPath);
                }
                return false;
            }
        }

        private void CheckForLargeRefreshDelay(DateTimeOffset currentDate)
        {
            if (lastRefresh - TimeSpan.FromSeconds(10) < currentDate)
            {
                logger.Log(
                    LogLevel.Warn,
                    string.Format(
                        "Detected large difference between current and previous refresh of this engine. "
                        + "Engine for character: {0}, last refresh: {1} current refresh: {2}",
                        characterName,
                        lastRefresh,
                        currentDate),
                    this,
                    null);
            }
        }
    }
}
