using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldurSoft.Core;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.Utility;

namespace AldurSoft.WurmApi.Modules.Wurm.LogFiles
{
    public class WurmCharacterLogFiles : IWurmCharacterLogFiles, IDisposable, IRequireRefresh
    {
        private readonly ILogger logger;
        private readonly LogFileInfoFactory logFileInfoFactory;

        public CharacterName CharacterName { get; private set; }
        public string FullDirPathToCharacterLogsDir { get; private set; }

        private readonly Dictionary<LogType, LogTypeManager> wurmLogTypeToLogTypeManagerMap =
            new Dictionary<LogType, LogTypeManager>();

        private readonly FileSystemChangeMonitor fileChangeMonitor;

        private DateTime oldestLogFileDate = Time.Clock.LocalNow;

        public DateTime OldestLogFileDate { get { return oldestLogFileDate; }}

        private readonly HashSet<string> blacklistedFileNames = new HashSet<string>();

        public WurmCharacterLogFiles(
            CharacterName characterName,
            string fullDirPathToCharacterLogsDir,
            ILogger logger,
            LogFileInfoFactory logFileInfoFactory)
        {
            if (characterName == null) throw new ArgumentNullException("characterName");
            if (fullDirPathToCharacterLogsDir == null) throw new ArgumentNullException("fullDirPathToCharacterLogsDir");
            if (logger == null) throw new ArgumentNullException("logger");
            if (logFileInfoFactory == null) throw new ArgumentNullException("logFileInfoFactory");
            this.logger = logger;
            this.logFileInfoFactory = logFileInfoFactory;
            CharacterName = characterName;
            FullDirPathToCharacterLogsDir = fullDirPathToCharacterLogsDir;

            fileChangeMonitor = new FileSystemChangeMonitor()
            {
                FullPath = fullDirPathToCharacterLogsDir,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            RebuildFilesCache();
        }

        public IEnumerable<LogFileInfo> TryGetLogFiles(DateTime dateFrom, DateTime dateTo)
        {
            List<LogFileInfo> files = new List<LogFileInfo>();
            foreach (var typeManager in wurmLogTypeToLogTypeManagerMap.Values)
            {
                files.AddRange(typeManager.GetLogFileInfos(dateFrom, dateTo));
            }
            return files.OrderBy(info => info.LogFileDate.DateTime).ToList();
        }

        public IEnumerable<LogFileInfo> TryGetLogFiles(DateTime dateFrom, DateTime dateTo, LogType logType)
        {
            LogTypeManager logTypeManager;
            if (wurmLogTypeToLogTypeManagerMap.TryGetValue(logType, out logTypeManager))
            {
                var files = logTypeManager.GetLogFileInfos(dateFrom, dateTo);
                return files;
            }
            else
            {
                return new LogFileInfo[0];
            }
        }

        public IEnumerable<LogFileInfo> TryGetLogFilesForSpecificPm(DateTime dateFrom, DateTime dateTo, CharacterName pmCharacterName)
        {
            var allPmLogs = TryGetLogFiles(dateFrom, dateTo, LogType.Pm);
            return
                allPmLogs.Where(
                    info => info.FileName.IndexOf(pmCharacterName.Normalized, StringComparison.InvariantCultureIgnoreCase) > -1);
        }

        public void Refresh()
        {
            if (fileChangeMonitor.GetAnyChangeAndReset())
            {
                RebuildFilesCache();
            }
        }

        private void RebuildFilesCache()
        {
            List<LogFileInfo> parsedFiles = ObtainLatestFiles();
            if (parsedFiles.Any())
            {
                oldestLogFileDate = parsedFiles.Min(info => info.LogFileDate.DateTime);
            }
            UpdateTypeManagers(parsedFiles);
        }

        private void UpdateTypeManagers(List<LogFileInfo> parsedFiles)
        {
            var groupedFiles = parsedFiles.GroupBy(info => info.LogType);

            bool changed = false;

            foreach (var group in groupedFiles)
            {
                var logType = group.Key;
                var logFileInfos = group.AsEnumerable();
                LogTypeManager typeManager;
                if (!wurmLogTypeToLogTypeManagerMap.TryGetValue(logType, out typeManager))
                {
                    typeManager = new LogTypeManager();
                    typeManager.Rebuild(logFileInfos);
                    wurmLogTypeToLogTypeManagerMap.Add(logType, typeManager);
                    changed = true;
                }
                else
                {
                    changed = typeManager.Rebuild(logFileInfos);
                }
            }

            if (changed)
            {
                OnFilesAddedOrRemoved();
            }
        }

        private List<LogFileInfo> ObtainLatestFiles()
        {
            var parsedFiles = new List<LogFileInfo>();

            var dir = new DirectoryInfo(FullDirPathToCharacterLogsDir);
            var allFiles = dir.GetFiles();

            foreach (var fileInfo in allFiles)
            {
                if (blacklistedFileNames.Contains(fileInfo.Name))
                {
                    continue;
                }
                var logFileInfo = logFileInfoFactory.Create(fileInfo);
                if (!logFileInfo.ParsingError)
                {
                    parsedFiles.Add(logFileInfo);
                }
                else
                {
                    blacklistedFileNames.Add(fileInfo.Name);
                    GenerateLogMessageForFile(fileInfo);
                }
            }

            return parsedFiles;
        }

        private void GenerateLogMessageForFile(FileInfo fileInfo)
        {
            if (fileInfo.Name.StartsWith("irc.rizon.net"))
            {
                logger.Log(
                    LogLevel.Info,
                    "Skipping log file, that appears to be log from IRC (unsupported), file name: " + fileInfo.FullName,
                    this,
                    null);
            }

            else if (fileInfo.Name.StartsWith("test."))
            {
                logger.Log(
                    LogLevel.Info,
                    "Skipping log file, that appears to be from test server (unsupported), file name: "
                    + fileInfo.FullName,
                    this,
                    null);
            }
            else
            {
                logger.Log(
                    LogLevel.Warn,
                    "Skipping file that had parsing errors, file name: " + fileInfo.FullName,
                    this,
                    null);
            }
        }

        public void Dispose()
        {
            fileChangeMonitor.Dispose();
        }

        public event EventHandler FilesAddedOrRemoved;

        protected virtual void OnFilesAddedOrRemoved()
        {
            EventHandler handler = FilesAddedOrRemoved;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}