using System;
using System.Collections.Generic;
using AldurSoft.WurmApi.Wurm.Characters;

namespace AldurSoft.WurmApi.Wurm.Logs
{
    /// <summary>
    /// Provides access to wurm log files for specific character.
    /// </summary>
    public interface IWurmCharacterLogFiles
    {
        /// <summary>
        /// Occurs when log files are added or removed for a single character.
        /// </summary>
        event EventHandler FilesAddedOrRemoved;

        /// <summary>
        /// Gets the date of oldest log file for this character.
        /// </summary>
        DateTime OldestLogFileDate { get; }

        /// <summary>
        /// Gets all current log files matching dates, only date part is significant.
        /// </summary>
        IEnumerable<LogFileInfo> TryGetLogFiles(DateTime timeNow, DateTime dateFrom);

        /// <summary>
        /// Gets all current log files matching type and dates, only date part is significant.
        /// </summary>
        IEnumerable<LogFileInfo> TryGetLogFiles(DateTime dateFrom, DateTime dateTo, LogType logType);

        /// <summary>
        /// Gets all current PM log files matching dates and recipient, only date part is significant.
        /// </summary>
        IEnumerable<LogFileInfo> TryGetLogFilesForSpecificPm(
            DateTime dateFrom,
            DateTime dateTo,
            CharacterName pmCharacterName);
    }
}