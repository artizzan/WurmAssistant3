using System;
using System.Collections.Generic;

namespace AldurSoft.WurmApi
{
    /// <summary>
    /// Provides access to wurm log files for specific character.
    /// </summary>
    public interface IWurmCharacterLogFiles
    {
        /// <summary>
        /// Gets the date of oldest log file for this character.
        /// </summary>
        DateTime OldestLogFileDate { get; }

        /// <summary>
        /// Informs if any log files have already been identified.
        /// This may be false if errors occour.
        /// </summary>
        bool HasBeenInitiallyRefreshed { get; }

        /// <summary>
        /// Gets all current log files matching dates, only date part is significant.
        /// </summary>
        /// <exception cref="Exception"></exception>
        IEnumerable<LogFileInfo> GetLogFiles(DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Gets all current log files matching type and dates, only date part is significant.
        /// </summary>
        /// <exception cref="Exception"></exception>
        IEnumerable<LogFileInfo> GetLogFiles(DateTime dateFrom, DateTime dateTo, LogType logType);

        /// <summary>
        /// Gets all current PM log files matching dates and recipient, only date part is significant.
        /// </summary>
        /// <exception cref="Exception"></exception>
        IEnumerable<LogFileInfo> TryGetLogFilesForSpecificPm(
            DateTime dateFrom,
            DateTime dateTo,
            CharacterName pmCharacterName);
    }
}