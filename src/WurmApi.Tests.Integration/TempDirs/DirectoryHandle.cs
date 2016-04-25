using System;
using System.Collections.Generic;
using System.IO;
using AldursLab.WurmApi.FileSystem;

namespace AldursLab.WurmApi.Tests.TempDirs
{
    public sealed class DirectoryHandle : IDisposable
    {
        readonly TempDirectoriesManager manager;

        public bool IsDisposed { get; private set; }

        internal DirectoryHandle(TempDirectoriesManager manager, DirectoryInfo dir)
        {
            if (manager == null) throw new ArgumentNullException(nameof(manager));
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            this.manager = manager;
            Dir = dir;

            manager.RegisterHandle(this);
        }

        internal DirectoryInfo Dir { get; }

        public string AbsolutePath => Dir.FullName;

        public bool Exists => Dir.Exists;

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
                targetRelativePath != null ? Path.Combine(Dir.FullName, targetRelativePath) : Dir.FullName,
                true);
            return this;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                manager.FreeHandle(this);
                Dir.Refresh();
                IsDisposed = true;
            }
        }
    }
}