using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using AldurSoft.WurmApi.JobRunning;
using AldurSoft.WurmApi.Modules.Events;
using AldurSoft.WurmApi.Modules.Events.Internal;
using AldurSoft.WurmApi.Modules.Events.Public;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Utility
{
    /// <summary>
    /// Provides cached info about subdirectories and notifies when they change.
    /// </summary>
    abstract class WurmSubdirsMonitor : IDisposable
    {
        protected readonly string DirectoryFullPath;
        readonly TaskManager taskManager;
        readonly Action onChanged;
        readonly FileSystemWatcher fileSystemWatcher;

        IReadOnlyDictionary<string, string> dirNameToFullPathMap = new Dictionary<string, string>();

        readonly TaskHandle task;

        public WurmSubdirsMonitor([NotNull] string directoryFullPath, [NotNull] TaskManager taskManager,
            [NotNull] Action onChanged)
        {
            if (directoryFullPath == null) throw new ArgumentNullException("directoryFullPath");
            if (taskManager == null) throw new ArgumentNullException("taskManager");
            if (onChanged == null) throw new ArgumentNullException("onChanged");
            this.DirectoryFullPath = directoryFullPath;
            this.taskManager = taskManager;
            this.onChanged = onChanged;

            task = new TaskHandle(Refresh, "WurmSubdirsMonitor for path: " + directoryFullPath);
            taskManager.Add(task);

            Refresh();

            fileSystemWatcher = new FileSystemWatcher(directoryFullPath) {NotifyFilter = NotifyFilters.DirectoryName};
            fileSystemWatcher.Created += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Renamed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Deleted += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Changed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            task.Trigger();
        }

        private void DirectoryMonitorOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            task.Trigger();
        }

        private void Refresh()
        {
            var di = new DirectoryInfo(DirectoryFullPath);
            var newMap = di.GetDirectories().ToDictionary(info => info.Name.ToUpperInvariant(), info => info.FullName);

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

        public IEnumerable<string> AllDirectoryNamesNormalized
        {
            get
            {
                return dirNameToFullPathMap.Keys;
            }
        }

        public IEnumerable<string> AllDirectoriesFullPaths
        {
            get
            {
                return this.dirNameToFullPathMap.Values;
            }
        }

        public void Dispose()
        {
            taskManager.Remove(task);
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        protected string GetFullPathForDirName([NotNull] string dirName)
        {
            if (dirName == null) throw new ArgumentNullException("dirName");

            string directoryFullPath;
            if (!dirNameToFullPathMap.TryGetValue(dirName.ToUpperInvariant(), out directoryFullPath))
            {
                throw new DataNotFoundException(dirName);
            }
            return directoryFullPath;
        }
    }
}
