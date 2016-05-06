using System;
using System.IO;
using AldursLab.Essentials.FileSystem;
using AldursLab.Essentials.Synchronization;

namespace AldursLab.Persistence.Simple
{
    public class FlatFilesDataStorage : IDataStorage, IDisposable
    {
        readonly string rootPath;
        readonly FileLock fileLock;
        bool disposed;

        public FlatFilesDataStorage(string rootPath)
        {
            if (rootPath == null) throw new ArgumentNullException(nameof(rootPath));
            this.rootPath = rootPath;

            fileLock = FileLock.EnterWithCreate(Path.Combine(rootPath, "dir.lock"));
        }

        public void Save(string setId, string objectId, string data)
        {
            ThrowIfDisposed();
            var fullPath = MakeFilePath(setId, objectId);
            ReliableFileOps.SaveFileContents(fullPath, data);
        }

        public string TryLoad(string setId, string objectId)
        {
            ThrowIfDisposed();
            var fullPath = MakeFilePath(setId, objectId);
            var data = ReliableFileOps.TryReadFileContents(fullPath);
            return data;
        }

        public void Delete(string setId, string objectId)
        {
            ThrowIfDisposed();
            var fullPath = MakeFilePath(setId, objectId);
            var file = new FileInfo(fullPath);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        public void DeleteObjectSet(string setId)
        {
            ThrowIfDisposed();
            var fullPath = MakeDirPath(setId);
            var dir = new DirectoryInfo(fullPath);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
        }

        public void DeleteAllObjectSets()
        {
            ThrowIfDisposed();
            var fullPath = rootPath;
            var dir = new DirectoryInfo(fullPath);
            if (dir.Exists)
            {
                dir.Delete(true);
            }
        }

        public void Dispose()
        {
            fileLock.Dispose();
            disposed = true;
        }

        string MakeDirPath(string storagePath)
        {
            return Path.Combine(rootPath, storagePath);
        }

        string MakeFilePath(string storagePath, string objectId)
        {
            return Path.Combine(rootPath, storagePath, objectId) + ".dat";
        }

        void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(FlatFilesDataStorage));
        }
    }
}