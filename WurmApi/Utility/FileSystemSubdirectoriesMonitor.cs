using System;
using System.Collections.Generic;
using System.IO;

namespace AldursLab.WurmApi.Utility
{
    /// <summary>
    /// Monitors for create / delete / rename of direct subdirectories in specific directory.
    /// </summary>
    class FileSystemSubdirectoriesMonitor : IDisposable
    {
        private readonly string directoryFullPath;
        FileSystemChangeMonitor fileSystemChangeMonitor;

        public FileSystemSubdirectoriesMonitor(string directoryFullPath)
        {
            this.directoryFullPath = directoryFullPath;
            fileSystemChangeMonitor = new FileSystemChangeMonitor
            {
                FullPath = directoryFullPath,
                NotifyFilter = NotifyFilters.DirectoryName,
                IncludeSubdirectories = false
            };
            fileSystemChangeMonitor.EnableRaisingEvents = true;
        }

        public event EventHandler DirectoriesChanged;

        public IEnumerable<DirectoryInfo> GetAllDirectories()
        {
            var dirInfo = new DirectoryInfo(directoryFullPath);
            return dirInfo.GetDirectories();
        }

        protected virtual void OnDirectoriesChanged()
        {
            EventHandler handler = DirectoriesChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public void Refresh()
        {
            if (fileSystemChangeMonitor.GetAnyChangeAndReset())
            {
                OnDirectoriesChanged();
            }
        }

        public void Dispose()
        {
            if (fileSystemChangeMonitor != null)
            {
                fileSystemChangeMonitor.EnableRaisingEvents = false;
                fileSystemChangeMonitor.Dispose();
                fileSystemChangeMonitor = null;
            }
        }
    }
}
