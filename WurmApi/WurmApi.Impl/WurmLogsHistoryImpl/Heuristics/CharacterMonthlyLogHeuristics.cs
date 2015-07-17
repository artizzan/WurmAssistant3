using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AldurSoft.Core;
using AldurSoft.SimplePersist;
using AldurSoft.WurmApi.DataModel.LogsHistoryModel;

namespace AldurSoft.WurmApi.Impl.WurmLogsHistoryImpl.Heuristics
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
        private readonly IPersistent<WurmCharacterLogsEntity> heuristicsRepository;
        private readonly MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory;
        private readonly IWurmCharacterLogFiles wurmCharacterLogFiles;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1,1);

        public CharacterMonthlyLogHeuristics(
            IPersistent<WurmCharacterLogsEntity> heuristicsRepository,
            MonthlyHeuristicsExtractorFactory monthlyHeuristicsExtractorFactory,
            IWurmCharacterLogFiles wurmCharacterLogFiles)
        {
            if (heuristicsRepository == null) throw new ArgumentNullException("heuristicsRepository");
            if (monthlyHeuristicsExtractorFactory == null) throw new ArgumentNullException("monthlyHeuristicsExtractorFactory");
            if (wurmCharacterLogFiles == null) throw new ArgumentNullException("wurmCharacterLogFiles");
            this.heuristicsRepository = heuristicsRepository;
            this.monthlyHeuristicsExtractorFactory = monthlyHeuristicsExtractorFactory;
            this.wurmCharacterLogFiles = wurmCharacterLogFiles;
        }

        public virtual async Task<MonthlyFileHeuristics> GetFullHeuristicsForMonthAsync(LogFileInfo logFileInfo)
        {
            try
            {
                await semaphore.WaitAsync();
                WurmLogMonthlyFile fileData = await GetEntityForFile(logFileInfo);
                return CreateMonthlyFileHeuristics(fileData);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private MonthlyFileHeuristics CreateMonthlyFileHeuristics(WurmLogMonthlyFile wurmLogMonthlyFile)
        {
            return new MonthlyFileHeuristics(
                wurmLogMonthlyFile.LogDate,
                wurmLogMonthlyFile.DayToHeuristicsMap.ToDictionary(
                    pair => pair.Key,
                    pair => new DayInfo(pair.Value.FilePositionInBytes, pair.Value.LinesCount)));
        }

        private async Task<WurmLogMonthlyFile> GetEntityForFile(LogFileInfo logFileInfo)
        {
            WurmLogMonthlyFile fileData;
            bool isNewFile = false;
            bool needsSaving = false;
            if (!heuristicsRepository.Entity.WurmLogFiles.TryGetValue(logFileInfo.FileNameNormalized, out fileData))
            {
                fileData = new WurmLogMonthlyFile { FileName = logFileInfo.FileNameNormalized };
                isNewFile = true;
                needsSaving = true;
            }

            FileInfo fileInfo = new FileInfo(logFileInfo.FullPath);
            if (fileData.LastKnownSizeInBytes < fileInfo.Length)
            {
                var extractor = monthlyHeuristicsExtractorFactory.Create(logFileInfo);
                var results = await extractor.ExtractDayToPositionMapAsync();
                fileData.LogDate = results.LogDate;
                fileData.DayToHeuristicsMap = results.Heuristics;
                needsSaving = true;
            }
            fileData.LastUpdated = Time.Clock.LocalNowOffset; 

            if (isNewFile) heuristicsRepository.Entity.WurmLogFiles.Add(fileData.FileName, fileData);
            if (needsSaving) heuristicsRepository.Save();
            return fileData;
        }

        DateTimeOffset GetDateWithoutTime(DateTimeOffset dateTimeOffset)
        {
            return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, 0, dateTimeOffset.Offset);
        }
    }
}