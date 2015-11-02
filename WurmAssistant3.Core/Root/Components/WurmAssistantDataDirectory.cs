using System.IO;
using System.Threading;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Shared;
using AldursLab.WurmAssistant3.Core.Root.Contracts;

namespace AldursLab.WurmAssistant3.Core.Root.Components
{
    public class WurmAssistantDataDirectory : IWurmAssistantDataDirectory
    {
        readonly DirectoryInfo dataDir;
        FileLock currentAppLock;

        /// <summary>
        /// Prepares data directory and obtains exclusive lock for the directory.
        /// </summary>
        /// <param name="consoleArgs"></param>
        /// <exception cref="LockFailedException"></exception>
        public WurmAssistantDataDirectory(ConsoleArgsManager consoleArgs)
        {
            var dataDirPath = consoleArgs.WurmUnlimitedMode
                ? AppPaths.WurmAssistantUnlimited.DataDir.FullPath
                : AppPaths.WurmAssistant3.DataDir.FullPath;

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

            var lockFile = new FileInfo(AppPaths.WurmAssistant3.DataDir.LockFilePath);
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