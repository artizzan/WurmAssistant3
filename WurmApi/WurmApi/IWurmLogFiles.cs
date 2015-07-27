using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides access to wurm log files.
    /// </summary>
    public interface IWurmLogFiles
    {
        /// <summary>
        /// Gets the log files provider for specific character.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <exception cref="DataNotFoundException">This character does not appear to exist.</exception>
        IWurmCharacterLogFiles GetManagerForCharacter(CharacterName characterName);
    }
}