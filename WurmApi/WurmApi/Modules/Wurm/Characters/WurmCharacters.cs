using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Characters
{
    class WurmCharacters : IWurmCharacters, IDisposable
    {
        readonly IWurmCharacterDirectories characterDirectories;
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmServers wurmServers;
        readonly IWurmServerHistory wurmServerHistory;
        readonly ILogger logger;
        readonly TaskManager taskManager;

        readonly IDictionary<CharacterName, WurmCharacter> allCharacters = new Dictionary<CharacterName, WurmCharacter>();

        readonly object locker = new object();

        public WurmCharacters([NotNull] IWurmCharacterDirectories characterDirectories,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory, [NotNull] ILogger logger, 
            [NotNull] TaskManager taskManager)
        {
            this.characterDirectories = characterDirectories;
            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;
            this.taskManager = taskManager;
            if (characterDirectories == null) throw new ArgumentNullException("characterDirectories");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (taskManager == null) throw new ArgumentNullException("taskManager");

            var allChars = characterDirectories.GetAllCharacters();
            foreach (var characterName in allChars)
            {
                try
                {
                    Create(characterName);
                }
                catch (Exception exception)
                {
                    logger.Log(LogLevel.Error, "Could not initialize character for name {0}", this, exception);
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
            if (name == null) throw new ArgumentNullException("name");

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
                    taskManager
                    );
                allCharacters.Add(name, character);
                return character;
            }
            else
            {
                throw new DataNotFoundException(string.Format("Directory for character {0} does not exist.", name));
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