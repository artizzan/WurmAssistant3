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
        /// </summary>
        /// <param name="searchParameters">The search parameters.</param>
        /// <exception cref="WurmApiException">No manager for this character found</exception>
        IEnumerable<LogFileInfo> TryGetLogFiles(LogSearchParameters searchParameters);

        /// <summary>
        /// Gets the log files provider for specific character.
        /// </summary>
        /// <param name="characterName">Name of the character.</param>
        /// <exception cref="WurmApiException">No manager for this character found</exception>
        IWurmCharacterLogFiles GetManagerForCharacter(CharacterName characterName);
    }
}