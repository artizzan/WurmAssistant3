using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Wurm.Logs.WurmLogFilesModule
{
    class LogTypeManager
    {
        private List<LogFileInfo> logs = new List<LogFileInfo>();

        /// <summary>
        /// Returns true if any files changed.
        /// </summary>
        /// <param name="logFileInfos"></param>
        /// <returns></returns>
        public bool Rebuild(IEnumerable<LogFileInfo> logFileInfos)
        {
            var newFiles = logFileInfos.OrderBy(info => info.LogFileDate.DateTime).ToList();
            var anyChanged = DetectChanges(logs, newFiles);
            logs = newFiles;
            return anyChanged;
        }

        private bool DetectChanges(List<LogFileInfo> orderedOldFiles, List<LogFileInfo> orderedNewFiles)
        {
            if (orderedOldFiles.Count != orderedNewFiles.Count)
            {
                return true;
            }

            for (int i = 0; i < orderedOldFiles.Count; i++)
            {
                if (orderedOldFiles[i].FileName != orderedNewFiles[i].FileName)
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<LogFileInfo> GetLogFileInfos(DateTime from, DateTime to)
        {
            DateTime dailyAdjustedFrom = new DateTime(from.Year, from.Month, from.Day);
            DateTime monthlyAdjustedFrom = new DateTime(from.Year, from.Month, 1);
            DateTime dailyAdjustedTo = new DateTime(to.Year, to.Month, to.Day);
            DateTime monthlyAdjustedTo = new DateTime(to.Year, to.Month, 1);

            List<LogFileInfo> result = new List<LogFileInfo>();

            foreach (var logFileInfo in logs)
            {
                if (MatchesFromDate(logFileInfo, dailyAdjustedFrom, monthlyAdjustedFrom) &&
                    MatchesToDate(logFileInfo, dailyAdjustedTo, monthlyAdjustedTo))
                {
                    result.Add(logFileInfo);
                }
            }

            return result;
        }

        private static bool MatchesToDate(LogFileInfo logFileInfo, DateTime dailyAdjustedTo, DateTime monthlyAdjustedTo)
        {
            return (logFileInfo.LogFileDate.LogSavingType == LogSavingType.Daily
                    && logFileInfo.LogFileDate.DateTime <= dailyAdjustedTo)
                   || (logFileInfo.LogFileDate.LogSavingType == LogSavingType.Monthly
                       && logFileInfo.LogFileDate.DateTime <= monthlyAdjustedTo);
        }

        private static bool MatchesFromDate(LogFileInfo logFileInfo, DateTime dailyAdjustedFrom, DateTime monthlyAdjustedFrom)
        {
            return (logFileInfo.LogFileDate.LogSavingType == LogSavingType.Daily
                    && logFileInfo.LogFileDate.DateTime >= dailyAdjustedFrom)
                   || (logFileInfo.LogFileDate.LogSavingType == LogSavingType.Monthly
                       && logFileInfo.LogFileDate.DateTime >= monthlyAdjustedFrom);
        }
    }
}