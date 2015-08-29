using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AldursLab.Essentials.Synchronization;

namespace AldursLab.WurmAssistant3.Core.Infrastructure
{
    public class SharedDataDirectory : IDisposable
    {
        readonly DirectoryInfo dataDir;
        readonly FileLock appLock;

        /// <summary>
        /// Prepares data directory and obtains exclusive lock for the directory.
        /// </summary>
        /// <exception cref="LockFailedException"></exception>
        public SharedDataDirectory()
        {
            var dataDirPath =
                Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
                    "AldursLab",
                    "WurmAssistantData");

            dataDir = new DirectoryInfo(dataDirPath);
            if (!dataDir.Exists) dataDir.Create();
            
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

            appLock = FileLock.Enter(lockFile.FullName);
        }

        public string FullName { get { return dataDir.FullName; } }

        public void Dispose()
        {
            if (appLock != null) appLock.Dispose();
        }
    }
}
