using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class LogsMonitorEngineManager
    {
        private readonly CharacterName characterName;
        private readonly CharacterLogsMonitorEngineFactory characterLogsMonitorEngineFactory;
        private CharacterLogsMonitorEngine engine;

        private readonly Dictionary<EventHandler<LogsMonitorEventArgs>, EngineSubscription> subscriptions =
            new Dictionary<EventHandler<LogsMonitorEventArgs>, EngineSubscription>();
        private readonly Dictionary<EventHandler<LogsMonitorEventArgs>, EnginePmSubscription> pmSubscriptions =
            new Dictionary<EventHandler<LogsMonitorEventArgs>, EnginePmSubscription>();

        public LogsMonitorEngineManager(
            CharacterName characterName,
            CharacterLogsMonitorEngineFactory characterLogsMonitorEngineFactory)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (characterLogsMonitorEngineFactory == null) throw new ArgumentNullException("characterLogsMonitorEngineFactory");
            this.characterName = characterName;
            this.characterLogsMonitorEngineFactory = characterLogsMonitorEngineFactory;
        }

        public bool IsActive { get { return engine != null; }}
        public CharacterName CharacterName { get { return characterName; } }

        public void AddSubscription(LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            if (engine == null) CreateEngine();

            subscriptions.Add(
                eventHandler,
                new EngineSubscription() { LogType = logType, LogsMonitorEventHandler = eventHandler });
        }

        public void AddPmSubscription(string pmRecipient, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            if (engine == null) CreateEngine();

            pmSubscriptions.Add(
                eventHandler,
                new EnginePmSubscription()
                {
                    PmRecipientNormalized = pmRecipient.ToUpperInvariant(),
                    LogsMonitorPmEventHandler = eventHandler
                });
        }

        public void RemoveSubscription(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            EngineSubscription manager;
            if (subscriptions.TryGetValue(eventHandler, out manager))
            {
                manager.LogsMonitorEventHandler = null;
                subscriptions.Remove(eventHandler);

                if (NoSubscriptions())
                {
                    StopEngine();
                }
            }
        }

        public void RemovePmSubscription(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            EnginePmSubscription manager;
            if (pmSubscriptions.TryGetValue(eventHandler, out manager))
            {
                manager.LogsMonitorPmEventHandler = null;
                pmSubscriptions.Remove(eventHandler);

                if (NoSubscriptions())
                {
                    StopEngine();
                }
            }
        }

        public IEnumerable<MonitorEvents> RefreshAndGetNewEvents()
        {
            if (engine == null)
            {
                return new MonitorEvents[0];
            }

            MonitorEvents[] events = engine.RefreshAndGetNewEvents().ToArray();

            Dictionary<LogType, MonitorEvents[]> groupedEvents =
                events.GroupBy(monitorEvents => monitorEvents.LogFileInfo.LogType)
                    .ToDictionary(eventses => eventses.Key, eventses => eventses.ToArray());

            BroadcastNormalEvents(groupedEvents);
            BroadcastPmEvents(groupedEvents);

            return events;
        }

        private void BroadcastNormalEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents)
        {
            foreach (var engineSubscription in subscriptions.Values)
            {
                MonitorEvents[] outEvents;
                if (groupedEvents.TryGetValue(engineSubscription.LogType, out outEvents))
                {
                    if (engineSubscription.LogType == LogType.Pm)
                    {
                        MonitorEvents[] pmEvents;
                        if (groupedEvents.TryGetValue(LogType.Pm, out pmEvents))
                        {
                            Dictionary<string, MonitorEvents[]> pmLogEntries =
                                pmEvents.GroupBy(entry => entry.PmRecipient)
                                    .ToDictionary(eventses => eventses.Key, eventses => eventses.ToArray());

                            foreach (var pair in pmLogEntries)
                            {
                                engineSubscription.LogsMonitorEventHandler(
                                this,
                                new LogsMonitorEventArgs(
                                    characterName,
                                    engineSubscription.LogType,
                                    pair.Value.SelectMany(monitorEvents => monitorEvents.LogEntries).ToArray(),
                                    pair.Key));
                            }
                        }
                    }
                    else
                    {
                        engineSubscription.LogsMonitorEventHandler(
                        this,
                        new LogsMonitorEventArgs(
                            characterName,
                            engineSubscription.LogType,
                            outEvents.SelectMany(monitorEvents => monitorEvents.LogEntries).ToArray(),
                            null));
                    }
                }
            }
        }

        private void BroadcastPmEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents)
        {
            MonitorEvents[] pmEvents;
            if (groupedEvents.TryGetValue(LogType.Pm, out pmEvents))
            {
                Dictionary<string, MonitorEvents[]> pmLogEntries =
                    pmEvents.GroupBy(entry => entry.PmRecipient)
                        .ToDictionary(eventses => eventses.Key, eventses => eventses.ToArray());

                foreach (var enginePmSubscription in pmSubscriptions.Values)
                {
                    MonitorEvents[] monitorEvents;
                    if (pmLogEntries.TryGetValue(enginePmSubscription.PmRecipientNormalized, out monitorEvents))
                    {
                        // broadcast events
                        enginePmSubscription.LogsMonitorPmEventHandler(
                            this,
                            new LogsMonitorEventArgs(
                                characterName,
                                LogType.Pm,
                                monitorEvents.SelectMany(events1 => events1.LogEntries),
                                enginePmSubscription.PmRecipientNormalized));
                    }
                }
            }
        }

        private void CreateEngine()
        {
            engine = characterLogsMonitorEngineFactory.Create(characterName);
        }

        private bool NoSubscriptions()
        {
            return subscriptions.Count + pmSubscriptions.Count == 0;
        }

        private void StopEngine()
        {
            engine = null;
        }

        class EngineSubscription
        {
            public LogType LogType { get; set; }

            public EventHandler<LogsMonitorEventArgs> LogsMonitorEventHandler { get; set; }
        }

        class EnginePmSubscription
        {
            public string PmRecipientNormalized { get; set; }

            public EventHandler<LogsMonitorEventArgs> LogsMonitorPmEventHandler { get; set; }
        }
    }
}