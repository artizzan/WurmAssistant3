using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class LogsMonitorEngineManager : IDisposable
    {
        readonly CharacterName characterName;
        readonly CharacterLogsMonitorEngineFactory characterLogsMonitorEngineFactory;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly ILogger logger;

        readonly CharacterLogsMonitorEngine engine;

        readonly ConcurrentDictionary<EventHandler<LogsMonitorEventArgs>, EngineSubscription> subscriptions =
            new ConcurrentDictionary<EventHandler<LogsMonitorEventArgs>, EngineSubscription>();

        readonly ConcurrentDictionary<PmSubscriptionKey, EnginePmSubscription> pmSubscriptions =
            new ConcurrentDictionary<PmSubscriptionKey, EnginePmSubscription>();

        public LogsMonitorEngineManager([NotNull] CharacterName characterName, [NotNull] CharacterLogsMonitorEngineFactory characterLogsMonitorEngineFactory,
            [NotNull] IPublicEventInvoker publicEventInvoker, [NotNull] ILogger logger)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (characterLogsMonitorEngineFactory == null) throw new ArgumentNullException("characterLogsMonitorEngineFactory");
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            if (logger == null) throw new ArgumentNullException("logger");
            this.characterName = characterName;
            this.characterLogsMonitorEngineFactory = characterLogsMonitorEngineFactory;
            this.publicEventInvoker = publicEventInvoker;
            this.logger = logger;

            engine = characterLogsMonitorEngineFactory.Create(characterName);
        }

        public CharacterName CharacterName { get { return characterName; } }

        public void AddSubscription(LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            bool exists = subscriptions.TryAdd(
                eventHandler,
                new EngineSubscription() { LogType = logType, LogsMonitorEventHandler = eventHandler });
            if (exists)
            {
                logger.Log(LogLevel.Warn,
                    "Attempted to AddSubscription with handler, that's already subscribed. " +
                    "Additional subscription will be ignored. " +
                    "LogType: " + logType +
                    "Handler pointing to method: "
                    + eventHandler.Method.DeclaringType.FullName + "." + eventHandler.Method.Name, this, null);
            }
        }

        public void AddPmSubscription(EventHandler<LogsMonitorEventArgs> eventHandler, string pmRecipient)
        {
            var key = new PmSubscriptionKey(eventHandler, pmRecipient);
            bool exists = pmSubscriptions.TryAdd(
                key,
                new EnginePmSubscription()
                {
                    PmRecipientNormalized = pmRecipient.ToUpperInvariant(),
                    LogsMonitorPmEventHandler = eventHandler
                });
            if (exists)
            {
                logger.Log(LogLevel.Warn,
                    "Attempted to AddPmSubscription with handler, that's already subscribed. " +
                    "Additional subscription will be ignored. " +
                    "PmRecipient: " + pmRecipient +
                    "Handler pointing to method: "
                    + eventHandler.Method.DeclaringType.FullName + "." + eventHandler.Method.Name, this, null);
            }
        }

        public void RemoveSubscription(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            EngineSubscription manager;
            if (subscriptions.TryGetValue(eventHandler, out manager))
            {
                manager.LogsMonitorEventHandler = null;
                subscriptions.TryRemove(eventHandler, out manager);
            }
        }

        public void RemovePmSubscription(EventHandler<LogsMonitorEventArgs> eventHandler, string pmRecipient)
        {
            var key = new PmSubscriptionKey(eventHandler, pmRecipient);
            EnginePmSubscription manager;
            if (pmSubscriptions.TryGetValue(key, out manager))
            {
                manager.LogsMonitorPmEventHandler = null;
                pmSubscriptions.TryRemove(key, out manager);
            }
        }

        public void Update(EventSubscriptionsTsafeHashset globalSubscriptions)
        {
            MonitorEvents[] events = engine.RefreshAndGetNewEvents().ToArray();

            var anyGlobalSubscriptions = globalSubscriptions.Any;
            var anyLocalSubscriptions = AnyLocalSubscriptions;

            if (anyGlobalSubscriptions || anyLocalSubscriptions)
            {
                Dictionary<LogType, MonitorEvents[]> groupedEvents =
                    events.GroupBy(monitorEvents => monitorEvents.LogFileInfo.LogType)
                        .ToDictionary(eventses => eventses.Key, eventses => eventses.ToArray());
                var globalSubs = globalSubscriptions.ToArray();

                if (anyLocalSubscriptions)
                {
                    BroadcastNormalEvents(groupedEvents);
                    BroadcastPmEvents(groupedEvents);
                }
                if (anyGlobalSubscriptions)
                {
                    BroadcastGlobalEvents(groupedEvents, globalSubs);
                }
            }
        }

        public void RemoveAllSubscriptions(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            RemoveSubscription(eventHandler);
            RemoveAllPmSubscriptions(eventHandler);
        }

        void RemoveAllPmSubscriptions(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var keysToRemove = pmSubscriptions.Keys.Where(key => key.EventHandler == eventHandler);
            foreach (var pmSubscriptionKey in keysToRemove)
            {
                RemovePmSubscription(pmSubscriptionKey.EventHandler, pmSubscriptionKey.PmRecipient);
            }
        }

        void BroadcastNormalEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents)
        {
            foreach (var engineSubscription in subscriptions.Values)
            {
                MonitorEvents[] outEvents;
                if (groupedEvents.TryGetValue(engineSubscription.LogType, out outEvents))
                {
                    publicEventInvoker.TriggerInstantly(engineSubscription.LogsMonitorEventHandler, this,
                        new LogsMonitorEventArgs(
                            characterName,
                            engineSubscription.LogType,
                            outEvents.SelectMany(monitorEvents => monitorEvents.LogEntries).ToArray(),
                            null));
                }
            }
        }

        void BroadcastGlobalEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents, EventHandler<LogsMonitorEventArgs>[] globalSubs)
        {
            foreach (var pair in groupedEvents)
            {
                foreach (var eventHandler in globalSubs)
                {
                    publicEventInvoker.TriggerInstantly(eventHandler, this,
                        new LogsMonitorEventArgs(
                            characterName,
                            pair.Key,
                            pair.Value.SelectMany(monitorEvents => monitorEvents.LogEntries).ToArray(),
                            null));
                }
            }
        }

        void BroadcastPmEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents)
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
                        publicEventInvoker.TriggerInstantly(enginePmSubscription.LogsMonitorPmEventHandler, this,
                            new LogsMonitorEventArgs(
                                characterName,
                                LogType.Pm,
                                monitorEvents.SelectMany(events1 => events1.LogEntries),
                                enginePmSubscription.PmRecipientNormalized));
                    }
                }
            }
        }

        bool AnyLocalSubscriptions
        {
            get { return subscriptions.Count + pmSubscriptions.Count == 0; }
        }

        public void Dispose()
        {
            subscriptions.Clear();
            pmSubscriptions.Clear();
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