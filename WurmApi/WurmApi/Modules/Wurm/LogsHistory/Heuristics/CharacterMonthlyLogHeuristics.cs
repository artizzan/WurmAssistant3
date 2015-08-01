using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials;
using AldursLab.PersistentObjects;
using AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics.PersistentModel;

namespace AldurSoft.WurmApi.Modules.Wurm.LogsHistory.Heuristics
{
    // 2014-11-11: 
    // Note that in current LogFileStreamReader implementation, start position for
    // future day may actually point to last char in the log file.
    // This is, because there is nothing after it. There are no guarantees, how many bytes
    // have to be added to properly indicate beginning of next line.
    // For that reason, any log reader relying on these heuristics,
    // has to be prepared to handle malformed lines.
    // This is almost guaranteed to happen after automatic rebuilding cache on midnight.

    public class CharacterMonthlyLogHeuristics
    {
        private readonly IPersistent<WurmCharacterLogsEntity> persistentData;
        private readonly MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory;
        private readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

        public CharacterMonthlyLogHeuristics(AldursLab.PersistentObjects.IPersistent<WurmCharacterLogsEntity> persistentData,
            MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (persistentData == null) throw new ArgumentNullException("persistentData");
            if (monthlyHeuristicsExtractorFactory == null) throw new ArgumentNullException("monthlyHeuristicsExtractorFactory");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            this.persistentData = persistentData;
            this.monthlyHeuristicsExtractorFactory = monthlyHeuristicsExtractorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;
        }

        public MonthlyFileHeuristics GetFullHeuristicsForMonth(LogFileInfo logFileInfo)
        {
            WurmLogMonthlyFile fileData = GetEntityForFile(logFileInfo);
            return CreateMonthlyFileHeuristics(fileData);
        }

        private MonthlyFileHeuristics CreateMonthlyFileHeuristics(WurmLogMonthlyFile wurmLogMonthlyFile)
        {
            return new MonthlyFileHeuristics(
                wurmLogMonthlyFile.LogDate,
                wurmLogMonthlyFile.DayToHeuristicsMap.ToDictionary(
                    pair => pair.Key,
                    pair => new DayInfo(pair.Value.FilePositionInBytes, pair.Value.LinesCount)));
        }

        private WurmLogMonthlyFile GetEntityForFile(LogFileInfo logFileInfo)
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
            if (fileData.LastKnownSizeInBytes < fileInfo.Length)
            {
                var extractor = monthlyHeuristicsExtractorFactory.Create(logFileInfo);
                var results = extractor.ExtractDayToPositionMapAsync();
                fileData.LogDate = results.LogDate;
                fileData.DayToHeuristicsMap = results.Heuristics;
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