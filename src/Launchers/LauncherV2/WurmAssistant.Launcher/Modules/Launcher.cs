using System;
using System.IO;
using AldursLab.Essentials.Synchronization;
using AldursLab.WurmAssistant.Launcher.Dto;
using JetBrains.Annotations;

namespace AldursLab.WurmAssistant.Launcher.Modules
{
    public class Launcher
    {
        readonly string launcherDirPath;
        const string LockFileName = "launcher.lock";

        FileLock fileLock;

        volatile bool entered;

        public Launcher([NotNull] ControllerConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");

            this.launcherDirPath = config.LauncherBinDirFullPath;

            if (!Path.IsPathRooted(launcherDirPath))
            {
                throw new InvalidOperationException("rootPath must be absolute");
            }

            if (!Directory.Exists(launcherDirPath))
            {
                Directory.CreateDirectory(launcherDirPath);
            }
        }

        public void EnterLock()
        {
            if (!entered)
            {
                fileLock = FileLock.EnterWithCreateWait(Path.Combine(launcherDirPath, LockFileName), TimeSpan.Zero);
                entered = true;
            }
        }

        public void ReleaseLock()
        {
            if (entered)
            {
                try
                {
                    fileLock.Dispose();
                }
                finally
                {
                    entered = false;
                }
            }
        }
    }
}
