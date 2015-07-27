using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides means of working with wurm in-game characters.
    /// </summary>
    public interface IWurmCharacters
    {
        IEnumerable<IWurmCharacter> All { get; }

        /// <summary>
        /// Returns character matching the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        IWurmCharacter Get(CharacterName name);
    }
}