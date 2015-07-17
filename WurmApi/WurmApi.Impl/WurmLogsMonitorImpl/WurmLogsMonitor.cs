using System;
using System.Collections.Generic;
using System.Linq;

using AldurSoft.WurmApi.Impl.Utility;

namespace AldurSoft.WurmApi.Impl.WurmLogsMonitorImpl
{
    public class WurmLogsMonitor : IWurmLogsMonitor, IRequireRefresh
    {
        private readonly IWurmLogFiles wurmLogFiles;
        private readonly ILogger logger;
        private readonly IThreadGuard threadGuard;
        private readonly Dictionary<CharacterName, LogsMonitorEngineManager> characterNameToEngineManagers =
            new Dictionary<CharacterName, LogsMonitorEngineManager>();

        private readonly List<EventHandler<LogsMonitorEventArgs>> allEventSubscriptions =
            new List<EventHandler<LogsMonitorEventArgs>>(); 

        public WurmLogsMonitor(IWurmLogFiles wurmLogFiles, ILogger logger, IThreadGuard threadGuard)
        {
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (logger == null) throw new ArgumentNullException("logger");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.wurmLogFiles = wurmLogFiles;
            this.logger = logger;
            this.threadGuard = threadGuard;
        }

        public virtual void Subscribe(CharacterName characterName, LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            threadGuard.ValidateCurrentThread();
            var manager = GetManager(characterName);
            manager.AddSubscription(logType, eventHandler);
        }

        public virtual void Unsubscribe(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            threadGuard.ValidateCurrentThread();
            foreach (var manager in characterNameToEngineManagers.Values)
            {
                manager.RemoveSubscription(eventHandler);
            }
        }

        public void SubscribePm(CharacterName characterName, string pmRecipient, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            threadGuard.ValidateCurrentThread();
            var manager = GetManager(characterName);
            manager.AddPmSubscription(pmRecipient, eventHandler);
        }

        public void UnsubscribePm(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            threadGuard.ValidateCurrentThread();
            foreach (var manager in characterNameToEngineManagers.Values)
            {
                manager.RemovePmSubscription(eventHandler);
            }
        }

        public void SubscribeAll(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            threadGuard.ValidateCurrentThread();
            allEventSubscriptions.Add(eventHandler);
        }

        public void UnsubscribeAll(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            threadGuard.ValidateCurrentThread();
            allEventSubscriptions.Remove(eventHandler);
        }

        public void Refresh()
        {
            var activeEngineManagers = characterNameToEngineManagers.Values.Where(manager => manager.IsActive);
            foreach (var logsMonitorEngineManager in activeEngineManagers)
            {
                BroadcastAllEventsFromManager(logsMonitorEngineManager);
            }
        }

        private void BroadcastAllEventsFromManager(LogsMonitorEngineManager logsMonitorEngineManager)
        {
            IEnumerable<MonitorEvents> events = logsMonitorEngineManager.RefreshAndGetNewEvents();
            var eventsGroupedByType = events.GroupBy(monitorEvents => monitorEvents.LogFileInfo.LogType);
            foreach (var eventGroup in eventsGroupedByType)
            {
                if (eventGroup.Key == LogType.Pm)
                {
                    BroadcastPmEvents(logsMonitorEngineManager, eventGroup);
                }
                else
                {
                    BroadcastNormalEvents(logsMonitorEngineManager, eventGroup);
                }
            }
        }

        private void BroadcastNormalEvents(LogsMonitorEngineManager logsMonitorEngineManager, IGrouping<LogType, MonitorEvents> eventGroup)
        {
            foreach (var allEventSubscription in allEventSubscriptions)
            {
                allEventSubscription(
                    this,
                    new LogsMonitorEventArgs(
                        logsMonitorEngineManager.CharacterName,
                        eventGroup.Key,
                        eventGroup.SelectMany(monitorEvents => monitorEvents.LogEntries).ToArray(),
                        null));
            }
        }

        private void BroadcastPmEvents(LogsMonitorEngineManager logsMonitorEngineManager, IGrouping<LogType, MonitorEvents> eventGroup)
        {
            Dictionary<string, MonitorEvents[]> pmLogEntries =
                eventGroup.GroupBy(entry => entry.PmRecipient)
                    .ToDictionary(eventses => eventses.Key, eventses => eventses.ToArray());

            foreach (var allEventSubscription in allEventSubscriptions)
            {
                foreach (var pair in pmLogEntries)
                {
                    allEventSubscription(
                        this,
                        new LogsMonitorEventArgs(
                            logsMonitorEngineManager.CharacterName,
                            eventGroup.Key,
                            eventGroup.SelectMany(monitorEvents => monitorEvents.LogEntries).ToArray(),
                            pair.Key));
                }
            }
        }

        private LogsMonitorEngineManager GetManager(CharacterName characterName)
        {
            LogsMonitorEngineManager manager;
            if (!characterNameToEngineManagers.TryGetValue(characterName, out manager))
            {
                manager = new LogsMonitorEngineManager(
                    characterName,
                    new CharacterLogsMonitorEngineFactory(
                        logger,
                        new SingleFileMonitorFactory(
                            new LogFileStreamReaderFactory(),
                            new LogFileParser(new ParsingHelper(), logger)),
                        wurmLogFiles.GetManagerForCharacter(characterName)));
                characterNameToEngineManagers.Add(characterName, manager);
            }
            return manager;
        }
    }
}
