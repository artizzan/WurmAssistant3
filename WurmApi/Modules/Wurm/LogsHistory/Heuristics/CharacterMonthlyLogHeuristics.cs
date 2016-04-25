using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;
using AldursLab.WurmApi.PersistentObjects;

namespace AldursLab.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    // 2014-11-11: 
    // Note that in current LogFileStreamReader implementation, start position for
    // future day may actually point to last char in the log file.
    // This is, because there is nothing after it. There are no guarantees, how many bytes
    // have to be added to properly indicate beginning of next line.
    // For that reason, any log reader relying on these heuristics,
    // has to be prepared to handle malformed lines.
    // This is almost guaranteed to happen after automatic rebuilding cache on midnight.

    class CharacterMonthlyLogHeuristics
    {
        private readonly IPersistent<WurmCharacterLogsEntity> persistentData;
        private readonly MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory;
        private readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

        public CharacterMonthlyLogHeuristics(IPersistent<WurmCharacterLogsEntity> persistentData,
            MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (persistentData == null) throw new ArgumentNullException(nameof(persistentData));
            if (monthlyHeuristicsExtractorFactory == null) throw new ArgumentNullException(nameof(monthlyHeuristicsExtractorFactory));
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException(nameof(wurmCharacterLogFiles));
            this.persistentData = persistentData;
            this.monthlyHeuristicsExtractorFactory = monthlyHeuristicsExtractorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;
        }

        public MonthlyFileHeuristics GetFullHeuristicsForMonth(LogFileInfo logFileInfo)
        {
            WurmLogMonthlyFile fileData = GetEntityForFile(logFileInfo);
            return CreateMonthlyFileHeuristics(fileData);
        }

        MonthlyFileHeuristics CreateMonthlyFileHeuristics(WurmLogMonthlyFile wurmLogMonthlyFile)
        {
            var dayMap = new Dictionary<int, DayInfo>();
            var days = wurmLogMonthlyFile.DayToHeuristicsMap.OrderBy(pair => pair.Key).ToArray();
            int totalLines = 0;
            foreach (var pair in days)
            {
                dayMap.Add(pair.Key,
                    new DayInfo(
                        pair.Value.FilePositionInBytes,
                        pair.Value.LinesCount,
                        totalLines
                        ));
                totalLines += pair.Value.LinesCount;
            }

            return new MonthlyFileHeuristics(
                wurmLogMonthlyFile.LogDate,
                dayMap,
                wurmLogMonthlyFile.HasValidBytePositions);
        }

        WurmLogMonthlyFile GetEntityForFile(LogFileInfo logFileInfo)
        {
            WurmLogMonthlyFile fileData;
            bool isNewFile = false;
            bool needsSaving = false;
            if (!persistentData.Entity.WurmLogFiles.TryGetValue(logFileInfo.FileNameNormalized, out fileData))
            {
                fileData = new WurmLogMonthlyFile { FileName = logFileInfo.FileNameNormalized };
                isNewFile = true;
                needsSaving = true;
            }

            FileInfo fileInfo = new FileInfo(logFileInfo.FullPath);
            var fileLength = fileInfo.Length;
            if (fileData.LastKnownSizeInBytes < fileLength)
            {
                var extractor = monthlyHeuristicsExtractorFactory.Create(logFileInfo);
                var results = extractor.ExtractDayToPositionMap();
                fileData.LogDate = results.LogDate;
                fileData.DayToHeuristicsMap = results.Heuristics;
                fileData.HasValidBytePositions = results.HasValidBytePositions;
                fileData.LastKnownSizeInBytes = fileLength;
                needsSaving = true;
            }
            fileData.LastUpdated = Time.Get.LocalNowOffset; 

            if (isNewFile) persistentData.Entity.WurmLogFiles.Add(fileData.FileName, fileData);
            if (needsSaving) persistentData.FlagAsChanged();
            return fileData;
        }

        DateTimeOffset GetDateWithoutTime(DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, 0, dateTimeOffset.Offset);
        }
    }
}