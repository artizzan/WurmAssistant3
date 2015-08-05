using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class WurmLogsMonitor : IWurmLogsMonitor, IDisposable, IHandle<CharacterDirectoriesChanged>, IWurmLogsMonitorInternal
    {
        readonly IWurmLogFiles wurmLogFiles;
        readonly ILogger logger;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IInternalEventAggregator internalEventAggregator;
        readonly IWurmCharacterDirectories wumCharacterDirectories;
        readonly InternalEventInvoker internalEventInvoker;

        IReadOnlyDictionary<CharacterName, LogsMonitorEngineManager> characterNameToEngineManagers =
            new Dictionary<CharacterName, LogsMonitorEngineManager>();

        readonly EventSubscriptionsTsafeHashset allEventSubscriptionsTsafe = new EventSubscriptionsTsafeHashset();

        readonly Task updater;
        volatile bool stop = false;
        readonly object locker = new object();

        public WurmLogsMonitor([NotNull] IWurmLogFiles wurmLogFiles, [NotNull] ILogger logger,
            [NotNull] IPublicEventInvoker publicEventInvoker, [NotNull] IInternalEventAggregator internalEventAggregator,
            [NotNull] IWurmCharacterDirectories wumCharacterDirectories,
            [NotNull] InternalEventInvoker internalEventInvoker)
        {
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (logger == null) throw new ArgumentNullException("logger");
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            if (internalEventAggregator == null) throw new ArgumentNullException("internalEventAggregator");
            if (wumCharacterDirectories == null) throw new ArgumentNullException("wumCharacterDirectories");
            if (internalEventInvoker == null) throw new ArgumentNullException("internalEventInvoker");
            this.wurmLogFiles = wurmLogFiles;
            this.logger = logger;
            this.publicEventInvoker = publicEventInvoker;
            this.internalEventAggregator = internalEventAggregator;
            this.wumCharacterDirectories = wumCharacterDirectories;
            this.internalEventInvoker = internalEventInvoker;

            internalEventAggregator.Subscribe(this);

            Rebuild();

            updater = new Task(() =>
            {
                while (true)
                {
                    if (stop) return;
                    Thread.Sleep(500);
                    if (stop) return;

                    foreach (var logsMonitorEngineManager in characterNameToEngineManagers.Values)
                    {
                        logsMonitorEngineManager.Update(allEventSubscriptionsTsafe);
                    }
                }
            }, TaskCreationOptions.LongRunning);
            updater.Start();
        }

        public void Handle(CharacterDirectoriesChanged message)
        {
            Rebuild();
        }

        public void Subscribe(CharacterName characterName, LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(characterName);
            manager.AddSubscription(logType, eventHandler);
        }

        public void SubscribeInternal(CharacterName characterName, LogType logType,
            EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(characterName);
            manager.AddSubscriptionInternal(logType, eventHandler);
        }

        public void Unsubscribe(CharacterName characterName, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(characterName);
            manager.RemoveSubscription(eventHandler);
        }

        public void SubscribePm(CharacterName characterName, CharacterName pmRecipient, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(characterName);
            manager.AddPmSubscription(eventHandler, pmRecipient.Normalized);
        }

        public void UnsubscribePm(CharacterName characterName, CharacterName pmRecipient, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(characterName);
            manager.RemovePmSubscription(eventHandler, pmRecipient.Normalized);
        }

        public void SubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            TrySubscribeAllActive(eventHandler);
        }

        public void SubscribeAllActiveInternal(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            TrySubscribeAllActive(eventHandler, true);
        }

        void TrySubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler, bool internalSubscription = false)
        {
            var added = allEventSubscriptionsTsafe.Add(new AllEventsSubscription(eventHandler, internalSubscription));
            if (!added)
            {
                logger.Log(LogLevel.Warn,
                    string.Format(
                        "Attempted to SubscribeAllActive with handler, that's already subscribed. "
                        + "Additional subscription will be ignored. "
                        + "Handler pointing to method: {0}",
                        eventHandler.MethodInformationToString()), this, null);
            }
        }

        public void UnsubscribeAllActive(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            allEventSubscriptionsTsafe.Remove(eventHandler);
        }

        public void UnsubscribeFromAll(EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            UnsubscribeAllActive(eventHandler);
            foreach (var characterNameToEngineManager in characterNameToEngineManagers.Values)
            {
                characterNameToEngineManager.RemoveAllSubscriptions(eventHandler);
            }
        }

        private LogsMonitorEngineManager GetManager(CharacterName characterName)
        {
            LogsMonitorEngineManager manager;
            if (!characterNameToEngineManagers.TryGetValue(characterName, out manager))
            {
                throw new DataNotFoundException("Character does not exist or unknown: " + characterName);
            }
            return manager;
        }

        private void Rebuild()
        {
            lock (locker)
            {
                var characters = wumCharacterDirectories.GetAllCharacters().ToHashSet();
                var newMap = characterNameToEngineManagers.ToDictionary(pair => pair.Key, pair => pair.Value);
                foreach (var characterName in characters)
                {
                    LogsMonitorEngineManager man;
                    if (!newMap.TryGetValue(characterName, out man))
                    {
                        var manager = new LogsMonitorEngineManager(
                            characterName,
                            new CharacterLogsMonitorEngineFactory(
                                logger,
                                new SingleFileMonitorFactory(
                                    new LogFileStreamReaderFactory(),
                                    new LogFileParser(logger)),
                                wurmLogFiles.GetForCharacter(characterName), 
                                internalEventAggregator),
                            publicEventInvoker,
                            logger,
                            internalEventInvoker);
                        newMap.Add(characterName, manager);
                    }
                }
                characterNameToEngineManagers = newMap;
            }
        }

        public void Dispose()
        {
            stop = true;
            updater.Wait();
            updater.Dispose();
            lock (locker)
            {
                foreach (var characterNameToEngineManager in characterNameToEngineManagers.Values)
                {
                    characterNameToEngineManager.Dispose();
                }
            }
        }
    }
}
