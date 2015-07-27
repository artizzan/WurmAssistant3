using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public interface IWurmCharacterDirectories
    {
        IEnumerable<string> AllDirectoryNamesNormalized { get; }

        IEnumerable<string> AllDirectoriesFullPaths { get; }

        event EventHandler<EventArgs> DirectoriesChanged;

        /// <summary>
        /// </summary>
        /// <param name="characterName"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        string GetFullDirPathForCharacter(CharacterName characterName);

        IEnumerable<CharacterName> GetAllCharacters();
    }
}