using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.WurmApi.Extensions.DotNet;
using AldursLab.WurmApi.Extensions.DotNet.Collections.Generic;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Modules.Events.Public;
using AldursLab.WurmApi.Modules.Wurm.LogReading;
using AldursLab.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.LogsMonitor
{
    class WurmLogsMonitor : IWurmLogsMonitor, IDisposable, IHandle<CharacterDirectoriesChanged>, IWurmLogsMonitorInternal
    {
        readonly IWurmLogFiles wurmLogFiles;
        readonly IWurmApiLogger logger;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly IInternalEventAggregator internalEventAggregator;
        readonly IWurmCharacterDirectories wurmCharacterDirectories;
        readonly InternalEventInvoker internalEventInvoker;
        readonly TaskManager taskManager;
        readonly LogFileStreamReaderFactory logFileStreamReaderFactory;

        IReadOnlyDictionary<CharacterName, LogsMonitorEngineManager> characterNameToEngineManagers =
            new Dictionary<CharacterName, LogsMonitorEngineManager>();

        readonly EventSubscriptionsTsafeHashset allEventSubscriptionsTsafe = new EventSubscriptionsTsafeHashset();

        readonly Task updater;
        volatile bool stop = false;
        readonly object locker = new object();

        readonly TaskHandle taskHandle;

        public WurmLogsMonitor([NotNull] IWurmLogFiles wurmLogFiles, [NotNull] IWurmApiLogger logger,
            [NotNull] IPublicEventInvoker publicEventInvoker, [NotNull] IInternalEventAggregator internalEventAggregator,
            [NotNull] IWurmCharacterDirectories wurmCharacterDirectories,
            [NotNull] InternalEventInvoker internalEventInvoker, [NotNull] TaskManager taskManager,
            [NotNull] LogFileStreamReaderFactory logFileStreamReaderFactory)
        {
            if (wurmLogFiles == null) throw new ArgumentNullException(nameof(wurmLogFiles));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (publicEventInvoker == null) throw new ArgumentNullException(nameof(publicEventInvoker));
            if (internalEventAggregator == null) throw new ArgumentNullException(nameof(internalEventAggregator));
            if (wurmCharacterDirectories == null) throw new ArgumentNullException(nameof(wurmCharacterDirectories));
            if (internalEventInvoker == null) throw new ArgumentNullException(nameof(internalEventInvoker));
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            if (logFileStreamReaderFactory == null) throw new ArgumentNullException(nameof(logFileStreamReaderFactory));
            this.wurmLogFiles = wurmLogFiles;
            this.logger = logger;
            this.publicEventInvoker = publicEventInvoker;
            this.internalEventAggregator = internalEventAggregator;
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.internalEventInvoker = internalEventInvoker;
            this.taskManager = taskManager;
            this.logFileStreamReaderFactory = logFileStreamReaderFactory;

            try
            {
                Rebuild();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at WurmLogsMonitor initial rebuild", this, exception);
            }

            internalEventAggregator.Subscribe(this);

            taskHandle = new TaskHandle(Rebuild, "WurmLogsMonitor rebuild");
            taskManager.Add(taskHandle);

            updater = new Task(() =>
            {
                while (true)
                {
                    if (stop) return;
                    Thread.Sleep(500);
                    if (stop) return;

                    try
                    {
                        foreach (var logsMonitorEngineManager in characterNameToEngineManagers.Values)
                        {
                            logsMonitorEngineManager.Update(allEventSubscriptionsTsafe);
                        }
                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Error, "WurmLogsMonitor 'updater' task crashed", this, exception);
                    }
                }
            }, TaskCreationOptions.LongRunning);
            updater.Start();

            taskHandle.Trigger();
        }

        public void Handle(CharacterDirectoriesChanged message)
        {
            taskHandle.Trigger();
        }

        public void Subscribe(string characterName, LogType logType, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(new CharacterName(characterName));
            manager.AddSubscription(logType, eventHandler);
        }

        public void SubscribeInternal(CharacterName characterName, LogType logType,
            EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            Rebuild();
            var manager = GetManager(characterName);
            manager.AddSubscriptionInternal(logType, eventHandler);
        }

        public void Unsubscribe(string characterName, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            if (characterName == null)
            {
                logger.Log(LogLevel.Error,
                    string.Format("Unsubscribe attempted with null characterName, handler details: {0} ",
                        eventHandler.MethodInformationToString()),
                    this,
                    null);
                return;
            }

            try
            {
                var manager = GetManager(new CharacterName(characterName));
                manager.RemoveSubscription(eventHandler);
            }
            catch (DataNotFoundException exception)
            {
                logger.Log(LogLevel.Info,
                    string.Format(
                        "Could not unsubscribe an event handler from logs monitor for character {0}. No manager found for this character. , handler details: {1}",
                        characterName,
                        eventHandler.MethodInformationToString()),
                    this,
                    exception);
            }
        }

        public void SubscribePm(string characterName, string pmRecipientName, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            var manager = GetManager(new CharacterName(characterName));
            manager.AddPmSubscription(eventHandler, new CharacterName(pmRecipientName));
        }

        public void UnsubscribePm(string characterName, string pmRecipientName, EventHandler<LogsMonitorEventArgs> eventHandler)
        {
            if (characterName == null)
            {
                logger.Log(LogLevel.Error,
                    string.Format("UnsubscribePm attempted with null characterName, handler details: {0} ",
                        eventHandler.MethodInformationToString()),
                    this,
                    null);
                return;
            }

            try
            {
                var manager = GetManager(new CharacterName(characterName));
                manager.RemovePmSubscription(eventHandler, new CharacterName(pmRecipientName).Normalized);
            }
            catch (DataNotFoundException exception)
            {
                logger.Log(LogLevel.Info,
                    string.Format(
                        "Could not unsubscribe an event handler from logs monitor for character {0} (PM sub: {1}). No manager found for this character. Handler details: {2}",
                        characterName,
                        pmRecipientName,
                        eventHandler),
                    this,
                    exception);
            }
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
            List<Exception> exceptions = new List<Exception>();
            lock (locker)
            {
                var characters = wurmCharacterDirectories.GetAllCharacters().ToHashSet();
                var newMap = characterNameToEngineManagers.ToDictionary(pair => pair.Key, pair => pair.Value);
                foreach (var characterName in characters)
                {
                    try
                    {
                        LogsMonitorEngineManager man;
                        if (!newMap.TryGetValue(characterName, out man))
                        {
                            var manager = new LogsMonitorEngineManager(
                                characterName,
                                new CharacterLogsMonitorEngineFactory(
                                    logger,
                                    new SingleFileMonitorFactory(
                                        logFileStreamReaderFactory,
                                        new LogFileParser(logger)),
                                    wurmLogFiles.GetForCharacter(characterName),
                                    internalEventAggregator),
                                publicEventInvoker,
                                logger,
                                internalEventInvoker);
                            newMap.Add(characterName, manager);
                        }
                    }
                    catch (Exception exception)
                    {
                        exceptions.Add(exception);
                    }
                }
                characterNameToEngineManagers = newMap;
            }
            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        public void Dispose()
        {
            taskManager.Remove(taskHandle);
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
