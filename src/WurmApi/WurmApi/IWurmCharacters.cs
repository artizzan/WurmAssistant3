using System;
using System.Collections.Generic;

namespace AldursLab.WurmApi
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
        /// <exception cref="Exception">Character could not be initialized.</exception>
        IWurmCharacter Get(CharacterName name);

        /// <summary>
        /// Returns character matching the name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Character could not be initialized.</exception>
        IWurmCharacter Get(string name);
    }
}