using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
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
        readonly FileSystemWatcher fileSystemWatcher;

        IReadOnlyDictionary<string, string> dirNameToFullPathMap = new Dictionary<string, string>();

        volatile int rebuildPending = 1;
        volatile object locker = new object();

        public WurmSubdirsMonitor([NotNull] string directoryFullPath)
        {
            if (directoryFullPath == null) throw new ArgumentNullException("directoryFullPath");
            this.DirectoryFullPath = directoryFullPath;

            fileSystemWatcher = new FileSystemWatcher(directoryFullPath) {NotifyFilter = NotifyFilters.DirectoryName};
            fileSystemWatcher.Created += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Renamed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Deleted += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            Refresh(false);
        }

        private void DirectoryMonitorOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            rebuildPending = 1;
            OnDirectoriesChanged();
        }

        protected virtual void OnDirectoriesChanged() { }

        public IEnumerable<string> AllDirectoryNamesNormalized
        {
            get
            {
                Refresh();
                return dirNameToFullPathMap.Keys;
            }
        }

        public IEnumerable<string> AllDirectoriesFullPaths
        {
            get
            {
                Refresh();
                return this.dirNameToFullPathMap.Values;
            }
        }

        public void Dispose()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        protected string GetFullPathForDirName([NotNull] string dirName)
        {
            if (dirName == null) throw new ArgumentNullException("dirName");

            Refresh();

            string directoryFullPath;
            if (!dirNameToFullPathMap.TryGetValue(dirName.ToUpperInvariant(), out directoryFullPath))
            {
                throw new DataNotFoundException(dirName);
            }
            return directoryFullPath;
        }

        private void Refresh(bool sendEvents = true)
        {
            if (rebuildPending == 1)
            {
                lock (locker)
                {
                    var stillPending = Interlocked.CompareExchange(ref rebuildPending, 0, 1) == 1;
                    if (stillPending)
                    {
                        bool changed = false;

                        var di = new DirectoryInfo(DirectoryFullPath);
                        var newMap = di.GetDirectories().ToDictionary(info => info.Name.ToUpperInvariant(), info => info.FullName);

                        var oldDirs = dirNameToFullPathMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();
                        var newDirs = newMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();

                        changed = !oldDirs.SequenceEqual(newDirs);

                        if (changed)
                        {
                            dirNameToFullPathMap = newMap;
                            if (sendEvents) OnDirectoriesChanged();
                        }
                    }
                }
            }
        }
    }
}
