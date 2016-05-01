using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Public;
using AldursLab.WurmApi.Modules.Wurm.LogsMonitor;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.Characters
{
    class WurmCharacters : IWurmCharacters, IDisposable
    {
        readonly IWurmCharacterDirectories characterDirectories;
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmServers wurmServers;
        readonly IWurmServerHistory wurmServerHistory;
        readonly IWurmApiLogger logger;
        readonly TaskManager taskManager;
        readonly IWurmLogsMonitorInternal wurmLogsMonitor;
        readonly IPublicEventInvoker publicEventInvoker;
        readonly InternalEventAggregator internalEventAggregator;
        readonly IWurmPaths wurmPaths;
        readonly IWurmLogsHistory wurmLogsHistory;
        readonly IWurmServerGroups serverGroups;

        readonly IDictionary<CharacterName, WurmCharacter> allCharacters = new Dictionary<CharacterName, WurmCharacter>();

        readonly object locker = new object();

        public WurmCharacters([NotNull] IWurmCharacterDirectories characterDirectories,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory, [NotNull] IWurmApiLogger logger, 
            [NotNull] TaskManager taskManager, [NotNull] IWurmLogsMonitorInternal wurmLogsMonitor,
            [NotNull] IPublicEventInvoker publicEventInvoker, [NotNull] InternalEventAggregator internalEventAggregator,
            [NotNull] IWurmPaths wurmPaths, [NotNull] IWurmLogsHistory wurmLogsHistory,
            [NotNull] IWurmServerGroups serverGroups)
        {
            this.characterDirectories = characterDirectories;
            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;
            this.taskManager = taskManager;
            this.wurmLogsMonitor = wurmLogsMonitor;
            this.publicEventInvoker = publicEventInvoker;
            this.internalEventAggregator = internalEventAggregator;
            this.wurmPaths = wurmPaths;
            this.wurmLogsHistory = wurmLogsHistory;
            this.serverGroups = serverGroups;
            if (characterDirectories == null) throw new ArgumentNullException(nameof(characterDirectories));
            if (wurmConfigs == null) throw new ArgumentNullException(nameof(wurmConfigs));
            if (wurmServers == null) throw new ArgumentNullException(nameof(wurmServers));
            if (wurmServerHistory == null) throw new ArgumentNullException(nameof(wurmServerHistory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            if (wurmLogsMonitor == null) throw new ArgumentNullException(nameof(wurmLogsMonitor));
            if (publicEventInvoker == null) throw new ArgumentNullException(nameof(publicEventInvoker));
            if (internalEventAggregator == null) throw new ArgumentNullException(nameof(internalEventAggregator));
            if (wurmPaths == null) throw new ArgumentNullException(nameof(wurmPaths));
            if (wurmLogsHistory == null) throw new ArgumentNullException(nameof(wurmLogsHistory));
            if (serverGroups == null) throw new ArgumentNullException(nameof(serverGroups));

            var allChars = characterDirectories.GetAllCharacters();
            foreach (var characterName in allChars)
            {
                try
                {
                    Create(characterName);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error,
                        string.Format("Could not initialize character for name {0}", characterName),
                        this,
                        exception);
                }
            }
        }

        public IEnumerable<IWurmCharacter> All
        {
            get
            {
                lock (locker)
                {
                    return allCharacters.Values.ToArray();
                }
            }
        }

        public IWurmCharacter Get([NotNull] CharacterName name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            lock (locker)
            {
                WurmCharacter character;
                if (!allCharacters.TryGetValue(name, out character))
                {
                    character = Create(name);
                }
                return character;
            }
        }

        public IWurmCharacter Get([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return Get(new CharacterName(name));
        }

        WurmCharacter Create(CharacterName name)
        {
            if (characterDirectories.Exists(name))
            {
                var character = new WurmCharacter(
                    name,
                    characterDirectories.GetFullDirPathForCharacter(name),
                    wurmConfigs,
                    wurmServers,
                    wurmServerHistory,
                    logger,
                    taskManager,
                    wurmLogsMonitor,
                    publicEventInvoker,
                    internalEventAggregator,
                    wurmLogsHistory,
                    wurmPaths,
                    serverGroups
                    );
                allCharacters.Add(name, character);
                return character;
            }
            else
            {
                throw new DataNotFoundException($"Directory for character {name} does not exist.");
            }
        }

        public void Dispose()
        {
            lock (locker)
            {
                foreach (var wurmCharacter in allCharacters.Values.ToArray())
                {
                    wurmCharacter.Dispose();
                }
            }
        }
    }
}