using System;
using System.Collections.Generic;
using System.IO;
using AldursLab.Essentials.FileSystem;

namespace AldursLab.Testing
{
    public sealed class DirectoryHandle : IDisposable
    {
        readonly TempDirectoriesManager manager;
        readonly DirectoryInfo dir;

        public bool IsDisposed { get; private set; }

        internal DirectoryHandle(TempDirectoriesManager manager, DirectoryInfo dir)
        {
            if (manager == null) throw new ArgumentNullException("manager");
            if (dir == null) throw new ArgumentNullException("dir");
            this.manager = manager;
            this.dir = dir;

            manager.RegisterHandle(this);
        }

        internal DirectoryInfo Dir
        {
            get { return dir; }
        }

        public string AbsolutePath
        {
            get { return Dir.FullName; }
        }

        public string FullName
        {
            get { return AbsolutePath; }
        }

        public bool Exists
        {
            get { return Dir.Exists; }
        }

        public IEnumerable<FileInfo> GetFiles()
        {
            return Dir.EnumerateFiles();
        }

        public IEnumerable<DirectoryInfo> GetDirectories()
        {
            return Dir.EnumerateDirectories();
        }

        public DirectoryHandle AmmendFromSourceDirectory(string sourceDirFullPath, string targetRelativePath = null)
        {
            DirectoryOps.CopyRecursively(sourceDirFullPath,
                targetRelativePath != null ? Path.Combine(dir.FullName, targetRelativePath) : dir.FullName,
                true);
            return this;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                manager.FreeHandle(this, ignoreFileSystemError:true);
                Dir.Refresh();
                IsDisposed = true;
            }
        }
    }
}