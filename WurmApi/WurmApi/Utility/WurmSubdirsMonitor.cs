using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AldurSoft.WurmApi.Infrastructure;
using JetBrains.Annotations;

namespace AldurSoft.WurmApi.Utility
{
    /// <summary>
    /// Provides cached info about subdirectories and notifies when they change.
    /// </summary>
    public abstract class WurmSubdirsMonitor : IDisposable
    {
        protected readonly string DirectoryFullPath;
        readonly FileSystemWatcher fileSystemWatcher;

        IReadOnlyDictionary<string, string> dirNameToFullPathMap = new Dictionary<string, string>();
        readonly RepeatableThreadedOperation rebuilder;
        volatile bool initiallyRebuilt = false;

        public WurmSubdirsMonitor(string directoryFullPath, ILogger logger)
        {
            this.DirectoryFullPath = directoryFullPath;

            rebuilder = new RepeatableThreadedOperation(() =>
            {
                bool changed = false;

                var di = new DirectoryInfo(DirectoryFullPath);
                var newMap = di.GetDirectories().ToDictionary(info => info.Name.ToUpperInvariant(), info => info.FullName);

                var oldDirs = dirNameToFullPathMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();
                var newDirs = newMap.Select(pair => pair.Key).OrderBy(s => s).ToArray();

                changed = oldDirs.SequenceEqual(newDirs);

                if (changed)
                {
                    dirNameToFullPathMap = newMap;
                    if (initiallyRebuilt)
                    {
                        OnDirectoriesChanged();
                    }
                }

                initiallyRebuilt = true;
            });

            rebuilder.OperationError += (sender, args) =>
            {
                const int retryDelay = 5;
                logger.Log(LogLevel.Error,
                    string.Format("Error at directory refresh job, retrying in {0} seconds", retryDelay), this, args.Exception);
                rebuilder.DelayedSignal(TimeSpan.FromSeconds(retryDelay));
            };

            fileSystemWatcher = new FileSystemWatcher(directoryFullPath) {NotifyFilter = NotifyFilters.DirectoryName};
            fileSystemWatcher.Created += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Renamed += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.Deleted += DirectoryMonitorOnDirectoriesChanged;
            fileSystemWatcher.EnableRaisingEvents = true;

            rebuilder.Signal();
            rebuilder.WaitSynchronouslyForInitialOperation(TimeSpan.FromSeconds(30));
        }

        private void DirectoryMonitorOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            rebuilder.Signal();
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

        public event EventHandler DirectoriesChanged;

        protected virtual void OnDirectoriesChanged()
        {
            EventHandler handler = DirectoriesChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
            rebuilder.Dispose();
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
