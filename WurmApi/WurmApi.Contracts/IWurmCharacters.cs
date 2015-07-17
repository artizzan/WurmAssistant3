using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of working with wurm in-game characters.
    /// </summary>
    public interface IWurmCharacters
    {
        IEnumerable<IWurmCharacter> All { get; }

        IWurmCharacter Get(CharacterName name);
    }
}