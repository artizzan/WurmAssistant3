using System;
using System.IO;
using System.Linq;
using System.Threading;
using AldursLab.Essentials.Extensions.DotNet.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Shared;
using AldursLab.WurmAssistant3.Areas.Core.Contracts;

namespace AldursLab.WurmAssistant3.Areas.Core.Singletons
{
    public class WurmAssistantDataDirectory : IWurmAssistantDataDirectory, IDisposable
    {
        readonly DirectoryInfo dataDir;
        FileLock currentAppLock;

        /// <summary>
        /// Prepares data directory and obtains exclusive lock for the directory.
        /// </summary>
        /// <param name="consoleArgs"></param>
        /// <exception cref="LockFailedException"></exception>
        public WurmAssistantDataDirectory(IConsoleArgs consoleArgs)
        {
            string dataDirPath;
            if (consoleArgs.UseRelativeDataDir)
            {
                var finder = new LauncherDirFinder();
                var launcherBinPath = finder.TryFindLauncherParentDirPath();
                if (launcherBinPath != null)
                {
                    dataDirPath = consoleArgs.WurmUnlimitedMode
                        ? Path.Combine(launcherBinPath, "data-wa-u")
                        : Path.Combine(launcherBinPath, "data-wa-o");
                }
                else
                {
                    dataDirPath = consoleArgs.WurmUnlimitedMode
                        ? Path.Combine(GetType().Assembly.CodeBaseLocalPath(), "data-wa-u")
                        : Path.Combine(GetType().Assembly.CodeBaseLocalPath(), "data-wa-o");
                }
            }
            else
            {
                dataDirPath = consoleArgs.WurmUnlimitedMode
                    ? AppPaths.WurmAssistantUnlimited.DataDir.FullPath
                    : AppPaths.WurmAssistant3.DataDir.FullPath;
            }

            dataDir = new DirectoryInfo(dataDirPath);
            if (!dataDir.Exists) dataDir.Create();
            Lock();
        }

        public string DirectoryPath { get { return dataDir.FullName; } }
        void Lock()
        {
            if (currentAppLock != null)
            {
                // lock already taken
                return;
            }

            var lockFile = new FileInfo(Path.Combine(dataDir.FullName, AppPaths.LockFileRelativePath));
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

        void Unlock()
        {
            if (currentAppLock != null)
            {
                currentAppLock.Dispose();
                currentAppLock = null;
            }
        }

        public void Dispose()
        {
            Unlock();
        }
    }

    class LauncherDirFinder
    {
        const string LauncherExeName = "AldursLab.WurmAssistant.Launcher.exe";

        public string TryFindLauncherParentDirPath()
        {
            var thisDllPath = GetType().Assembly.CodeBaseLocalPath();
            var info = new DirectoryInfo(thisDllPath);
            var dirPath = TryFindLauncherDir(info.Parent);
            return dirPath;
        }

        string TryFindLauncherDir(DirectoryInfo dir, int maxDepth = 4)
        {
            if (dir == null) return null;

            if (maxDepth > 0)
            {
                var file = dir.EnumerateFiles(LauncherExeName).FirstOrDefault();
                if (file != null)
                {
                    return Path.GetDirectoryName(file.FullName);
                }
                else
                {
                    return TryFindLauncherDir(dir.Parent, maxDepth - 1);
                }
            }
            return null;
        }
    }
}