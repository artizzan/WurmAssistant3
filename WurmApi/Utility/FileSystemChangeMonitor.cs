using System;
using System.IO;
using System.Threading;

namespace AldursLab.WurmApi.Utility
{
    class FileSystemChangeMonitor : IDisposable
    {
        private readonly FileSystemWatcher fileSystemWatcher;

        private int _changeToken;
        private volatile bool _changed;
        private volatile bool _created;
        private volatile bool _deleted;
        private volatile bool _renamed;

        public FileSystemChangeMonitor()
        {
            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Changed += (sender, args) =>
                {
                    Changed = true;
                    Interlocked.Increment(ref _changeToken);
                };
            fileSystemWatcher.Created += (sender, args) =>
                {
                    Created = true;
                    Interlocked.Increment(ref _changeToken);
                };
            fileSystemWatcher.Deleted += (sender, args) =>
                {
                    Deleted = true;
                    Interlocked.Increment(ref _changeToken);
                };
            fileSystemWatcher.Renamed += (sender, args) =>
                {
                    Renamed = true;
                    Interlocked.Increment(ref _changeToken);
                };
        }

        /// <summary>
        /// Gets or sets a value indicating whether subdirectories within the specified path should be monitored.
        /// </summary>
        public bool IncludeSubdirectories
        {
            get { return fileSystemWatcher.IncludeSubdirectories; }
            set { fileSystemWatcher.IncludeSubdirectories = value; }
        }

        /// <summary>
        /// Gets or sets the type of changes to watch for.
        /// </summary>
        public NotifyFilters NotifyFilter
        {
            get { return fileSystemWatcher.NotifyFilter; }
            set { fileSystemWatcher.NotifyFilter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the component is enabled.
        /// </summary>
        public bool EnableRaisingEvents
        {
            get { return fileSystemWatcher.EnableRaisingEvents; }
            set { fileSystemWatcher.EnableRaisingEvents = value; }
        }

        /// <summary>
        /// Gets or sets the filter string used to determine what files are monitored in a directory.
        /// </summary>
        public string Filter
        {
            get { return fileSystemWatcher.Filter; }
            set { fileSystemWatcher.Filter = value; }
        }

        /// <summary>
        /// Gets or sets the path of the directory to watch.
        /// </summary>
        public string FullPath
        {
            get { return fileSystemWatcher.Path; }
            set { fileSystemWatcher.Path = value; }
        }

        /// <summary>
        /// File/directory change was detected since last reset.
        /// </summary>
        public bool Changed
        {
            get { return _changed; }
            private set { _changed = value; }
        }

        /// <summary>
        /// File/directory was created since last reset.
        /// </summary>
        public bool Created
        {
            get { return _created; }
            private set { _created = value; }
        }

        /// <summary>
        /// File/directory was deleted since last reset.
        /// </summary>
        public bool Deleted
        {
            get { return _deleted; }
            private set { _deleted = value; }
        }

        /// <summary>
        /// File/directory was renamed since last reset.
        /// </summary>
        public bool Renamed
        {
            get { return _renamed; }
            private set { _renamed = value; }
        }

        /// <summary>
        /// Something happened since last reset.
        /// </summary>
        public bool AnyChange => Changed || Created || Deleted || Renamed;

        /// <summary>
        /// Attempt to reset the counters. If false is returned, 
        /// a change happened since provided changeToken and no reset was performed.
        /// </summary>
        /// <param name="changeToken"></param>
        /// <returns></returns>
        public bool Reset(int changeToken)
        {
            if (changeToken == _changeToken)
            {
                Changed = Created = Deleted = Renamed = false;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Obtain current ChangeToken, that can be used to prevent "lost" changes.
        /// Should be obtained before any reads are done.
        /// </summary>
        public int ChangeToken => _changeToken;

        public void Dispose()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        /// <summary>
        /// Helper for getting the AnyChange indicator and resetting indicator if there was no change in between operations.
        /// </summary>
        /// <returns></returns>
        public bool GetAnyChangeAndReset()
        {
            var token = ChangeToken;
            var changed = AnyChange;
            Reset(token);
            return changed;
        }
    }
}
