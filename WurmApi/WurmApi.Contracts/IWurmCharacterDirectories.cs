using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    public interface IWurmCharacterDirectories
    {
        IEnumerable<string> AllDirectoryNamesNormalized { get; }

        IEnumerable<string> AllDirectoriesFullPaths { get; }

        event EventHandler DirectoriesChanged;

        string GetFullDirPathForCharacter(CharacterName characterName);

        IEnumerable<CharacterName> GetAllCharacters();
    }
}