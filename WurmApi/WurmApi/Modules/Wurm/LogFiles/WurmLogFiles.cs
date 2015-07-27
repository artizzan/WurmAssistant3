using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.LogFiles
{
    /// <summary>
    /// Provides accurate information about Wurm log files.
    /// </summary>
    public class WurmLogFiles : IWurmLogFiles, IDisposable
    {
        readonly IWurmCharacterDirectories wurmCharacterDirectories;
        readonly ILogger logger;
        readonly IWurmLogDefinitions wurmLogDefinitions;

        IReadOnlyDictionary<CharacterName, WurmCharacterLogFiles> characterNormalizedNameToWatcherMap =
            new Dictionary<CharacterName, WurmCharacterLogFiles>();

        Task updaterTask;
        readonly TaskCompletionSource<bool> initialUpdateAwaiter = new TaskCompletionSource<bool>();
        volatile int managersRequireRefresh = 0;
        volatile bool stop = false;

        public WurmLogFiles(IWurmCharacterDirectories wurmCharacterDirectories, ILogger logger, IWurmLogDefinitions wurmLogDefinitions)
        {
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmLogDefinitions == null) throw new ArgumentNullException("wurmLogDefinitions");
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.logger = logger;
            this.wurmLogDefinitions = wurmLogDefinitions;

            updaterTask = new Task(() =>
            {
                while (true)
                {
                    if (stop)
                    {
                        return;
                    }

                    try
                    {
                        if (Interlocked.CompareExchange(ref managersRequireRefresh, 0, 1) == 1)
                        {
                            RefreshManagers();
                            initialUpdateAwaiter.TrySetResult(true);
                        }

                        foreach (var characterLogFiles in characterNormalizedNameToWatcherMap.Values)
                        {
                            characterLogFiles.Refresh();
                        }
                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Error, "Error during log files manager update: " + exception.Message, this, exception);
                    }

                    if (stop)
                    {
                        return;
                    }
                    Thread.Sleep(500);
                }
            }, TaskCreationOptions.LongRunning);
            updaterTask.Start();

            wurmCharacterDirectories.DirectoriesChanged += WurmCharacterDirectoriesOnDirectoriesChanged;
        }

        private void WurmCharacterDirectoriesOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            managersRequireRefresh = 1;
        }

        public IEnumerable<LogFileInfo> TryGetLogFiles(LogSearchParameters searchParameters)
        {
            searchParameters.AssertAreValid();

            WaitForInitialUpdate();

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

            WaitForInitialUpdate();

            WurmCharacterLogFiles manager;
            characterNormalizedNameToWatcherMap.TryGetValue(characterName, out manager);
            if (manager == null)
            {
                throw new DataNotFoundException("No manager found for character " + characterName);
            }
            return manager;
        }

        void WaitForInitialUpdate()
        {
            if (!initialUpdateAwaiter.Task.Wait(TimeSpan.FromSeconds(30)))
            {
                throw new TimeoutException();
            }
        }

        private void RefreshManagers()
        {
            var allDirNames = wurmCharacterDirectories.AllDirectoryNamesNormalized.ToArray();
            var oldMap = characterNormalizedNameToWatcherMap;
            var newMap = new Dictionary<CharacterName, WurmCharacterLogFiles>();

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
                            new LogFileInfoFactory(wurmLogDefinitions, logger));

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

            characterNormalizedNameToWatcherMap = newMap;
        }

        public void Dispose()
        {
            stop = true;
            updaterTask.Wait();
            foreach (var characterLogFiles in characterNormalizedNameToWatcherMap.Values)
            {
                characterLogFiles.Dispose();
            }
        }
    }
}
