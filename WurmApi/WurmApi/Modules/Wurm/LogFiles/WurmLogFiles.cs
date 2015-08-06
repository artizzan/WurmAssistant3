using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.JobRunning;
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
        readonly IInternalEventInvoker internalEventInvoker;
        readonly TaskManager taskManager;

        IReadOnlyDictionary<CharacterName, WurmCharacterLogFiles> characterNormalizedNameToWatcherMap =
            new Dictionary<CharacterName, WurmCharacterLogFiles>();

        readonly object locker = new object();

        readonly TaskHandle taskHandle;

        internal WurmLogFiles(IWurmCharacterDirectories wurmCharacterDirectories, ILogger logger, IWurmLogDefinitions wurmLogDefinitions,
            [NotNull] IInternalEventAggregator eventAggregator, [NotNull] IInternalEventInvoker internalEventInvoker,
            [NotNull] TaskManager taskManager)
        {
            if (wurmCharacterDirectories == null) throw new ArgumentNullException("wurmCharacterDirectories");
            if (logger == null) throw new ArgumentNullException("logger");
            if (wurmLogDefinitions == null) throw new ArgumentNullException("wurmLogDefinitions");
            if (eventAggregator == null) throw new ArgumentNullException("eventAggregator");
            if (internalEventInvoker == null) throw new ArgumentNullException("internalEventInvoker");
            if (taskManager == null) throw new ArgumentNullException("taskManager");
            this.wurmCharacterDirectories = wurmCharacterDirectories;
            this.logger = logger;
            this.wurmLogDefinitions = wurmLogDefinitions;
            this.eventAggregator = eventAggregator;
            this.internalEventInvoker = internalEventInvoker;
            this.taskManager = taskManager;

            try
            {
                Refresh();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at initial WurmLogFiles refresh", this, exception);
            }

            eventAggregator.Subscribe(this);
            
            taskHandle = new TaskHandle(Refresh, "WurmLogFiles refresh");
            taskManager.Add(taskHandle);

            taskHandle.Trigger();
        }

        public void Handle(CharacterDirectoriesChanged message)
        {
            taskHandle.Trigger();
        }

        public IWurmCharacterLogFiles GetForCharacter([NotNull] CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");

            WurmCharacterLogFiles manager;
            characterNormalizedNameToWatcherMap.TryGetValue(characterName, out manager);
            if (manager == null)
            {
                throw new DataNotFoundException("No manager found for character " + characterName);
            }
            return manager;
        }


        void Refresh()
        {
            List<Exception> errors = null;
            lock (locker)
            {
                var allDirNames = wurmCharacterDirectories.AllDirectoryNamesNormalized.ToArray();
                var oldMap = characterNormalizedNameToWatcherMap;
                var newMap = new Dictionary<CharacterName, WurmCharacterLogFiles>();

                errors = AddNewFileManagers(allDirNames, oldMap, newMap);

                characterNormalizedNameToWatcherMap = newMap;
            }
            if (errors != null && errors.Any())
            {
                throw new AggregateException(errors);
            }
        }

        List<Exception> AddNewFileManagers(string[] allDirNames, IReadOnlyDictionary<CharacterName, WurmCharacterLogFiles> oldMap, Dictionary<CharacterName, WurmCharacterLogFiles> newMap)
        {
            List<Exception> exceptions = new List<Exception>();

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
                            internalEventInvoker,
                            taskManager);

                        newMap.Add(charName, logFiles);
                    }
                    catch (Exception exception)
                    {
                        logger.Log(LogLevel.Error, "Error at WurmLogFiles refresh for character: " + charName, this, exception);
                        exceptions.Add(exception);
                    }
                }
                else
                {
                    newMap.Add(charName, logFiles);
                }
            }

            return exceptions;
        }

        public void Dispose()
        {
            taskManager.Remove(taskHandle);
            eventAggregator.Unsubscribe(this);
            foreach (var characterLogFiles in characterNormalizedNameToWatcherMap.Values)
            {
                characterLogFiles.Dispose();
            }
        }
    }
}
