using System;
using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Modules.Events.Internal;
using AldursLab.WurmApi.Modules.Events.Internal.Messages;
using AldursLab.WurmApi.Utility;
using AldursLab.WurmApi.Validation;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Modules.Wurm.CharacterDirectories
{
    /// <summary>
    /// Manages directory information about wurm character folders
    /// </summary>
    class WurmCharacterDirectories : WurmSubdirsMonitor, IWurmCharacterDirectories
    {
        readonly IWurmPaths wurmPaths;
        readonly IInternalEventAggregator eventAggregator;

        public WurmCharacterDirectories([NotNull] IWurmPaths wurmPaths, [NotNull] IInternalEventAggregator eventAggregator,
            TaskManager taskManager, IWurmApiLogger logger)
            : base(
                wurmPaths.CharactersDirFullPath,
                taskManager,
                () => eventAggregator.Send(new CharacterDirectoriesChanged()),
                logger,
                ValidateDirectory,
                wurmPaths)
        {
            if (wurmPaths == null) throw new ArgumentNullException(nameof(wurmPaths));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            this.wurmPaths = wurmPaths;
            this.eventAggregator = eventAggregator;
        }

        static void ValidateDirectory(string directoryPath, IWurmPaths wurmPaths)
        {
            CharacterDirectoryValidator.ValidateFullPath(directoryPath, wurmPaths);
        }

        public string GetFullDirPathForCharacter([NotNull] CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException(nameof(characterName));
            return GetFullPathForDirName(characterName.Normalized);
        }

        public IEnumerable<CharacterName> GetAllCharacters()
        {
            return base.AllDirectoryNamesNormalized.Select(s => new CharacterName(s)).ToArray();
        }

        public bool Exists(CharacterName characterName)
        {
            return base.AllDirectoryNamesNormalized.Any(s => new CharacterName(s) == characterName);
        }
    }
}
