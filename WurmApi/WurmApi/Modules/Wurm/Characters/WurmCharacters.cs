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
        IReadOnlyDictionary<CharacterName, WurmCharacter> allCharacters = new Dictionary<CharacterName, WurmCharacter>();
        readonly RepeatableThreadedOperation rebuilder;

        public WurmCharacters([NotNull] IWurmCharacterDirectories characterDirectories,
            [NotNull] IWurmConfigs wurmConfigs, [NotNull] IWurmServers wurmServers,
            [NotNull] IWurmServerHistory wurmServerHistory, [NotNull] ILogger logger)
        {
            if (characterDirectories == null) throw new ArgumentNullException("characterDirectories");
            if (wurmConfigs == null) throw new ArgumentNullException("wurmConfigs");
            if (wurmServers == null) throw new ArgumentNullException("wurmServers");
            if (wurmServerHistory == null) throw new ArgumentNullException("wurmServerHistory");
            if (logger == null) throw new ArgumentNullException("logger");

            rebuilder = new RepeatableThreadedOperation(() =>
            {
                var allChars = characterDirectories.GetAllCharacters();
                Dictionary<CharacterName, WurmCharacter> newDict = new Dictionary<CharacterName, WurmCharacter>();
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
                            logger);

                    }
                    newDict.Add(characterName, character);
                }
                allCharacters = newDict;
            });

            rebuilder.OperationError +=
                (sender, args) =>
                {
                    const int retryDelay = 5;
                    logger.Log(LogLevel.Error,
                        string.Format("Error during characters rebuild, retrying in {0} seconds", retryDelay), this, args.Exception);
                    rebuilder.DelayedSignal(TimeSpan.FromSeconds(retryDelay));
                };

            //characterDirectories.DirectoriesChanged += (sender, args) => rebuilder.Signal();
            rebuilder.Signal();
            rebuilder.WaitSynchronouslyForInitialOperation(TimeSpan.FromSeconds(30));
        }

        public IEnumerable<IWurmCharacter> All
        {
            get
            {
                return allCharacters.Values.ToArray();
            }
        }

        public IWurmCharacter Get([NotNull] CharacterName name)
        {
            if (name == null) throw new ArgumentNullException("name");

            WurmCharacter character;
            if (!allCharacters.TryGetValue(name, out character))
            {
                throw DataNotFoundException.CreateFromKeys(name);
            }
            return character;
        }

        public void Dispose()
        {
            rebuilder.Dispose();
            foreach (var wurmCharacter in allCharacters.Values.ToArray())
            {
                wurmCharacter.Dispose();
            }
        }
    }
}