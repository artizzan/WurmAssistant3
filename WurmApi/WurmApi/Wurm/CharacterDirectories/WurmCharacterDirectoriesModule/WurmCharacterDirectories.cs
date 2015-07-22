using System;
using System.Collections.Generic;
using System.Linq;
using AldurSoft.WurmApi.Utility;
using AldurSoft.WurmApi.Wurm.Characters;
using AldurSoft.WurmApi.Wurm.Paths;

namespace AldurSoft.WurmApi.Wurm.CharacterDirectories.WurmCharacterDirectoriesModule
{
    /// <summary>
    /// Manages directory information about wurm character folders
    /// </summary>
    public class WurmCharacterDirectories : WurmSubdirsMonitor, IWurmCharacterDirectories
    {
        public WurmCharacterDirectories(
            IWurmPaths wurmPaths) : base(wurmPaths.CharactersDirFullPath)
        {
        }

        /// <exception cref="WurmApiException">No directory available for this character</exception>
        public string GetFullDirPathForCharacter(CharacterName characterName)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");

            string directoryFullPath = TryGetFullPathForDirName(characterName.Normalized);
            if (directoryFullPath == null)
            {
                throw new WurmApiException("No directory available for character: " + characterName);
            }

            return directoryFullPath;
        }

        public IEnumerable<CharacterName> GetAllCharacters()
        {
            return base.AllDirectoryNamesNormalized.Select(s => new CharacterName(s)).ToArray();
        }
    }
}
