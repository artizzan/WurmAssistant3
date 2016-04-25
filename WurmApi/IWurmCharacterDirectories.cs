using System.Collections.Generic;

namespace AldursLab.WurmApi
{
    public interface IWurmCharacterDirectories
    {
        /// <summary>
        /// </summary>
        IEnumerable<string> AllDirectoryNamesNormalized { get; }

        /// <summary>
        /// </summary>
        IEnumerable<string> AllDirectoriesFullPaths { get; }

        /// <summary>
        /// Returns absolute directory path for specified character.
        /// </summary>
        /// <param name="characterName">Case insensitive</param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException">
        /// Directory for this character is not available.
        /// </exception>
        string GetFullDirPathForCharacter(CharacterName characterName);

        /// <summary>
        /// Returns all game characters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<CharacterName> GetAllCharacters();

        bool Exists(CharacterName characterName);
    }
}