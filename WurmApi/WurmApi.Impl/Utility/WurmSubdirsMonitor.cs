using System;
using System.Collections.Generic;
using System.Linq;

namespace AldurSoft.WurmApi.Impl.Utility
{
    /// <summary>
    /// Provides cached info about subdirectories and notifies when they change.
    /// </summary>
    public abstract class WurmSubdirsMonitor : IDisposable, IRequireRefresh
    {
        protected readonly string DirectoryFullPath;
        readonly FileSystemSubdirectoriesMonitor directoryMonitor;

        private readonly Dictionary<string, string> dirNameToFullPathMap = new Dictionary<string, string>();

        public WurmSubdirsMonitor(string directoryFullPath)
        {
            this.DirectoryFullPath = directoryFullPath;
            directoryMonitor = new FileSystemSubdirectoriesMonitor(directoryFullPath);
            directoryMonitor.DirectoriesChanged += DirectoryMonitorOnDirectoriesChanged;

            Rebuild();
        }

        private void DirectoryMonitorOnDirectoriesChanged(object sender, EventArgs eventArgs)
        {
            Rebuild();
        }

        public IEnumerable<string> AllDirectoryNamesNormalized
        {
            get { return dirNameToFullPathMap.Keys; }
        }

        public IEnumerable<string> AllDirectoriesFullPaths
        {
            get { return this.dirNameToFullPathMap.Values; }
        }

        public event EventHandler DirectoriesChanged;

        protected virtual void OnDirectoriesChanged()
        {
            EventHandler handler = DirectoriesChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public void Refresh()
        {
            directoryMonitor.Refresh();
        }

        private void Rebuild()
        {
            var currentDirs = directoryMonitor.GetAllDirectories().ToArray();
            bool changed = false;
            foreach (var directoryInfo in currentDirs)
            {
                var dirName = directoryInfo.Name.ToUpperInvariant();
                if (!dirNameToFullPathMap.ContainsKey(dirName))
                {
                    dirNameToFullPathMap.Add(dirName, directoryInfo.FullName);
                    changed = true;
                }
            }

            foreach (var keyvaluepair in dirNameToFullPathMap.ToArray())
            {
                if (currentDirs.All(info => info.Name.ToUpperInvariant() != keyvaluepair.Key))
                {
                    dirNameToFullPathMap.Remove(keyvaluepair.Key);
                    changed = true;
                }
            }
            if (changed)
            {
                OnDirectoriesChanged();
            }
        }

        public void Dispose()
        {
            directoryMonitor.Dispose();
        }

        protected string TryGetFullPathForDirName(string dirName)
        {
            string directoryFullPath;
            dirNameToFullPathMap.TryGetValue(dirName.ToUpperInvariant(), out directoryFullPath);
            if (directoryFullPath == null)
            {
                Rebuild();
                dirNameToFullPathMap.TryGetValue(dirName.ToUpperInvariant(), out directoryFullPath);
            }
            return directoryFullPath;
        }
    }
}
