using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public interface IWurmCharacterDirectories
    {
        IEnumerable<string> AllDirectoryNamesNormalized { get; }

        IEnumerable<string> AllDirectoriesFullPaths { get; }

        /// <summary>
        /// Returns absolute directory path for specified character.
        /// </summary>
        /// <param name="characterName">Case insensitive</param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException">Character does not exist.</exception>
        string GetFullDirPathForCharacter(CharacterName characterName);

        /// <summary>
        /// Returns all game characters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<CharacterName> GetAllCharacters();

        bool Exists(CharacterName characterName);
    }
}