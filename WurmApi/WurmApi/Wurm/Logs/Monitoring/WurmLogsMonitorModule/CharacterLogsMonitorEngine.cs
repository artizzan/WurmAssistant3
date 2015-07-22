using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.Core;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.Characters;

namespace AldurSoft.WurmApi.Wurm.Logs.Monitoring.WurmLogsMonitorModule
{
    public class CharacterLogsMonitorEngine
    {
        private readonly CharacterName characterName;
        private readonly ILogger logger;
        private readonly SingleFileMonitorFactory singleFileMonitorFactory;
        private readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        private DateTimeOffset lastRefresh;

        private readonly Dictionary<string, SingleFileMonitor> monitors = new Dictionary<string, SingleFileMonitor>();
        private readonly HashSet<string> loggedUnknownFiles = new HashSet<string>();

        public CharacterLogsMonitorEngine(
            CharacterName characterName,
            ILogger logger,
            SingleFileMonitorFactory singleFileMonitorFactory,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (logger == null) throw new ArgumentNullException("logger");
            if (singleFileMonitorFactory == null) throw new ArgumentNullException("singleFileMonitorFactory");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            this.characterName = characterName;
            this.logger = logger;
            this.singleFileMonitorFactory = singleFileMonitorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;

            lastRefresh = Time.Clock.LocalNowOffset;

            RebuildAllMonitors(initialRebuild:true);

            wurmCharacterLogFiles.FilesAddedOrRemoved += WurmCharacterLogFilesOnFilesAddedOrRemoved;
        }

        internal IEnumerable<MonitorEvents> RefreshAndGetNewEvents()
        {
            var currentDate = Time.Clock.LocalNowOffset;
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

        private void WurmCharacterLogFilesOnFilesAddedOrRemoved(object sender, EventArgs eventArgs)
        {
            RebuildAllMonitors();
        }

        private void RebuildAllMonitors(bool initialRebuild = false)
        {
            var timeNow = Time.Clock.LocalNow;
            RemoveOldMonitors(timeNow);
            var files = wurmCharacterLogFiles.TryGetLogFiles(timeNow, timeNow);
            foreach (var file in files)
            {
                if (ShouldFileBeMonitored(file, timeNow))
                {
                    if (!monitors.ContainsKey(file.FullPath))
                    {
                        SingleFileMonitor monitor;
                        // when initializing engine, we ignore events already in files
                        // when adding engines for new files, we want to always read from start of file
                        if (initialRebuild)
                        {
                            monitor = singleFileMonitorFactory.Create(file);
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
                    logger.Log(LogLevel.Warn, "CharacterLogsMonitorEngine found file with unknown saving type: " + file.FullPath, this);
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
                    this);
            }
        }
    }
}
