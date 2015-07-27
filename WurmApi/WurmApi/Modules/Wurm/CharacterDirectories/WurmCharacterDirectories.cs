using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Utility;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Modules.Wurm.CharacterDirectories
{
    /// <summary>
    /// Manages directory information about wurm character folders
    /// </summary>
    public class WurmCharacterDirectories : WurmSubdirsMonitor, IWurmCharacterDirectories
    {
        public WurmCharacterDirectories(
            IWurmPaths wurmPaths, ILogger logger) : base(wurmPaths.CharactersDirFullPath, logger)
        {
        }

        public string GetFullDirPathForCharacter([NotNull] CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            return GetFullPathForDirName(characterName.Normalized);
        }

        public IEnumerable<CharacterName> GetAllCharacters()
        {
            return base.AllDirectoryNamesNormalized.Select(s => new CharacterName(s)).ToArray();
        }
    }
}
