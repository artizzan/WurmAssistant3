using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Extensions.DotNet.Collections.Generic;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsMonitor
{
    class WurmLogsMonitor : IWurmLogsMonitor, IDisposable, IHandle<CharacterDirectoriesChanged>
    {
        readonly IWurmLogFiles wurmLogFiles;
        readonly ILogger logger;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IInternalEventAggregator internalEventAggregator;
        readonly IWurmCharacterDirectories wumCharacterDirectories;

        IReadOnlyDictionary<CharacterName, LogsMonitorEngineManager> characterNameToEngineManagers =
            new Dictionary<CharacterName, LogsMonitorEngineManager>();

        readonly EventSubscriptionsTsafeHashset allEventSubscriptionsTsafe = new EventSubscriptionsTsafeHashset();

        readonly Task updater;
        volatile bool stop = false;
        readonly object locker = new object();

        public WurmLogsMonitor([NotNull] IWurmLogFiles wurmLogFiles, [NotNull] ILogger logger,
            [NotNull] IPublicEventInvoker publicEventInvoker, [NotNull] IInternalEventAggregator internalEventAggregator,
            [NotNull] IWurmCharacterDirectories wumCharacterDirectories)
        {
            if (wurmLogFiles == null) throw new ArgumentNullException("wurmLogFiles");
            if (logger == null) throw new ArgumentNullException("logger");
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            if (internalEventAggregator == null) throw new ArgumentNullException("internalEventAggregator");
            if (wumCharacterDirectories == null) throw new ArgumentNullException("wumCharacterDirectories");
            this.wurmLogFiles = wurmLogFiles;
            this.logger = logger;
            this.publicEventInvoker = publicEventInvoker;
            this.internalEventAggregator = internalEventAggregator;
            this.wumCharacterDirectories = wumCharacterDirectories;

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

        public virtual void Subscribe(CharacterName characterName, LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(characterName);
            manager.AddSubscription(logType, eventHandler);
        }

        public virtual void Unsubscribe(CharacterName characterName, EventHandler<LogsMonitorEventArgs> eventHandler)
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
            var exists = allEventSubscriptionsTsafe.Add(eventHandler);
            if (exists)
            {
                logger.Log(LogLevel.Warn,
                    "Attempted to SubscribeAllActive with handler, that's already subscribed. " +
                    "Additional subscription will be ignored. " +
                    "Handler pointing to method: "
                    + eventHandler.Method.DeclaringType.FullName + eventHandler.Method.Name, this, null);
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
                LogsMonitorEngineManager man;
                foreach (var characterName in characters)
                {
                    if (!newMap.TryGetValue(characterName, out man))
                    {
                        var manager = new LogsMonitorEngineManager(
                            characterName,
                            new CharacterLogsMonitorEngineFactory(
                                logger,
                                new SingleFileMonitorFactory(
                                    new LogFileStreamReaderFactory(),
                                    new LogFileParser(logger)),
                                wurmLogFiles.GetManagerForCharacter(characterName), 
                                internalEventAggregator),
                            publicEventInvoker,
                            logger);
                        newMap.Add(characterName, manager);
                    }
                }
                foreach (var pair in newMap)
                {
                    if (!characters.Contains(pair.Key))
                    {
                        pair.Value.Dispose();
                        newMap.Remove(pair.Key);
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
