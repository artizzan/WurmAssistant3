using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    class LogsMonitorEngineManager : IDisposable
    {
        readonly CharacterName characterName;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IWurmApiLogger logger;
        readonly IInternalEventInvoker internalEventInvoker;

        readonly CharacterLogsMonitorEngine engine;

        readonly ConcurrentDictionary<EventHandler<LogsMonitorEventArgs>, EngineSubscription> subscriptions =
            new ConcurrentDictionary<EventHandler<LogsMonitorEventArgs>, EngineSubscription>();

        readonly ConcurrentDictionary<PmSubscriptionKey, EnginePmSubscription> pmSubscriptions =
            new ConcurrentDictionary<PmSubscriptionKey, EnginePmSubscription>();

        public LogsMonitorEngineManager(
            [NotNull] CharacterName characterName,
            [NotNull] CharacterLogsMonitorEngineFactory characterLogsMonitorEngineFactory,
            [NotNull] IPublicEventInvoker publicEventInvoker, 
            [NotNull] IWurmApiLogger logger,
            [NotNull] IInternalEventInvoker internalEventInvoker)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            if (characterLogsMonitorEngineFactory == null) throw new ArgumentNullException(nameof(characterLogsMonitorEngineFactory));
            if (publicEventInvoker == null) throw new ArgumentNullException(nameof(publicEventInvoker));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (internalEventInvoker == null) throw new ArgumentNullException(nameof(internalEventInvoker));
            this.characterName = characterName;
            this.publicEventInvoker = publicEventInvoker;
            this.logger = logger;
            this.internalEventInvoker = internalEventInvoker;

            engine = characterLogsMonitorEngineFactory.Create(characterName);
        }

        public CharacterName CharacterName => characterName;

        public void AddSubscription(LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            TryAddSubscription(logType, eventHandler);
        }

        void TryAddSubscription(LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler, bool internalSub = false)
        {
            bool success = subscriptions.TryAdd(
                eventHandler,
                new EngineSubscription() {LogType = logType, LogsMonitorEventHandler = eventHandler, InternalSubscription = internalSub});
            if (!success)
            {
                logger.Log(LogLevel.Warn,
                    string.Format(
                        "Attempted to AddSubscription with handler, that's already subscribed. "
                        + "Additional subscription will be ignored. "
                        + "LogType: {0} Handler pointing to method: {1}", logType, eventHandler.MethodInformationToString()), this, null);
            }
        }

        public void AddSubscriptionInternal(LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            TryAddSubscription(logType, eventHandler, true);
        }

        public void AddPmSubscription(EventHandler<LogsMonitorEventArgs> eventHandler, CharacterName pmRecipient)
        {
            var key = new PmSubscriptionKey(eventHandler, pmRecipient.Normalized);
            bool success = pmSubscriptions.TryAdd(
                key,
                new EnginePmSubscription()
                {
                    PmRecipientNormalized = pmRecipient.Normalized,
                    LogsMonitorPmEventHandler = eventHandler
                });
            if (!success)
            {
                logger.Log(LogLevel.Warn,
                    string.Format(
                        "Attempted to AddPmSubscription with handler, that's already subscribed. "
                        + "Additional subscription will be ignored. "
                        + "PmRecipient: {0} Handler pointing to method: {1}", pmRecipient,
                        eventHandler.MethodInformationToString()), this, null);
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
                if (engineSubscription.LogType == LogType.AllLogs)
                {
                    foreach (var events in groupedEvents)
                    {
                        SendNormalEvents(engineSubscription, events.Value);
                    }
                }
                else if (groupedEvents.TryGetValue(engineSubscription.LogType, out outEvents))
                {
                    SendNormalEvents(engineSubscription, outEvents);
                }
            }
        }

        void SendNormalEvents(EngineSubscription engineSubscription, MonitorEvents[] allEvents)
        {
            foreach (var events in allEvents)
            {
                if (engineSubscription.InternalSubscription)
                {
                    internalEventInvoker.TriggerInstantly(engineSubscription.LogsMonitorEventHandler, this,
                        new LogsMonitorEventArgs(
                            characterName,
                            events.LogFileInfo.LogType,
                            events.LogEntries.ToArray(),
                            events.PmRecipientNormalized));
                }
                else
                {
                    publicEventInvoker.TriggerInstantly(engineSubscription.LogsMonitorEventHandler, this,
                        new LogsMonitorEventArgs(
                            characterName,
                            events.LogFileInfo.LogType,
                            events.LogEntries.ToArray(),
                            events.PmRecipientNormalized));
                }
            }
        }

        void BroadcastGlobalEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents, AllEventsSubscription[] globalSubs)
        {
            foreach (var pair in groupedEvents)
            {
                foreach (var events in pair.Value)
                {
                    foreach (var sub in globalSubs)
                    {
                        if (sub.InternalSubscription)
                        {
                            internalEventInvoker.TriggerInstantly(sub.EventHandler, this,
                                new LogsMonitorEventArgs(
                                    characterName,
                                    events.LogFileInfo.LogType,
                                    events.LogEntries.ToArray(),
                                    events.PmRecipientNormalized));
                        }
                        else
                        {
                            publicEventInvoker.TriggerInstantly(sub.EventHandler, this,
                                new LogsMonitorEventArgs(
                                    characterName,
                                    events.LogFileInfo.LogType,
                                    events.LogEntries.ToArray(),
                                    events.PmRecipientNormalized));
                        }
                    }
                }
            }
        }

        void BroadcastPmEvents(Dictionary<LogType, MonitorEvents[]> groupedEvents)
        {
            MonitorEvents[] pmEvents;
            if (groupedEvents.TryGetValue(LogType.Pm, out pmEvents))
            {
                Dictionary<string, MonitorEvents[]> pmLogEntries =
                    pmEvents.GroupBy(entry => entry.PmRecipientNormalized)
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

        bool AnyLocalSubscriptions => subscriptions.Count + pmSubscriptions.Count > 0;

        public void Dispose()
        {
            subscriptions.Clear();
            pmSubscriptions.Clear();
        }

        class EngineSubscription
        {
            public LogType LogType { get; set; }

            public EventHandler<LogsMonitorEventArgs> LogsMonitorEventHandler { get; set; }

            public bool InternalSubscription { get; set; }
        }

        class EnginePmSubscription
        {
            public string PmRecipientNormalized { get; set; }

            public EventHandler<LogsMonitorEventArgs> LogsMonitorPmEventHandler { get; set; }
        }
    }
}