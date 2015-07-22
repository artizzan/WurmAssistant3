using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Logging;
using AldurSoft.WurmApi.Wurm.CharacterDirectories;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Logs.Searching;

namespace AldurSoft.WurmApi.Wurm.Logs.WurmLogFilesModule
{
    /// <summary>
    /// Provides accurate information about Wurm log files.
    /// </summary>
    public class WurmLogFiles : IWurmLogFiles, IRequireRefresh, IDisposable
    {
        private readonly IWurmCharacterDirectories wurmCharacterDirectories;
        private readonly ILogger logger;
        private readonly IWurmLogDefinitions wurmLogDefinitions;

        private readonly Dictionary<CharacterName, WurmCharacterLogFiles> characterNormalizedNameToWatcherMap =
            new Dictionary<CharacterName, WurmCharacterLogFiles>();
        private readonly List<CharacterName> charsThatRequireRetry = new List<CharacterName>();

        public WurmLogFiles(IWurmCharacterDirectories wurmCharacterDirectories, ILogger logger, IWurmLogDefinitions wurmLogDefinitions)
        {
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmLogDefinitions == null) throw new ArgumentNullException("wurmLogDefinitions");
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.logger = logger;
            this.wurmLogDefinitions = wurmLogDefinitions;

            var characters = wurmCharacterDirectories.AllDirectoryNamesNormalized.Select(s => new CharacterName(s));
            foreach (var character in characters)
            {
                var manager = TryConstructForCharacter(character);
                if (manager == null)
                {
                    charsThatRequireRetry.Add(character);
                }
                else
                {
                    characterNormalizedNameToWatcherMap.Add(character, manager);
                }
            }

            wurmCharacterDirectories.DirectoriesChanged += WurmCharacterDirectoriesOnDirectoriesChanged;
        }

        public IEnumerable<LogFileInfo> TryGetLogFiles(LogSearchParameters searchParameters)
        {
            searchParameters.AssertAreValid();

            WurmCharacterLogFiles characterLogFiles;
            if (characterNormalizedNameToWatcherMap.TryGetValue(searchParameters.CharacterName, out characterLogFiles))
            {
                if (searchParameters.PmCharacterName != null)
                {
                    return characterLogFiles.TryGetLogFilesForSpecificPm(
                        searchParameters.DateFrom,
                        searchParameters.DateTo,
                        searchParameters.PmCharacterName);
                }
                else
                {
                    return characterLogFiles.TryGetLogFiles(searchParameters.DateFrom,
                        searchParameters.DateTo, searchParameters.LogType);
                }
            }
            else
            {
                logger.Log(
                    LogLevel.Warn,
                    string.Format("No log files watcher found for given search parameters: {0}", searchParameters),
                    this);
                return new LogFileInfo[0];
            }
        }

        public IWurmCharacterLogFiles GetManagerForCharacter(CharacterName characterName)
        {
            WurmCharacterLogFiles manager;
            characterNormalizedNameToWatcherMap.TryGetValue(characterName, out manager);
            if (manager == null)
            {
                RefreshManagers();
                characterNormalizedNameToWatcherMap.TryGetValue(characterName, out manager);
                if (manager == null)
                {
                    throw new WurmApiException("No manager for character " + characterName);
                }
            }
            return manager;
        }

        private void WurmCharacterDirectoriesOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            RefreshManagers();
        }

        private void RefreshManagers()
        {
            var allDirNames = wurmCharacterDirectories.AllDirectoryNamesNormalized.ToArray();
            foreach (var dirName in allDirNames)
            {
                var charName = new CharacterName(dirName);
                WurmCharacterLogFiles logFiles;
                if (!characterNormalizedNameToWatcherMap.TryGetValue(charName, out logFiles))
                {
                    var filesManager = TryConstructForCharacter(charName);
                    if (filesManager == null)
                    {
                        charsThatRequireRetry.Add(charName);
                    }
                    else
                    {
                        characterNormalizedNameToWatcherMap.Add(charName, filesManager);
                    }
                }
            }

            foreach (var characterName in characterNormalizedNameToWatcherMap.Keys.ToArray())
            {
                if (!allDirNames.Contains(characterName.Normalized))
                {
                    WurmCharacterLogFiles logFiles;
                    if (characterNormalizedNameToWatcherMap.TryGetValue(characterName, out logFiles))
                    {
                        logFiles.Dispose();
                        characterNormalizedNameToWatcherMap.Remove(characterName);
                    }
                    else
                    {
                        logger.Log(
                            LogLevel.Warn,
                            string.Format(
                                "Characters directory no longer contains character {0}, but such character logs were not being monitored.",
                                characterName),
                            this);
                    }
                }
            }
        }

        private WurmCharacterLogFiles TryConstructForCharacter(CharacterName characterName)
        {
            var path = GetLogsDirForCharacter(characterName);

            if (!Directory.Exists(path))
            {
                return null;
            }

            return new WurmCharacterLogFiles(
                characterName,
                path,
                logger,
                new LogFileInfoFactory(wurmLogDefinitions, logger));
        }

        private string GetLogsDirForCharacter(CharacterName characterName)
        {
            return Path.Combine(GetPlayerDirForCharacter(characterName), "logs");
        }

        private string GetPlayerDirForCharacter(CharacterName characterName)
        {
            return wurmCharacterDirectories.GetFullDirPathForCharacter(characterName);
        }

        public void Refresh()
        {
            foreach (var characterLogFiles in characterNormalizedNameToWatcherMap.Values)
            {
                characterLogFiles.Refresh();
            }
            if (charsThatRequireRetry.Any())
            {
                foreach (var characterName in charsThatRequireRetry)
                {
                    if (!characterNormalizedNameToWatcherMap.ContainsKey(characterName))
                    {
                        var manager = TryConstructForCharacter(characterName);
                        if (manager != null)
                        {
                            characterNormalizedNameToWatcherMap.Add(characterName, manager);
                            charsThatRequireRetry.Remove(characterName);
                        }
                        else if (!Directory.Exists(GetPlayerDirForCharacter(characterName)))
                        {
                            charsThatRequireRetry.Remove(characterName);
                        }
                    }
                    else
                    {
                        charsThatRequireRetry.Remove(characterName);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var characterLogFiles in characterNormalizedNameToWatcherMap.Values)
            {
                characterLogFiles.Dispose();
            }
        }
    }
}
