using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Internal.Messages;
using AldurSoft.WurmApi.Modules.Events.Public;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogFiles
{
    /// <summary>
    /// Provides accurate information about Wurm log files.
    /// </summary>
    public class WurmLogFiles : IWurmLogFiles, IDisposable, IHandle<CharacterDirectoriesChanged>
    {
        readonly IWurmCharacterDirectories wurmCharacterDirectories;
        readonly ILogger logger;
        readonly IWurmLogDefinitions wurmLogDefinitions;
        readonly IInternalEventAggregator eventAggregator;
        readonly IPublicEventInvoker publicEventInvoker;

        IReadOnlyDictionary<CharacterName, WurmCharacterLogFiles> characterNormalizedNameToWatcherMap =
            new Dictionary<CharacterName, WurmCharacterLogFiles>();

        volatile int rebuildRequired = 1;
        readonly object locker = new object();

        internal WurmLogFiles(IWurmCharacterDirectories wurmCharacterDirectories, ILogger logger, IWurmLogDefinitions wurmLogDefinitions,
            [NotNull] IInternalEventAggregator eventAggregator, [NotNull] IPublicEventInvoker publicEventInvoker)
        {
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmLogDefinitions == null) throw new ArgumentNullException("wurmLogDefinitions");
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            if (publicEventInvoker == null) throw new ArgumentNullException("publicEventInvoker");
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.logger = logger;
            this.wurmLogDefinitions = wurmLogDefinitions;
            this.eventAggregator = eventAggregator;
            this.publicEventInvoker = publicEventInvoker;

            eventAggregator.Subscribe(this);
        }

        public void Handle(CharacterDirectoriesChanged message)
        {
            rebuildRequired = 1;
        }

        public IEnumerable<LogFileInfo> TryGetLogFiles(LogSearchParameters searchParameters)
        {
            searchParameters.AssertAreValid();

            Refresh();

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
                    this,
                    null);
                return new LogFileInfo[0];
            }
        }

        public IWurmCharacterLogFiles GetManagerForCharacter([NotNull] CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");

            Refresh();

            WurmCharacterLogFiles manager;
            characterNormalizedNameToWatcherMap.TryGetValue(characterName, out manager);
            if (manager == null)
            {
                throw new DataNotFoundException("No manager found for character " + characterName);
            }
            return manager;
        }


        private void Refresh()
        {
            if (rebuildRequired == 1)
            {
                lock (locker)
                {
                    if (Interlocked.CompareExchange(ref rebuildRequired, 0, 1) == 1)
                    {
                        var allDirNames = wurmCharacterDirectories.AllDirectoryNamesNormalized.ToArray();
                        var oldMap = characterNormalizedNameToWatcherMap;
                        var newMap = new Dictionary<CharacterName, WurmCharacterLogFiles>();

                        AddNewFileManagers(allDirNames, oldMap, newMap);

                        DisposeOldFileManagers(oldMap, allDirNames);

                        characterNormalizedNameToWatcherMap = newMap;
                    }
                }
            }
            
        }

        void DisposeOldFileManagers(IReadOnlyDictionary<CharacterName, WurmCharacterLogFiles> oldMap, string[] allDirNames)
        {
            foreach (var characterName in oldMap.Keys.ToArray())
            {
                if (!allDirNames.Contains(characterName.Normalized))
                {
                    WurmCharacterLogFiles logFiles;
                    if (oldMap.TryGetValue(characterName, out logFiles))
                    {
                        logFiles.Dispose();
                    }
                    else
                    {
                        logger.Log(
                            LogLevel.Warn,
                            string.Format(
                                "Characters directory no longer contains character {0}, but that character logs were not being monitored.",
                                characterName),
                            this,
                            null);
                    }
                }
            }
        }

        void AddNewFileManagers(string[] allDirNames, IReadOnlyDictionary<CharacterName, WurmCharacterLogFiles> oldMap, Dictionary<CharacterName, WurmCharacterLogFiles> newMap)
        {
            foreach (var dirName in allDirNames)
            {
                var charName = new CharacterName(dirName);
                WurmCharacterLogFiles logFiles;
                if (!oldMap.TryGetValue(charName, out logFiles))
                {
                    try
                    {
                        var fullDirPathForCharacter = wurmCharacterDirectories.GetFullDirPathForCharacter(charName);
                        var logsDirPath = Path.Combine(fullDirPathForCharacter, "logs");

                        logFiles = new WurmCharacterLogFiles(
                            charName,
                            logsDirPath,
                            logger,
                            new LogFileInfoFactory(wurmLogDefinitions, logger),
                            publicEventInvoker);

                        newMap.Add(charName, logFiles);
                    }
                    catch (DataNotFoundException exception)
                    {
                        logger.Log(LogLevel.Warn,
                            string.Format(
                                "Could not create log files manager for character {0} due to missing logs directory. Perhaps it was deleted?",
                                charName), this, exception);
                    }
                }
                else
                {
                    newMap.Add(charName, logFiles);
                }
            }
        }

        public void Dispose()
        {
            eventAggregator.Unsubscribe(this);
            foreach (var characterLogFiles in characterNormalizedNameToWatcherMap.Values)
            {
                characterLogFiles.Dispose();
            }
        }
    }
}
