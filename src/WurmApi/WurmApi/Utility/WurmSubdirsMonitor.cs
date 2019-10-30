using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AldursLab.WurmApi.JobRunning;
using AldursLab.WurmApi.Validation;
using JetBrains.Annotations;

namespace AldursLab.WurmApi.Utility
{
    /// <summary>
    /// Provides cached info about subdirectories and notifies when they change.
    /// </summary>
    abstract class WurmSubdirsMonitor : IDisposable
    {
        protected readonly string DirectoryFullPath;
        readonly TaskManager taskManager;
        readonly Action onChanged;
        readonly Action<string, IWurmPaths> validateDirectory;
        readonly IWurmPaths wurmPaths;
        readonly IWurmApiLogger logger;
        readonly FileSystemWatcher fileSystemWatcher;

        IReadOnlyDictionary<string, string> dirNameToFullPathMap = new Dictionary<string, string>();

        readonly TaskHandle task;

        readonly Blacklist<string> directoryBlacklist;

        protected WurmSubdirsMonitor([NotNull] string directoryFullPath, [NotNull] TaskManager taskManager,
            [NotNull] Action onChanged, [NotNull] IWurmApiLogger logger,
            [NotNull] Action<string, IWurmPaths> validateDirectory, [NotNull] IWurmPaths wurmPaths)
        {
            if (directoryFullPath == null) throw new ArgumentNullException(nameof(directoryFullPath));
            if (taskManager == null) throw new ArgumentNullException(nameof(taskManager));
            if (onChanged == null) throw new ArgumentNullException(nameof(onChanged));
            if (validateDirectory == null) throw new ArgumentNullException(nameof(validateDirectory));
            if (wurmPaths == null) throw new ArgumentNullException(nameof(wurmPaths));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            DirectoryFullPath = directoryFullPath;
            this.taskManager = taskManager;
            this.onChanged = onChanged;
            this.validateDirectory = validateDirectory;
            this.wurmPaths = wurmPaths;
            this.logger = logger;

            directoryBlacklist = new Blacklist<string>(logger, "Character directories blacklist");

            task = new TaskHandle(Refresh, "WurmSubdirsMonitor for path: " + directoryFullPath);
            taskManager.Add(task);

            try
            {
                Refresh();
            }
            catch (Exception exception)
            {
                logger.Log(LogLevel.Error, "Error at initial Refresh of " + GetType().Name, this, exception);
            }

            fileSystemWatcher = new FileSystemWatcher(directoryFullPath) {NotifyFilter = NotifyFilters.DirectoryName};
            fileSystemWatcher.Created += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Renamed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Deleted += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Changed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            task.Trigger();
        }

        void DirectoryMonitorOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            task.Trigger();
        }

        void Refresh()
        {
            var di = new DirectoryInfo(DirectoryFullPath);
            var allDirs = di.GetDirectories().Where(info => (info.Attributes & FileAttributes.Hidden) == 0);
            var newMap = new Dictionary<string, string>();

            foreach (var directoryInfo in allDirs)
            {
                if (directoryBlacklist.IsOnBlacklist(directoryInfo.FullName))
                {
                    continue;
                }
                try
                {
                    validateDirectory(directoryInfo.FullName, wurmPaths);
                    newMap.Add(directoryInfo.Name.ToUpperInvariant(), directoryInfo.FullName);
                }
                catch (ValidationException exception)
                {
                    directoryBlacklist.ReportIssue(directoryInfo.FullName);
                    logger.Log(LogLevel.Warn, "Validation issue", this, exception);
                    // todo: need to log this as warking, solving with quick solution
                    // consider: extend AggregateException to carry logging level and handle that universally in TaskManager
                    task.SetErrorAndRetrigger();
                }
            }
            
            var oldDirs = dirNameToFullPathMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();
            var newDirs = newMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();

            var changed = !oldDirs.SequenceEqual(newDirs);

            if (changed)
            {
                dirNameToFullPathMap = newMap;
                OnDirectoriesChanged();
            }
        }

        private void OnDirectoriesChanged()
        {
            onChanged();
        }

        public IEnumerable<string> AllDirectoryNamesNormalized => dirNameToFullPathMap.Keys;

        public IEnumerable<string> AllDirectoriesFullPaths => dirNameToFullPathMap.Values;

        public void Dispose()
        {
            taskManager.Remove(task);
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        protected string GetFullPathForDirName([NotNull] string dirName)
        {
            if (dirName == null) throw new ArgumentNullException(nameof(dirName));

            string directoryFullPath;
            if (!dirNameToFullPathMap.TryGetValue(dirName.ToUpperInvariant(), out directoryFullPath))
            {
                throw new DataNotFoundException(dirName);
            }
            return directoryFullPath;
        }
    }
}
