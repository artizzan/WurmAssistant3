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
        /// Gets the log files based on search parameters.
        /// Returns zero-count enumerable if nothing found.
        /// </summary>
        /// <param name="searchParameters">The search parameters.</param>
        [Obsolete]
        IEnumerable<LogFileInfo> TryGetLogFiles(LogSearchParameters searchParameters);

        /// <summary>
        /// Gets the log files provider for specific character.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <exception cref="DataNotFoundException">This character does not appear to exist.</exception>
        IWurmCharacterLogFiles GetManagerForCharacter(CharacterName characterName);
    }
}