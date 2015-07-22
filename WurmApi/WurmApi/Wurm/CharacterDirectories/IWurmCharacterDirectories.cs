using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.Wurm.Characters;

namespace AldurSoft.WurmApi.Wurm.CharacterDirectories
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