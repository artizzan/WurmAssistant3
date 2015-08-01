using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.Characters
{
    public class WurmCharacters : IWurmCharacters, IDisposable
    {
        readonly IWurmCharacterDirectories characterDirectories;
        readonly IWurmConfigs wurmConfigs;
        readonly IWurmServers wurmServers;
        readonly IWurmServerHistory wurmServerHistory;
        readonly ILogger logger;
        
        readonly IDictionary<CharacterName, WurmCharacter> allCharacters = new Dictionary<CharacterName, WurmCharacter>();

        readonly object locker = new object();

        public WurmCharacters([NotNull] IWurmCharacterDirectories characterDirectories,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory, [NotNull] ILogger logger)
        {
            this.characterDirectories = characterDirectories;
            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.logger = logger;
            if (characterDirectories == null) throw new ArgumentNullException("characterDirectories");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");
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
                    character = TryCreate(name);
                }
                if (character == null)
                {
                    throw DataNotFoundException.CreateFromKeys(name);
                }
                return character;
            }
        }

        WurmCharacter TryCreate(CharacterName name)
        {
            WurmCharacter character = null;
            if (characterDirectories.Exists(name))
            {
                character = new WurmCharacter(
                    name,
                    characterDirectories.GetFullDirPathForCharacter(name),
                    wurmConfigs,
                    wurmServers,
                    wurmServerHistory,
                    logger);
                allCharacters.Add(name, character);
            }
            return character;
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