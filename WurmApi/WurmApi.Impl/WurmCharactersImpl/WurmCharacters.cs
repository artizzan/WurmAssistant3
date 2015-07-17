using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Impl.WurmCharactersImpl
{
    public class WurmCharacters : IWurmCharacters, IRequireRefresh, IDisposable
    {
        private readonly IWurmCharacterDirectories characterDirectories;
        private readonly IWurmConfigs wurmConfigs;
        private readonly IWurmServers wurmServers;
        private readonly IWurmServerHistory wurmServerHistory;
        private readonly IThreadGuard threadGuard;
        private readonly Dictionary<CharacterName, WurmCharacter> allCharacters = new Dictionary<CharacterName, WurmCharacter>();

        public WurmCharacters(
            IWurmCharacterDirectories characterDirectories,
            IWurmConfigs wurmConfigs,
            IWurmServers wurmServers,
            IWurmServerHistory wurmServerHistory,
            IThreadGuard threadGuard)
        {
            if (characterDirectories == null) throw new ArgumentNullException("characterDirectories");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (threadGuard == null) throw new ArgumentNullException("threadGuard");
            this.characterDirectories = characterDirectories;
            this.wurmConfigs = wurmConfigs;
            this.wurmServers = wurmServers;
            this.wurmServerHistory = wurmServerHistory;
            this.threadGuard = threadGuard;

            RebuildCharacters();

            characterDirectories.DirectoriesChanged += (sender, args) => RebuildCharacters();
        }

        private void RebuildCharacters()
        {
            var allChars = characterDirectories.GetAllCharacters();
            foreach (var characterName in allChars)
            {
                WurmCharacter character;
                if (!allCharacters.TryGetValue(characterName, out character))
                {
                    character = new WurmCharacter(
                        characterName,
                        characterDirectories.GetFullDirPathForCharacter(characterName),
                        wurmConfigs,
                        wurmServers,
                        wurmServerHistory,
                        threadGuard);
                    allCharacters.Add(characterName, character);
                }
            }
        }

        public IEnumerable<IWurmCharacter> All
        {
            get
            {
                threadGuard.ValidateCurrentThread();
                return allCharacters.Values.ToArray();
            }
        }

        /// <exception cref="WurmApiException">Character with this name does not exist.</exception>
        public IWurmCharacter Get(CharacterName name)
        {
            threadGuard.ValidateCurrentThread();
            WurmCharacter character;
            if (!allCharacters.TryGetValue(name, out character))
            {
                throw new WurmApiException(string.Format("Character {0} does not exist.", name));
            }
            return character;
        }

        public void Refresh()
        {
            foreach (var wurmCharacter in allCharacters.Values.ToArray())
            {
                wurmCharacter.Refresh();
            }
        }

        public void Dispose()
        {
            foreach (var wurmCharacter in allCharacters.Values.ToArray())
            {
                wurmCharacter.Dispose();
            }
        }
    }
}