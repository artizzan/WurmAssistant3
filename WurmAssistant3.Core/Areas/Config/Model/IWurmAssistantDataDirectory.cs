using System;
using System.IO;
using System.Threading;
using AldursLab.Essentials.Synchronization;

namespace AldursLab.WurmAssistant3.Core.Areas.Config.Model
{
    public interface IWurmAssistantDataDirectory
    {
        string DirectoryPath { get; }
    }

    public class WurmAssistantDataDirectory : IWurmAssistantDataDirectory
    {
        readonly DirectoryInfo dataDir;
        FileLock currentAppLock;

        /// <summary>
        /// Prepares data directory and obtains exclusive lock for the directory.
        /// </summary>
        /// <exception cref="LockFailedException"></exception>
        public WurmAssistantDataDirectory()
        {
            var dataDirPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                "AldursLab",
                "WurmAssistantData");

            dataDir = new DirectoryInfo(dataDirPath);
            if (!dataDir.Exists) dataDir.Create();
        }

        public string DirectoryPath { get { return dataDir.FullName; } }
        public void Lock()
        {
            if (currentAppLock != null)
            {
                // lock already taken
                return;
            }

            var lockFile = new FileInfo(Path.Combine(dataDir.FullName, "app.lock"));
            if (!lockFile.Exists)
            {
                try
                {
                    lockFile.Create().Dispose();
                }
                catch (IOException)
                {
                    // wait a moment to ensure file system is updated
                    Thread.Sleep(100);
                    lockFile.Refresh();
                    if (lockFile.Exists)
                    {
                        // ignore, something else created the lock file
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            currentAppLock = FileLock.Enter(lockFile.FullName);
        }

        public void Unlock()
        {
            if (currentAppLock != null)
            {
                currentAppLock.Dispose();
                currentAppLock = null;
            }
        }
    }
}
